using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MineLayerEnemy : Enemy
{
    bool _moveRight;

    protected override void HandleMovement()
    {
        if (transform.position.y > _bounds_Y)
            transform.Translate(-transform.up * (_speed / 2) * Time.deltaTime);
        else
        {
            transform.Translate((_moveRight ? transform.right : -transform.right) * _speed * Time.deltaTime);

            if (transform.position.x < -_bounds_X)
                _moveRight = true;
            if (transform.position.x > _bounds_X)
                _moveRight = false;
        }
    }

    protected override void FireWeapon()
    {
        if (transform.position.y <= _bounds_Y)
            base.FireWeapon();
    }
}
