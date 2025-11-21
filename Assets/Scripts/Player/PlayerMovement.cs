using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(BoxCollider2D))]
public class PlayerMovement : MonoBehaviour
{
    [SerializeField] GroundChecker _groundChecker;
    
    public float _moveSpeed;
    public float _jumpPower;
    public bool _isGrounded;

    private Vector2 _moveInput;
    private Rigidbody2D _rigid;


    private void Awake()
    {
        _rigid = GetComponent<Rigidbody2D>();
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    void FixedUpdate()
    {
        Vector2 velocity = _rigid.linearVelocity;
        velocity.x = _moveInput.x * _moveSpeed;
        if (_moveInput.x > 0)
            gameObject.transform.rotation = Quaternion.Euler(0, 0, 0);
        else if (_moveInput.x < 0)
            gameObject.transform.rotation = Quaternion.Euler(0, 180, 0);
        _rigid.linearVelocity = velocity;
    }

    public void OnMove(InputAction.CallbackContext ctx)
    {
            _moveInput = ctx.ReadValue<Vector2>();
    }

    public void OnJump(InputAction.CallbackContext ctx)
    {
        if (ctx.started && _isGrounded == true)
        {
            _rigid.linearVelocity = new Vector2(0, _jumpPower);
            _groundChecker._coyoteTime = -0.1f;

        }
    }
}
