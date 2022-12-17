using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HalfcircleEnemy : Enemy
{
    [SerializeField] bool _moveRight;
    [SerializeField] float _rotationSpeed;

    protected override void HandleMovement()
    {
        transform.Translate((_moveRight ? transform.right : -transform.right) * _speed * Time.deltaTime);

        _rigidbody.angularVelocity = _moveRight ? -_rotationSpeed : _rotationSpeed;

        if (transform.position.x < -_bounds_X)
            _moveRight = true;
        if (transform.position.x > _bounds_X)
            _moveRight = false;
    }
}
