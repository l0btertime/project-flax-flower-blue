using UnityEngine;
using System.Collections;

public class ScreenShake : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        //if(Input.GetMouseButtonDown(0)) StartCoroutine(Shake(0.1f, 0.04f, 0.5f));
    }

    public void Shake(float duration, float intensity)
    {
        StartCoroutine(ShakeCoroutine(duration, 0.04f, intensity));
    }
    private IEnumerator ShakeCoroutine(float duration, float interval, float startSize)
    {
        float timer = duration;
        
        while (timer > 0)
        {
            float size = Mathf.Lerp(0, startSize, (timer / duration));
            transform.position = PosInCube(size);
            float time = Random.Range(0f, interval);
            timer -= time;
            yield return new WaitForSeconds(time);
        }
    }

    private Vector3 PosInCube(float size)
    {
        return new Vector3(Random.Range(-0.5f * size, 0.5f * size), Random.Range(-0.5f * size, 0.5f * size), Random.Range(-0.5f * size, 0.5f * size));
    }
}
