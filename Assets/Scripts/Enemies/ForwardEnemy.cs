using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ForwardEnemy : Enemy
{
    [SerializeField] protected Boxcaster _obstacleDetection;

    protected override void HandleMovement()
    {
        if (_obstacleDetection.Hit)
        {
            bool dodgeDirection = _player.transform.position.x > transform.position.x; //True if player is to the right
            transform.Translate((-transform.up + (dodgeDirection ? -transform.right : transform.right)) * _speed * Time.deltaTime);
        }
        else
            transform.Translate(-transform.up * _speed * Time.deltaTime);

        if (transform.position.y < -_bounds_Y)
            transform.position = new Vector2(Random.Range(-_bounds_X, _bounds_X), _bounds_Y);
    }
}
