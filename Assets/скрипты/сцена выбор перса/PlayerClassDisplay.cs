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
    public TextMeshPro classNameText; // 3D текст над головой

    private Renderer characterRenderer;

    void Awake()
    {
        // Автоматически ищем рендерер модели (даже если он на дочернем объекте)
        characterRenderer = GetComponentInChildren<Renderer>();
    }

    void Start()
    {
        Photon.Realtime.Player targetPlayer = photonView.IsMine ? PhotonNetwork.LocalPlayer : photonView.Owner;
        ApplyClassFromProperties(targetPlayer);
    }

    void ApplyClassFromProperties(Photon.Realtime.Player player)
    {
        if (player.CustomProperties.ContainsKey("selectedClass"))
        {
            int classIndex = (int)player.CustomProperties["selectedClass"];
            
            // 1. Красим модель персонажа
            if (characterRenderer != null && classIndex < classColors.Length)
            {
                characterRenderer.material.color = classColors[classIndex];
            }

            // 2. Показываем имя класса над головой
            if (classNameText != null && classIndex < classNames.Length)
            {
                classNameText.text = $"{classNames[classIndex]}\n({classRoles[classIndex]})";
            }
        }
    }
}