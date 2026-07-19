using UnityEngine;
using TMPro;

public class CharacterSelector : MonoBehaviour
{
    [System.Serializable]
    public class CharacterInfo
    {
        public GameObject character; // Куб персонажа (перетащи из Hierarchy)
    }

    public CharacterInfo[] characters;
    
    [Header("Кубы-кнопки")]
    public GameObject prevButton;
    public GameObject nextButton;
    
    private int currentIndex = 0;

    void Start()
    {
        // Проверка на пустой массив
        if (characters == null || characters.Length == 0)
        {
            Debug.LogError("Массив Characters пуст! Перетащи персонажей в Inspector.");
            return;
        }
        UpdateSelection();
    }

    public void OnPrevButtonClick()
    {
        currentIndex = (currentIndex - 1 + characters.Length) % characters.Length;
        UpdateSelection();
    }

    public void OnNextButtonClick()
    {
        currentIndex = (currentIndex + 1) % characters.Length;
        UpdateSelection();
    }

    void UpdateSelection()
    {
        for (int i = 0; i < characters.Length; i++)
        {
            // Проверка на null перед SetActive
            if (characters[i].character != null)
            {
                characters[i].character.SetActive(i == currentIndex);
            }
        }

        int previousIndex = (currentIndex - 1 + characters.Length) % characters.Length;
        int nextIndex = (currentIndex + 1) % characters.Length;

        // Берём имя прямо из названия объекта!
        string prevName = characters[previousIndex].character.name;
        string nextName = characters[nextIndex].character.name;

        UpdateButtonText(prevButton, prevName);
        UpdateButtonText(nextButton, nextName);
    }

    void UpdateButtonText(GameObject button, string name)
    {
        if (button == null) return;
        
        TextMeshProUGUI[] texts = button.GetComponentsInChildren<TextMeshProUGUI>();
        if (texts.Length > 0) texts[0].text = name;
        
        TextMeshPro[] texts3D = button.GetComponentsInChildren<TextMeshPro>();
        if (texts3D.Length > 0) texts3D[0].text = name;
    }
}