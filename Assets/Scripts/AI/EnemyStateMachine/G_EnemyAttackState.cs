using UnityEngine;

public class G_EnemyAttackState: G_EnemyBaseState
{
    public override void EnterState(G_EnemyStateMachine stateMachine)
    {
        Debug.Log("Entering Attack State");
        _enemy = stateMachine.Enemy;
        _enemy.Stop();
    }

    public override void UpdateState(G_EnemyStateMachine stateMachine)
    {
        Transform playerTransform;

        if (!_enemy.TryDetectPlayer(out playerTransform))
        {
            stateMachine.ChangeState(stateMachine.AlertState);
            return;
        }

        Vector2 playerPos = playerTransform.position;
        Vector2 enemyPos = _enemy.transform.position;
        float playerDir = playerPos.x - enemyPos.x;

        if (MathUtils.DistanceX(enemyPos, playerPos) > _enemy.Data.AttackRange || MathUtils.DistanceY(enemyPos, playerPos) > _enemy.Data.AttackVerticalTolerance)
        {
            stateMachine.ChangeState(stateMachine.ChaseState);
            return;
        }

        _enemy.FaceDirection(playerDir);
    }

    public override void ExitState(G_EnemyStateMachine stateMachine)
    {
        // Implementation for exiting attack state
    }
}
