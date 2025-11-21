using UnityEngine;

public class GroundChecker : MonoBehaviour
{
    [SerializeField] PlayerMovement player;

    public float _coyoteTime;

    private Collider2D _collider;
    private bool _isFloat;

    
    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.gameObject.layer == 6)
        {
            _isFloat = false;
            player._isGrounded = true;
            _coyoteTime = 0.2f;
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.layer == 6)
        {
            _isFloat = true;
        }
    }
    private void Update()
    {
        if (_isFloat == true)
        {
            _coyoteTime -= Time.deltaTime;
            if (_coyoteTime < 0)
            {
                player._isGrounded = false;
            }
        }
    }
}
