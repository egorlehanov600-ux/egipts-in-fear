using UnityEngine;
using System.Collections;
using Photon.Pun; // ✅ Добавлено для работы с Photon

public class CubeChoiceAction : MonoBehaviourPunCallbacks // ✅ Изменено на MonoBehaviourPunCallbacks
{
    [Header("📦 Ссылки на объекты")]
    public Transform playerTransform;       // Игрок
    public Transform[] guardsToFollow;      // Охранники
    public Transform gatesTransform;        // ← ВОРОТА (то, что будет подниматься!)

    [Header("🔼 1. Поднятие ВОРОТ (вверх)")]
    public float gatesStartY = 2.19663f;    // Начальная высота ворот
    public float gatesEndY = 5.65f;         // Конечная высота ворот
    public float gatesDuration = 1.5f;      // Время поднятия

    [Header("🏃 2. Движение Игрока (по X)")]
    public float playerStartX = 4.97f;
    public float playerEndX = 1.073f;
    public float playerMoveXDuration = 2f;

    [Header("🔄 3. Поворот Игрока (по Y)")]
    public float playerTargetRotationY = -90f;
    public float playerRotateDuration = 1f;

    [Header("🚶 4. Движение Игрока (по Z) и Охрана")]
    public float playerStartZ = -2.16f;
    public float playerEndZ = -12f;
    public float playerMoveZDuration = 4f;
    
    public float guardFollowSpeed = 3f;
    public float guardDistanceBehind = 2f;

    [Header("🌐 5. Мультиплеер и переход")]
    public string nextSceneName = "LobbyScene"; // ✅ Имя сцены с ячейками (проверь в Build Settings!)

    private bool isSequenceActive = false;

    void OnMouseDown()
    {
        if (isSequenceActive) return;
        isSequenceActive = true;
        
        // ✅ Гарантируем, что мы подключены к Photon перед переходом
        if (!PhotonNetwork.IsConnected)
        {
            PhotonNetwork.ConnectUsingSettings();
        }
        
        StartCoroutine(PlayCinematicSequence());
    }

    IEnumerator PlayCinematicSequence()
    {
        // ✅ ЭТАП 1: Поднимаем ВОРОТА
        if (gatesTransform != null)
        {
            yield return StartCoroutine(SmoothMoveY(gatesTransform, gatesStartY, gatesEndY, gatesDuration));
        }
        else
        {
            Debug.LogWarning("⚠️ Gates Transform не назначен! Пропускаем поднятие ворот.");
            yield return new WaitForSeconds(gatesDuration);
        }

        // ✅ ЭТАП 2: Игрок идет вбок
        yield return StartCoroutine(SmoothMoveX(playerTransform, playerStartX, playerEndX, playerMoveXDuration));

        // ✅ ЭТАП 3: Игрок поворачивается
        yield return StartCoroutine(SmoothRotateY(playerTransform, playerTargetRotationY, playerRotateDuration));

        // ✅ ЭТАП 4: Игрок идет вперед + охрана следует
        foreach (Transform guard in guardsToFollow)
        {
            GuardFollow guardScript = guard.GetComponent<GuardFollow>();
            if (guardScript != null)
            {
                guardScript.StartFollowing(playerTransform, guardDistanceBehind, guardFollowSpeed);
            }
        }

        yield return StartCoroutine(SmoothMoveZ(playerTransform, playerStartZ, playerEndZ, playerMoveZDuration));

        // ✅ ЭТАП 5: Остановка охраны
        foreach (Transform guard in guardsToFollow)
        {
            GuardFollow guardScript = guard.GetComponent<GuardFollow>();
            if (guardScript != null)
            {
                guardScript.StopFollowing();
            }
        }

        // ✅ ЭТАП 6: Переход в мультиплеерное лобби
        yield return new WaitForSeconds(1f); // Небольшая пауза для плавности
        
        Debug.Log("🔄 Переход в сцену: " + nextSceneName);
        
        // Включаем автосинхронизацию сцен для всех игроков
        PhotonNetwork.AutomaticallySyncScene = true;
        
        // Загружаем сцену через Photon (все, кто в комнате, перейдут вместе)
        if (!string.IsNullOrEmpty(nextSceneName))
        {
            PhotonNetwork.LoadLevel(nextSceneName);
        }
        else
        {
            Debug.LogError("❌ Имя следующей сцены не указано в инспекторе!");
        }
    }

    // === Вспомогательные методы ===
    
    IEnumerator SmoothMoveY(Transform t, float start, float end, float duration)
    {
        float elapsed = 0;
        Vector3 pos = t.position; pos.y = start; t.position = pos;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            pos = t.position;
            pos.y = Mathf.Lerp(start, end, elapsed / duration);
            t.position = pos;
            yield return null;
        }
        pos = t.position; pos.y = end; t.position = pos;
    }

    IEnumerator SmoothMoveX(Transform t, float start, float end, float duration)
    {
        float elapsed = 0;
        Vector3 pos = t.position; pos.x = start; t.position = pos;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            pos = t.position;
            pos.x = Mathf.Lerp(start, end, elapsed / duration);
            t.position = pos;
            yield return null;
        }
        pos = t.position; pos.x = end; t.position = pos;
    }

    IEnumerator SmoothMoveZ(Transform t, float start, float end, float duration)
    {
        float elapsed = 0;
        Vector3 pos = t.position; pos.z = start; t.position = pos;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            pos = t.position;
            pos.z = Mathf.Lerp(start, end, elapsed / duration);
            t.position = pos;
            yield return null;
        }
        pos = t.position; pos.z = end; t.position = pos;
    }

    IEnumerator SmoothRotateY(Transform t, float targetY, float duration)
    {
        float elapsed = 0;
        float startY = t.eulerAngles.y;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            Vector3 rot = t.eulerAngles;
            rot.y = Mathf.LerpAngle(startY, targetY, elapsed / duration);
            t.eulerAngles = rot;
            yield return null;
        }
        Vector3 finalRot = t.eulerAngles;
        finalRot.y = targetY;
        t.eulerAngles = finalRot;
    }
}