using UnityEngine;

public class Soldier : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        //게임이 시작되기 전까지는 플레이어와 닿는다 하더라도 사라지거나 점수에 영향을 줘선 안 됩니다.
        if (GameManager.Instance._isStarted && collision.gameObject.layer == 15)
        {
            GameManager.Instance._savedSoldier++;
            this.gameObject.SetActive(false);
        }
    }
}
