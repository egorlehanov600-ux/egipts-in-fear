using UnityEngine;
using Photon.Pun;

public class PlayerSync : MonoBehaviourPun
{
    [Header("Цвета классов")]
    public Color[] classColors = { Color.red, Color.green, Color.blue };

    void Start()
    {
        // Применяем цвет только для локального игрока
        if (photonView.IsMine)
        {
            ApplyColorFromSelection();
        }
    }

    void ApplyColorFromSelection()
    {
        // Читаем выбранный класс из Custom Properties
        if (PhotonNetwork.LocalPlayer.CustomProperties.ContainsKey("selectedClass"))
        {
            int classIndex = (int)PhotonNetwork.LocalPlayer.CustomProperties["selectedClass"];
            
            if (classIndex >= 0 && classIndex < classColors.Length)
            {
                Renderer rend = GetComponent<Renderer>();
                if (rend != null)
                {
                    rend.material.color = classColors[classIndex];
                }
                
                Debug.Log($"Применён цвет класса {classIndex}");
            }
        }
    }
}