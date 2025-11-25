using UnityEngine;

public class AttackCollider : MonoBehaviour
{

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision != null && collision.gameObject.layer == 14)
        {
            EnemyStatePattern enemy = collision.GetComponent<EnemyStatePattern>();
            enemy.GetHit();
            Debug.Log("타격 성공");
        }
    }
}
