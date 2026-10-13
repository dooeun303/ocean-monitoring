using UnityEngine;
using DG.Tweening;

public class SubMenuGroup : MonoBehaviour
{
    public SubMenuItem[] items;

    private SubMenuItem currentItem;

    public void SelectItem(SubMenuItem target)
    {
        if (currentItem == target) return;

        if (currentItem != null)
        {
            currentItem.page.SetActive(false);
            currentItem.SetNormal();
        }

        currentItem = target;
        currentItem.SetActive();

        // ∆‰¿Ã¡ˆ FadeUp
        GameObject page = target.page;
        page.SetActive(true);

        RectTransform rect = page.GetComponent<RectTransform>();
        CanvasGroup cg = page.GetComponent<CanvasGroup>();
        if (cg == null) cg = page.AddComponent<CanvasGroup>();

        Vector2 originalPos = rect.anchoredPosition;

        rect.anchoredPosition = originalPos - new Vector2(0, 12f);
        cg.alpha = 0f;

        rect.DOAnchorPos(originalPos, 0.5f).SetEase(Ease.OutQuad);
        cg.DOFade(1f, 0.5f);
    }
}