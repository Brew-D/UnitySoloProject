using UnityEngine;
using UnityEngine.InputSystem;

public class EnemyStatePattern : MonoBehaviour
{
    public float _health = 5;
    [SerializeField] PathFinder _pathFinder;
    [SerializeField] JumpPowerChecker[] _jumpPowerCheck = new JumpPowerChecker[4];
    [SerializeField] EnemyMovement _mover;
    public Vector2 _spawnLocation;

    public BoxCollider2D _attackRange;
    public float _jumpPower = 0;
    public Rigidbody2D _rigid;
    public bool _isAttack;
    public bool _isHit;
    public bool _isMove;
    public bool _isGround = true;

    private Vector2 direction;
    public Animator _anim;
    private IEnemyState _currentState;

    private void Start()
    {
        SetState(new EnemyIdleState(this));
    }
    private void OnEnable()
    {
        _spawnLocation = gameObject.transform.position;
    }
    public void FixedUpdate()
    {
        if (_mover.direction != Vector2.zero)
            MoveCheck(true);
        else
            MoveCheck(false);
        if (Mathf.Abs(_mover._pathFinder._targetPos.x - gameObject.transform.position.x) < 1.3f)
            AttackCheck(true);
        else
            AttackCheck(false);
    }
    private void Update()
    {
        _currentState.OnUpdate();
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.layer == 12)
        {
            HitCheck(true);
        }
    }
    public void GetHit()
    {
        _health--;
    }
   

    public void Death()
    {
        GameManager.Instance._killedEnemy++;
        GameObject parent = transform.parent.gameObject;
        parent.SetActive(false);
    }
    public void AttackStart()
    {
        _attackRange.gameObject.SetActive(true);
    }
    public void AttackEnd()
    {
        _attackRange.gameObject.SetActive(false);
    }

    public void SetState(IEnemyState newState)
    {
        _currentState?.OnExit();
        _currentState = newState;
        _currentState.OnEnter();
    }

    public void HitCheck(bool value)
    {
        _isHit = value;
        _anim.SetBool("isHit", _isHit);
    }
    public void HitRecovery()
    {
        _isHit = false;
        _anim.SetBool("isHit", false);
    }

    public void AttackCheck(bool value)
    {
            _isAttack = value;
            _anim.SetBool("isTargetInRange", _isAttack);
    }

    public void MoveCheck(bool value)
    {
        if (value == true)
            _anim.SetBool("isMoving", true);
        else
            _anim.SetBool("isMoving", false);
    }
}
