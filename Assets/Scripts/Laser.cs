using UnityEngine;

public class Laser : MonoBehaviour
{
    [SerializeField] float _speed;
    [SerializeField] float _maxY;
    [SerializeField] bool _isEnemy;

    Player _player;

    private void Start()
    {
        _player = GameObject.Find("Player").GetComponent<Player>();

        if (_player == null)
            Debug.LogError("Player is NULL.");
    }

    void Update()
    {
        HandleMovement();
    }

    void HandleMovement()
    {
        transform.Translate(_speed * Time.deltaTime * (_isEnemy ? Vector2.down : Vector2.up));

        if (transform.position.y < -_maxY || transform.position.y > _maxY)
        {
            if (transform.parent != null)
                Destroy(transform.parent.gameObject);

            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Player" && _isEnemy)
        {
            _player.TakeDamage();
            Destroy(gameObject);
        }
    }
}
