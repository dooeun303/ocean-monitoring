using System.Collections;
using UnityEngine;

// "고장예지 알람 이력" 페이지에 들어오고 3초 뒤, 씬에 미리 만들어둔 예지 알람 팝업(삭제팝업을
// 복제해서 텍스트/버튼만 고친 것)을 띄운다. 팝업 자체의 내용/버튼 동작은 Hierarchy에 직접
// 만들어둔 오브젝트가 그대로 가지고 있으므로 이 스크립트는 보여주는 타이밍만 담당한다.
//
// 사용법: "알람이력 페이지" 오브젝트에 이 컴포넌트를 붙이고, Inspector의 alarmPopup에
// 씬에 만들어둔 예지 알람 팝업 오브젝트를 드래그. 팝업은 평소엔 비활성 상태로 둘 것.
public class PredictiveAlarmPopup : MonoBehaviour
{
    const float DelaySeconds = 3f;

    [Tooltip("씬에 미리 만들어둔 예지 알람 팝업 오브젝트 (평소엔 비활성 상태)")]
    [SerializeField] GameObject alarmPopup;

    void OnEnable()
    {
        if (alarmPopup != null) alarmPopup.SetActive(false);
        StopAllCoroutines();
        StartCoroutine(ShowAfterDelay());
    }

    void OnDisable()
    {
        StopAllCoroutines();
    }

    IEnumerator ShowAfterDelay()
    {
        yield return new WaitForSeconds(DelaySeconds);
        if (alarmPopup != null) alarmPopup.SetActive(true);
    }
}
