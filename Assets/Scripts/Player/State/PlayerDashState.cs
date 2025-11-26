using UnityEngine;
using static UnityEditor.Experimental.GraphView.GraphView;

public class PlayerDashState : IPlayerState
{
    private PlayerStatePattern _player;
    private float _moveSpeed = 5f;
    private Vector2 _moveInput;
    private bool _isGround;

    //대시 지속시간
    private float duration = 0.2f;
    //시간 체크용
    private float timeChecker = 0;

    private bool isActivate = false;

    /// <summary>
    /// 플레이어의 상태패턴 관리용 코드를 받아 해당 코드에서 필요한 내용을 가져옵니다.
    /// </summary>
    /// <param name="player">PlayerStatePattern 코드</param>
    public PlayerDashState(PlayerStatePattern player)
    {
        _player = player;
        _moveSpeed = _player._moveSpeed;
        if (_player._moveInput.x < 0)
            _moveInput = new Vector2(-1, 0);
        else if (_player._moveInput.x > 0)
            _moveInput = new Vector2(1, 0);
        _isGround = _player._isGrounded;
    }

    /// <summary>
    /// 해당 상태에 진입했을 때 시행되어야 하는 코드입니다.
    /// </summary>
    public void OnEnter()
    {
        Debug.Log("질주 상태 인식 완료");
        _player.MoveChecker(true);
        _player.DashChecker(true);
        _player.AttackChecker(false);
        isActivate = true;
        timeChecker = 0;
    }

    /// <summary>
    /// 해당 상태에서 벗어났을 때 시행되어야 하는 코드입니다.
    /// </summary>
    public void OnExit()
    {

    }

    private void Update()
    {

    }

    /// <summary>
    /// 해당 상태가 유지되는 동안 시행되어야 하는 코드입니다.
    /// </summary>
    public void OnUpdate()
    {
        timeChecker += Time.deltaTime;
        //플레이어의 움직임 입력과 바닥 접촉 여부를 확인합니다.
        _isGround = _player.IsGrounded;
        //질주 상태가 유지될 동안 이동하는 속도를 더 높게 적용시킵니다.
        Dash();
        if (timeChecker >= duration)
        {
            _moveSpeed = _player._moveSpeed;
            if (_isGround == false)
            {
                Debug.Log("땅에서 떨어짐");
                _player.SetState(new PlayerJumpState(_player));
            }
            else if (_isGround == true && _player._isMoving)
            {
                Debug.Log("착지 후 이동중");
                _player.SetState(new PlayerIdleState(_player));
            }
            else if (_isGround == true && !_player._isMoving)
            {
                Debug.Log("착지 후 대기중");
                _player.SetState(new PlayerIdleState(_player));
            }
        }
    }

    public void Dash()
    {
        if (timeChecker < duration && _player._isDash)
        {
            _moveSpeed = _player._moveSpeed * 4f;
            _player._rigid.gravityScale = 1;
            Vector2 velocity = _player._rigid.linearVelocity;
            velocity.x = _moveInput.x * _moveSpeed;
            _player._rigid.linearVelocity = velocity;
            if (_isGround == false)
                _player._groundChecker._coyoteTime = 0.4f;
        }
        else
        {
            _moveSpeed = _player._moveSpeed;
            _player._rigid.gravityScale = 1;
            _player._isDash = false;
        }
    }
}
