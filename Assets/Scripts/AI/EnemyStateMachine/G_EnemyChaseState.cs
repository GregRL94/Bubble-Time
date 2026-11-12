using UnityEngine;

public class G_EnemyChaseState: G_EnemyBaseState
{
    public override void EnterState(G_EnemyStateMachine stateMachine)
    {
        _enemy = stateMachine.Enemy;
    }

    public override void UpdateState(G_EnemyStateMachine stateMachine)
    {
        Transform playerTransform;
        if (_enemy.TryDetectPlayer(out playerTransform))
        {
            float playerX = playerTransform.position.x;
            float distanceXToPlayer = Mathf.Abs(playerX - _enemy.transform.position.x);
            float distanceYToPlayer = Mathf.Abs(playerTransform.position.y - _enemy.transform.position.y);

            _enemy.FaceDirection(playerX);

            if (distanceXToPlayer <= _enemy.Data.AttackRange && distanceYToPlayer <= _enemy.Data.AttackRange)
            {
                stateMachine.ChangeState(stateMachine.AttackState);
                return;
            }

            // TODO dont chase beyond a certain distance from the last waypoint, or the player will just run away and the enemy will chase forever
            _enemy.MoveTowardsX(playerTransform.position.x, _enemy.Data.ChaseSpeed);
        }
        else
        {
            stateMachine.ChangeState(stateMachine.PatrolState);
        }
    }

    public override void ExitState(G_EnemyStateMachine stateMachine)
    {
        // Implementation for exiting chase state
    }
}
