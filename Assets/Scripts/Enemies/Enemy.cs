using System.Collections;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [SerializeField] protected float _speed;
    [SerializeField] protected float _bounds_X = 14, _bounds_Y = 10;
    [SerializeField] protected int _baseShieldChance = 5;
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
    protected Color32 _shieldColor;

    void Start()
    {
        _player = GameObject.Find("Player").GetComponent<Player>();
        _collider = GetComponent<BoxCollider2D>();
        _rigidbody = GetComponent<Rigidbody2D>();
        _audioSource = GetComponent<AudioSource>();

        _shieldSprite = _shieldObject.GetComponent<SpriteRenderer>();
        _shieldParticleSystem = _shieldObject.GetComponent<ParticleSystem>();

        _shieldColor = _shieldSprite.color;

        _weaponCooldown = Time.time + 1f;

        EnableShield();
    }

    void Update()
    {
        if (_isDead)
            return;

        if (Time.time > _weaponCooldown && _player)
            FireWeapon();

        HandleMovement();

        InheritedUpdate();
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

            if (_player != null)
                _player.SetScore(10);

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
        _isDead = true;
        _collider.enabled = false;
        SpawnManager.Instance.SpawnExplosion(transform.position);
        SpawnManager.Instance.ActiveEnemies.Remove(gameObject);
        Destroy(gameObject, 1f);
    }

    protected virtual void EnableShield()
    {
        int RNG = Random.Range(0, 100);
        if (RNG <= _baseShieldChance-- * SpawnManager.Instance.CurrentWave && SpawnManager.Instance.CurrentWave >= 4)
        {
            _shieldHealth = 2;
            _shieldColor.a = 170;
            _shieldSprite.color = _shieldColor;
        }
    }

    protected virtual void InheritedUpdate() { }
}
