using UnityEngine;

public class PlayerRunState : IPlayerState
{
    private PlayerStatePattern _player;
    private float _moveSpeed = 5f;
    private Vector2 _moveInput;
    private bool _isGround;

    public PlayerRunState(PlayerStatePattern player)
    {
        _player = player;
        _moveSpeed = _player._moveSpeed;
        _moveInput = _player._moveInput;
        _isGround = _player._isGrounded;
    }

    public void OnEnter()
    {
        Debug.Log("이동 상태 인식 완료");
        _player.MoveChecker(true);
        _player.JumpChecker(false);
        _player.AttackChecker(false);
        _player.DashChecker(false);
    }

    public void OnExit()
    {

    }

    private void Update()
    {

    }

    public void OnUpdate()
    {
        _moveInput = _player.MoveInput;
        _isGround = _player.IsGrounded;
        if (_isGround == false)
        {
            Debug.Log("땅에서 떨어짐");
            _player.SetState(new PlayerJumpState(_player));
        }
        else if (_isGround == true && !_player._isMoving)
        {
            Debug.Log("움직임 멈춤");
            _player.SetState(new PlayerIdleState(_player));
        }
        else if (_player._isDash)
        {
            Debug.Log("질주 발동");
            _player.SetState(new PlayerDashState(_player));
        }
        else if (_player._isAttack == true)
        {
            Debug.Log("공격 입력 인식");
            _player.SetState(new PlayerAttackState(_player));
        }
    }
}
