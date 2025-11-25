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
        _isGround = _player.IsGrounded;
        _moveInput = _player.MoveInput;
        OnUpdate();
    }
    public void OnEnter()
    {
        _player.MoveChecker(false);
        _player.JumpChecker(false);
    }

    public void OnExit()
    {

    }

    public void OnUpdate()
    {
        if (_isGround != true)
        {
            _player.SetState(new PlayerJumpState(_player));
        }
        else if (_moveInput.x != 0)
        {
            _player.SetState(new PlayerRunState(_player));
        }
    }
}
