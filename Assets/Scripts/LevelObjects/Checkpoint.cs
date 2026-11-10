using UnityEngine;

public class Checkpoint : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        LevelManager.Instance.NextCheckpoint();
        gameObject.SetActive(false);
    }
}
