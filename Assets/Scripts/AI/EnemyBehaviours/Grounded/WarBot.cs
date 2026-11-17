using UnityEngine;

public class WarBot : GroundEnemy2DBase
{
    public float AttackRadius = 0.5f;

    public override void PerformAttack()
    {
        base.PerformAttack();
        Debug.Log("Dealing Damage !");
        foreach (Collider2D hit in Physics2D.OverlapCircleAll(AttackSettings.AttackPoint.position, AttackRadius, AttackSettings.AttacksAffectWhat))
        {
            if (hit.TryGetComponent<IHit>(out IHit hittable))
            {
                hittable.OnHit(Data.Damage);
            }
        }
    }

    protected override void CreateStateMachine()
    {
        _stateMachine = new G_EnemyStateMachine(this);
    }
}
