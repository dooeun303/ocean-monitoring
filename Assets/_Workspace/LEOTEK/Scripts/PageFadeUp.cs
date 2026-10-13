using UnityEngine;
using XCharts.Runtime;
using DG.Tweening;

public class PageFadeUp : MonoBehaviour
{
    public float duration = 0.5f;
    public float offsetY = 12f;
    public BaseChart[] charts;  // 페이지에 있는 차트들

    private Vector3[] originalPositions;
    private CanvasGroup[] canvasGroups;

    void OnEnable()
    {
        int count = transform.childCount;
        originalPositions = new Vector3[count];
        canvasGroups = new CanvasGroup[count];

        for (int i = 0; i < count; i++)
        {
            Transform child = transform.GetChild(i);
            originalPositions[i] = child.localPosition;

            canvasGroups[i] = child.GetComponent<CanvasGroup>();
            if (canvasGroups[i] == null)
                canvasGroups[i] = child.gameObject.AddComponent<CanvasGroup>();
        }
    }

    public void OnPageRefresh()
    {
        // 차트 애니메이션 리셋
        if (charts != null)
        {
            foreach (var chart in charts)
            {
                if (chart != null)
                {
                    chart.AnimationReset();
                    //chart.AnimationFadeIn();
                }
            }
        }

        // 페이드업 실행
        PlayFadeUp();
    }

    public void PlayFadeUp()
    {
        if (canvasGroups == null || canvasGroups.Length != transform.childCount)
            OnEnable();

        for (int i = 0; i < transform.childCount; i++)
        {
            Transform child = transform.GetChild(i);
            CanvasGroup cg = canvasGroups[i];

            if (child == null || cg == null) continue;

            child.DOKill();
            cg.DOKill();

            child.localPosition = new Vector3(
                originalPositions[i].x,
                originalPositions[i].y - offsetY,
                originalPositions[i].z
            );
            cg.alpha = 0f;

            child.DOLocalMoveY(originalPositions[i].y, duration).SetEase(Ease.OutQuad);
            cg.DOFade(1f, duration);
        }
    }
}