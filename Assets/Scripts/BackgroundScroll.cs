using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackgroundScroll : MonoBehaviour
{
    [SerializeField] GameObject[] _backgroundObjects;
    [SerializeField] float _speed = 1;
    [SerializeField] float _bounds_Y = 18.25f;

    void Update()
    {
        for (int i = 0; i < _backgroundObjects.Length; i++)
        {
            GameObject backgroundImage = _backgroundObjects[i];
            backgroundImage.transform.Translate(_speed * Time.deltaTime * Vector2.down);
            if (backgroundImage.transform.position.y < -_bounds_Y)
                backgroundImage.transform.position = new (0, _bounds_Y);
        }
    }
}
