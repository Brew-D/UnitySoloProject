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
        if (_player._isDash)
        {
            _player.SetState(new PlayerDashState(_player));
        }
        else if (_isGround == false)
        {
            _player.SetState(new PlayerJumpState(_player));
        }
        else if (_isGround == true && !_player._isMoving)
        {
            _player.SetState(new PlayerIdleState(_player));
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
