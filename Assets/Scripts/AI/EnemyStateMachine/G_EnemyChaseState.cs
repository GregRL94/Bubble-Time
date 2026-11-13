using UnityEngine;

public class G_EnemyChaseState: G_EnemyBaseState
{
    private WaypointsSettings _waypointsSettings;
    private float _minBoundaryX;
    private float _maxBoundaryX;
    private bool _isChaseBoundariesSet;

    public override void EnterState(G_EnemyStateMachine stateMachine)
    {
        Debug.Log("Entering Patrol State");
        if (!_isChaseBoundariesSet)
        {
            _enemy = stateMachine.Enemy;
            _waypointsSettings = _enemy.WaypointsSettings;

            _minBoundaryX = Mathf.Min(_waypointsSettings.Waypoints[0].position.x, _waypointsSettings.Waypoints[1].position.x);
            _maxBoundaryX = Mathf.Max(_waypointsSettings.Waypoints[0].position.x, _waypointsSettings.Waypoints[1].position.x);
            _isChaseBoundariesSet = true;
        }
    }

    public override void UpdateState(G_EnemyStateMachine stateMachine)
    {
        Transform playerTransform;

        if (!_enemy.TryDetectPlayer(out playerTransform))
        {
            stateMachine.ChangeState(stateMachine.AlertState);
            return;
        }

        float playerX = playerTransform.position.x;
        float distanceXToPlayer = Mathf.Abs(playerX - _enemy.transform.position.x);
        float distanceYToPlayer = Mathf.Abs(playerTransform.position.y - _enemy.transform.position.y);

        _enemy.FaceDirection(playerX);

        if (distanceXToPlayer <= _enemy.AttackRange && distanceYToPlayer <= _enemy.Data.AttackVerticalTolerance)
        {
            stateMachine.ChangeState(stateMachine.AttackState);
            return;
        }

        ChasePlayer(playerX);
    }

    public override void ExitState(G_EnemyStateMachine stateMachine)
    {
        Debug.Log("Exiting Chase State");
    }

    private void ChasePlayer(float playerX)
    {
        float clampedPLayerX = Mathf.Clamp(playerX, _minBoundaryX, _maxBoundaryX);

        if (Mathf.Abs(clampedPLayerX - _enemy.transform.position.x) <= _waypointsSettings.WaypointTolerance)
        {
            _enemy.Stop();
            return;
        }
        _enemy.MoveTowardsX(clampedPLayerX, _enemy.Data.ChaseSpeed);
    }
}
