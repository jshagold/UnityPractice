using System.Collections;
using UnityEngine;


[RequireComponent(typeof(Camera))] // Camera객체에만 script를 사용할수 있게 하는 옵션
public class CameraShake : MonoBehaviour
{
    Camera camera; // cache
    Vector3 originPos;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        camera = GetComponent<Camera>();
        originPos = camera.transform.localPosition;
    }

    public void Shake(float second, float magnitude)
    {
        StopCoroutine("C_Shake");
        ResetPosition();
        StartCoroutine(C_Shake(second, magnitude));
    }

    private IEnumerator C_Shake(float second, float magnitude)
    {
        float time = 0;
        while(true)
        {
            if(time >= second)
            {
                break;
            }

            Vector3 offset = Random.insideUnitCircle * magnitude;
            transform.localPosition = new Vector3(offset.x, offset.y, 0f);

            time += Time.deltaTime;
            yield return null;
        }

        transform.localPosition = originPos;
    }

    private void ResetPosition()
    {
        transform.localPosition = originPos;
    }
}
