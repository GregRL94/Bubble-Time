using UnityEngine;

public class GroundEnemy2DBase : MonoBehaviour, IHit
{
    [field: SerializeField, Tooltip("The enemy configuration data.")] public EnemyData Data { get; protected set; }
    [field: SerializeField, Tooltip("The patrol waypoints for the enemy.")] public Transform[] Waypoints { get; set; }
    [field: SerializeField, Tooltip("The tolerance for reaching waypoints.")] public float WaypointTolerance { get; private set; }
    [field: SerializeField, Tooltip("Pause after reaching a waypoint.")] public float WaypointPauseTime { get; private set; }

    public float CurrentHealth { get; protected set; }
    public bool isAttacking { get; protected set; }
    public bool isDead { get; protected set; }
    protected G_EnemyStateMachine _stateMachine { get; private set; }
    protected Rigidbody2D _rb2d;
    protected Animator _animator;
    protected Transform _player;
    protected float _nextAttackTimer;
    protected float _targetVelocityX;
    protected int _facing;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        CurrentHealth = Data.MaxHealth;
        CreateStateMachine();
    }

    // Update is called once per frame
    void Update()
    {
        UpdateTimers();
        _stateMachine.UpdateStateMachine();
    }

    public bool TryDetectPlayer(out Transform playerTransform)
    {
        playerTransform = null;

        Collider2D hit = Physics2D.OverlapCircle(transform.position, Data.DetectionRadius, Data.PlayerLayer);
        if (hit != null)
        {
            return LineOfSightCheck(hit.transform, out playerTransform);
        }
        return false;
    }

    protected bool LineOfSightCheck(Transform target, out Transform playerTransform)
    {
        Vector2 directionToTarget = (target.position - transform.position).normalized;
        float distanceToTarget = Vector2.Distance(transform.position, target.position);

        RaycastHit2D hit = Physics2D.Raycast(transform.position, directionToTarget, distanceToTarget, Data.VisionObstacleMask);
        if (hit.collider != null)
        {
            // There is an obstacle in the way
            playerTransform = null;
            return false;
        }
        playerTransform = target;
        return true;
    }

    public void Stop()
    {
        _targetVelocityX = 0f;
    }

    public void MoveTowardsX(float targetX, float speed)
    {
        float dir = Mathf.Sign(targetX - transform.position.x);
        _targetVelocityX = dir * speed;
        FaceDirection(dir);
    }

    public void FaceDirection(float dirX)
    {
        if (Mathf.Abs(dirX) < 0.01f) { return; }

        int newFacing = dirX > 0 ? 1 : -1;
        if (newFacing == _facing) { return; }

        _facing = newFacing;
        Vector3 newLocalScale = transform.localScale;
        newLocalScale.x = Mathf.Abs(newLocalScale.x) * _facing;
        transform.localScale = newLocalScale;
    }

    public virtual void TryAttack()
    {
        if (isAttacking || isDead) { return; }
        if (_nextAttackTimer <= 0f)
        {
            Attack();
            _nextAttackTimer = 1 / Data.AttackSpeed;
        }
    }

    protected virtual void Attack()
    {
        isAttacking = true;
        Debug.Log("Attacking Player");
    }

    protected virtual void TakeDamage(float damage)
    {
        if (isDead) { return; }
        CurrentHealth -= damage;
        CurrentHealth = Mathf.Clamp(CurrentHealth, 0f, Data.MaxHealth);
        Debug.Log(gameObject.name + " took " + damage + " damage. Current health: " + CurrentHealth);
        if (CurrentHealth <= 0)
        {
            Die();
        }
    }

    protected virtual void Die()
    {
        isDead = true;
        _targetVelocityX = 0f;
        _rb2d.linearVelocity = new Vector2(0f, _rb2d.linearVelocityY);
        Debug.Log(gameObject.name + " has died.");
        _Destroy();
    }

    protected virtual void _Destroy()
    {
        Destroy(gameObject);
    }

    protected virtual void CreateStateMachine()
    {
        _stateMachine = new G_EnemyStateMachine(this);
    }

    protected virtual void UpdateTimers()
    {
        if (_nextAttackTimer > 0f)
        {
            _nextAttackTimer -= Time.deltaTime;
        }
    }

    public void OnHit(float damage)
    {
        TakeDamage(damage);
    }

    public void OnHit() { }
}
