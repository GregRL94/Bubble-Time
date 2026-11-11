public abstract class G_EnemyBaseState
{
    public abstract void EnterState(G_EnemyStateMachine stateMachine);
    public abstract void UpdateState(G_EnemyStateMachine stateMachine);
    public abstract void ExitState(G_EnemyStateMachine stateMachine);
}
