using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class TableRowHighlight : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    [SerializeField] private Image backgroundImage;
    [SerializeField] private Color normalColor = Color.white;
    [SerializeField] private Color hoverColor = new Color(0.9f, 0.95f, 1f);
    [SerializeField] private Color selectedColor = new Color(0.8f, 0.9f, 1f);

    private bool isSelected = false;
    private bool isHovering = false;

    private void Awake()
    {
        if (backgroundImage == null)
            backgroundImage = GetComponent<Image>();

        UpdateColor();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        isHovering = true;
        UpdateColor();
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        isHovering = false;
        UpdateColor();
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        isSelected = !isSelected;
        UpdateColor();
    }

    private void UpdateColor()
    {
        if (isSelected)
            backgroundImage.color = selectedColor;
        else if (isHovering)
            backgroundImage.color = hoverColor;
        else
            backgroundImage.color = normalColor;
    }
}