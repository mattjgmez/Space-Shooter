using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField] float _speed = 10;
    [SerializeField] float _speedIncreaseTotal;
    [SerializeField] float _dashAmount = 5;
    [SerializeField] float _dashDuration = 2;
    [SerializeField] float _dashCooldown = 5;
    [SerializeField] float _bounds_X = 15.5f, _bounds_Y = 7.5f;
    [SerializeField] float _fireRate = 0.25f;
    [SerializeField] int _lives = 3;
    [SerializeField] int _score;
    [SerializeField] GameObject _projectilePrefab;
    [SerializeField] GameObject _tripleShotPrefab; 
    [SerializeField] GameObject _shieldObject;
    [SerializeField] GameObject _rightEngineFire, _leftEngineFire;
    [SerializeField] AudioClip _laserSound, _explosionSound;
    [SerializeField] AudioClip _shieldHit, _shieldDestroyed;

    bool _tripleShot;
    bool _canDash = true;
    bool _shield;
    int _shieldHealth;
    float _nextFire;
    AudioSource _audioSource;
    ParticleSystem _particleSystem, _shieldParticleSystem;
    SpriteRenderer _shieldSprite;
    Color32 _shieldColor;

    void Start()
    {
        transform.position = Vector3.zero;

        _audioSource = GetComponent<AudioSource>();
        _particleSystem = GetComponent<ParticleSystem>();
        _shieldSprite = _shieldObject.GetComponent<SpriteRenderer>();
        _shieldParticleSystem = _shieldObject.GetComponent<ParticleSystem>();
        _shieldColor = _shieldSprite.color;

        if (_audioSource == null)
            Debug.LogError("AudioSource = NULL.");
        if (_particleSystem == null)
            Debug.LogError("ParticleSystem = NULL.");
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.LeftShift) && _canDash)
            StartCoroutine(DashCoroutine());

        HandleMovement();

        if (Input.GetKey(KeyCode.Space) && Time.time > _nextFire)
            HandleShooting();
    }

    void HandleShooting()
    {
        _nextFire = Time.time + _fireRate;

        //Instantiates triple shot prefab if powerup is active, otherwise fires single laser prefab
        if (_tripleShot) Instantiate(_tripleShotPrefab, transform.position, Quaternion.identity);
        else Instantiate(_projectilePrefab, transform.position + new Vector3(0, 1.07f, 0f), Quaternion.identity);

        _audioSource.PlayOneShot(_laserSound);
    }

    void HandleMovement()
    {
        Vector2 direction = new (Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));

        float currentSpeed = _speed + _speedIncreaseTotal;
        transform.Translate(currentSpeed * Time.deltaTime * direction);

        //Makes player object wrap when leaving the left and right boundaries
        if (transform.position.x < -_bounds_X) transform.position = new Vector2(_bounds_X, transform.position.y);
        else if (transform.position.x > _bounds_X) transform.position = new Vector2(-_bounds_X, transform.position.y);

        //Keeps player within the top and bottom of the screen
        transform.position = new Vector3(transform.position.x, Mathf.Clamp(transform.position.y, -_bounds_Y, _bounds_Y), 0);
    }

    IEnumerator DashCoroutine()
    {
        _canDash = false;
        _speedIncreaseTotal += _dashAmount;
        _particleSystem.Play(false);

        yield return new WaitForSeconds(_dashDuration);
        _speedIncreaseTotal -= _dashAmount;
        _particleSystem.Stop();

        yield return new WaitForSeconds(_dashCooldown -= _dashDuration);
        _canDash = true;
    }

    public void TakeDamage()
    {
        if (_shield)
        {
            _shieldHealth -= 1;
            if (_shieldHealth > 0)
            {
                _shieldColor.a -= 85;
                _shieldSprite.color = _shieldColor;
                _audioSource.PlayOneShot(_shieldHit);
            }
            if (_shieldHealth == 0)
            {
                _shieldColor.a = 0;
                _shieldSprite.color = _shieldColor;
                _shieldParticleSystem.Play();
                _audioSource.PlayOneShot(_shieldDestroyed);
                _shield = false;
            }
            return;
        }

        _lives -= 1;
        UIManager.Instance.UpdateLives(_lives);

        if (_lives < 1)
        {
            SpawnManager.Instance.OnPlayerDeath();
            GameManager.Instance.GameOver();
            UIManager.Instance.ShowGameOverText();
            _audioSource.PlayOneShot(_explosionSound);
            Destroy(gameObject);
        }
        else if (_lives == 2)
            _rightEngineFire.SetActive(true);
        else if (_lives == 1)
            _leftEngineFire.SetActive(true);
    }

    #region Powerup Methods
    public void TripleShotEnable()
    {
        _tripleShot = true;
        StartCoroutine(TripleShotDisable());
    }

    IEnumerator TripleShotDisable()
    {
        yield return new WaitForSeconds(5);
        _tripleShot = false;
    }

    public void SpeedUpEnable()
    {
        _speedIncreaseTotal += 10;
        StartCoroutine(SpeedUpDisable());
    }

    IEnumerator SpeedUpDisable()
    {
        yield return new WaitForSeconds(3);
        _speedIncreaseTotal -= 10;
    }

    public void ShieldEnable()
    {
        _shield = true;
        _shieldHealth = 3;
        _shieldColor.a = 255;
        _shieldSprite.color = _shieldColor;
    }
    #endregion

    public void SetScore(int amount)
    {
        _score += amount;
        UIManager.Instance.UpdateScore(_score);
    }
}
