using UnityEngine;

public class Projectile : MonoBehaviour
{
    [SerializeField] float _speed;
    [SerializeField] float _maxY;
    [SerializeField] bool _isEnemy;

    void Update()
    {
        transform.Translate(_speed * Time.deltaTime * (_isEnemy ? Vector2.down : Vector2.up));

        if (transform.position.y > _maxY)
        {
            if (transform.parent != null)
                Destroy(transform.parent.gameObject);

            Destroy(gameObject);
        }
    }
}
