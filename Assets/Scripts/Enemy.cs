using System.Collections;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [SerializeField] float _speed;
    [SerializeField] float _bounds_X = 14, _bounds_Y = 10;
    [SerializeField] AudioClip _explosionSound;

    bool _isDead;
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
    }

    void Update()
    {
        if (_isDead)
            return;

        //if laser cooldown finished
        //fire laser

        transform.Translate(Vector2.down * _speed * Time.deltaTime);

        if (transform.position.y < -_bounds_Y)
            transform.position = new Vector2(Random.Range(-_bounds_X, _bounds_X), _bounds_Y);
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

    void FireLaser()
    {
        //fire laser
        //refresh laser cooldown
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
