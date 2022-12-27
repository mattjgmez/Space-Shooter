using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SmartMine : Collectable
{
    [SerializeField] float _rotateSpeed = 10f;
    [SerializeField] float _targetingRange = 5f;

    bool _targetFound;
    float _targetDistance;
    Vector3 _targetPosition;
    GameObject _player;

    void Start()
    {
        _player = GameObject.Find("Player");

        StartCoroutine(FindTargetPosition());
    }

    IEnumerator FindTargetPosition()
    {
        while (!_targetFound)
        {
            _targetDistance = Vector3.Distance(transform.position, _player.transform.position);

            _targetFound = _targetDistance < _targetingRange;

            yield return null;
        }

        _targetPosition = _player.transform.position;
    }

    protected override void HandleMovement()
    {
        transform.Rotate(_rotateSpeed * Time.deltaTime * Vector3.forward, Space.World);

        if (_targetFound)
        {
            transform.Translate(_speed * 1.25f * Time.deltaTime * -(transform.position - _targetPosition), Space.World);

            if (Vector3.Distance(transform.position, _targetPosition) < 1)
                TriggerDeath();
        }
        else
            base.HandleMovement();
    }

    protected override void OnCollected(Collider2D other)
    {
        if (other.tag == "Projectile")
        {
            Destroy(other.gameObject);
            TriggerDeath();
            return;
        }

        if (other.tag == "Player")
        {
            Player player = other.GetComponent<Player>();

            player.TakeDamage();

            TriggerDeath();
        }
    }

    private void TriggerDeath()
    {
        gameObject.SetActive(false);

        SpawnManager.Instance.SpawnExplosion(transform.position);

        Destroy(gameObject, 1f);
    }
}
