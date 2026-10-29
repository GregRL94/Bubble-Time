using UnityEngine;

public class PlayerDash : MonoBehaviour
{
    [field: SerializeField, Tooltip("The factor by which the player's speed increases when dashing.")] public float DashSpeedFactor { get; private set; } = 3.0f;
    [field: SerializeField, Tooltip("The duration of the dash.")] public float DashDuration { get; private set; } = 0.5f;
    [field: SerializeField, Tooltip("The cooldown period after the dash.")] public float DashCooldown { get; private set; } = 2.0f;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
