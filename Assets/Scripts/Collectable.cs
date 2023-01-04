using UnityEngine;

public class Collectable : MonoBehaviour
{
    [SerializeField] protected float _speed = 3;
    [SerializeField] float _bounds_Y = -10;
    //0 = Triple Shot, 1 = Speed Up, 2 = Shield, 3 = Ammo, 4 = Health, 5 = Missiles, 6 = Explosive
    [SerializeField] int _collectableID; 
    [SerializeField] AudioClip _audioClip;

    protected GameObject _player;

    protected virtual void Start()
    {
        _player = GameObject.Find("Player");
    }

    protected virtual void Update()
    {
        if (Input.GetKey(KeyCode.C) && Vector3.Distance(transform.position, _player.transform.position) < 4f) 
            MoveToPlayer(); 
        else 
            HandleMovement();

        if (transform.position.y < _bounds_Y)
        {
            if (transform.parent != null)
                Destroy(transform.parent.gameObject);
            Destroy(gameObject);
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        OnCollected(other);
    }

    protected virtual void HandleMovement()
    {
        transform.Translate(_speed * Time.deltaTime * Vector3.down, Space.World);
    }

    protected virtual void MoveToPlayer()
    {
        transform.Translate(_speed * 1.25f * Time.deltaTime * -(transform.position - _player.transform.position), Space.World);
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
}
