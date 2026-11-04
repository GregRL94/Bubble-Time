using UnityEngine;

public class PlayerEnergyGun : MonoBehaviour, IPlayerActionComponent
{
    #region Attributes
    [Header("Gun GeneralSettings")]
    [SerializeField, Tooltip("The prefab for the projectile")] private GameObject _projectilePrefab;
    [SerializeField, Tooltip("The point from which the projectile will be fired")] private Transform _shootingPoint;
    public Transform ShootingPoint => _shootingPoint;
    [SerializeField, Range(1, 20), Tooltip("Projectiles per second")] private int _fireRate = 5;

    [Header("Gun Dispersion Settings")]
    [SerializeField] private bool _angularDispersion;
    [SerializeField, Range(0f, 15f), Tooltip("Maximum angular dispersion in degrees")] private float _maxAngularDispersion = 2.5f;
    [SerializeField, Range(0f, 5f), Tooltip("Angular dispersion increase per shot")] private float _angularDispersionIncrease = 0.5f;
    [SerializeField, Range(0f, 5f), Tooltip("Angular dispersion decrease per second")] private float _angularDispersionDecrease = 1f;

    private PlayerActionsController _actionsController;
    private float _timeBetweenShots;
    private float _timeSinceLastShot;
    private float _currentAngularDispersion;
    private bool _canShoot = true;
    #endregion Attributes

    #region MonoBehaviour Methods
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _timeBetweenShots = 1f / _fireRate;
        _timeSinceLastShot = _timeBetweenShots; // Allow immediate shooting at the start
    }

    // Update is called once per frame
    void Update()
    {
        if (_timeSinceLastShot < _timeBetweenShots)
        {
            _timeSinceLastShot += Time.deltaTime;
        }
        if (!_canShoot && _timeSinceLastShot >= _timeBetweenShots)
        {
            _canShoot = true;
        }
        if (_timeSinceLastShot >= _timeBetweenShots)
        {
            DecreaseDispersion();
        }
    }
    #endregion MonoBehaviour Methods

    #region Methods
    private void Shoot()
    {
        if (_canShoot)
        {
            ApplyDispersion();
            Instantiate(_projectilePrefab, transform.position, transform.rotation);
            IncreaseDispersion();
            _canShoot = false;
        }
    }
    #endregion Methods

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
}
