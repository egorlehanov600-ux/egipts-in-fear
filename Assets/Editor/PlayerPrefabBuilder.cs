using UnityEngine;
using UnityEditor;
using UnityEngine.UI;
using TMPro;
using Photon.Pun;
using System.IO;

public class PlayerPrefabBuilder : EditorWindow
{
    [MenuItem("Tools/Создать PlayerCapsule Префаб")]
    public static void ShowWindow()
    {
        if (EditorUtility.DisplayDialog("Создание префаба игрока", 
            "Будет создан полностью настроенный префаб PlayerCapsule со всем UI и скриптами.\n\nПродолжить?", 
            "Да", "Нет"))
        {
            BuildPlayerPrefab();
        }
    }

    static void BuildPlayerPrefab()
    {
        Debug.Log("🔨 Начинаем создание префаба PlayerCapsule...");

        // 1. Создаём капсулу
        GameObject capsule = GameObject.CreatePrimitive(PrimitiveType.Capsule);
        capsule.name = "PlayerCapsule";
        
        // Удаляем лишний Collider (оставим только если нужен)
        // DestroyImmediate(capsule.GetComponent<CapsuleCollider>());

        // 2. Добавляем PhotonView
        PhotonView photonView = capsule.AddComponent<PhotonView>();
        photonView.OwnershipTransfer = OwnershipOption.Takeover;
        photonView.Synchronization = ViewSynchronization.Unreliable;

        // 3. Добавляем скрипт PlayerColor
        // (предполагаем что скрипт уже есть в проекте)
        var playerColorType = System.Type.GetType("PlayerColor");
        if (playerColorType != null)
        {
            capsule.AddComponent(playerColorType);
            Debug.Log("✅ Добавлен PlayerColor");
        }
        else
        {
            Debug.LogWarning("️ Скрипт PlayerColor не найден!");
        }

        // 4. Добавляем скрипт PlayerCamera
        var playerCameraType = System.Type.GetType("PlayerCamera");
        if (playerCameraType != null)
        {
            capsule.AddComponent(playerCameraType);
            Debug.Log("✅ Добавлен PlayerCamera");
        }
        else
        {
            Debug.LogWarning("⚠️ Скрипт PlayerCamera не найден!");
        }

        // 5. Создаём Canvas
        GameObject canvasObj = new GameObject("Canvas");
        canvasObj.transform.SetParent(capsule.transform, false);
        Canvas canvas = canvasObj.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.WorldSpace;
        canvasObj.AddComponent<CanvasScaler>();
        canvasObj.AddComponent<GraphicRaycaster>();

        // Настройка Canvas
        canvasObj.transform.localPosition = new Vector3(0, -1.0f, 0);
        canvasObj.transform.localRotation = Quaternion.Euler(0, 0, 0);
        canvasObj.transform.localScale = Vector3.one * 0.005f;

        RectTransform canvasRect = canvasObj.GetComponent<RectTransform>();
        canvasRect.sizeDelta = new Vector2(300, 100);

        Debug.Log("✅ Создан Canvas");

        // 6. Создаём левую кнопку
        GameObject btnLeftObj = CreateButton(canvasObj, "BtnLeft", "<", new Vector2(-100, 0));
        
        // 7. Создаём правую кнопку
        GameObject btnRightObj = CreateButton(canvasObj, "BtnRight", ">", new Vector2(100, 0));

        // 8. Создаём текст класса
        GameObject classTextObj = new GameObject("ClassText");
        classTextObj.transform.SetParent(canvasObj.transform, false);
        TextMeshProUGUI classText = classTextObj.AddComponent<TextMeshProUGUI>();
        classText.text = "Сара";
        classText.fontSize = 24;
        classText.alignment = TextAlignmentOptions.Center;
        classText.color = Color.white;
        
        RectTransform classTextRect = classTextObj.GetComponent<RectTransform>();
        classTextRect.sizeDelta = new Vector2(100, 30);
        classTextRect.anchoredPosition = Vector2.zero;

        Debug.Log("✅ Создан ClassText");

        // 9. Добавляем CharacterSelector
        var charSelectorType = System.Type.GetType("CharacterSelector");
        CharacterSelector charSelector = null;
        if (charSelectorType != null)
        {
            charSelector = (CharacterSelector)capsule.AddComponent(charSelectorType);
            Debug.Log("✅ Добавлен CharacterSelector");
        }
        else
        {
            Debug.LogWarning("⚠️ Скрипт CharacterSelector не найден!");
        }

        // 10. Связываем UI со скриптом
        if (charSelector != null)
        {
            charSelector.classText = classText;
            charSelector.btnLeft = btnLeftObj.GetComponent<Button>();
            charSelector.btnRight = btnRightObj.GetComponent<Button>();
            Debug.Log("✅ UI связан со CharacterSelector");
        }

        // 11. Сохраняем как префаб в Resources
        string resourcesPath = "Assets/Resources";
        if (!Directory.Exists(resourcesPath))
        {
            Directory.CreateDirectory(resourcesPath);
            Debug.Log(" Создана папка Resources");
        }

        string prefabPath = "Assets/Resources/PlayerCapsule.prefab";
        
        // Если префаб уже существует - удаляем
        if (AssetDatabase.LoadAssetAtPath<GameObject>(prefabPath) != null)
        {
            AssetDatabase.DeleteAsset(prefabPath);
        }

        PrefabUtility.SaveAsPrefabAsset(capsule, prefabPath);
        Debug.Log($"✅ Префаб сохранён: {prefabPath}");

        // 12. Удаляем объект из сцены (он теперь в префабе)
        DestroyImmediate(capsule);

        Debug.Log("🎉 Готово! Префаб PlayerCapsule создан и готов к использованию!");
        
        EditorUtility.DisplayDialog("Успех!", 
            "Префаб PlayerCapsule создан в папке Resources!\n\nНе забудь:\n1. Проверить что все скрипты существуют\n2. Настроить цвета классов в CharacterSelector", 
            "OK");
    }

    static GameObject CreateButton(GameObject parent, string name, string text, Vector2 position)
    {
        // Создаём кнопку
        GameObject btnObj = new GameObject(name);
        btnObj.transform.SetParent(parent.transform, false);
        
        Button button = btnObj.AddComponent<Button>();
        Image image = btnObj.AddComponent<Image>();
        image.color = Color.white;
        image.raycastTarget = true;
        
        button.targetGraphic = image;
        
        // Настраиваем размер
        RectTransform rect = btnObj.GetComponent<RectTransform>();
        rect.sizeDelta = new Vector2(50, 50);
        rect.anchoredPosition = position;

        // Создаём текст внутри кнопки
        GameObject textObj = new GameObject("Text (TMP)");
        textObj.transform.SetParent(btnObj.transform, false);
        
        TextMeshProUGUI tmpText = textObj.AddComponent<TextMeshProUGUI>();
        tmpText.text = text;
        tmpText.fontSize = 20;
        tmpText.alignment = TextAlignmentOptions.Center;
        tmpText.color = Color.black;
        tmpText.raycastTarget = true; // Важно для кликов!
        
        RectTransform textRect = textObj.GetComponent<RectTransform>();
        textRect.sizeDelta = new Vector2(50, 50);
        textRect.anchoredPosition = Vector2.zero;

        Debug.Log($"✅ Создана кнопка: {name}");
        
        return btnObj;
    }
}