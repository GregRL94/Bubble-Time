using UnityEngine;

public class Turret : MonoBehaviour, IHit
{
    [SerializeField] private float _health = 40f;
    [SerializeField] private Transform _shootingPoint;
    [SerializeField] private float _offsetAngleDeg = -90f; // Default angle. Math shenanigans.
    [SerializeField] private GameObject _projectilePrefab;
    [SerializeField] private float _fireRate;
    [SerializeField] private float _projectileSpeed;
    [SerializeField] private float _projectileDamage;
    [SerializeField] private float _projectileLifetime;
    [SerializeField] private LayerMask _projectileImpactLayer;
    [SerializeField] private LayerMask _visionObstacleMask;
    private Transform _playerTransform;
    private Animator _animator;
    private float _nextFireTimer;
    private float _currentHealth;
    private bool _isReadyToShoot;

    private void Awake()
    {
        _animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        if (_nextFireTimer > 0)
        {
            _nextFireTimer -= Time.deltaTime;
        }

        if (!_playerTransform) { return; }
        
        if (!LineOfSightCheck()) { return; }

        Aim();
        if (_nextFireTimer <= 0 && _isReadyToShoot)
        {
            Shoot();
            _nextFireTimer = 1f / _fireRate;
        }
    }

    public void Activate()
    {
        _animator.SetTrigger("Activate");
        _playerTransform = GameObject.FindGameObjectWithTag("Player").transform;
    }

    public void Deactivate()
    {
        _isReadyToShoot = false;
        _playerTransform = null;
        _animator.SetTrigger("Deactivate");
    }

    public void SetReady()
    {
        _isReadyToShoot = true;
    }

    public void SetNotReady()
    {
        _isReadyToShoot = false;
    }

    private bool LineOfSightCheck()
    {
        if (_playerTransform == null) { return false; }
        Vector2 direction = (_playerTransform.position - _shootingPoint.position).normalized;
        float distance = Vector2.Distance(_playerTransform.position, _shootingPoint.position);
        RaycastHit2D hit = Physics2D.Raycast(_shootingPoint.position, direction, distance, _visionObstacleMask);
        if (hit.collider != null)
        {
            Debug.DrawLine(transform.position, _playerTransform.position, Color.red);
            return false;
        }
        Debug.DrawLine(transform.position, _playerTransform.position, Color.green);
        return true;
    }

    private void Aim()
    {
        if (_playerTransform != null)
        {
            Vector2 aimDir = _playerTransform.position - _shootingPoint.position;
            _shootingPoint.eulerAngles = new Vector3(0f, 0f, MathUtils.DirToAngleRad(aimDir.x, aimDir.y, _offsetAngleDeg));
        }
    }

    private void Shoot()
    {
        _animator.SetTrigger("Shoot");
    }

    public void InstantiateProjectile()
    {
        GameObject projectile = Instantiate(_projectilePrefab, _shootingPoint.position, _shootingPoint.rotation);
        if (projectile.TryGetComponent(out IProjectile projectileComponent))
        {
            projectileComponent.SetupProjectile(_projectileSpeed, _projectileDamage, _projectileLifetime, _projectileImpactLayer);
            projectile.transform.SetParent(null);
        }
    }

    private void TakeDamage(float damage)
    {
        _currentHealth -= damage;
        if (_currentHealth <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        _animator.SetTrigger("Die");
    }

    public void _Destroy()
    {
        Destroy(gameObject);
    }

    public void OnHit(float damage)
    {
        TakeDamage(damage);
    }
}
