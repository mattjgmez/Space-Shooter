using UnityEngine;

public class Enemy : MonoBehaviour
{
    [SerializeField] protected float _speed;
    [SerializeField] protected float _bounds_X = 14, _bounds_Y = 10;
    [SerializeField] protected int _baseShieldChance = 5;
    [SerializeField] protected bool _shootsPowerups;
    [SerializeField] protected AudioClip _laserSound, _shieldHit, _shieldDestroyed;
    [SerializeField] protected GameObject _enemyProjectile;
    [SerializeField] protected GameObject _shieldObject;

    protected bool _isDead = false;
    protected int _shieldHealth;
    protected float _weaponCooldown;
    protected Player _player;
    protected BoxCollider2D _collider;
    protected AudioSource _audioSource;
    protected Rigidbody2D _rigidbody;
    protected SpriteRenderer _shieldSprite;
    protected ParticleSystem _shieldParticleSystem;
    protected Boxcaster _boxcaster;
    protected Color32 _shieldColor;

    protected virtual void Start()
    {
        _player = GameObject.Find("Player").GetComponent<Player>();
        _collider = GetComponent<BoxCollider2D>();
        _rigidbody = GetComponent<Rigidbody2D>();
        _audioSource = GetComponent<AudioSource>();

        if (_shootsPowerups)
            _boxcaster = GetComponent<Boxcaster>();

        _shieldSprite = _shieldObject.GetComponent<SpriteRenderer>();
        _shieldParticleSystem = _shieldObject.GetComponent<ParticleSystem>();

        _shieldColor = _shieldSprite.color;

        _weaponCooldown = Time.time + 1f;

        EnableShield();
    }

    protected virtual void Update()
    {
        if (_isDead)
            return;

        if (Time.time > _weaponCooldown && _player && (!_shootsPowerups || _boxcaster.Hits.Length > 0))
        {
            FireWeapon();
            if (_shootsPowerups)
                _boxcaster.ClearHits();
        }

        if (Input.GetKeyDown(KeyCode.K))
            TakeDamage();

        HandleMovement();
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Player")
        {
            _player.TakeDamage();
            TakeDamage();
        }

        if (other.tag == "Projectile")
        {
            Destroy(other.gameObject);

            TakeDamage();
        }
    }

    protected virtual void HandleMovement() 
    {
        transform.Translate(-transform.up * _speed * Time.deltaTime);

        if (transform.position.y < -_bounds_Y)
            transform.position = new Vector2(Random.Range(-_bounds_X, _bounds_X), _bounds_Y);
    }

    protected virtual void FireWeapon()
    {
        _weaponCooldown = Time.time + Random.Range(3f, 7f);
        Instantiate(_enemyProjectile, transform.position, transform.rotation);
        _audioSource.PlayOneShot(_laserSound);
    }

    protected virtual void TakeDamage()
    {
        if (_shieldHealth > 0)
        {
            _shieldHealth--;
            if (_shieldHealth > 0)
            {
                _shieldColor.a -= 85;
                _shieldSprite.color = _shieldColor;
                _audioSource.PlayOneShot(_shieldHit);
            }
            else if (_shieldHealth == 0)
            {
                _shieldColor.a = 0;
                _shieldSprite.color = _shieldColor;
                _shieldParticleSystem.Play();
                _audioSource.PlayOneShot(_shieldDestroyed);
            }
        }
        else TriggerDeath();
    }

    protected virtual void TriggerDeath()
    {
        if (_player != null)
            _player.SetScore(10);

        _isDead = true;
        _collider.enabled = false;
        SpawnManager.Instance.SpawnExplosion(transform.position);
        SpawnManager.Instance.ActiveEnemies.Remove(gameObject);
        Destroy(gameObject, 1f);
    }

    protected virtual void EnableShield()
    {
        int RNG = Random.Range(0, 100);
        if (RNG <= _baseShieldChance-- * SpawnManager.Instance.CurrentWave && SpawnManager.Instance.CurrentWave >= 5)
        {
            _shieldHealth = 2;
            _shieldColor.a = 170;
            _shieldSprite.color = _shieldColor;
        }
    }
}
