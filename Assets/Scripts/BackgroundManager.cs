using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackgroundManager : MonoSingleton<BackgroundManager>
{
    [SerializeField] GameObject[] _backgroundObjects;
    [SerializeField] float _speed = 1;
    [SerializeField] float _boundsY;

    void Update()
    {
        for (int i = 0; i < _backgroundObjects.Length; i++)
        {
            GameObject backgroundImage = _backgroundObjects[i];
            backgroundImage.transform.Translate(_speed * Time.deltaTime * Vector2.down);
            if (backgroundImage.transform.position.y < -_boundsY)
                backgroundImage.transform.position = new (0, _boundsY);
        }
    }
}
