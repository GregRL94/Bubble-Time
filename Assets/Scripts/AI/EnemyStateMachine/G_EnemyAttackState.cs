using UnityEngine;

public class G_EnemyAttackState: G_EnemyBaseState
{
    private float _nextAttackTimer;

    public override void EnterState(G_EnemyStateMachine stateMachine)
    {
        Debug.Log("Entering Attack State");
        if (_enemy == null) { _enemy = stateMachine.Enemy; }
        _enemy.Stop();
        _nextAttackTimer = 0f;
    }

    public override void UpdateState(G_EnemyStateMachine stateMachine)
    {
        if (_enemy.isAttacking) { return; }

        if (_nextAttackTimer > 0f) { _nextAttackTimer -= Time.deltaTime; }

        Transform playerTransform;

        if (!_enemy.TryDetectPlayer(out playerTransform))
        {
            // STATE TRANSITION //
            stateMachine.ChangeState(stateMachine.AlertState);
            return;
        }

        Vector2 playerPos = playerTransform.position;
        Vector2 enemyPos = _enemy.transform.position;
        float playerDir = playerPos.x - enemyPos.x;

        if (MathUtils.DistanceX(enemyPos, playerPos) > _enemy.AttackRange || MathUtils.DistanceY(enemyPos, playerPos) > _enemy.Data.AttackVerticalTolerance)
        {
            // STATE TRANSITION //
            stateMachine.ChangeState(stateMachine.ChaseState);
            return;
        }

        _enemy.AnimatorAI.SetTrigger("Alert");
        _enemy.FaceDirection(playerDir);

        if (_nextAttackTimer <= 0f)
        {
            _enemy.TryAttack();
            _nextAttackTimer = 1 / _enemy.Data.AttackSpeed;
        }
    }

    public override void ExitState(G_EnemyStateMachine stateMachine)
    {
        Debug.Log("Exiting Attack State");
    }
}
