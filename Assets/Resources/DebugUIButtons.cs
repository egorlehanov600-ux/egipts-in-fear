using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class DebugUIButtons : MonoBehaviour
{
    void Start()
    {
        Debug.Log("=== 🎯 ОТЛАДКА КНОПОК ===");
        
        // Проверяем Canvas
        Canvas canvas = GetComponent<Canvas>();
        if (canvas == null)
        {
            Debug.LogError("❌ Canvas компонент не найден!");
            return;
        }
        
        Debug.Log($"✅ Canvas найден: {canvas.renderMode}");
        Debug.Log($" Canvas World Position: {transform.position}");
        Debug.Log($"📐 Canvas World Rotation: {transform.eulerAngles}");
        
        // Проверяем кнопки
        Button[] buttons = GetComponentsInChildren<Button>(true);
        Debug.Log($"🔍 Найдено кнопок: {buttons.Length}");
        
        foreach (Button btn in buttons)
        {
            Debug.Log($"\n--- Кнопка: {btn.name} ---");
            
            // Проверяем Image
            Image img = btn.GetComponent<Image>();
            if (img == null)
            {
                // Ищем в детях
                img = btn.GetComponentInChildren<Image>();
                if (img == null)
                    Debug.LogWarning("⚠️ Image не найден на кнопке!");
            }
            
            if (img != null)
            {
                Debug.Log($"✅ Image: {img.name}");
                Debug.Log($"✅ Raycast Target: {img.raycastTarget}");
                if (!img.raycastTarget)
                    Debug.LogError(" ВКЛЮЧИ Raycast Target на Image!");
            }
            
            // Проверяем Target Graphic
            if (btn.targetGraphic == null)
                Debug.LogWarning("⚠️ Target Graphic не назначен!");
            else
                Debug.Log($"✅ Target Graphic: {btn.targetGraphic.name}");
            
            // Проверяем OnClick
            Debug.Log($" OnClick событий: {btn.onClick.GetPersistentEventCount()}");
            
            // Позиция кнопки
            Debug.Log($" Позиция кнопки: {btn.transform.position}");
            Debug.Log($" Локальная позиция: {btn.transform.localPosition}");
        }
        
        // Проверяем камеру
        Camera mainCam = Camera.main;
        if (mainCam == null)
        {
            Debug.LogWarning("⚠️ Camera.main не найдена!");
        }
        else
        {
            Debug.Log($"\n Камера: {mainCam.name}");
            Debug.Log($"📷 Позиция камеры: {mainCam.transform.position}");
            
            // Проверяем расстояние до Canvas
            float distance = Vector3.Distance(mainCam.transform.position, transform.position);
            Debug.Log($"📏 Расстояние до Canvas: {distance:F2}");
            
            // Проверяем, видит ли камера Canvas
            Vector3 toCanvas = transform.position - mainCam.transform.position;
            float angle = Vector3.Angle(mainCam.transform.forward, toCanvas);
            Debug.Log($"👁️ Угол обзора: {angle:F1}° (должно быть < {mainCam.fieldOfView / 2})");
            
            // Проверяем слои
            Debug.Log($" Слой Canvas: {LayerMask.LayerToName(gameObject.layer)}");
            Debug.Log($"🎭 Culling Mask камеры: {mainCam.cullingMask}");
            
            // Raycast проверка
            RaycastHit hit;
            if (Physics.Raycast(mainCam.transform.position, toCanvas.normalized, out hit, distance + 1))
            {
                Debug.Log($"🎯 Raycast попал в: {hit.collider.name}");
                if (hit.transform != transform)
                    Debug.LogWarning("⚠️ Raycast попал НЕ в Canvas, а в " + hit.transform.name);
            }
            else
            {
                Debug.LogWarning("⚠️ Raycast НЕ попал ни во что!");
            }
        }
        
        Debug.Log("=== КОНЕЦ ОТЛАДКИ ===");
    }
    
    // Добавляем визуализацию в Scene View
    void OnDrawGizmos()
    {
        // Рисуем границы Canvas
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireCube(transform.position, Vector3.one * 2);
        
        // Рисуем направление к камере
        Camera mainCam = Camera.main;
        if (mainCam != null)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawLine(transform.position, mainCam.transform.position);
            
            // Рисуем сферу на позиции камеры
            Gizmos.color = Color.blue;
            Gizmos.DrawWireSphere(mainCam.transform.position, 0.5f);
        }
    }
}