using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class Player : MonoBehaviour
{
    #region Serialized Variables
    [SerializeField] float _speed = 10;
    [SerializeField] float _totalSpeedBonus = 0f;
    [SerializeField] float _boostAmount = 5;
    [SerializeField] float _boostDuration = 2;
    [SerializeField] float _boostCooldown = 4;
    [SerializeField] float _bounds_X = 15.5f, _bounds_Y = 7.5f;
    [SerializeField] float _fireRate = 0.25f;
    [SerializeField] float _reloadTime = 3f;
    [SerializeField] int _lives = 3;
    [SerializeField] int _score = 0;
    [SerializeField] int _maxAmmo = 15;
    [SerializeField] GameObject _laserPrefab;
    [SerializeField] GameObject _tripleShotPrefab;
    [SerializeField] GameObject _missilePrefab;
    [SerializeField] GameObject _shieldObject;
    [SerializeField] GameObject _rightEngineFire, _leftEngineFire;
    [SerializeField] AudioClip _laserSound, _missileSound, _explosionSound;
    [SerializeField] AudioClip _shieldHit, _shieldDestroyed;
    [SerializeField] AudioClip _reloadSound;
    #endregion
    #region Local Variables
    bool _canBoost = true;
    bool _shield;
    bool _reloading;
    int _shieldHealth;
    int _currentAmmo;
    int _fireMode; //0 laser, 1 triple shot, 2 missile
    int _currentMissiles = 5;
    float _nextFire;
    AudioSource _audioSource;
    ParticleSystem _particleSystem, _shieldParticleSystem;
    SpriteRenderer _shieldSprite;
    Color32 _shieldColor;
    #endregion

    void Start()
    {
        transform.position = Vector3.zero;

        _audioSource = GetComponent<AudioSource>();
        _particleSystem = GetComponent<ParticleSystem>();
        _shieldSprite = _shieldObject.GetComponent<SpriteRenderer>();
        _shieldParticleSystem = _shieldObject.GetComponent<ParticleSystem>();

        _shieldColor = _shieldSprite.color;
        _currentAmmo = _maxAmmo;
        UIManager.Instance.UpdateAmmo(_currentAmmo.ToString());
        UIManager.Instance.UpdateMissiles(_currentMissiles);

        if (_audioSource == null)
            Debug.LogError("AudioSource = NULL.");
        if (_particleSystem == null)
            Debug.LogError("ParticleSystem = NULL.");
    }

    void Update()
    {
        //change fire mode to 0 if 2 or to 2 if 0
        //does nothing if triple shot is active
        if (Input.GetKeyDown(KeyCode.R) && _fireMode != 1 && _currentMissiles > 0)
            _fireMode = _fireMode == 0 ? 2 : 0;

        if (Input.GetKeyDown(KeyCode.LeftShift) && _canBoost)
            StartCoroutine(BoostCoroutine());

        HandleMovement();

        if (Input.GetKey(KeyCode.Space) && Time.time > _nextFire && !_reloading)
            HandleShooting();

        //Used for debugging methods
        if (Input.GetKeyDown(KeyCode.Tab))
            TakeDamage();
    }

    void HandleShooting()
    {
        _nextFire = Time.time + _fireRate;

        //Instantiates projectile prefab based on current Fire Mode
        switch (_fireMode)
        {
            case 0://laser
                if (_currentAmmo <= 0)
                    return;
                _currentAmmo--;
                UIManager.Instance.UpdateAmmo(_currentAmmo.ToString());
                if (_currentAmmo <= 0)
                    StartCoroutine(ReloadCoroutine());
                Instantiate(_laserPrefab, transform.position + new Vector3(0, 1.07f, 0f), Quaternion.identity);
                _audioSource.PlayOneShot(_laserSound);
                break;

            case 1://triple shot
                Instantiate(_tripleShotPrefab, transform.position, Quaternion.identity);
                _audioSource.PlayOneShot(_laserSound);
                break;

            case 2://missile
                _currentMissiles--;
                if (_currentMissiles <= 0)
                    _fireMode = 0;
                UIManager.Instance.UpdateMissiles(_currentMissiles);
                Instantiate(_missilePrefab, transform.position, Quaternion.identity);
                _audioSource.PlayOneShot(_missileSound);
                break;
        }
    }

    void HandleMovement()
    {
        Vector2 direction = new (Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));

        float currentSpeed = _speed + _totalSpeedBonus;
        transform.Translate(currentSpeed * Time.deltaTime * direction);

        //Makes player object wrap when leaving the left and right boundaries
        if (transform.position.x < -_bounds_X) transform.position = new Vector2(_bounds_X, transform.position.y);
        else if (transform.position.x > _bounds_X) transform.position = new Vector2(-_bounds_X, transform.position.y);

        //Keeps player within the top and bottom of the screen
        transform.position = new Vector3(transform.position.x, Mathf.Clamp(transform.position.y, -_bounds_Y, _bounds_Y), 0);
    }

    IEnumerator BoostCoroutine()
    {
        _canBoost = false;
        _totalSpeedBonus += _boostAmount;
        _particleSystem.Play(false);
        StartCoroutine(UIManager.Instance.BoostUICoroutine(_boostCooldown));

        yield return new WaitForSeconds(_boostDuration);
        _totalSpeedBonus -= _boostAmount;
        _particleSystem.Stop();

        yield return new WaitForSeconds(_boostCooldown - _boostDuration);
        _canBoost = true;
    }

    IEnumerator ReloadCoroutine()
    {
        _reloading = true;
        yield return new WaitForSeconds(0.2f);

        UIManager.Instance.UpdateAmmo("Reloading .");
        yield return new WaitForSeconds(_reloadTime / 3);

        UIManager.Instance.UpdateAmmo("Reloading . .");
        yield return new WaitForSeconds(_reloadTime / 3);

        UIManager.Instance.UpdateAmmo("Reloading . . .");
        yield return new WaitForSeconds(_reloadTime / 3);

        _currentAmmo = _maxAmmo;
        _reloading = false;
        UIManager.Instance.UpdateAmmo(_currentAmmo.ToString());
        _audioSource.PlayOneShot(_reloadSound);
    }

    public void TakeDamage()
    {
        if (_shield)
        {
            _shieldHealth--;
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

        _lives--;
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
        {
            StartCoroutine(CameraManager.Instance.CameraShake());
            _rightEngineFire.SetActive(true);
        }
        else if (_lives == 1)
        {
            StartCoroutine(CameraManager.Instance.CameraShake());
            _leftEngineFire.SetActive(true);
        }
    }

    #region Powerup Methods
    public void EnablePowerup(int PowerupID)
    {
        switch (PowerupID)
        {
            case 0: //Triple Shot
                _fireMode = 1;
                StartCoroutine(TripleShotDisable());
                break;

            case 1: //Speedup
                _totalSpeedBonus += 10;
                StartCoroutine(SpeedUpDisable());
                break;

            case 2: //Shield
                _shield = true;
                _shieldHealth = 3;
                _shieldColor.a = 255;
                _shieldSprite.color = _shieldColor;
                break;

            case 3: //Ammo
                _currentAmmo = _maxAmmo;
                UIManager.Instance.UpdateAmmo(_currentAmmo.ToString());
                break;

            case 4: //Health
                if (_lives < 3)
                    _lives++;
                UIManager.Instance.UpdateLives(_lives);
                if (_lives == 3) _rightEngineFire.SetActive(false);
                if (_lives == 2) _leftEngineFire.SetActive(false);
                break;

            case 5: //Missile
                _currentMissiles = 5;
                UIManager.Instance.UpdateMissiles(_currentMissiles);
                break;
        }
    }

    IEnumerator TripleShotDisable()
    {
        yield return new WaitForSeconds(5);
        _fireMode = 0;
    }

    IEnumerator SpeedUpDisable()
    {
        yield return new WaitForSeconds(3);
        _totalSpeedBonus -= 10;
    }
    #endregion

    public void SetScore(int amount)
    {
        _score += amount;
        UIManager.Instance.UpdateScore(_score);
    }
}
