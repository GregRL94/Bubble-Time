/// <summary>
/// Defines the base class for enemy states.
/// </summary>
/// <remarks>Provides lifecycle methods for entering, updating, and exiting an enemy state.</remarks>
public abstract class G_EnemyBaseState
{
    protected GroundEnemy2DBase _enemy;
    public abstract void EnterState(G_EnemyStateMachine stateMachine);
    public abstract void UpdateState(G_EnemyStateMachine stateMachine);
    public abstract void ExitState(G_EnemyStateMachine stateMachine);
}
