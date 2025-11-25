using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(BoxCollider2D))]
public class PlayerStatePattern : MonoBehaviour
{
    #region 매개변수
    //바닥을 밟고 있는지 판정하기 위한 코드를 담고 있는 _groundChecker 오브젝트
    [SerializeField] GroundChecker _groundChecker;
    
    //땅을 밟고 있는지 여부
    public bool _isGrounded;

    //애니메이터 관련
    public bool _isAttack;
    public bool _isJump;
    public bool _isYIncreasing;
    public bool _isHit;
    public bool _isDash;
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
        _anim = GetComponentInChildren<Animator>();
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void Start()
    {
        SetState(new PlayerIdleState(this));
    }

    private void FixedUpdate()
    {
        Vector2 velocity = _rigid.linearVelocity;
        velocity.x = _moveInput.x * _moveSpeed;
        if (_moveInput.x > 0)
            gameObject.transform.rotation = Quaternion.Euler(0, 0, 0);
        else if (_moveInput.x < 0)
            gameObject.transform.rotation = Quaternion.Euler(0, 180, 0);
        _rigid.linearVelocity = velocity;
    }

    private void Update()
    {
        _currentState.OnUpdate();
    }
    public void OnMove(InputAction.CallbackContext ctx)
    {
        if(_isAttack == false && _isHit == false)
            _moveInput = ctx.ReadValue<Vector2>();
    }

    public void OnJump(InputAction.CallbackContext ctx)
    {
        //버튼을 눌렀고 바닥에 있는 판정이라면 점프 시행
        if (ctx.started && _isGrounded == true)
        {
            _rigid.linearVelocity = new Vector2(0, _jumpPower);
            _groundChecker._coyoteTime = -0.1f;
        }
    }

    public void SetState(IPlayerState newState)
    {
        _currentState?.OnExit();
        _currentState = newState;
        _currentState.OnEnter();
    }

    public void HitChecker(bool value)
    {
        if (value == true)
        {
            _isHit = true;
            _anim.SetBool("isHit", true);
        }
        else if (value == false)
        {
            _isHit = false;
            _anim.SetBool("isHit", false);
        }
    }
    public void AttackChecker(bool value)
    {
        _isAttack = value;
        _anim.SetBool("isAttack", _isAttack);
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
}
