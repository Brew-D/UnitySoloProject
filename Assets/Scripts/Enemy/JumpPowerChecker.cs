using UnityEngine;

public class JumpPowerChecker : MonoBehaviour
{
    public bool _isBlocked = false;
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision != null && collision.gameObject.layer == 6)
                _isBlocked = true;
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if(collision != null && collision.gameObject.layer == 6)
                _isBlocked = false;
    }
}
