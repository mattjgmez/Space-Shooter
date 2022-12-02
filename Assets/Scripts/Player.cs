using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField] float _speed = 10;
    [SerializeField] float _speedIncreaseTotal;
    [SerializeField] float _boostAmount = 5;
    [SerializeField] float _boostDuration = 2;
    [SerializeField] float _boostCooldown = 5;
    [SerializeField] float _bounds_X = 15.5f, _bounds_Y = 7.5f;
    [SerializeField] float _fireRate = 0.25f;
    [SerializeField] int _lives = 3;
    [SerializeField] int _score;
    [SerializeField] GameObject _projectilePrefab;
    [SerializeField] GameObject _tripleShotPrefab; 
    [SerializeField] GameObject _shieldObject;
    [SerializeField] GameObject _rightEngineFire, _leftEngineFire;
    [SerializeField] AudioClip _laserSound, _explosionSound;

    bool _tripleShot;
    bool _shield;
    bool _startBoost, _canBoost = true;
    float _nextFire;
    AudioSource _audioSource;
    ParticleSystem _particleSystem;

    void Start()
    {
        transform.position = Vector3.zero;

        _audioSource = GetComponent<AudioSource>();
        _particleSystem = GetComponent<ParticleSystem>();

        if (_audioSource == null)
            Debug.LogError("AudioSource = NULL.");
        if (_particleSystem == null)
            Debug.LogError("ParticleSystem = NULL.");
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.LeftShift) && _canBoost)
            StartCoroutine(BoostCoroutine());

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

    IEnumerator BoostCoroutine()
    {
        _startBoost = false;
        _canBoost = false;
        _speedIncreaseTotal += _boostAmount;
        _particleSystem.Play();

        yield return new WaitForSeconds(_boostDuration);
        _speedIncreaseTotal -= _boostAmount;
        _particleSystem.Stop();

        yield return new WaitForSeconds(_boostCooldown);
        _canBoost = true;
    }

    public void TakeDamage()
    {
        if (_shield)
        {
            _shield = false;
            _shieldObject.GetComponent<SpriteRenderer>().enabled = false;
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
        _shieldObject.GetComponent<SpriteRenderer>().enabled = true;
        StartCoroutine(SpeedUpDisable());
    }
    #endregion

    public void SetScore(int amount)
    {
        _score += amount;
        UIManager.Instance.UpdateScore(_score);
    }
}
