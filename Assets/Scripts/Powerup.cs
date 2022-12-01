using UnityEngine;

public class Powerup : MonoBehaviour
{
    [SerializeField] float _speed = 3;
    [SerializeField] float _minY = -10;
    [SerializeField] int _powerupID; //0 = Triple Shot, 1 = Speed Up, 2 = Shield 
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

            switch (_powerupID)
            {
                default:
                    Debug.Log("Invalid powerupID");
                    break;

                case 0:
                    player.TripleShotEnable();
                    break;

                case 1:
                    player.SpeedUpEnable();
                    break;

                case 2:
                    player.ShieldEnable();
                    break;
            }

            AudioSource.PlayClipAtPoint(_clip, transform.position);
            Destroy(gameObject);
        }
    }
}
