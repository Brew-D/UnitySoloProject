using UnityEngine;

public class PlayerJumpState : IPlayerState
{
    private PlayerStatePattern _player;
    private float _moveSpeed = 5f;
    private float _jumpPower = 5f;
    private Rigidbody2D _rigid;
    private Vector2 _moveInput;
    private bool _isGround;

    private float previousCoords;
    private bool isHigherThanBefore;

    public PlayerJumpState(PlayerStatePattern player)
    {
        _player = player;
        _moveSpeed = _player._moveSpeed;
        _jumpPower = _player._jumpPower;
        _isGround = _player._isGrounded;
        _moveInput = _player._moveInput;
        previousCoords = _player.transform.position.y;
    }

    public void OnEnter()
    {
        _player.JumpChecker(true);
    }
    private void Update()
    {
        OnUpdate();
    }

    public void OnUpdate()
    {
        _moveInput = _player.MoveInput;
        _isGround = _player.IsGrounded;
        if (previousCoords > _player.transform.position.y)
            _player.HeightChecker(false);
        else
            previousCoords = _player.transform.position.y;

        if (_isGround == true && _moveInput.x != 0)
        {
            _player.SetState(new PlayerRunState(_player));
        }
        else if (_isGround == true && _moveInput.x == 0)
        {
            _player.SetState(new PlayerIdleState(_player));
        }
    }

    public void OnExit()
    {

    }
}
