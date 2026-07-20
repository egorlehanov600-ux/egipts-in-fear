using UnityEngine;
using Photon.Pun;

public class PlayerCamera : MonoBehaviour
{
    public float distance = 5f;
    public float height = 2f;
    public float smoothSpeed = 5f;

    private Camera playerCamera;
    private Transform followTarget;

    void Start()
    {
        // Камера создается только для локального игрока
        if (GetComponent<PhotonView>().IsMine)
        {
            followTarget = transform;
            
            // Создаем камеру
            GameObject camObj = new GameObject("PlayerCam");
            playerCamera = camObj.AddComponent<Camera>();
            camObj.tag = "MainCamera"; // Важно для Unity
            
            // Смещение камеры
            camObj.transform.position = transform.position + new Vector3(0, height, -distance);
            camObj.transform.LookAt(transform);
        }
    }

    void LateUpdate()
    {
        if (playerCamera != null && followTarget != null)
        {
            Vector3 desiredPos = followTarget.position + new Vector3(0, height, -distance);
            playerCamera.transform.position = Vector3.Lerp(playerCamera.transform.position, desiredPos, smoothSpeed * Time.deltaTime);
            playerCamera.transform.LookAt(followTarget);
        }
    }
}