using UnityEngine;

#region Settings Classes
/// <summary>
/// Configures waypoint navigation behavior.
/// </summary>
/// <remarks>Defines the patrol waypoints, waypoint arrival tolerance, and pause duration between
/// waypoints.</remarks>
[System.Serializable]
public class WaypointsSettings
{
    [field: SerializeField, Tooltip("The patrol waypoints for the enemy.")] public Transform[] Waypoints { get; protected set; }
    [field: SerializeField, Tooltip("How close the enemy needs to be to a waypoint to consider it reached.")] public float WaypointTolerance { get; private set; }
    [field: SerializeField, Tooltip("Pause after reaching a waypoint.")] public float WaypointPauseTime { get; private set; }
}

/// <summary>
/// Provides settings for enemy detection.
/// </summary>
[System.Serializable]
public class DetectionSettings
{
    [field: SerializeField, Tooltip("The center of the detection zone.")] public Transform DetectionCenter { get; private set; }
    [field: SerializeField, Tooltip("The detection zone readius.")] public float DetectionRadius { get; private set; }
    [field: SerializeField, Tooltip("The layer of the player.")] public LayerMask PlayerLayer { get; private set; }
    [field: SerializeField, Tooltip("Objects on these layers will block the enemy's vision.")] public LayerMask VisionObstacleMask { get; private set; }
}

[System.Serializable]
public class AttackSettings
{
    [field: SerializeField, Tooltip("The point from which the attack is performed.")] public Transform AttackPoint { get; private set; }
    [field: SerializeField, Tooltip("The layers that the attack can affect.")] public LayerMask AttacksAffectWhat { get; private set; }
}
#endregion Settings Classes

/// <summary>
/// Provides base behavior for a 2D ground enemy.
/// </summary>
/// <remarks>Manages enemy health, movement, detection, combat, facing direction, and state machine
/// updates.</remarks>
public class GroundEnemy2DBase : MonoBehaviour, IHit
{
    #region Attributes
    [field: SerializeField, Tooltip("The enemy configuration data.")] public EnemyData Data { get; protected set; }
    public WaypointsSettings WaypointsSettings = new WaypointsSettings();
    public DetectionSettings DetectionSettings = new DetectionSettings();
    public AttackSettings AttackSettings = new AttackSettings();

    public float CurrentHealth { get; protected set; }
    public bool isAttacking { get; protected set; }
    public bool isDead { get; protected set; }

    protected G_EnemyStateMachine _stateMachine { get; set; }
    protected Rigidbody2D _rb2d;
    protected Collider2D _collider2d;
    public Animator AnimatorAI { get; protected set; }
    protected Transform _player;
    protected float _nextAttackTimer;
    public float AttackRange { get { return Mathf.Abs(AttackSettings.AttackPoint.localPosition.x); } }
    protected float _targetVelocityX;
    protected int _facing;
    #endregion Attributes

    #region Monobehaviour Methods
    private void Awake()
    {
        _rb2d = GetComponent<Rigidbody2D>();
        _collider2d = GetComponent<Collider2D>();
        AnimatorAI = GetComponent<Animator>();
    }

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

    void FixedUpdate()
    {
        if (isDead) { return; }
        _rb2d.linearVelocity = new Vector2(_targetVelocityX, _rb2d.linearVelocityY);
    }
    #endregion Monobehaviour Methods

    #region Detection Methods
    /// <summary>
    /// Detects the player within the detection area and verifies line of sight.
    /// </summary>
    /// <param name="playerTransform">When this method returns, contains the detected player's transform if a player is detected and visible;
    /// otherwise, `null`.</param>
    /// <returns>`true` if a player is detected and has line of sight; otherwise, `false`.</returns>
    public bool TryDetectPlayer(out Transform playerTransform)
    {
        playerTransform = null;

        Collider2D hit = Physics2D.OverlapCircle(DetectionSettings.DetectionCenter.position, DetectionSettings.DetectionRadius, DetectionSettings.PlayerLayer);
        if (hit != null)
        {
            return LineOfSightCheck(hit.transform, out playerTransform);
        }
        return false;
    }

    /// <summary>
    /// Determines whether an unobstructed line of sight exists between the current transform and the target.
    /// </summary>
    /// <param name="target">The target transform to check.</param>
    /// <param name="playerTransform">When the target is visible, receives the target transform; otherwise, receives null.</param>
    /// <returns>true if no vision obstacle blocks the target; otherwise, false.</returns>
    protected bool LineOfSightCheck(Transform target, out Transform playerTransform)
    {
        Vector2 directionToTarget = (target.position - transform.position).normalized;
        float distanceToTarget = Vector2.Distance(transform.position, target.position);

        RaycastHit2D hit = Physics2D.Raycast(transform.position, directionToTarget, distanceToTarget, DetectionSettings.VisionObstacleMask);
        if (hit.collider != null)
        {
            // There is an obstacle in the way
            Debug.DrawLine(transform.position, target.position, Color.red);
            playerTransform = null;
            return false;
        }
        Debug.DrawLine(transform.position, target.position, Color.green);
        playerTransform = target;
        return true;
    }
    #endregion Detection Methods

    #region Movement Methods
    /// <summary>
    /// Stops horizontal movement.
    /// </summary>
    public void Stop()
    {
        _targetVelocityX = 0f;
    }

    /// <summary>
    /// Sets horizontal movement toward the target x-coordinate at the specified speed and faces the movement direction.
    /// </summary>
    /// <param name="targetX">The target x-coordinate.</param>
    /// <param name="speed">The horizontal movement speed.</param>
    public void MoveTowardsX(float targetX, float speed)
    {
        float dir = Mathf.Sign(targetX - transform.position.x);
        _targetVelocityX = dir * speed;
        FaceDirection(dir);
    }

    /// <summary>
    /// Updates the object's horizontal facing direction.
    /// </summary>
    /// <param name="dirX">The horizontal direction, where positive values face right and negative values face left.</param>
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

    /// <summary>
    /// Reverses the facing direction and flips the sign of the local X scale.
    /// </summary>
    public void FlipDirection()
    {
        _facing *= -1;
        Vector3 newLocalScale = transform.localScale;
        newLocalScale.x = Mathf.Abs(newLocalScale.x) * _facing;
        transform.localScale = newLocalScale;
    }
    #endregion Movement Methods

    #region Combat Methods
    public virtual void TryAttack()
    {
        Debug.Log("Starting Attack !");
        AnimatorAI.SetTrigger("Attack");
        isAttacking = true;
    }

    public virtual void PerformAttack()
    {
        isAttacking = false;
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
        _collider2d.enabled = false;
        _rb2d.gravityScale = 0f;
        _stateMachine.ChangeState(_stateMachine.DeadState);
        Debug.Log(gameObject.name + " has died.");
    }
    #endregion Combat Methods

    #region Utility Methods
    public virtual void _Destroy()
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

    protected virtual void OnDrawGizmos()
    {
        if (DetectionSettings.DetectionCenter != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(DetectionSettings.DetectionCenter.position, DetectionSettings.DetectionRadius);
        }
    }
    #endregion Utility Methods

    #region IHit Implementation
    public void OnHit(float damage)
    {
        TakeDamage(damage);
    }
    #endregion IHit Implementation
}
