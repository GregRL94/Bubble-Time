using UnityEngine;

public class G_EnemyDeadState: G_EnemyBaseState
{
    public override void EnterState(G_EnemyStateMachine stateMachine)
    {
        _enemy = stateMachine.Enemy;
        _enemy.AnimatorAI.SetTrigger("Die");
    }

    public override void UpdateState(G_EnemyStateMachine stateMachine)
    {
        //if (_enemy.AnimatorAI.GetCurrentAnimatorStateInfo(0).IsName("death") &&
        //    _enemy.AnimatorAI.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1f)
        //{
        //    _enemy._Destroy();
        //}
    }

    public override void ExitState(G_EnemyStateMachine stateMachine)
    {
        // No exit logic needed for the dead state
    }
}
