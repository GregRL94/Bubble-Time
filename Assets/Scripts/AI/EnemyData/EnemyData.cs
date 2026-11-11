using UnityEngine;

[CreateAssetMenu(fileName = "EnemyData", menuName = "Scriptable Objects/Enemy/EnemyData")]
public class EnemyData : ScriptableObject
{
    [field: SerializeField] public float MaxHealth { get; private set; }
    [field: SerializeField] public float BaseSpeed { get; private set; }
    [field: SerializeField] public float PatrolSpeed { get; private set; }
    [field: SerializeField] public float ChaseSpeed { get; private set; }
    [field: SerializeField] public float AttackRange { get; private set; }
    [field: SerializeField] public float AttackVerticalTolerance { get; private set; }
    [field: SerializeField] public float AttackSpeed { get; private set; }
    [field: SerializeField] public LayerMask PlayerLayer { get; private set; }
    [field: SerializeField] public float Damage { get; private set; }
    [field: SerializeField] public float DetectionRadius { get; private set; }
    [field: SerializeField] public LayerMask VisionObstacleMask { get; private set; }
}
