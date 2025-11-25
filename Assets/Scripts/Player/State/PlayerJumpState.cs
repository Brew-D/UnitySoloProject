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
        Debug.Log("체공 상태 인식 완료");
        _player.JumpChecker(true);
        _player.DashChecker(false);
        _player.AttackChecker(false);
    }
    private void Update()
    {

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
            Debug.Log("착지 후 이동중");
            _player.SetState(new PlayerRunState(_player));
        }
        else if (_isGround == true && _moveInput.x == 0)
        {
            Debug.Log("착지 후 대기중");
            _player.SetState(new PlayerIdleState(_player));
        }
        else if (_player._isDash)
        {
            Debug.Log("질주 발동");
            _player.SetState(new PlayerDashState(_player));
        }
    }

    public void OnExit()
    {

    }
}
