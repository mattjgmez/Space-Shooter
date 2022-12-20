using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnManager : MonoSingleton<SpawnManager>
{
    [SerializeField] float bounds_X = 14;
    [SerializeField] GameObject[] _enemyPrefab;
    [SerializeField] GameObject _enemyContainer;
    [SerializeField] GameObject[] _powerupPrefabs;
    [SerializeField] GameObject[] _rarePowerupPrefabs;

    bool _stopSpawning = false;
    int _currentWave = 0;
    int _spawnedEnemies = 0;
    float _spawnDelay = 5f;
    Vector3 _spawnPosition;
    Quaternion _spawnRotation;

    public List<GameObject> ActiveEnemies;

    public void StartSpawning()
    {
        StartWave();

        StartCoroutine(SpawnPowerup());
    }

    void StartWave()
    {
        _currentWave++;
        _spawnedEnemies = 0;
        UIManager.Instance.UpdateWaveText(_currentWave);

        //Set wave enemy limit and spawn delay exponentially
        int enemyAmount = (int)((_currentWave + 3) * Mathf.Pow(1 + 0.25f, 2));
        _spawnDelay = Mathf.Clamp(_spawnDelay * Mathf.Pow(1 - .1f, 2), 0.5f, 3f);

        StartCoroutine(SpawnEnemyCoroutine(enemyAmount));
    }

    IEnumerator SpawnEnemyCoroutine(int enemyAmount)
    {
        yield return new WaitForSeconds(3);

        while (!_stopSpawning && _spawnedEnemies < enemyAmount)
        {
            _spawnedEnemies++;
            int enemyType = Random.Range(0, _enemyPrefab.Length);

            SpawnEnemy(enemyType);

            yield return new WaitForSeconds(_spawnDelay);
        }

        while (!_stopSpawning && ActiveEnemies.Count > 0) //Prevents next wave from spawning until all active enemies are slain
            yield return null;

        StartWave();
    }

    void SpawnEnemy(int enemyType)
    {
        //Spawns an enemy based on rolled enemy type
        switch (enemyType)
        {
            case 0: //Basic enemy
                _spawnPosition = new(Random.Range(-bounds_X, bounds_X), 10, 0);
                GameObject newEnemy =
                    Instantiate(_enemyPrefab[enemyType], _spawnPosition, Quaternion.identity, _enemyContainer.transform);
                newEnemy.transform.parent = _enemyContainer.transform;

                //Adds spawned enemy to list of active enemies.
                ActiveEnemies.Add(newEnemy);
                break;

            case 1: //Half circle enemy
                if (Random.Range(0, 2) == 0)//Spawns enemy on random side of screen
                {
                    _spawnPosition = new(17, 2.5f, 0);
                    _spawnRotation = Quaternion.Euler(0, 0, -14.3f);
                }
                else
                {
                    _spawnPosition = new(-17, 2.5f, 0);
                    _spawnRotation = Quaternion.Euler(0, 0, 14.3f);
                }
                newEnemy =
                    Instantiate(_enemyPrefab[enemyType], _spawnPosition, _spawnRotation, _enemyContainer.transform);
                newEnemy.transform.parent = _enemyContainer.transform;

                ActiveEnemies.Add(newEnemy);
                break;

            case 2: //Beam enemy
                _spawnPosition = new(Random.Range(-bounds_X, bounds_X), 10, 0);
                newEnemy =
                    Instantiate(_enemyPrefab[enemyType], _spawnPosition, Quaternion.identity, _enemyContainer.transform);
                newEnemy.transform.parent = _enemyContainer.transform;

                //Adds spawned enemy to list of active enemies.
                ActiveEnemies.Add(newEnemy);
                break;
        }
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
}
