using UnityEngine;

public class BasicProjectile : MonoBehaviour, IProjectile
{
    #region Attributes
    [SerializeField] private GameObject _explosionEffect;
    private float _speed;
    private float _damage;
    private float _lifetime;
    private float _timer;
    private LayerMask _impactLayerMask;
    #endregion Attributes

    #region MonoBehaviour Methods
    // Update is called once per frame
    void Update()
    {
        Move();
        _timer += Time.deltaTime;
        if (_timer >= _lifetime)
        {
            _Destroy();
        }
    }
    #endregion MonoBehaviour Methods

    #region Methods
    private void Move()
    {
        transform.Translate(Vector3.up * _speed * Time.deltaTime);
    }

    private void _Destroy()
    {
        Instantiate(_explosionEffect, transform.position, Quaternion.identity);
        Destroy(gameObject);
    }
    #endregion Methods

    #region Trigger Methods
    private void OnTriggerEnter2D(Collider2D collider)
    {
        if (!(((1 << collider.gameObject.layer) & _impactLayerMask) != 0)) { return; }
        //if (collider.TryGetComponent(out IHit hitComponent))
        //{
        //    hitComponent.OnHit(_damage);
        //}
        _Destroy();
    }
    #endregion Trigger Methods

    #region IProjectile Implementation
    public void SetupProjectile(float speed, float damage, float lifetime, LayerMask impactLayerMask)
    {
        _speed = speed;
        _damage = damage;
        _lifetime = lifetime;
        _impactLayerMask = impactLayerMask;
    }
    #endregion IProjectile Implementation
}
