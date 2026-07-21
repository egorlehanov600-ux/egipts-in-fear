using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using TMPro;

public class CharacterSelector : MonoBehaviour
{
    [Header("UI Ссылки")]
    public TextMeshProUGUI classText;
    public Button btnLeft;
    public Button btnRight;

    [Header("Настройки классов")]
    public string[] classNames = { "Сара", "Маду", "Инни-Херрит" };
    public Color[] classColors = { Color.red, Color.green, Color.blue };

    private int currentIndex = 0;
    private PhotonView photonView;

    void Start()
    {
        photonView = GetComponent<PhotonView>();

        // Если это мой игрок - вешаем кнопки, если чужой - скрываем их
        if (photonView.IsMine)
        {
            if (btnLeft != null) btnLeft.onClick.AddListener(GoLeft);
            if (btnRight != null) btnRight.onClick.AddListener(GoRight);
        }
        else
        {
            if (btnLeft != null) btnLeft.gameObject.SetActive(false);
            if (btnRight != null) btnRight.gameObject.SetActive(false);
        }

        // Применяем начальный класс
        ApplyClass(currentIndex);
    }

    // ✅ PUBLIC - чтобы кнопки видели эти методы в On Click
    public void GoLeft()
    {
        currentIndex--;
        if (currentIndex < 0) currentIndex = classNames.Length - 1;

        // Синхронизируем выбор со всеми игроками
        photonView.RPC("RPC_UpdateClass", RpcTarget.All, currentIndex);
    }

    // ✅ PUBLIC - чтобы кнопки видели эти методы в On Click
    public void GoRight()
    {
        currentIndex++;
        if (currentIndex >= classNames.Length) currentIndex = 0;

        // Синхронизируем выбор со всеми игроками
        photonView.RPC("RPC_UpdateClass", RpcTarget.All, currentIndex);
    }

    [PunRPC]
    void RPC_UpdateClass(int newIndex)
    {
        currentIndex = newIndex;
        ApplyClass(currentIndex);
    }

    void ApplyClass(int index)
    {
        // Меняем текст в UI
        if (classText != null)
            classText.text = classNames[index];

        // Меняем цвет капсулы
        if (index < classColors.Length)
        {
            Renderer rend = GetComponent<Renderer>();
            if (rend != null)
            {
                rend.material.color = classColors[index];
            }
        }
    }
}