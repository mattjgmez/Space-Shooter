using UnityEngine;

public class SmartEnemy : Enemy
{
    [SerializeField] float _turretRotationSpeed;
    [SerializeField] GameObject _turretObject;
    [SerializeField] protected Boxcaster _obstacleDetection;

    protected override void Update()
    {
        base.Update();
        if (_player != null) HandleTurretRotation();
    }

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

    protected override void FireWeapon()
    {
        _weaponCooldown = Time.time + Random.Range(3f, 5f);
        Instantiate(_enemyProjectile, _turretObject.transform.position, _turretObject.transform.rotation);
        _audioSource.PlayOneShot(_laserSound);
    }

    void HandleTurretRotation()
    {
        //Get the direction of the target 
        Vector2 targetDirection = _player.transform.position - _turretObject.transform.position;
        targetDirection.Normalize();

        //Rotate toward target 
        float rotation = Mathf.Atan2(targetDirection.y, targetDirection.x) * Mathf.Rad2Deg;
        _turretObject.transform.rotation = Quaternion.Euler(0, 0, rotation + 90);
    }
}
