using UnityEngine;

public class ClickableCube : MonoBehaviour
{
    public CharacterSelector selector;
    public bool isNextButton;

    void OnMouseDown()
    {
        if (selector != null)
        {
            if (isNextButton)
                selector.OnNextButtonClick();
            else
                selector.OnPrevButtonClick();
        }
    }
}