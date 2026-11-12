using UnityEngine;

public class G_EnemyPatrolState: G_EnemyBaseState
{
    private float _patrolWaypointPauseTime;
    private float _minX, _maxX;
    private float _patrolTargetX;

    public override void EnterState(G_EnemyStateMachine stateMachine)
    {
        Debug.Log("Entering Patrol State");
        _enemy = stateMachine.Enemy;
        _minX = Mathf.Min(_enemy.Waypoints[0].position.x, _enemy.Waypoints[1].position.x);
        _maxX = Mathf.Max(_enemy.Waypoints[0].position.x, _enemy.Waypoints[1].position.x);
        _patrolTargetX = _maxX;
        _enemy.MoveTowardsX(_patrolTargetX, _enemy.Data.PatrolSpeed);
    }

    public override void UpdateState(G_EnemyStateMachine stateMachine)
    {
        if (_enemy.TryDetectPlayer(out _))
        {
            stateMachine.ChangeState(stateMachine.ChaseState);
            return;
        }

        if (_patrolWaypointPauseTime > 0f)
        {
            _patrolWaypointPauseTime -= Time.deltaTime;
            _enemy.Stop();
            return;
        }

        Patrol();
    }

    public override void ExitState(G_EnemyStateMachine stateMachine)
    {
        Debug.Log("Exiting Patrol State");
    }

    private void Patrol()
    {
        float enemyAIPosX = _enemy.transform.position.x;
        float distanceX = Mathf.Abs(_patrolTargetX - enemyAIPosX);
        float tolerance = _enemy.WaypointTolerance;

        // Check if the current waypoint has been reached
        if (distanceX <= tolerance)
        {
            Debug.Log("Checkoint reached. Pausing for " + _enemy.WaypointPauseTime + " seconds.");
            _patrolTargetX = Mathf.Approximately(_patrolTargetX, _maxX) ? _minX : _maxX;
            _patrolWaypointPauseTime = _enemy.WaypointPauseTime;
            _enemy.Stop();
            return;
        }
        _enemy.MoveTowardsX(_patrolTargetX, _enemy.Data.PatrolSpeed);
    }
}
