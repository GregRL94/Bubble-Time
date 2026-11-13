using Unity.AppUI.Core;
using UnityEngine;

public class BombVessel : MonoBehaviour
{
    [SerializeField] private float _speed = 10f;
    [SerializeField] private float _explosionRadius = 0.2f;
    [SerializeField] private LayerMask _playerLayer;
    private Transform _playertransform;
    private Rigidbody2D _rigidbody2D;
    private Collider2D _collider2D;
    private bool _isActive;
    private bool _isArmed;
    private float _targetVelocityX;

    private void Awake()
    {
        _rigidbody2D = GetComponent<Rigidbody2D>();
        _collider2D = GetComponent<Collider2D>();
    }

    // Update is called once per frame
    void Update()
    {
        if (_playertransform == null) { return; }

        if (_isArmed)
        {
            float dir = Mathf.Sign(_playertransform.position.x - transform.position.x);
            _targetVelocityX = dir * _speed;
        }

        if (_isArmed && MathUtils.DistanceX(_playertransform.position, transform.position) < _explosionRadius)
        {
            Debug.Log("Player is within explosion radius!");
            Collider2D playerCollider = Physics2D.OverlapCircle(transform.position, _explosionRadius, _playerLayer);
            if (playerCollider)
            {
                if (playerCollider.TryGetComponent<IHit>(out IHit hittable))
                {
                    hittable.OnHit(1f);
                    Debug.Log("Player hit by bomb vessel explosion!");
                }
            }
            Destroy(gameObject);
        }
    }

    private void FixedUpdate()
    {
        _rigidbody2D.linearVelocity = new Vector2(_targetVelocityX, _rigidbody2D.linearVelocity.y);
    }

    public void ArmBomb()
    {
        _isArmed = true;
        Debug.Log("Bomb vessel armed!");
    }

    private void ActivateBomb()
    {
        _isActive = true;
        _collider2D.enabled = true;
        Debug.Log("Bomb vessel activated!");
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            Debug.Log("Player entered the bomb vessel trigger.");
            _playertransform = collision.transform;
            ActivateBomb();
        }
    }
}
