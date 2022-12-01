using System.Collections;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [SerializeField] float _speed;
    [SerializeField] float _bounds_X = 14, _bounds_Y = 10;
    [SerializeField] AudioClip _explosionSound, _laserSound;
    [SerializeField] GameObject _enemyLaserPrefab;

    bool _isDead;
    float _laserCooldown;
    Player _player;
    Animator _anim;
    BoxCollider2D _collider;
    AudioSource _audioSource;

    void Start()
    {
        _player = GameObject.Find("Player").GetComponent<Player>();
        _anim = GetComponent<Animator>();
        _collider = GetComponent<BoxCollider2D>();
        _audioSource = GetComponent<AudioSource>();

        _laserCooldown = Time.time + 1f;
    }

    void Update()
    {
        if (_isDead)
            return;

        if (Time.time > _laserCooldown)
            FireLaser();

        HandleMovement();
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

    void HandleMovement()
    {
        transform.Translate(Vector2.down * _speed * Time.deltaTime);

        if (transform.position.y < -_bounds_Y)
            transform.position = new Vector2(Random.Range(-_bounds_X, _bounds_X), _bounds_Y);
    }

    void FireLaser()
    {
        _laserCooldown = Time.time + Random.Range(3f, 7f);
        Instantiate(_enemyLaserPrefab, transform.position, Quaternion.identity);
        _audioSource.PlayOneShot(_laserSound);
    }

    void TriggerDeath()
    {
        _isDead = true;
        _collider.enabled = false;
        _anim.SetTrigger("OnDeath");
        _audioSource.PlayOneShot(_explosionSound);
        Destroy(gameObject, 2.62f);
    }
}
