using UnityEngine;

public class PlayerAttackState : IPlayerState
{
    private PlayerStatePattern _player;
    private Vector2 _moveInput;
    private bool _isGround;
    private BoxCollider2D _attackRange; 

    //공격 모션이 지속될 시간
    private float _firstAttackDuration = 0.58f;
    private float _secondAttackDuration = 0.66f;
    private float _currentAttackDuration;
    //공격 모션 시간 체크용
    private float _timeChecker = 0;

    /// <summary>
    /// 플레이어의 상태패턴 관리용 코드를 받아 해당 코드에서 필요한 내용을 가져옵니다.
    /// </summary>
    /// <param name="player">PlayerStatePattern 코드</param>
    public PlayerAttackState(PlayerStatePattern player)
    {
        _player = player;
        _moveInput = _player._moveInput;
        _isGround = _player._isGrounded;
        _attackRange = _player._attackRange;
        _currentAttackDuration = _firstAttackDuration;
        _attackRange.enabled = true;
    }

    public void OnEnter()
    {
        _player.AttackChecker(true);
        _player.HitChecker(false);
        _player.DashChecker(false);
        _player.MoveChecker(false);
    }

    public void OnExit()
    {

    }

    public void OnUpdate()
    {
        _timeChecker += Time.deltaTime;
        if (_timeChecker > _currentAttackDuration)
        {
            _timeChecker = 0;
            if(_player._isAttackPerforming)
            {
                switch(_currentAttackDuration)
                {
                    case 0.58f:
                        _currentAttackDuration = _secondAttackDuration;
                        _attackRange.enabled = true;
                        break;
                    case 0.66f:
                        _currentAttackDuration = _firstAttackDuration;
                        _attackRange.enabled = true;
                        break;
                }
            }
            else if (!_player._isAttackPerforming)
            {
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
        }
        if (_player._health < 0.1)
            _player._anim.SetFloat("Health", 0);
    }
}
