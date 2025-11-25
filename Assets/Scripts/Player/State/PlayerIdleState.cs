using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerIdleState : IPlayerState
{
    private PlayerStatePattern _player;
    private bool _isGround;
    private bool _isAttack;
    private Rigidbody2D _rigid;
    private Vector2 _moveInput;

    public PlayerIdleState(PlayerStatePattern player)
    {
        _player = player;
        _moveInput = _player._moveInput;
        _rigid = _player._rigid;
        _isGround = _player._isGrounded;
        _isAttack = false;
    }

    private void Start()
    {
        _rigid = _player._rigid;
    }
    private void Update()
    {

    }
    public void OnEnter()
    {
        Debug.Log("정지 상태 인식 완료");
        _player.JumpChecker(false);
        _player.MoveChecker(false);
        _player.DashChecker(false);
        _player.AttackChecker(false);
    }

    public void OnExit()
    {

    }

    public void OnUpdate()
    {
        _isGround = _player.IsGrounded;
        _moveInput = _player.MoveInput;
        if (_isGround == false)
        {
            Debug.Log("땅에서 떨어짐");
            _player.SetState(new PlayerJumpState(_player));
        }
        else if (_isGround == true && _player._isMoving)
        {
            Debug.Log("움직임");
            _player.SetState(new PlayerRunState(_player));
        }
        else if (_player._isAttack == true)
        {
            Debug.Log("공격 입력 인식");
            _player.SetState(new PlayerAttackState(_player));
        }
    }
}
