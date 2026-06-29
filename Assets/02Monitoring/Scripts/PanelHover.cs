using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class PanelHover : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [Header("Components")]
    public Image panelImage;      // 복합패널 이미지
    public GameObject shadowImage; // 호버용그림자 오브젝트

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (panelImage != null) panelImage.enabled = false;
        if (shadowImage != null) shadowImage.SetActive(true);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (panelImage != null) panelImage.enabled = true;
        if (shadowImage != null) shadowImage.SetActive(false);
    }
}