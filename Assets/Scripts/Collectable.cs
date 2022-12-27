using UnityEngine;

public class Collectable : MonoBehaviour
{
    [SerializeField] protected float _speed = 3;
    [SerializeField] float _bounds_Y = -10;
    //0 = Triple Shot, 1 = Speed Up, 2 = Shield, 3 = Ammo, 4 = Health, 5 = Missiles, 6 = Explosive
    [SerializeField] int _collectableID; 
    [SerializeField] AudioClip _audioClip;

    void Update()
    {
        HandleMovement();

        if (transform.position.y < _bounds_Y)
            Destroy(gameObject);

        InheritedUpdate();//Used by inherited scripts to add onto Update    
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        OnCollected(other);
    }

    protected virtual void HandleMovement()
    {
        transform.Translate(_speed * Time.deltaTime * Vector3.down, Space.World);
    }

    protected virtual void OnCollected(Collider2D other)
    {
        if (other.tag == "Player")
        {
            Player player = other.GetComponent<Player>();

            player.TriggerCollectable(_collectableID);

            AudioSource.PlayClipAtPoint(_audioClip, transform.position);
            Destroy(gameObject);
        }
    }

    protected virtual void InheritedUpdate() { } 
}
