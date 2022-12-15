using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnManager : MonoSingleton<SpawnManager>
{
    [SerializeField] float bounds_X;
    [SerializeField] GameObject _enemyPrefab, _enemyContainer;
    [SerializeField] GameObject[] _powerupPrefabs;
    [SerializeField] GameObject[] _rarePowerupPrefabs;

    bool _stopSpawning = false;

    public List<GameObject> ActiveEnemies;

    public void StartSpawning()
    {
        StartCoroutine(SpawnEnemy());
        StartCoroutine(SpawnPowerup());
    }

    IEnumerator SpawnEnemy()
    {
        yield return new WaitForSeconds(3f);

        //Spawns an enemy every 5 seconds.
        while (!_stopSpawning)
        {
            Vector3 spawnPos = new (Random.Range(-bounds_X, bounds_X), 10, 0);
            GameObject newEnemy = Instantiate(_enemyPrefab, spawnPos, Quaternion.identity, _enemyContainer.transform);
            newEnemy.transform.parent = _enemyContainer.transform;

            //Adds spawned enemy to list of active enemies.
            ActiveEnemies.Add(newEnemy);

            yield return new WaitForSeconds(3);
        }
    }

    IEnumerator SpawnPowerup()
    {
        yield return new WaitForSeconds(3f);

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
