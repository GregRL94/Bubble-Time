using UnityEngine;

public class G_EnemyIdleState: G_EnemyBaseState
{
    private Vector2 _idlePoint;
    private bool _isIdlePointSet;
    private float _flipTime;
    private float _nextFlipTimer;

    public override void EnterState(G_EnemyStateMachine stateMachine)
    {
        Debug.Log("Entering Idle State");
        if (!_isIdlePointSet)
        {
            _enemy = stateMachine.Enemy;
            _idlePoint = _enemy.transform.position;
            _flipTime = 1f / _enemy.Data.IdleCheckAroundRate;
            _isIdlePointSet = true;
            Debug.Log("New idle point defined: " + _idlePoint);
        }

        if (MathUtils.DistanceX(_enemy.transform.position, _idlePoint) > _enemy.WaypointsSettings.WaypointTolerance)
        {
            Debug.Log(_enemy.name + " is not at idle point. Moving towards idle point.");
            _enemy.MoveTowardsX(_idlePoint.x, _enemy.Data.PatrolSpeed);
            return;
        }

        _enemy.Stop();

        // STATE TRANSITION //
        _enemy.AnimatorAI.SetTrigger("Idle");
        _nextFlipTimer = _flipTime;
    }

    public override void UpdateState(G_EnemyStateMachine stateMachine)
    {
        _nextFlipTimer -= Time.deltaTime;

        if (_enemy.TryDetectPlayer(out _))
        {
            // STATE TRANSITION //
            stateMachine.ChangeState(stateMachine.ChaseState);
            return;
        }

        if (_nextFlipTimer <= 0f)
        {
            Debug.Log(_enemy.name + " Hmmm time to check around !");
            _enemy.FlipDirection();
            _nextFlipTimer = _flipTime;
        }
    }

    public override void ExitState(G_EnemyStateMachine stateMachine)
    {
        Debug.Log("Exiting Idle State");
    }
}
