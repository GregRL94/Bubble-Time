using UnityEngine;

public class G_EnemyPatrolState: G_EnemyBaseState
{
    private WaypointsSettings _waypointsSettings;
    private float _patrolWaypointPauseTime;
    private float _minX, _maxX;
    private float _patrolTargetX;
    private bool _isPatrolSet;

    public override void EnterState(G_EnemyStateMachine stateMachine)
    {
        Debug.Log("Entering Patrol State");
        if (!_isPatrolSet)
        {
            _enemy = stateMachine.Enemy;
            _waypointsSettings = _enemy.WaypointsSettings;
            _minX = Mathf.Min(_waypointsSettings.Waypoints[0].position.x, _waypointsSettings.Waypoints[1].position.x);
            _maxX = Mathf.Max(_waypointsSettings.Waypoints[0].position.x, _waypointsSettings.Waypoints[1].position.x);

            float closestX = Mathf.Abs(_enemy.transform.position.x - _minX) < Mathf.Abs(_enemy.transform.position.x - _maxX) ? _minX : _maxX;
            _patrolTargetX = closestX;
            _isPatrolSet = true;
        }

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
        float tolerance = _waypointsSettings.WaypointTolerance;

        // Check if the current waypoint has been reached
        if (distanceX <= tolerance)
        {
            Debug.Log("Checkoint reached. Pausing for " + _waypointsSettings.WaypointPauseTime + " seconds.");
            _patrolTargetX = Mathf.Approximately(_patrolTargetX, _maxX) ? _minX : _maxX;
            _patrolWaypointPauseTime = _waypointsSettings.WaypointPauseTime;
            _enemy.Stop();
            return;
        }
        _enemy.MoveTowardsX(_patrolTargetX, _enemy.Data.PatrolSpeed);
    }
}
