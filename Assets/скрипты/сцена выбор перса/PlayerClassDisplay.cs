using UnityEngine;
using Photon.Pun;
using TMPro;

public class PlayerClassDisplay : MonoBehaviourPun
{
    [Header("Настройки классов")]
    public string[] classNames = { "Ини-херит", "Сара", "Маду" };
    public string[] classRoles = { "Мечник", "Лучник", "Вор" };
    public Color[] classColors = { Color.red, Color.green, Color.blue };

    [Header("UI для отображения имени класса")]
    public TextMeshPro classNameText; // 3D текст над капсулой

    void Start()
    {
        if (photonView.IsMine)
        {
            // Для локального игрока читаем из своих свойств
            ApplyClassFromProperties(PhotonNetwork.LocalPlayer);
        }
        else
        {
            // Для чужих игроков читаем из их свойств
            ApplyClassFromProperties(photonView.Owner);
        }
    }

    void ApplyClassFromProperties(Photon.Realtime.Player player)
    {
        if (player.CustomProperties.ContainsKey("selectedClass"))
        {
            int classIndex = (int)player.CustomProperties["selectedClass"];
            
            // Красим капсулу
            Renderer rend = GetComponent<Renderer>();
            if (rend != null && classIndex < classColors.Length)
            {
                rend.material.color = classColors[classIndex];
            }

            // Показываем имя класса над капсулой
            if (classNameText != null && classIndex < classNames.Length)
            {
                classNameText.text = $"{classNames[classIndex]}\n({classRoles[classIndex]})";
            }

            Debug.Log($"Игрок {player.NickName}: {classNames[classIndex]} ({classRoles[classIndex]})");
        }
    }
}