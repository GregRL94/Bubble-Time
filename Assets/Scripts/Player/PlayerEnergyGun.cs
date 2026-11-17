using UnityEngine;

/* TODO: Make this script inherit from a base weapon class */
public class PlayerEnergyGun : MonoBehaviour, IPlayerActionComponent
{
    #region Attributes
    [Header("Gun GeneralSettings")]
    [SerializeField, Tooltip("The prefab for the projectile")] private GameObject _projectilePrefab;
    [SerializeField, Tooltip("The point from which the projectile will be fired")] private Transform _shootingPoint;
    public Transform ShootingPoint => _shootingPoint;
    [SerializeField, Range(1, 20), Tooltip("Projectiles per second")] private int _fireRate = 5;

    [Header("Projectile Settings")]
    [SerializeField, Tooltip("The speed of the projectile")] private float _projectileSpeed = 10f;
    [SerializeField, Tooltip("The damage dealt by the projectile")] private float _projectileDamage = 10f;
    [SerializeField, Tooltip("The lifetime of the projectile")] private float _projectileLifetime = 5f;
    [SerializeField, Tooltip("The layer mask for the projectile's impact")] private LayerMask _projectileImpactLayerMask;

    [Header("Gun Dispersion Settings")]
    [SerializeField] private bool _angularDispersion;
    [SerializeField, Range(0f, 15f), Tooltip("Maximum angular dispersion in degrees")] private float _maxAngularDispersion = 2.5f;
    [SerializeField, Range(0f, 5f), Tooltip("Angular dispersion increase per shot")] private float _angularDispersionIncrease = 0.5f;
    [SerializeField, Range(0f, 5f), Tooltip("Angular dispersion decrease per second")] private float _angularDispersionDecrease = 1f;

    private PlayerActionsController _actionsController;
    private float _timeBetweenShots;
    private float _timeSinceLastShot;
    private float _currentAngularDispersion;
    private bool _shootRequested;
    private bool _canShoot;
    #endregion Attributes

    #region MonoBehaviour Methods
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _timeBetweenShots = 1f / _fireRate;
        _timeSinceLastShot = _timeBetweenShots;
        _canShoot = true;
    }

    // Update is called once per frame
    void Update()
    {
        UpdateTimers();
        UpdateFlags();
        Shoot();
        UpdateDispersion();
    }
    #endregion MonoBehaviour Methods

    #region Methods
    private void Shoot()
    {
        if (_shootRequested && _canShoot)
        {
            if (_actionsController.PlayerAnimator) 
            {
                _actionsController.PlayerAnimator.SetTrigger("Shoot");
            }            
            _timeSinceLastShot = 0f;
            _canShoot = false;
        }
    }

    public void InstantiateProjectile() 
    {
        ApplyDispersion();
        GameObject projectile = Instantiate(_projectilePrefab, _shootingPoint.position, _shootingPoint.rotation);
        projectile.transform.SetParent(null);
        projectile.GetComponent<IProjectile>().SetupProjectile(_projectileSpeed, _projectileDamage, _projectileLifetime, _projectileImpactLayerMask);
        IncreaseDispersion();
    }
    #endregion Methods

    #region Update Methods
    private void UpdateTimers()
    {
        if (_timeSinceLastShot < _timeBetweenShots)
        {
            _timeSinceLastShot += Time.deltaTime;
        }
    }

    private void UpdateFlags()
    {
        if (!_canShoot && _timeSinceLastShot >= _timeBetweenShots)
        {
            _canShoot = true;
        }
    }

    private void UpdateDispersion()
    {
        if (_timeSinceLastShot >= _timeBetweenShots)
        {
            DecreaseDispersion();
        }
    }
    #endregion Update Methods

    #region Dispersion Methods
    private void IncreaseDispersion()
    {
        if (_angularDispersion)
        {
            _currentAngularDispersion += _angularDispersionIncrease;
            _currentAngularDispersion = Mathf.Clamp(_currentAngularDispersion, 0f, _maxAngularDispersion);
        }
    }

    private void DecreaseDispersion()
    {
        if (_angularDispersion && _currentAngularDispersion > 0f)
        {
            _currentAngularDispersion = Mathf.Lerp(_currentAngularDispersion, 0f, Time.deltaTime * _angularDispersionDecrease);
        }
    }

    private void ApplyDispersion()
    {
        if (_angularDispersion && _currentAngularDispersion > 0f)
        {
            float randomAngle = Random.Range(-_currentAngularDispersion, _currentAngularDispersion);
            _shootingPoint.transform.Rotate(0f, 0f, randomAngle);
        }
    }
    #endregion Dispersion Methods

    #region IPlayerActionComponent Implementation
    public void Initialize(PlayerActionsController actionsController)
    {
        _actionsController = actionsController;
    }
    #endregion IPlayerActionComponent Implementation

    #region Input Event Hooks
    public void StartShooting()
    {
        _shootRequested = true;
        Shoot();
    }

    public void StopShooting()
    {
        _shootRequested = false;
    }
    #endregion Input Event Hooks
}
