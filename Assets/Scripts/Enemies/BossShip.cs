using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossShip : MonoBehaviour
{
    [SerializeField] GameObject _plasmaTurretHolder, _beamTurretHolder;
    [SerializeField] GameObject _laserPrefab, _mineVolleyPrefab, _mineWallPrefab;
    [SerializeField] List<GameObject> _leftBeams, _rightBeams, _oddBeams, _evenBeams;
    [SerializeField] AudioClip _laserSound, _beamSound;

    int _maxHealth;
    int _currentHealth;
    bool _firingLasers;
    bool _firingBeams;
    bool _isDead;
    AudioSource _audioSource;
    GameObject _player;
    BoxCollider2D _collider;

    void Start()
    {
        _player = GameObject.Find("Player");
        _audioSource = GetComponent<AudioSource>();
        _collider = GetComponent<BoxCollider2D>();

        StartCoroutine(MoveIntoScreen());

        _maxHealth = 100 * (SpawnManager.Instance.CurrentWave / 5);
        _currentHealth = _maxHealth;

        StartCoroutine(UIManager.Instance.InitializeBossHealthBar(_maxHealth));
    }

    void Update()
    {
        if (_isDead)
            transform.Translate(Vector2.right * Time.deltaTime * 3);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Projectile")
        {
            Destroy(other.gameObject);

            TakeDamage();
        }
    }

    IEnumerator MoveIntoScreen()
    {
        while (transform.position.y > 7.5f)
        {
            transform.Translate(Vector2.down * Time.deltaTime * 3);
            yield return null;
        }
        StartCoroutine(LaserAttack(7));
    }

    #region Damage Methods
    void TakeDamage()
    {
        --_currentHealth;
        UIManager.Instance.UpdateBossHealthBar(_currentHealth);
        if (_currentHealth <= 0) StartCoroutine(TriggerDeath());
    }

    IEnumerator TriggerDeath()
    {
        _collider.enabled = false;

        _isDead = true;

        if (_player != null)
            _player.GetComponent<Player>().SetScore(500);

        SpawnManager.Instance.SpawnExplosion
            (new Vector3(Random.Range(-10f, 10f), Random.Range(-2f, 0f), 0) + transform.position);

        StartCoroutine(CameraManager.Instance.CameraShake(.1f));
        StartCoroutine(GameManager.Instance.HitStop(.25f));

        while (transform.position.x < 32)
        {
            for (int i = 0; i < Random.Range(1, 3); i++)
            {
                SpawnManager.Instance.SpawnExplosion
                    (new Vector3(Random.Range(-10f, 10f), Random.Range(-2f, 0f), 0) + transform.position);
            }
            yield return new WaitForSeconds(Random.Range(0.5f, 1f));
        }

        SpawnManager.Instance.ActiveEnemies.Remove(gameObject);
        Destroy(gameObject);
    }
    #endregion

    #region Laser Attack Methods
    IEnumerator LaserAttack(float duration)
    {
        while (_plasmaTurretHolder.transform.localPosition.y > 0)
        {
            _plasmaTurretHolder.transform.Translate(Vector2.down * Time.deltaTime);
            yield return null;
        }
        _plasmaTurretHolder.transform.localPosition = Vector2.zero;

        _firingLasers = true;
        StartCoroutine(HandleTurretRotation());

        float timer = Time.time + duration;
        while (Time.time < timer && _player != null && !_isDead)
        {
            foreach (Transform turret in _plasmaTurretHolder.transform)
            {
                Instantiate(_laserPrefab, turret.transform.position, turret.transform.rotation);
                _audioSource.PlayOneShot(_laserSound);

                yield return new WaitForSeconds(.15f);
            }
        }
        _firingLasers = false;

        if (_player != null && !_isDead)
            StartCoroutine(BeamAttack(2));
        while (_plasmaTurretHolder.transform.localPosition.y < 0.5f)
        {
            _plasmaTurretHolder.transform.Translate(Vector2.up * Time.deltaTime);
            yield return null;
        }
        _plasmaTurretHolder.transform.localPosition = new Vector3(0, .5f, 0);
    }

    IEnumerator HandleTurretRotation()
    {
        while (_firingLasers && _player != null)
        {
            foreach (Transform turret in _plasmaTurretHolder.transform)
            {
                //Get the direction of the target 
                Vector2 targetDirection = _player.transform.position - turret.transform.position;
                targetDirection.Normalize();

                //Rotate toward target 
                float rotation = Mathf.Atan2(targetDirection.y, targetDirection.x) * Mathf.Rad2Deg;
                turret.transform.rotation = Quaternion.Euler(0, 0, rotation + 90);
            }
            yield return null;
        }
        foreach (Transform turret in _plasmaTurretHolder.transform)
        {
            turret.transform.rotation = Quaternion.Euler(0, 0, 0);
        }
    }
    #endregion

    #region Beam Attack Methods
    IEnumerator BeamAttack(int duration)
    {
        StartCoroutine(SpawnSideMines());
        while (_beamTurretHolder.transform.localPosition.y > 0)
        {
            _beamTurretHolder.transform.Translate(Vector2.down * Time.deltaTime);
            yield return null;
        }
        _beamTurretHolder.transform.localPosition = Vector2.zero;

        _firingBeams = true;
        int rounds = 0;
        while (rounds < duration && _player != null && !_isDead)
        {
            rounds++;
            float delay = 1.01f;

            foreach (GameObject beam in _leftBeams)
            {
                StartCoroutine(BeamCoroutine(beam));
            }
            yield return new WaitForSeconds(delay);

            foreach (GameObject beam in _rightBeams)
            {
                StartCoroutine(BeamCoroutine(beam));
            }
            yield return new WaitForSeconds(delay);

            foreach (GameObject beam in _oddBeams)
            {
                StartCoroutine(BeamCoroutine(beam));
            }
            yield return new WaitForSeconds(delay);

            foreach (GameObject beam in _evenBeams)
            {
                StartCoroutine(BeamCoroutine(beam));
            }
            yield return new WaitForSeconds(delay);
        }
        _firingBeams = false;

        if (_player != null && !_isDead)
            StartCoroutine(MineVolley(10));
        while (_beamTurretHolder.transform.localPosition.y < 2)
        {
            _beamTurretHolder.transform.Translate(Vector2.up * Time.deltaTime);
            yield return null;
        }
        _beamTurretHolder.transform.localPosition = new Vector3(0, 2, 0);
    }

    IEnumerator SpawnSideMines()
    {
        GameObject rightWall = Instantiate(_mineWallPrefab, new Vector3(16, -0.7f, 0), Quaternion.Euler(0, 0, 90));
        GameObject leftWall = Instantiate(_mineWallPrefab, new Vector3(-16, -0.7f, 0), Quaternion.Euler(0, 0, -90));
        float speed = 2.8f;

        while(rightWall.transform.position.x > 10 && leftWall.transform.position.x < -10)
        {
            rightWall.transform.Translate(Vector2.left * Time.deltaTime * speed, Space.World);
            leftWall.transform.Translate(Vector2.right * Time.deltaTime * speed, Space.World);
            yield return null;
        }

        rightWall.transform.position = new Vector3(10, -0.7f, 0);
        leftWall.transform.position = new Vector3(-10, -0.7f, 0);

        while (_firingBeams)
            yield return null;

        while (rightWall.transform.position.x < 16 && leftWall.transform.position.x > -16)
        {
            rightWall.transform.Translate(Vector2.right * Time.deltaTime * speed, Space.World);
            leftWall.transform.Translate(Vector2.left * Time.deltaTime * speed, Space.World);
            yield return null;
        }
        Destroy(rightWall);
        Destroy(leftWall);
    }

    IEnumerator BeamCoroutine(GameObject beam)
    {
        _audioSource.PlayOneShot(_laserSound);
        Collider2D beamCollider = beam.GetComponent<Collider2D>();

        beam.transform.localScale = new Vector3(.1f, beam.transform.localScale.y, 1);
        yield return new WaitForSeconds(0.75f);
        beam.transform.localScale = new Vector3(2, beam.transform.localScale.y, 1);
        beamCollider.enabled = true;
        yield return new WaitForSeconds(.25f);
        beam.transform.localScale = new Vector3(0, beam.transform.localScale.y, 1);
        beamCollider.enabled = false;
    }
    #endregion

    #region Mine Volley Methods
    IEnumerator MineVolley(float duration)
    {
        float timer = Time.time + duration;
        while (Time.time < timer && _player != null && !_isDead)
        {
            GameObject newVolley = Instantiate(_mineVolleyPrefab, new Vector3(0, 10, 0), Quaternion.identity);

            foreach (Transform mine in newVolley.transform)
            {
                int RNG = Random.Range(0, 100);
                if (RNG < 60)
                    Destroy(mine.gameObject);
            }

            yield return new WaitForSeconds(Random.Range(1f, 2f));
        }
        if (_player != null && !_isDead)
            StartCoroutine(LaserAttack(7));
    }
    #endregion
}
