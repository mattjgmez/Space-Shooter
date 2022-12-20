using System.Collections;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [SerializeField] protected float _speed;
    [SerializeField] protected float _bounds_X = 14, _bounds_Y = 10;
    [SerializeField] protected AudioClip _laserSound;
    [SerializeField] protected GameObject _enemyProjectile, _explosionPrefab;

    protected bool _isDead = false;
    protected float _weaponCooldown;
    protected Player _player;
    protected BoxCollider2D _collider;
    protected AudioSource _audioSource;
    protected Rigidbody2D _rigidbody;

    void Start()
    {
        _player = GameObject.Find("Player").GetComponent<Player>();
        _collider = GetComponent<BoxCollider2D>();
        _rigidbody = GetComponent<Rigidbody2D>();
        _audioSource = GetComponent<AudioSource>();

        _weaponCooldown = Time.time + 1f;
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
            TriggerDeath();
        }

        if (other.tag == "Projectile")
        {
            Destroy(other.gameObject);

            if (_player != null)
                _player.SetScore(10);

            TriggerDeath();
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

    protected virtual void TriggerDeath()
    {
        _isDead = true;
        _collider.enabled = false;
        GameObject explosion = Instantiate(_explosionPrefab, transform.position, Quaternion.identity);
        Destroy(explosion, 2.63f);
        SpawnManager.Instance.ActiveEnemies.Remove(gameObject);
        Destroy(gameObject, 1f);
    }

    protected virtual void InheritedUpdate() { }
}
