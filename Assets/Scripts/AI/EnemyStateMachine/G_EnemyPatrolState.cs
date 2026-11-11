using UnityEngine;

public class G_EnemyPatrolState: G_EnemyBaseState
{
    private GroundEnemy2DBase _enemy;
    private float _patrolWaypointPauseTime;
    private int _lastWaypointIndex;
    private int _currentWaypointIndex;

    public override void EnterState(G_EnemyStateMachine stateMachine)
    {
        Debug.Log("Entering Patrol State");
        _enemy = stateMachine.Enemy;
        _currentWaypointIndex = _lastWaypointIndex;
        MoveToNextWaypoint();
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

        MoveToNextWaypoint();
    }

    public override void ExitState(G_EnemyStateMachine stateMachine)
    {
        Debug.Log("Exiting Patrol State");
    }

    private void MoveToNextWaypoint()
    {
        Vector3 enemyAIPos = _enemy.transform.position;
        Vector3 currentWaypointPos = _enemy.Waypoints[_currentWaypointIndex].position;
        float tolerance = _enemy.WaypointTolerance;

        // Check if the current waypoint has been reached
        if (Vector3.Distance(enemyAIPos, currentWaypointPos) <= tolerance)
        {
            Debug.Log("Waypoint " + _currentWaypointIndex + " reached.");
            _patrolWaypointPauseTime = _enemy.WaypointPauseTime;
            _lastWaypointIndex = _currentWaypointIndex; // Update the last waypoint index to the current one
            _currentWaypointIndex++;
            if (_currentWaypointIndex >= _enemy.Waypoints.Length)
            {
                _currentWaypointIndex = 0;
            }
            _enemy.MoveTowardsX(_enemy.Waypoints[_currentWaypointIndex].position.x, _enemy.Data.PatrolSpeed);
        }
    }
}
