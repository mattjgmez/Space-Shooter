using UnityEngine;

public class Asteroid : MonoBehaviour
{
    [SerializeField] float _rotateSpeed = 0.3f;

    void Update()
    {
        transform.Rotate(_rotateSpeed * Time.deltaTime * Vector3.forward);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Projectile")
        {
            gameObject.SetActive(false);

            SpawnManager.Instance.SpawnExplosion(transform.position);
            Destroy(other.gameObject);

            SpawnManager.Instance.StartSpawning();

            Destroy(gameObject, 1f);
        }
    }
}
