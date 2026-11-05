using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class BasicProjectile : MonoBehaviour, IProjectile
{
    #region Attributes
    [SerializeField] private GameObject _explosionEffect;
    private float _currentSpeed;
    private float _damage;
    private float _lifetime;
    private float _timer;
    private LayerMask _impactLayerMask;
    private Rigidbody2D _rb2D;
    #endregion Attributes

    #region MonoBehaviour Methods
    private void Awake()
    {
        _rb2D = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        UpdateTimers();
    }
    #endregion MonoBehaviour Methods

    #region Methods
    private void UpdateTimers()
    {
        _timer += Time.deltaTime;
        if (_timer >= _lifetime)
        {
            _Destroy();
        }
    }

    private void _Destroy()
    {
        if (_explosionEffect != null)
        {
            Instantiate(_explosionEffect, transform.position, Quaternion.identity);
        }
        Destroy(gameObject);
    }
    #endregion Methods

    #region Trigger Methods
    private void OnTriggerEnter2D(Collider2D collider)
    {
        if (!(((1 << collider.gameObject.layer) & _impactLayerMask) != 0)) { return; }
        if (collider.TryGetComponent(out IHit hitComponent))
        {
            hitComponent.OnHit(_damage);
        }
        _Destroy();
    }
    #endregion Trigger Methods

    #region IProjectile Implementation
    public void SetupProjectile(float speed, float damage, float lifetime, LayerMask impactLayerMask)
    {
        _currentSpeed = speed;
        _rb2D.linearVelocity = transform.up * _currentSpeed;
        _damage = damage;
        _lifetime = lifetime;
        _impactLayerMask = impactLayerMask;
    }
    #endregion IProjectile Implementation
}
