using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Explosive : Collectable
{
    [SerializeField] float _rotateSpeed = 10f;
    [SerializeField] GameObject _explosionPrefab;

    protected override void InheritedUpdate()
    {
        transform.Rotate(_rotateSpeed * Time.deltaTime * Vector3.forward, Space.World);
    }

    protected override void OnCollected(Collider2D other)
    {
        Player player = other.GetComponent<Player>();

        player.TakeDamage();

        gameObject.SetActive(false);

        GameObject explosion = Instantiate(_explosionPrefab, transform.position, Quaternion.identity);
        
        Destroy(explosion, 2.63f);
        Destroy(gameObject, 1f);
    }
}
