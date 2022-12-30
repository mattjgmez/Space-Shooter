using UnityEngine;

public class Laser : MonoBehaviour
{
    [SerializeField] float _speed;
    [SerializeField] float _bounds_Y;
    [SerializeField] bool _isEnemy;
    [SerializeField] bool _isBeam;

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
        transform.Translate((_isEnemy ? -transform.up : transform.up) * _speed * Time.deltaTime, Space.World);

        if (transform.position.y < -_bounds_Y || transform.position.y > _bounds_Y)
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
            if (!_isBeam)
                Destroy(gameObject);
        }

        if (other.tag == "Powerup" && _isEnemy)
        {
            SpawnManager.Instance.SpawnExplosion(other.transform.position);
            Destroy(other.gameObject);
            Destroy(gameObject);
        }
    }
}
