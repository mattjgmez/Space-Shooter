using UnityEngine;

public class Missile : MonoBehaviour
{
    [SerializeField] float _speed = 15, _rotationSpeed;
    [SerializeField] float _maxY = 9;

    Transform _target = null;
    Rigidbody2D _rb;

    void Start()
    {
        _rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        if (transform.position.y > _maxY || transform.position.y < -_maxY)
            Destroy(gameObject);

        if (!_target && SpawnManager.Instance.ActiveEnemies.Count > 0)
            FindTarget();
    }

    void FixedUpdate()
    {
        _rb.velocity = transform.up * _speed;

        if (_target)
            HandleRotation();
    }

    void HandleRotation()
    {
        //Get the direction of the target 
        Vector2 targetDirection = (Vector2)_target.position - _rb.position;
        targetDirection.Normalize();

        //Rotate toward target 
        float rotation = Vector3.Cross(targetDirection, transform.up).z;
        _rb.angularVelocity = -rotation * _rotationSpeed;
    }

    void FindTarget()
    {
        float shortestDistance = 100f;

        foreach (GameObject enemy in SpawnManager.Instance.ActiveEnemies)
        {
            float distance = Vector3.Distance(transform.position, enemy.transform.position);
            if (distance < shortestDistance)
            {
                _target = enemy.transform;
                shortestDistance = distance;
            }
        }
    }
}
