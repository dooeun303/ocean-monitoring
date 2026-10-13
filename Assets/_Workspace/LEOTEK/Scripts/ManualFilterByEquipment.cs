using System.Collections.Generic;
using UnityEngine;
using TMPro;

// 설비 종류에 따라 "작업 매뉴얼" 드롭다운을 필터링한다 - 펌프를 고르면 펌프 매뉴얼만, 밸브를
// 고르면 밸브 매뉴얼만 보이게. 설비명에 "펌프"/"밸브" 문자열이 포함돼있는지로 판별한다
// (고압펌프 #1/#2처럼 개별 호기 번호는 무시 - 매뉴얼은 보통 설비 종류 단위로 존재하지 개별
// 호기마다 다르지 않다고 가정).
//
// 사용법: 유지관리 등록 팝업(PopupCard 등) 오브젝트에 이 컴포넌트를 붙이고, Inspector에서
// equipmentDropdown/manualDropdown 필드에 각각 EquipmentDropdown/ManualDropdown을 드래그.
public class ManualFilterByEquipment : MonoBehaviour
{
    [SerializeField] TMP_Dropdown equipmentDropdown;
    [SerializeField] TMP_Dropdown manualDropdown;

    // 키워드 → 매뉴얼 목록. 설비명에 키워드가 포함돼 있으면 그 매뉴얼들을 보여준다. 실제
    // 설비-매뉴얼 매핑 데이터가 서버/DB에 아직 없어서 우선 하드코딩해뒀다 - 나중에 API가
    // 생기면 이 테이블을 그 응답으로 교체하면 된다.
    static readonly (string keyword, string[] manuals)[] ManualsByKeyword =
    {
        ("펌프", new[] { "고압펌프 베어링 점검 매뉴얼 v2.1", "고압펌프 긴급 점검 절차서 v1.3" }),
        ("밸브", new[] { "RO 멤브레인 정기점검 매뉴얼 v3.0", "임펠러 교체 작업 매뉴얼 v1.8" }),
    };

    const string PlaceholderText = "작업 매뉴얼";

    void OnEnable()
    {
        if (equipmentDropdown == null || manualDropdown == null)
        {
            Debug.LogWarning("[ManualFilterByEquipment] equipmentDropdown/manualDropdown이 Inspector에 연결 안 됨.");
            return;
        }
        equipmentDropdown.onValueChanged.AddListener(OnEquipmentChanged);
        OnEquipmentChanged(equipmentDropdown.value); // 팝업이 열릴 때 이미 설비가 선택돼 있을 수도 있으니 한 번 반영
    }

    void OnDisable()
    {
        if (equipmentDropdown != null)
            equipmentDropdown.onValueChanged.RemoveListener(OnEquipmentChanged);
    }

    void OnEquipmentChanged(int index)
    {
        string equipmentName = (index >= 0 && index < equipmentDropdown.options.Count)
            ? equipmentDropdown.options[index].text
            : "";

        var matched = new List<string>();
        foreach (var (keyword, manuals) in ManualsByKeyword)
        {
            if (equipmentName.Contains(keyword))
            {
                matched.AddRange(manuals);
                break;
            }
        }

        manualDropdown.ClearOptions();
        var options = new List<string> { PlaceholderText };
        options.AddRange(matched);
        manualDropdown.AddOptions(options);
        manualDropdown.value = 0;
        manualDropdown.RefreshShownValue();
    }
}
