using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraManager : MonoSingleton<CameraManager>
{
    public IEnumerator CameraShake()
    {
        Vector3 originalPos = transform.position;
        float timer = 0f;

        while (timer < .25f)
        {
            float x = Random.Range(-.5f, .5f);
            float y = Random.Range(-.5f, .5f);
            transform.position = new Vector3(x, y, transform.position.z);

            timer += Time.deltaTime;
            yield return null;
        }

        transform.position = originalPos;
    }
}
