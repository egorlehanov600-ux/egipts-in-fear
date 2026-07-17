using UnityEngine;

public class HoverCube : MonoBehaviour
{
    [Header("Настройки наведения")]
    [Tooltip("На сколько по Y поднимется куб при наведении")]
    public float hoverHeight = 0.5f; 
    
    [Tooltip("Скорость плавного движения (чем больше, тем резче)")]
    public float smoothSpeed = 10f;  

    private Vector3 startPosition;
    private Vector3 targetPosition;

    void Start()
    {
        // Запоминаем начальную позицию куба
        startPosition = transform.localPosition;
        targetPosition = startPosition;
    }

    void Update()
    {
        // Каждый кадр плавно двигаем куб к целевой позиции
        transform.localPosition = Vector3.Lerp(transform.localPosition, targetPosition, Time.deltaTime * smoothSpeed);
    }

    // Срабатывает, когда курсор наводится на объект
    void OnMouseEnter()
    {
        targetPosition = startPosition + Vector3.up * hoverHeight;
    }

    // Срабатывает, когда курсор уходит с объекта
    void OnMouseExit()
    {
        targetPosition = startPosition;
    }
}