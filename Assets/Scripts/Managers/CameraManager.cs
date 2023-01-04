using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraManager : MonoSingleton<CameraManager>
{
    public IEnumerator CameraShake(float duration)
    {
        Vector3 originalPos = transform.position;
        float timer = Time.time + duration;

        while (Time.time < timer)
        {
            float x = Random.Range(-.5f, .5f);
            float y = Random.Range(-.5f, .5f);
            transform.position = new Vector3(x, y, transform.position.z);
            yield return null;
        }

        transform.position = originalPos;
    }
}
