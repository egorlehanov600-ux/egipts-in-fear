using UnityEngine;

public class GuardFollow : MonoBehaviour
{
    private bool isFollowing = false;
    private Transform player;
    private float speed;
    private float distanceBehind;
    private Vector3 offset;
    private GuardMovement guardMovementScript;

    void Start()
    {
        // Находим и запоминаем скрипт GuardMovement на этом же объекте
        guardMovementScript = GetComponent<GuardMovement>();
    }

    // Метод вызывается из скрипта куба для начала следования
    public void StartFollowing(Transform playerTransform, float dist, float spd)
    {
        player = playerTransform;
        distanceBehind = dist;
        speed = spd;
        isFollowing = true;
        
        // Отключаем скрипт GuardMovement, чтобы он не мешал
        if (guardMovementScript != null)
        {
            guardMovementScript.enabled = false;
        }
        
        // Запоминаем смещение охранника относительно игрока
        offset = transform.position - player.position;
    }

    // Метод для остановки следования
    public void StopFollowing()
    {
        isFollowing = false;
    }

    void Update()
    {
        if (!isFollowing || player == null) return;

        // Целевая позиция: позиция игрока + смещение
        Vector3 targetPosition = player.position + offset;

        // Плавное движение к целевой точке
        transform.position = Vector3.MoveTowards(transform.position, targetPosition, speed * Time.deltaTime);
        
        // Опционально: охранник смотрит в сторону движения
        if (Vector3.Distance(transform.position, targetPosition) > 0.1f)
        {
            Vector3 lookDirection = targetPosition - transform.position;
            transform.rotation = Quaternion.LookRotation(lookDirection);
        }
    }
}