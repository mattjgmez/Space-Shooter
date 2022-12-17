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
    Vector3 _spawnPosition;
    Quaternion _spawnRotation;

    public List<GameObject> ActiveEnemies;

    public void StartSpawning()
    {
        StartCoroutine(SpawnEnemy());
        StartCoroutine(SpawnPowerup());
    }

    IEnumerator SpawnEnemy()
    {
        yield return new WaitForSeconds(3);

        //Spawns an enemy every 5 seconds.
        while (!_stopSpawning)
        {
            int enemyType = Random.Range(0, _enemyPrefab.Length);
            switch (enemyType)
            {
                case 0: //Spawns enemy in appropriate spawn area
                    _spawnPosition = new(Random.Range(-bounds_X, bounds_X), 10, 0);
                    GameObject newEnemy = 
                        Instantiate(_enemyPrefab[enemyType], _spawnPosition, Quaternion.identity, _enemyContainer.transform);
                    newEnemy.transform.parent = _enemyContainer.transform;

                    //Adds spawned enemy to list of active enemies.
                    ActiveEnemies.Add(newEnemy);
                    break;

                case 1: //Spawns side scrolling enemy on random screen side
                    if (Random.Range(0, 2) == 0)
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
            }

            yield return new WaitForSeconds(3);
        }
    }

    IEnumerator SpawnPowerup()
    {
        yield return new WaitForSeconds(3);

        //Spawns a powerup every 6 to 8 seconds.
        while (!_stopSpawning)
        {
            int nextPowerup = Random.Range(0, 100);
            Debug.Log(nextPowerup);
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
