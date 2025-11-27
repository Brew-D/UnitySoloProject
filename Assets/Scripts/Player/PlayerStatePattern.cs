using UnityEngine;
using UnityEngine.InputSystem;

//To make method related to attack and get attacked, player character needs a collider.
//And there is some cheat like go up by rubbing corner of platform when you can't go through by normal way,
//I choose BoxCollider2D to block those cheat.
[RequireComponent(typeof(BoxCollider2D))]
public class PlayerStatePattern : MonoBehaviour
{
    #region 매개변수
    //점프를 동작하게 하기 위한 GroundChecker 바닥확인입니다.
    [SerializeField] public GroundChecker _groundChecker;
    
    //"Enemy" 레이어와 충돌했을 경우 트리거 발동을 위한 공격범위 콜라이더입니다.
    [SerializeField] public BoxCollider2D _attackRange;

    //정찰 중 낭떠러지에 떨어지거나 본 게임에 돌입했을 경우 이동하게 될 위치입니다.
    public Vector3 _spawnLocation = new Vector2(-2.05f, 0.51f);

    //땅을 밟고 있는지 여부를 확인합니다.
    public bool _isGrounded;

    //공격 상태가 지속되고 있는지 확인합니다.
    public bool _isAttackPerforming = false;

    //애니메이터, 상태패턴 관련
    public bool _isAttack;
    public bool _isJump;
    public bool _isYIncreasing;
    public bool _isHit;
    public bool _isDash;
    public bool _isMoving;
    private float timer;

    //플레이어의 체력입니다. 0이 되면 사망 판정 처리됩니다.
    public float _health;
    
    //움직임 관련 요소
    public float _moveSpeed;
    public float _jumpPower;
    public Vector2 _moveInput;
    public Rigidbody2D _rigid;

    //다른 코드에 넘겨줄 필요 없이 해당 코드에서만 제어할 것들
    public Animator _anim;
    private IPlayerState _currentState;


    
    #endregion

    #region 프로퍼티
    public float MoveSpeed => _moveSpeed;
    public float JumpPower => _jumpPower;

    public Vector2 MoveInput => _moveInput;

    public bool IsAttack => _isAttack;
    public bool IsJump => _isJump;
    public bool IsYIncreasing => _isYIncreasing;
    public bool IsHit => _isHit;
    public bool IsDash => _isDash;
    public bool IsGrounded => _isGrounded;
    #endregion

    private void Awake()
    {
        _rigid = GetComponent<Rigidbody2D>();
        //애니메이터 적용된 이미지를 바로 플레이어 캐릭터로 사용할 경우 좌우 반전시에 위치가 변하는 문제가 있어
        //자식 오브젝트로 집어넣어 해결하였기 때문에 자식에서 찾아와야 합니다.
        _anim = GetComponentInChildren<Animator>();
    }

    private void Start()
    {
        //기본 상태는 "대기"상태입니다.
        SetState(new PlayerIdleState(this));
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        
        //게임이 진행되는 도중 적의 공격에 피격되었다면 체력을 감소시키기 위해 피격 상태를 참으로 지정해줍니다.
        if (GameManager.Instance._isStarted && collision.gameObject.layer == 11)
        {
            _isHit = true;
        }
    }

    private void FixedUpdate()
    {
        Vector2 velocity = _rigid.linearVelocity;
        //질주 상태일 때는 그 상태가 끝날 때까지 움직임을 질주 상태 자체가 관리합니다.
        //공격할 때는 집중하고 있으므로, 피격 중에는 경직에 의해 움직일 수 없습니다.
        //움직임 제어의 중첩을 막기 위해 해당 상태가 아닐 경우에만 움직임이 되도록 바꿉니다.
        if(!_isDash && !_isHit && !_isAttack)
            velocity.x = _moveInput.x * _moveSpeed;
        //플레이어가 입력받은 값이 양수였다면 바라보는 방향을 그대로, 음수였다면 방향을 전환시켜 자연스럽게 합니다.
        if (_moveInput.x > 0)
            gameObject.transform.rotation = Quaternion.Euler(0, 0, 0);
        else if (_moveInput.x < 0)
            gameObject.transform.rotation = Quaternion.Euler(0, 180, 0);

        _rigid.linearVelocity = velocity;
    }

