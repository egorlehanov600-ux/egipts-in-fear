using UnityEngine;
using Photon.Pun;

public class PlayerController : MonoBehaviourPun, IPunObservable
{
    private Vector3 networkPosition;
    private Quaternion networkRotation;
    private float smoothTime = 0.1f;
    private Vector3 positionVelocity;

    void Awake()
    {
        Debug.Log($" PlayerController Awake. IsMine: {photonView.IsMine}");
    }

    void Start()
    {
        Debug.Log($" PlayerController Start. IsMine: {photonView.IsMine}");
        
        if (photonView.IsMine)
        {
            // Локальный игрок - включаем ВСЕ компоненты
            MonoBehaviour[] allComponents = GetComponentsInChildren<MonoBehaviour>(true);
            foreach (var comp in allComponents)
            {
                if (comp != this && !(comp is PhotonView))
                {
                    comp.enabled = true;
                    Debug.Log($"  ✅ Включён: {comp.GetType().Name}");
                }
            }
            
            // Активируем камеру если есть
            Camera[] cameras = GetComponentsInChildren<Camera>(true);
            foreach (Camera cam in cameras)
            {
                cam.gameObject.SetActive(true);
                cam.enabled = true;
                Debug.Log($"   Камера активирована: {cam.name}");
            }
            
            Debug.Log("✅ Локальный игрок полностью активирован");
        }
        else
        {
            // Чужой игрок - отключаем управление
            MonoBehaviour[] allComponents = GetComponentsInChildren<MonoBehaviour>(true);
            foreach (var comp in allComponents)
            {
                string typeName = comp.GetType().Name;
                if (typeName.Contains("Controller") || 
                    typeName.Contains("Input") || 
                    typeName.Contains("Camera"))
                {
                    comp.enabled = false;
                    Debug.Log($"  🔒 Отключён: {typeName}");
                }
            }
            
            Debug.Log("🔒 Удалённый игрок деактивирован");
        }
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(transform.position);
            stream.SendNext(transform.rotation);
        }
        else
        {
            networkPosition = (Vector3)stream.ReceiveNext();
            networkRotation = (Quaternion)stream.ReceiveNext();
        }
    }

    void Update()
    {
        if (!photonView.IsMine)
        {
            transform.position = Vector3.SmoothDamp(transform.position, networkPosition, ref positionVelocity, smoothTime);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, networkRotation, 10f);
        }
    }
}