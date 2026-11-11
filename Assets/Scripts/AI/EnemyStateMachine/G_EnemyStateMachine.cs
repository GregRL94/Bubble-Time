public class G_EnemyStateMachine
{
    public G_EnemyIdleState IdleState { get; private set; }
    public G_EnemyPatrolState PatrolState { get; private set; }
    public G_EnemyChaseState ChaseState { get; private set; }
    public G_EnemyAttackState AttackState { get; private set; }
    public GroundEnemy2DBase Enemy { get; private set; }
    G_EnemyBaseState _currentState;


    public G_EnemyStateMachine(GroundEnemy2DBase enemy)
    {
        IdleState = new G_EnemyIdleState();
        PatrolState = new G_EnemyPatrolState();
        ChaseState = new G_EnemyChaseState();
        AttackState = new G_EnemyAttackState();
        Enemy = enemy;

        _currentState = Enemy.Waypoints.Length > 0 ? PatrolState : IdleState;
        _currentState.EnterState(this);
    }

    public void UpdateStateMachine()
    {
        _currentState.UpdateState(this);
    }

    public void ChangeState(G_EnemyBaseState newState)
    {
        _currentState.ExitState(this);
        _currentState = newState;
        _currentState.EnterState(this);
    }
}
