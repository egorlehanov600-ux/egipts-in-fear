using UnityEngine;
using Photon.Pun;
using TMPro;
using ExitGames.Client.Photon;

public class CharacterSelectionManager : MonoBehaviourPunCallbacks
{
    [Header("Капсула игрока")]
    public GameObject playerCapsule; // Капсула по центру
    public Renderer capsuleRenderer; // Renderer капсулы (Mesh Renderer)

    [Header("Кнопки выбора класса (3D кубы)")]
    public GameObject btnInni;    // Куб кнопка "Ини-херит"
    public GameObject btnSara;    // Куб кнопка "Сара"  
    public GameObject btnMadu;    // Куб кнопка "Маду"

    [Header("Текст описания (3D текст)")]
    public TextMeshPro descriptionText; // Текст слева с описанием

    [Header("Кнопка 'Выбрать' (3D куб)")]
    public GameObject btnSelect; // Куб кнопка "Выбрать"

    [Header("Настройки классов")]
    // Порядок важен: 0 - Ини-херит, 1 - Сара, 2 - Маду
    public string[] classNames = { "Ини-херит", "Сара", "Маду" };
    public string[] classRoles = { "Мечник", "Лучник", "Вор" };
    public string[] classDescriptions = { 
        "Танк/Ближний бой", 
        "ДД/Снайпер", 
        "Стелс/Обезвреживание ловушек" 
    };
    public Color[] classColors = { Color.red, Color.green, Color.blue };

    private int selectedIndex = 0;

    void Start()
    {
        // Подключаемся к Photon если еще не подключены
        if (!PhotonNetwork.IsConnected)
        {
            PhotonNetwork.ConnectUsingSettings();
        }

        // Настраиваем клики по 3D кнопкам
        SetupClickHandlers();

        // Применяем начальный класс (Ини-херит)
        ApplyClass(0);
    }

    void SetupClickHandlers()
    {
        // Назначаем обработчики для кнопок классов (индексы 0, 1, 2)
        AddClickHandler(btnInni, 0);
        AddClickHandler(btnSara, 1);
        AddClickHandler(btnMadu, 2);
        
        // Назначаем обработчик для кнопки "Выбрать" (-1)
        AddClickHandler(btnSelect, -1); 
    }

    void AddClickHandler(GameObject obj, int classIndex)
    {
        if (obj == null) return;
        
        // Добавляем коллайдер, если его нет (чтобы можно было кликнуть)
        if (obj.GetComponent<Collider>() == null)
        {
            obj.AddComponent<BoxCollider>();
        }
        
        // Добавляем или получаем скрипт клика
        ClickableButton clickScript = obj.GetComponent<ClickableButton>();
        if (clickScript == null)
        {
            clickScript = obj.AddComponent<ClickableButton>();
        }
        
        // Привязываем действие к кнопке
        clickScript.onClick = () => OnButtonClicked(classIndex);
    }

    void OnButtonClicked(int index)
    {
        if (index == -1)
        {
            // Нажата кнопка "Выбрать" -> сохраняем и переходим в лобби
            SelectCharacter();
        }
        else
        {
            // Нажата кнопка класса -> меняем превью
            selectedIndex = index;
            ApplyClass(index);
        }
    }

    void ApplyClass(int index)
    {
        // Меняем цвет капсулы
        if (capsuleRenderer != null && index < classColors.Length)
        {
            capsuleRenderer.material.color = classColors[index];
        }

        // Меняем текст описания (Имя + Роль + Описание)
        if (descriptionText != null && index < classNames.Length)
        {
            descriptionText.text = $"{classNames[index]} ({classRoles[index]})\n\n{classDescriptions[index]}";
        }

        Debug.Log($"Выбран класс: {classNames[index]} ({classRoles[index]})");
    }

    void SelectCharacter()
    {
        Debug.Log($"✅ Игрок выбрал: {classNames[selectedIndex]}");
        
        // Сохраняем выбор в свойствах игрока (чтобы передать в лобби)
        Hashtable props = new Hashtable
        {
            { "selectedClass", selectedIndex },
            { "className", classNames[selectedIndex] },
            { "classRole", classRoles[selectedIndex] }
        };
        
        PhotonNetwork.LocalPlayer.SetCustomProperties(props);
        
        // Переход в сцену лобби (убедись, что сцена добавлена в Build Settings и называется "лобби")
        PhotonNetwork.LoadLevel("лобби"); 
    }

    // --- Photon Callbacks ---

    public override void OnConnectedToMaster()
    {
        Debug.Log("✅ Подключено к Photon Master Server");
    }

    public override void OnJoinedRoom()
    {
        Debug.Log("✅ Игрок успешно зашел в комнату");
    }
}

// Вспомогательный скрипт для обработки кликов по 3D объектам
public class ClickableButton : MonoBehaviour
{
    public System.Action onClick;

    // Срабатывает при клике мышкой по объекту с коллайдером
    void OnMouseDown()
    {
        if (onClick != null)
        {
            onClick();
        }
    }
}