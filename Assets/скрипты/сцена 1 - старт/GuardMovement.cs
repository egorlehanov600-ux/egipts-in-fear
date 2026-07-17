using UnityEngine;

public class GuardMovement : MonoBehaviour
{
    [Header("Настройки движения охраны")]
    public float startZ = 7.26229f;  
    public float endZ = 1.52f;       

    [Header("Тайминги (в секундах)")]
    public float delayBeforeMove = 18f; 
    public float moveDuration = 4f;     

    [Header("Поворот (Дочерний объект)")]
    public Transform childToRotate; 
    public float rotationDuration = 1f; 

    [Header("Озвучка и текст")]
    public AudioSource voiceAudio;  
    public GameObject textBubble;   

    [Header("Кубы с вариантами выбора")]
    [Tooltip("Перетащите сюда первый куб")]
    public Transform choiceCube1;
    [Tooltip("Перетащите сюда второй куб")]
    public Transform choiceCube2;
    [Tooltip("За сколько секунд кубы плавно появятся")]
    public float appearDuration = 1f;

    private float startTime;
    private float rotationStartTime; 
    
    private bool hasFinishedWalking = false;
    private bool hasRotated = false;
    private bool hasStartedSpeaking = false;
    
    // Новые переменные для кубов
    private bool hasFinishedSpeaking = false;
    private float speakFinishTime;

    void Start()
    {
        Vector3 pos = transform.position;
        pos.z = startZ;
        transform.position = pos;

        startTime = Time.time;

        if (textBubble != null) textBubble.SetActive(false);
        
        // Скрываем кубы в самом начале (делаем их масштаб 0)
        if (choiceCube1 != null) choiceCube1.localScale = Vector3.zero;
        if (choiceCube2 != null) choiceCube2.localScale = Vector3.zero;
    }

    void Update()
    {
        float elapsed = Time.time - startTime;

        // --- ЭТАП 1: Ходьба ---
        if (!hasFinishedWalking)
        {
            if (elapsed < delayBeforeMove) return; 

            float moveElapsed = elapsed - delayBeforeMove;
            float t = Mathf.Clamp01(moveElapsed / moveDuration);

            Vector3 pos = transform.position;
            pos.z = Mathf.Lerp(startZ, endZ, t);
            transform.position = pos;

            if (t >= 1f) 
            {
                hasFinishedWalking = true; 
                rotationStartTime = Time.time; 
            }
        }

        // --- ЭТАП 2: Плавный поворот ---
        else if (!hasRotated)
        {
            if (childToRotate != null)
            {
                float rotationElapsed = Time.time - rotationStartTime;
                float t = Mathf.Clamp01(rotationElapsed / rotationDuration); 

                float currentY = Mathf.Lerp(0f, -25f, t);
                Vector3 currentRotation = childToRotate.localEulerAngles;
                currentRotation.y = currentY;
                childToRotate.localEulerAngles = currentRotation;

                if (t >= 1f) hasRotated = true; 
            }
            else hasRotated = true; 
        }

        // --- ЭТАП 3: Начало речи ---
        else if (!hasStartedSpeaking)
        {
            if (voiceAudio != null) voiceAudio.Play();
            if (textBubble != null) textBubble.SetActive(true);
            
            hasStartedSpeaking = true; 
        }

        // --- ЭТАП 4: Ожидание конца речи и появление кубов ---
        else if (!hasFinishedSpeaking)
        {
            // Если аудио не назначено, или оно закончило играть
            if (voiceAudio == null || !voiceAudio.isPlaying)
            {
                hasFinishedSpeaking = true;
                speakFinishTime = Time.time;
            }
        }
        else
        {
            // Плавное появление кубов (увеличение масштаба от 0 до 1)
            float appearElapsed = Time.time - speakFinishTime;
            float t = Mathf.Clamp01(appearElapsed / appearDuration);
            
            float scale = Mathf.Lerp(0f, 1f, t);
            if (choiceCube1 != null) choiceCube1.localScale = Vector3.one * scale;
            if (choiceCube2 != null) choiceCube2.localScale = Vector3.one * scale;
        }
    }
}