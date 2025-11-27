using UnityEngine;

public class EnemyMovement : MonoBehaviour
{
    [SerializeField] Vector2 _spawnLocation;
    [SerializeField] JumpPowerChecker[] _jumpPowerCheck = new JumpPowerChecker[4];
    public PathFinder _pathFinder;
    public float _jumpPower = 0;
    public Rigidbody2D _rigid;

    public bool _inRange = true;

    public Vector2 direction;
    private void FixedUpdate()
    {
        if(GameManager.Instance._isStarted)
        {
            EnemyChase();
            JumpCheck();
            if (_jumpPower > 0.1)
            {
                PathCheck();
            }
        }
    }
    void EnemyChase()
    {
        if (_pathFinder._path.Count > 0)
        {
            Vector2 EnemyPos = new Vector2(gameObject.transform.position.x, gameObject.transform.position.y);
            direction = _pathFinder._path[_pathFinder._index+1].Pos - EnemyPos;
            if (direction.x > 0)
                gameObject.transform.rotation = Quaternion.Euler(0, 0, 0);
            else if (direction.x < 0)
                gameObject.transform.rotation = Quaternion.Euler(0, 180, 0);
            if (direction.magnitude > 0.3f)
            {
                Vector2 velocity = new Vector2(_rigid.linearVelocityX, _rigid.linearVelocityY);
                velocity.x = direction.x * 2f;
                _rigid.linearVelocity = velocity;
            }
            else
            {
                _pathFinder._index++;
            }
        }
    }
    public void JumpCheck()
    {
        if (_jumpPowerCheck[3]._isBlocked)
            _jumpPower = 4;
        else if (_jumpPowerCheck[2]._isBlocked)
            _jumpPower = 3;
        else if (_jumpPowerCheck[1]._isBlocked)
            _jumpPower = 2;
        else if (_jumpPowerCheck[0]._isBlocked)
            _jumpPower = 1;
    }

    public void PathCheck()
    {
        if (_jumpPower == 1)
        {
            _rigid.linearVelocity = new Vector2(_rigid.linearVelocityX, 5.3f);
            _jumpPower = 0;
        }
        else if (_jumpPower == 2)
        {
            int index = _pathFinder._path.FindIndex(item => item.Pos.Equals(new Vector2((int)gameObject.transform.position.x, (int)gameObject.transform.position.y + _jumpPower)));
            if (index != -1)
            {
                _rigid.linearVelocity = new Vector2(_rigid.linearVelocityX, 6.8f);
            }
            _jumpPower = 0;
        }
        else if (_jumpPower == 3)
        {
            int index = _pathFinder._path.FindIndex(item => item.Pos.Equals(new Vector2((int)gameObject.transform.position.x, (int)gameObject.transform.position.y + _jumpPower)));
            if (index != -1)
            {
                _rigid.linearVelocity = new Vector2(_rigid.linearVelocityX, 8.3f);
            }
            _jumpPower = 0;
        }
        else if (_jumpPower == 4)
        {
            int index = _pathFinder._path.FindIndex(item => item.Pos.Equals(new Vector2((int)gameObject.transform.position.x, (int)gameObject.transform.position.y + _jumpPower)));
            if (index != -1)
            {
                _rigid.linearVelocity = new Vector2(_rigid.linearVelocityX, 9.4f);
            }
            _jumpPower = 0;
        }
    }
}
