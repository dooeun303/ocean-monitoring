using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;

public class ButtonHoverColor : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public TextMeshProUGUI buttonText;

    private Color normalColor;
    private readonly Color hoverColor = new Color(0x50 / 255f, 0xB8 / 255f, 0xB8 / 255f, 1f); // #50B8B8

    void Start()
    {
        normalColor = buttonText.color;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        buttonText.color = hoverColor;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        buttonText.color = normalColor;
    }
}