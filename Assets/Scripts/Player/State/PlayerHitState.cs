using UnityEngine;

public class PlayerHitState : IPlayerState
{
    private PlayerStatePattern _player;
    private Vector2 _moveInput;
    private bool _isGround;

    //피격 모션이 지속될 시간
    private float _hitDuration = 0.25f;
    //피격 모션 시간 체크용
    private float _timeChecker = 0;

    /// <summary>
    /// 플레이어의 상태패턴 관리용 코드를 받아 해당 코드에서 필요한 내용을 가져옵니다.
    /// </summary>
    /// <param name="player">PlayerStatePattern 코드</param>
    public PlayerHitState(PlayerStatePattern player)
    {
        _player = player;
        _moveInput = _player._moveInput;
        _isGround = _player._isGrounded;
    }

    public void OnEnter()
    {
        _player.HitChecker(true);
        _player._health--;
        _player.AttackChecker(false);
        _player.DashChecker(false);
        _player.MoveChecker(false);
    }

    public void OnExit()
    {
        
    }

    public void OnUpdate()
    {
        _timeChecker += Time.deltaTime;
        if (_timeChecker > _hitDuration)
        {
            _player._health--;
            _timeChecker = 0;
            _player.HitChecker(false);
            if (_player._health >= 0.1)
            {
                _player._anim.SetFloat("Health", _player._health);
                if (_isGround == false)
                {
                    _player.SetState(new PlayerJumpState(_player));
                }
                else if (_isGround == true && _player._isMoving)
                {
                    _player.SetState(new PlayerRunState(_player));
                }
                else if (_isGround == true && !_player._isMoving)
                {
                    _player.SetState(new PlayerIdleState(_player));
                }
            }
            else if (_player._health < 0.1)
                _player._anim.SetFloat("Health", 0);
        }
    }
}
