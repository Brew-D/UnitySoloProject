using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerIdleState : IPlayerState
{
    private PlayerStatePattern _player;
    private bool _isGround;
    private Vector2 _moveInput;

    public PlayerIdleState(PlayerStatePattern player)
    {
        _player = player;
        _moveInput = _player._moveInput;
        _isGround = _player._isGrounded;
    }

    public void OnEnter()
    {
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
            _player.SetState(new PlayerJumpState(_player));
        }
        else if (_isGround == true && _player._isMoving)
        {
            _player.SetState(new PlayerRunState(_player));
        }
        else if (_player._isAttack == true)
        {
            _player.SetState(new PlayerAttackState(_player));
        }
        else if (_player._isHit == true)
        {
            _player.SetState(new PlayerHitState(_player));
        }
        if (_player._health < 0.1)
            _player._anim.SetFloat("Health", 0);
    }
}
