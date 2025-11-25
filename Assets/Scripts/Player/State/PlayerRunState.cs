using UnityEngine;

public class PlayerRunState : IPlayerState
{
    private PlayerStatePattern _player;
    private float _moveSpeed = 5f;
    private LayerMask _groundLayer;
    private Transform _groundChecker;
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
    }

    public void OnExit()
    {

    }

    private void Update()
    {
        _moveInput = _player.MoveInput;
        _isGround = _player.IsGrounded;
        OnUpdate();
    }

    public void OnUpdate()
    {
        if (_isGround != true)
        {
            _player.SetState(new PlayerJumpState(_player));
        }
        else if (_moveInput.x == 0)
        {
            _player.SetState(new PlayerIdleState(_player));
        }
    }
}
