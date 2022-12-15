using UnityEngine;

public class Powerup : MonoBehaviour
{
    [SerializeField] float _speed = 3;
    [SerializeField] float _minY = -10;
    [SerializeField] int _powerupID; //0 = Triple Shot, 1 = Speed Up, 2 = Shield, 3 = Ammo, 4 = Health, 5 = Missiles
    [SerializeField] AudioClip _clip;

    void Update()
    {
        transform.Translate(_speed * Time.deltaTime * Vector2.down);

        if (transform.position.y < _minY)
            Destroy(gameObject);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Player")
        {
            Player player = other.GetComponent<Player>();

            player.EnablePowerup(_powerupID);

            AudioSource.PlayClipAtPoint(_clip, transform.position);
            Destroy(gameObject);
        }
    }
}
