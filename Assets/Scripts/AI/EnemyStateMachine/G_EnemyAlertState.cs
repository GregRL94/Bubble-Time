using UnityEngine;

public class G_EnemyAlertState: G_EnemyBaseState
{
    private float _alertTimer;
    private float _nextFlipTime;
    private bool _isAlertTimerSet;

    public override void EnterState(G_EnemyStateMachine stateMachine)
    {
        Debug.Log("Entering Alert State");
        if (!_isAlertTimerSet)
        {
            _enemy = stateMachine.Enemy;
            _alertTimer = _enemy.Data.AlertTime;
            _nextFlipTime = _alertTimer / 2f;
            _isAlertTimerSet = true;
        }
        _enemy.Stop();
    }

    public override void UpdateState(G_EnemyStateMachine stateMachine)
    {
        _alertTimer -= Time.deltaTime;

        if (stateMachine.Enemy.TryDetectPlayer(out _))
        {
            stateMachine.ChangeState(stateMachine.ChaseState);
            return;
        }

        if (_alertTimer <= 0f)
        {
            if (stateMachine.Enemy.WaypointsSettings.Waypoints.Length > 0)
            {
                stateMachine.ChangeState(stateMachine.PatrolState);
            }
            else
            {
                stateMachine.ChangeState(stateMachine.IdleState);
            }
            return;
        }

        if (_alertTimer <= _nextFlipTime)
        {
            stateMachine.Enemy.FlipDirection();
        }
    }

    public override void ExitState(G_EnemyStateMachine stateMachine)
    {
        Debug.Log("Exiting Alert State");
    }
}
