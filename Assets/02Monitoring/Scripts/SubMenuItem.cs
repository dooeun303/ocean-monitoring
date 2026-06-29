using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;

public class SubMenuItem : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    [Header("Components")]
    public Image backgroundImage;
    public TextMeshProUGUI label;
    public GameObject page;

    [Header("Group")]
    public SubMenuGroup group;

    // 기본
    private readonly Color normalTextColor = new Color32(0x7A, 0x9E, 0xAB, 0xFF); // #7A9EAB
    private readonly Color normalBgColor = new Color(0, 0, 0, 0);

    // 호버
    private readonly Color hoverTextColor = new Color32(0x50, 0xB8, 0xB8, 0xFF); // #50B8B8
    private readonly Color hoverBgColor = new Color(0x50 / 255f, 0xB8 / 255f, 0xB8 / 255f, 13f / 255f); // 투명도 5%

    // 액티브
    private readonly Color activeTextColor = new Color32(0x39, 0x78, 0xB8, 0xFF); // #3978B8
    private readonly Color activeBgColor = new Color(0x39 / 255f, 0x78 / 255f, 0xB8 / 255f, 15f / 255f); // 투명도 6%

    private bool isActive = false;

    void Start()
    {
        SetNormal();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (isActive) return;
        backgroundImage.DOColor(hoverBgColor, 0.1f);
        label.DOColor(hoverTextColor, 0.1f);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (isActive) return;
        backgroundImage.DOColor(normalBgColor, 0.1f);
        label.DOColor(normalTextColor, 0.1f);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        group.SelectItem(this);
    }

    public void SetActive()
    {
        isActive = true;
        backgroundImage.DOColor(activeBgColor, 0.1f);
        label.DOColor(activeTextColor, 0.1f);
        label.fontStyle = FontStyles.Bold;
    }

    public void SetNormal()
    {
        isActive = false;
        backgroundImage.DOColor(normalBgColor, 0.1f);
        label.DOColor(normalTextColor, 0.1f);
        label.fontStyle = FontStyles.Normal;
    }
}