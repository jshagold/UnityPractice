using UnityEngine;

[ExecuteAlways]
public class FullScreenBackground : MonoBehaviour
{
    void OnValidate()
    {
        if (!Application.isPlaying)
            FitToCamera();
    }
    void Awake() => FitToCamera();

    void FitToCamera()
    {
        var sr = GetComponent<SpriteRenderer>();
        var cam = Camera.main;
        float h = cam.orthographicSize * 2f;
        float w = h * cam.aspect;
        var s = sr.sprite.bounds.size;
        transform.localScale = new Vector3(w / s.x, h / s.y, 1f);
    }
}