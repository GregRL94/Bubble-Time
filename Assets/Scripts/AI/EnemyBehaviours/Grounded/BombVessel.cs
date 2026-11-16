using UnityEngine;

public class BombVessel : MonoBehaviour, IHit
{
    [SerializeField] private float _health = 20f;
    [SerializeField] private float _speed = 10f;
    [SerializeField] private float _explosionRadius = 0.75f;
    [SerializeField] private float _explosionDamage = 1f;
    [SerializeField] private GameObject _explosionEffectPrefab;
    [SerializeField] private float _selfDestructTime = 10f;
    [SerializeField] private LayerMask _playerLayer;
    private Transform _playertransform;
    private Rigidbody2D _rigidbody2D;
    private Collider2D _collider2D;
    private Animator _animator;
    private bool _isArmed;
    private float _targetVelocityX;
    private float _selfDestructTimer;

    private void Awake()
    {
        _rigidbody2D = GetComponent<Rigidbody2D>();
        _collider2D = GetComponent<Collider2D>();
        _animator = GetComponent<Animator>();
    }

    private void Start()
    {
        _collider2D.enabled = false;
        _isArmed = false;
        _selfDestructTimer = _selfDestructTime;
    }

    // Update is called once per frame
    void Update()
    {
        if (_isArmed)
        {
            _selfDestructTimer -= Time.deltaTime;
            if (_selfDestructTimer <= 0)
            {
                Explode();
                return;
            }
        }

        if (_playertransform == null) { return; }

        if (_isArmed)
        {
            float dir = Mathf.Sign(_playertransform.position.x - transform.position.x);
            _targetVelocityX = dir * _speed;
        }

        if (_isArmed && Vector2.Distance(_playertransform.position, transform.position) < _explosionRadius)
        {
            Debug.Log("Player is within explosion radius!");
            Explode();
        }
    }

    private void FixedUpdate()
    {
        _rigidbody2D.linearVelocity = new Vector2(_targetVelocityX, _rigidbody2D.linearVelocity.y);
    }

    private void ActivateBomb()
    {
        _collider2D.enabled = true;
        _animator.SetTrigger("Activate");
        Debug.Log("Bomb vessel activated!");
    }

    public void ArmBomb()
    {
        _isArmed = true;
        Debug.Log("Bomb vessel armed!");
        _animator.SetTrigger("Chase");
    }

    private void Explode()
    {
        Collider2D playerCollider = Physics2D.OverlapCircle(transform.position, _explosionRadius, _playerLayer);
        if (playerCollider)
        {
            if (playerCollider.TryGetComponent<IHit>(out IHit hittable))
            {
                hittable.OnHit(_explosionDamage);
                Debug.Log("Player hit by bomb vessel explosion!");
            }
        }
        Instantiate(_explosionEffectPrefab, transform.position, Quaternion.identity);
        Destroy(gameObject);
    }

    private void TakeDamage(float damage)
    {
        _health -= damage;
        Debug.Log($"Bomb vessel took {damage} damage. Remaining health: {_health}");
        if (_health <= 0)
        {
            Explode();
        }
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

    public void OnHit(float damage)
    {
        TakeDamage(damage);
    }
}
