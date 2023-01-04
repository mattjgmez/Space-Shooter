using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnManager : MonoSingleton<SpawnManager>
{
    [SerializeField] float bounds_X = 14;
    [SerializeField] Transform _enemyContainer;
    [SerializeField] GameObject[] _enemyPrefab;
    [SerializeField] GameObject[] _powerupPrefabs;
    [SerializeField] GameObject[] _rarePowerupPrefabs;
    [SerializeField] GameObject _explosionPrefab;

    bool _stopSpawning = false;
    public int _currentWave = 0;
    int _spawnedEnemies = 0;
    int _enemyToSpawnID;
    float _spawnDelay = 5f;
    List<GameObject> _activeEnemies = new List<GameObject>();

    public List<GameObject> ActiveEnemies { get { return _activeEnemies; } set { _activeEnemies = value; } }
    public int CurrentWave { get { return _currentWave; } }

    public void StartSpawning()
    {
        StartWave();

        StartCoroutine(SpawnPowerup());
    }

    void StartWave()
    {
        if (_stopSpawning) return;

        _currentWave++;
        _spawnedEnemies = 0;
        UIManager.Instance.UpdateWaveText(_currentWave);

        //Set wave enemy limit and spawn delay exponentially
        //Sets amount to 1 in boss waves
        int enemyAmount;
        if (_currentWave % 5 == 0)
        {
            enemyAmount = 1;
            _spawnDelay = 1;
        }
        else 
        {
            enemyAmount = (int)((_currentWave + 3) * Mathf.Pow(1 + 0.05f, 2));
            _spawnDelay = Mathf.Clamp(_spawnDelay * Mathf.Pow(1 - .05f, 2), 0.5f, 3f);
        }

        StartCoroutine(SpawnEnemyCoroutine(enemyAmount));
    }

    IEnumerator SpawnEnemyCoroutine(int enemyAmount)
    {
        yield return new WaitForSeconds(3);

        while (!_stopSpawning && _spawnedEnemies < enemyAmount)
        {
            _spawnedEnemies++;

            SpawnEnemy(RollEnemy());

            yield return new WaitForSeconds(_spawnDelay);
        }

        while (!_stopSpawning && _activeEnemies.Count > 0) //Prevents next wave from spawning until all active enemies are slain
            yield return null;

        StartWave();
    }

    GameObject RollEnemy()
    {
        int RNG = Random.Range(0, 100);

        if (_currentWave % 5 == 0)
            _enemyToSpawnID = 5; //Boss wave
        else if (RNG <= 3 * _currentWave && _currentWave >= 4)
            _enemyToSpawnID = 4; //Smart enemy
        else if (RNG <= 5 * _currentWave && _currentWave >= 3)
            _enemyToSpawnID = 3; //Mine Layer enemy
        else if (RNG <= 8 * _currentWave && _currentWave >= 3)
            _enemyToSpawnID = 2; //Beam enemy
        else if (RNG <= 10 * _currentWave && _currentWave >= 2)
            _enemyToSpawnID = 1; //Curve enemy
        else
            _enemyToSpawnID = 0; //Basic enemy

        return _enemyPrefab[_enemyToSpawnID];
    }

    void SpawnEnemy(GameObject enemyType)
    {
        Vector3 spawnPosition = new(Random.Range(-bounds_X, bounds_X), 10, 0);
        Vector3 spawnRotation = Vector3.zero;

        if (_enemyToSpawnID == 5)
        {
            spawnPosition = new Vector3(0, 12, 0);
        }
        else if (_enemyToSpawnID == 1)
        {
            //Randomly select left or right and set position/rotation accordingly
            bool isLeft = Random.Range(0, 2) == 0;
            spawnPosition = new (isLeft ? -17 : 17, 2.5f, 0);
            spawnRotation = new (0, 0, isLeft ? 14.3f : -14.3f);
        }

        GameObject newEnemy = Instantiate(enemyType, spawnPosition, Quaternion.Euler(spawnRotation), _enemyContainer);
        _activeEnemies.Add(newEnemy);
    }

    IEnumerator SpawnPowerup()
    {
        yield return new WaitForSeconds(3);

        //Spawns a powerup every 6 to 8 seconds.
        while (!_stopSpawning)
        {
            int nextPowerup = Random.Range(0, 100);
            Vector3 spawnPos = new (Random.Range(-bounds_X, bounds_X), 10, 0);

            if (nextPowerup < 90) 
                Instantiate(_powerupPrefabs[Random.Range(0, _powerupPrefabs.Length)], spawnPos, Quaternion.identity);
            if (nextPowerup >= 90)
                Instantiate(_rarePowerupPrefabs[Random.Range(0, _rarePowerupPrefabs.Length)], spawnPos, Quaternion.identity);

            yield return new WaitForSeconds(Random.Range(6f, 9f));
        }
    }

    public void OnPlayerDeath()
    {
        _stopSpawning = true;
    }

    public void SpawnExplosion(Vector3 position)
    {
        GameObject explosion = Instantiate(_explosionPrefab, position, Quaternion.identity);
        Destroy(explosion, 2.63f);
    }
}
