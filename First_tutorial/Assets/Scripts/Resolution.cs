using UnityEngine;

public class Resolution : MonoBehaviour
{
    // Update is called once per frame
    void Update()
    {
        var camera = GetComponent<Camera>();
        var rect = camera.rect;

        var scaleheight = ((float)Screen.width / (float)Screen.height) / (9f / 16f);
        var scalewidth = 1f / scaleheight; // 1 / 1
        if(scaleheight < 1f)
        {
            rect.height = scaleheight; //
            rect.y = (1f - scaleheight) / 2f;
        }
        else
        {
            rect.width = scalewidth; // 1
            rect.x = (1f - scalewidth) / 2f;
        }
        camera.rect = rect;

    }
}
