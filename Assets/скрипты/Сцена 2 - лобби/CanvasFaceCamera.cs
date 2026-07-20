using UnityEngine;

public class CanvasFaceCamera : MonoBehaviour
{
    void LateUpdate()
    {
        Camera cam = Camera.main;
        if (cam != null)
        {
            // Поворачиваем Canvas лицом к камере
            transform.LookAt(transform.position + cam.transform.rotation * Vector3.forward,
                           cam.transform.rotation * Vector3.up);
        }
    }
}