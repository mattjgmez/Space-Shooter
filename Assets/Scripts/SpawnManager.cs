using System.Collections;
using UnityEngine;

public class SpawnManager : MonoSingleton<SpawnManager>
{
    [SerializeField] float bounds_X;
    [SerializeField] GameObject _enemyPrefab, _enemyContainer;
    [SerializeField] GameObject[] _PowerupPrefabs;

    bool _stopSpawning = false;

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

            yield return new WaitForSeconds(3);
        }
    }

    IEnumerator SpawnPowerup()
    {
        yield return new WaitForSeconds(3f);

        //Spawns a powerup every 6 to 8 seconds.
        while (!_stopSpawning)
        {
            Vector3 spawnPos = new (Random.Range(-15, 15), 10, 0);
            Instantiate(_PowerupPrefabs[Random.Range(0, _PowerupPrefabs.Length)], spawnPos, Quaternion.identity);

            yield return new WaitForSeconds(Random.Range(6, 9));
        }
    }

    public void OnPlayerDeath()
    {
        _stopSpawning = true;
    }
}
