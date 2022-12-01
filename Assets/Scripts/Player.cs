using System.Collections;
using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField] float _speed = 10;
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
    bool _speedUp;
    bool _shield;
    float _nextFire;
    AudioSource _audioSource;

    void Start()
    {
        //Current position = new position (0, 0, 0)
        transform.position = Vector3.zero;

        _audioSource = GetComponent<AudioSource>();

        if (_audioSource == null)
            Debug.LogError("AudioSource = NULL.");
    }

    void Update()
    {
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

        transform.Translate((_speedUp ? _speed * 1.5f : _speed) * Time.deltaTime * direction);

        //Makes player object wrap when leaving the left and right boundaries
        if (transform.position.x < -_bounds_X) transform.position = new Vector2(_bounds_X, transform.position.y);
        else if (transform.position.x > _bounds_X) transform.position = new Vector2(-_bounds_X, transform.position.y);

        //Keeps player within the top and bottom of the screen
        transform.position = new Vector3(transform.position.x, Mathf.Clamp(transform.position.y, -_bounds_Y, _bounds_Y), 0);
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
        _speedUp = true;
        StartCoroutine(SpeedUpDisable());
    }

    IEnumerator SpeedUpDisable()
    {
        yield return new WaitForSeconds(3);
        _speedUp = false;
    }

    public void ShieldEnable()
    {
        _shield = true;
        _shieldObject.GetComponent<SpriteRenderer>().enabled = true;
        StartCoroutine(SpeedUpDisable());
    }

    public void SetScore(int amount)
    {
        _score += amount;
        UIManager.Instance.UpdateScore(_score);
    }
}
