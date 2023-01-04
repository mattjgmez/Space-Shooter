using System;
using UnityEngine;

[ExecuteInEditMode]

public class Boxcaster : MonoBehaviour
{
    [SerializeField] bool _isArray;
    [SerializeField] Rect _box;
    [SerializeField] Vector2 _direction;
    [SerializeField] float _distance;
    [SerializeField] LayerMask _mask;

    bool _hit;
    RaycastHit2D[] _hits;
    Vector2 _position;

    public bool Hit { get { return _hit; } }
    public RaycastHit2D[] Hits { get { return _hits; } }

#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        if (_hits != null && _isArray)
        {
            foreach (RaycastHit2D hit in _hits)
            {
                Gizmos.color = Color.red;
                Gizmos.DrawWireSphere(hit.collider.transform.position, 1f);
            }
        }

        Gizmos.color = Color.green;
        Gizmos.matrix = Matrix4x4.TRS(_position, transform.rotation, Vector3.one);
        Gizmos.DrawWireCube(Vector2.zero, _box.size);

        Gizmos.matrix = Matrix4x4.TRS(_position + (_direction.normalized * _distance), transform.rotation, Vector3.one);
        Gizmos.DrawWireCube(Vector2.zero, _box.size);

        Gizmos.color = _hit ? Color.red : Color.cyan;
        Gizmos.matrix = Matrix4x4.TRS(_position, Quaternion.identity, Vector3.one);
        Gizmos.DrawLine(Vector2.zero, _direction.normalized * _distance);
    }
#endif

    private void Update()
    {
        _direction = -transform.up;
        _position = (Vector2)transform.position;

        if (_isArray) _hits = Physics2D.BoxCastAll(_position, _box.size, transform.eulerAngles.z, _direction, _distance, _mask);
        else _hit = Physics2D.BoxCast(_position, _box.size, transform.eulerAngles.z, _direction, _distance, _mask);
    }

    public void ClearHits()
    {
        Array.Clear(_hits, 0, _hits.Length);
    }
}
