using UnityEngine;

public class PlayerColor : MonoBehaviour
{
    // Назначаем цвет в зависимости от слота (0 = красный, 1 = зеленый, 2 = синий)
    public void SetColorBySlot(int slotIndex)
    {
        Color color = Color.white;
        
        if (slotIndex == 0) color = Color.red;       // Класс 1
        else if (slotIndex == 1) color = Color.green; // Класс 2
        else if (slotIndex == 2) color = Color.blue;  // Класс 3
        else if (slotIndex == 3) color = Color.yellow;// Класс 4
        else if (slotIndex == 4) color = Color.magenta;// Класс 5

        GetComponent<Renderer>().material.color = color;
    }
}