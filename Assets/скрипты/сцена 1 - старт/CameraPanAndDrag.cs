using UnityEngine;
using UnityEngine.InputSystem; // Подключаем новый Input System

public class CameraPanAndDrag : MonoBehaviour
{
    [Header("Настройки автоматического движения")]
    public float startY = 35.72f;      
    public float endY = 3.43f;         
    public float duration = 20f;       

    [Header("Настройки перетаскивания")]
    public float dragSpeed = 0.1f;     
    public bool is2DGame = false;      

    private float startTime;
    private bool isMoving = true;

    private bool isDragging = false;
    private Vector2 lastMousePosition; // Изменили на Vector2, так как новый Input возвращает Vector2

    void Start()
    {
        Vector3 pos = transform.position;
        pos.y = startY;
        transform.position = pos;

        startTime = Time.time;
    }

    void Update()
    {
        // 1. Автоматическое плавное движение по оси Y
        if (isMoving)
        {
            float elapsed = Time.time - startTime;
            float t = Mathf.Clamp01(elapsed / duration); 

            float currentY = Mathf.Lerp(startY, endY, t);

            Vector3 pos = transform.position;
            pos.y = currentY;
            transform.position = pos;

            if (t >= 1f)
            {
                isMoving = false; 
            }
        }

        // 2. Обработка ручного перетаскивания мышкой
        HandleDragging();
    }

    void HandleDragging()
    {
        // Получаем доступ к мыши через новый Input System
        var mouse = Mouse.current;
        if (mouse == null) return; // Защита от ошибок, если мышь не найдена

        // Начало перетаскивания (Левая кнопка нажата)
        if (mouse.leftButton.wasPressedThisFrame)
        {
            isDragging = true;
            lastMousePosition = mouse.position.ReadValue();
        }

        // Конец перетаскивания (Кнопка отпущена)
        if (mouse.leftButton.wasReleasedThisFrame)
        {
            isDragging = false;
        }

        // Сам процесс перетаскивания
        if (isDragging)
        {
            Vector2 currentMousePos = mouse.position.ReadValue();
            Vector2 delta = currentMousePos - lastMousePosition;
            lastMousePosition = currentMousePos;

            if (is2DGame)
            {
                transform.Translate(-delta.x * dragSpeed, -delta.y * dragSpeed, 0);
            }
            else
            {
                transform.Translate(-delta.x * dragSpeed, 0, -delta.y * dragSpeed);
            }
        }
    }
}