    private void Update()
    {
        //현재 상태의 상태 진행중 메서드를 실행합니다.
        _currentState.OnUpdate();
        //플레이어의 입력을 받아 움직이고 있다면 이동 여부를 참으로 지정합니다.
        if (_moveInput.x != 0)
        {
            _isMoving = true;
        }
        else if(_moveInput.x == 0)
        {
            _isMoving = false;
        }
        if (_health < 0)
            DeathChecker();
    }
    public void OnMove(InputAction.CallbackContext ctx)
    {
        //공격 중이거나 피격 중에는 움직일 수 없습니다.
        //질주는 이동하는 방향을 받아올 필요가 있어 제외하지 않습니다.
        if(_isAttack == false && _isHit == false)
            _moveInput = ctx.ReadValue<Vector2>();
    }

    public void OnJump(InputAction.CallbackContext ctx)
    {
        //버튼을 눌렀고 바닥에 있는 판정이라면 점프를 시행합니다.
        if (ctx.started && _isGrounded == true)
        {
            _rigid.linearVelocity = new Vector2(0, _jumpPower);
            //체공 상태가 되더라도 일정 시간 점프가 가능하게 하는 코요테 타임은
            //점프에 성공했을 때는 사라지게 설정합니다.
            _groundChecker._coyoteTime = -0.1f;
        }
    }
    public void OnDash(InputAction.CallbackContext ctx)
    {
        //질주는 움직임을 받아와야 하고, 이미 질주중이거나 공격 및 피격 상태에서는 질주가 불가능합니다.
        if (_moveInput != Vector2.zero && ctx.started &&
            !_isDash && !_isAttack && !_isHit)
        {
            _isDash = true;
        }
    }

    public void OnAttack(InputAction.CallbackContext ctx)
    {
        //질주 중이거나 피격 중이지 않으며, 무게를 실을 수 있도록 공중에 있지 않은 상황에서만
        //공격이 가능합니다. 또한 공격 버튼의 입력이 중지되는 순간 공격 진행을 취소합니다.
        if (ctx.started && !_isDash && !_isHit && !_isJump)
        {
            _isAttack = true;
            _isAttackPerforming = true;
        }
        if (_isAttackPerforming && ctx.canceled)
            _isAttackPerforming = false;
    }

    public void SetState(IPlayerState newState)
    {
        //상태 패턴에서 상태가 끝났을 때의 코드 실행이 이루어져야 하나,
        //현재 사용한 상태 패턴에서는 애니메이션 변경에 초점을 두고 있어
        //제작하지 않은 상태 끝의 코드는 실행하지 않습니다.
        //_currentState?.OnExit();
        _currentState = newState;
        _currentState.OnEnter();
    }

    public void HitChecker(bool value)
    {
            _isHit = value;
            _anim.SetBool("isHit", _isHit);
    }
    public void AttackChecker(bool value)
    {
        if (!_isJump && !_isDash)
        {
            _isAttack = value;
            _anim.SetBool("isAttack", _isAttack);
            _attackRange.enabled = _isAttack;
        }
    }
    public void DashChecker(bool value)
    {
        _isDash = value;
        _anim.SetBool("isDash", _isDash);
    }
    public void JumpChecker(bool value)
    {
        _isGrounded = !value;
        _anim.SetBool("isGrounded", _isGrounded);
        _isJump = value;
        _anim.SetBool("isJump", _isJump);
        _isYIncreasing = value;
        _anim.SetBool("isYIncreasing", _isYIncreasing);
    }
    public void HeightChecker(bool value)
    {
        _isYIncreasing = value;
        _anim.SetBool("isYIncreasing", _isYIncreasing);
    }
    public void MoveChecker(bool value)
    {
        if (value == true)
            _anim.SetBool("isMoveInputOn", true);
        else
            _anim.SetBool("isMoveInputOn", false);
    }
    public void DeathChecker()
    {
        if (_health < 0.1 && Time.timeScale != 0)
        {
            timer += Time.deltaTime;
            if (timer > 3f)
            {
                Time.timeScale = 0;
                GameManager.Instance.Credit();
            }
        }
    }
}
