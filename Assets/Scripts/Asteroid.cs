using UnityEngine;

public class Asteroid : MonoBehaviour
{
    [SerializeField] float _rotateSpeed = 0.3f;
    [SerializeField] GameObject _explosionPrefab;

    void Update()
    {
        transform.Rotate(_rotateSpeed * Time.deltaTime * Vector3.forward);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Projectile")
        {
            gameObject.SetActive(false);

            GameObject explosion = Instantiate(_explosionPrefab, transform.position, Quaternion.identity);

            Destroy(other.gameObject);
            Destroy(explosion, 2.63f);

            SpawnManager.Instance.StartSpawning();

            Destroy(gameObject, 1f);
        }
    }
}
