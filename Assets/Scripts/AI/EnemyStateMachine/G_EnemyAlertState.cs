using UnityEngine;

public class G_EnemyAlertState: G_EnemyBaseState
{
    private float _alertTimer;
    private float _nextFlipTime;
    private bool _hasFlipped;

    public override void EnterState(G_EnemyStateMachine stateMachine)
    {
        Debug.Log("Entering Alert State");
        if (_enemy == null) { _enemy = stateMachine.Enemy; }
        _enemy.Stop();
        _alertTimer = _enemy.Data.AlertTime;
        _nextFlipTime = _alertTimer / 2f;
        _hasFlipped = false;

        // STATE TRANSITION //
        _enemy.AnimatorAI.SetTrigger("Alert");
    }

    public override void UpdateState(G_EnemyStateMachine stateMachine)
    {
        _alertTimer -= Time.deltaTime;

        if (stateMachine.Enemy.TryDetectPlayer(out _))
        {
            // STATE TRANSITION //
            stateMachine.ChangeState(stateMachine.ChaseState);
            return;
        }

        if (_alertTimer <= 0f)
        {
            if (stateMachine.Enemy.WaypointsSettings.Waypoints.Length > 0)
            {
                // STATE TRANSITION //
                stateMachine.ChangeState(stateMachine.PatrolState);
            }
            else
            {
                // STATE TRANSITION //
                stateMachine.ChangeState(stateMachine.IdleState);
            }
            return;
        }

        if (_alertTimer <= _nextFlipTime && !_hasFlipped)
        {
            stateMachine.Enemy.FlipDirection();
            _hasFlipped = true;
        }
    }

    public override void ExitState(G_EnemyStateMachine stateMachine)
    {
        Debug.Log("Exiting Alert State");
    }
}
