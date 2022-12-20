using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class BeamEnemy : Enemy
{
    [SerializeField] float _beamDelay = 0.75f;
    [SerializeField] float _rotationSpeed;
    [SerializeField] BoxCollider2D _beamCollider;

    bool _firingWeapon;

    protected override void InheritedUpdate()
    {
        if (!_firingWeapon && _player == null) HandleRotation();
        else _rigidbody.angularVelocity = 0;
    }

    void HandleRotation()
    {
        //Get the direction of the target 
        Vector2 targetDirection = (Vector2)_player.transform.position - _rigidbody.position;
        targetDirection.Normalize();

        //Rotate toward target 
        float rotation = Vector3.Cross(targetDirection, transform.up).z;
        _rigidbody.angularVelocity = rotation * _rotationSpeed;
    }

    protected override void HandleMovement()
    {
        if (transform.position.y < _bounds_Y)
            return;

        transform.Translate(Vector3.down * _speed * Time.deltaTime);
    }

    protected override void FireWeapon()
    {
        StartCoroutine(BeamCoroutine());
    }

    IEnumerator BeamCoroutine()
    {
        _weaponCooldown = Time.time + Random.Range(3f, 7f);
        _audioSource.PlayOneShot(_laserSound);
        _firingWeapon = true;

        _enemyProjectile.transform.localScale = new Vector3(.1f, _enemyProjectile.transform.localScale.y, 1);
        yield return new WaitForSeconds(_beamDelay);
        _enemyProjectile.transform.localScale = new Vector3(2, _enemyProjectile.transform.localScale.y, 1);
        _beamCollider.enabled = true;
        yield return new WaitForSeconds(.25f);
        _firingWeapon = false;
        _enemyProjectile.transform.localScale = new Vector3(0, _enemyProjectile.transform.localScale.y, 1);
        _beamCollider.enabled = false;
    }
}
