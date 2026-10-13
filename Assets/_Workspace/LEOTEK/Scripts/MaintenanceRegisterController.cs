using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

// "유지 관리 등록" 팝업의 "등록" 버튼을 누르면 입력된 값(설비/작업 내용/매뉴얼/작업자/상태/예정시각)을
// 그대로 위쪽 유지보수 목록 테이블에 새 행으로 추가한다.
//
// 테이블 쪽(rowContainer)은 이 팝업과 완전히 다른 계층(테이블.prefab 인스턴스)에 있어서 코드로
// 자동으로 찾을 수 없다 - Inspector에서 딱 하나만 연결해주면 된다. 나머지 입력 필드/버튼은
// 이름으로 자기 자식들 안에서 알아서 찾는다(GameObject 이름은 이미 확인된 것들이라 안전함).
//
// 사용법: "유지관리 등록 팝업"(PopupCard와 SuccessPanel을 형제로 갖는 상위 오브젝트)에 이
// 컴포넌트를 붙이고, Inspector의 rowContainer에 테이블의 행들이 들어있는 컨테이너(기존 행 하나를
// 열어보면 그 부모)를 드래그. PopupCard 자체에 붙이면 안 됨 - SuccessPanel이 PopupCard의 자식이
// 아니라 형제라서 못 찾는다.
public class MaintenanceRegisterController : MonoBehaviour
{
    [Tooltip("유지보수 목록 테이블에서 행(row)들이 들어있는 부모. 기존 행 하나를 자식으로 갖고 있어야 함(복제 원본으로 씀).")]
    [SerializeField] RectTransform rowContainer;

    TMP_Dropdown equipmentDropdown;
    TMP_InputField workContentInput;
    TMP_Dropdown manualDropdown;
    TMP_Dropdown technicianDropdown;
    TMP_Dropdown statusDropdown;
    TMP_InputField scheduledAtInput;
    Button registerButton;
    GameObject popupCard;
    GameObject successPanel;
    Image dimBackground; // "유지관리 등록 팝업" 자기 자신에 붙어있는 반투명 검정 배경 - PopupCard를 꺼도 이건 따로 안 꺼짐

    void Awake()
    {
        var dropdowns = GetComponentsInChildren<TMP_Dropdown>(true);
        equipmentDropdown = dropdowns.FirstOrDefault(d => d.name == "EquipmentDropdown");
        manualDropdown = dropdowns.FirstOrDefault(d => d.name == "ManualDropdown");
        technicianDropdown = dropdowns.FirstOrDefault(d => d.name == "TechnicianDropdown");
        statusDropdown = dropdowns.FirstOrDefault(d => d.name == "StatusDropdown");

        var inputFields = GetComponentsInChildren<TMP_InputField>(true);
        workContentInput = inputFields.FirstOrDefault(f => f.name == "WorkContentInput");
        scheduledAtInput = inputFields.FirstOrDefault(f => f.name == "ScheduledAtInput");

        // "등록" 버튼은 공용 버튼 프리팹 안에 중첩돼 있어서 이름으로 못 찾는다 - BtnRow 밑의
        // 버튼 중 라벨이 "등록"인 것을 찾는다("취소" 버튼과 구분).
        registerButton = GetComponentsInChildren<Button>(true)
            .FirstOrDefault(b => b.GetComponentInChildren<TMP_Text>(true)?.text == "등록");
        registerButton?.onClick.AddListener(OnRegisterClicked);

        var allChildren = GetComponentsInChildren<Transform>(true);
        popupCard = allChildren.FirstOrDefault(t => t.name == "PopupCard")?.gameObject;
        successPanel = allChildren.FirstOrDefault(t => t.name == "SuccessPanel")?.gameObject;
        successPanel?.SetActive(false);
        dimBackground = GetComponent<Image>();

        if (equipmentDropdown == null || workContentInput == null || manualDropdown == null ||
            technicianDropdown == null || statusDropdown == null || scheduledAtInput == null ||
            registerButton == null || popupCard == null)
        {
            Debug.LogWarning("[MaintenanceRegisterController] 팝업 안의 입력 필드/버튼 중 일부를 못 찾았습니다 - 이름이 바뀌었는지, 이 컴포넌트를 PopupCard의 부모에 붙였는지 확인하세요.");
        }
    }

    void OnRegisterClicked()
    {
        if (rowContainer == null)
        {
            Debug.LogWarning("[MaintenanceRegisterController] rowContainer가 Inspector에 연결 안 됨.");
            return;
        }

        string equipment = SelectedText(equipmentDropdown);
        string workContent = workContentInput != null ? workContentInput.text.Trim() : "";

        // 최소한의 검증 - 설비/작업 내용은 비어있으면 등록 의미가 없다.
        if (string.IsNullOrEmpty(equipment) || equipment == "설비 선택" || string.IsNullOrEmpty(workContent))
        {
            Debug.LogWarning("[MaintenanceRegisterController] 설비와 작업 내용은 필수입니다.");
            return;
        }

        string manual = SelectedText(manualDropdown);
        string technician = SelectedText(technicianDropdown);
        string status = ExtractStatusKey(SelectedText(statusDropdown));
        string scheduledAt = scheduledAtInput != null ? scheduledAtInput.text.Trim() : "";

        AddRow(
            id: (rowContainer.childCount + 1).ToString(),
            equipment: equipment,
            workContent: workContent,
            manual: manual,
            technician: technician,
            status: string.IsNullOrEmpty(status) ? "scheduled" : status,
            scheduledAt: string.IsNullOrEmpty(scheduledAt) ? "-" : scheduledAt,
            completedAt: "-");

        ClosePopup();
    }

    static string SelectedText(TMP_Dropdown dropdown)
    {
        if (dropdown == null || dropdown.options.Count == 0) return "";
        int i = Mathf.Clamp(dropdown.value, 0, dropdown.options.Count - 1);
        return dropdown.options[i].text;
    }

    // 드롭다운 표시 텍스트가 "대기 (scheduled)" 형태라 테이블 형식("scheduled")에 맞게 괄호 안만 뽑는다.
    static string ExtractStatusKey(string displayText)
    {
        int open = displayText.IndexOf('(');
        int close = displayText.IndexOf(')');
        if (open >= 0 && close > open)
            return displayText.Substring(open + 1, close - open - 1);
        return "";
    }

    // rowContainer의 마지막 자식(기존 행)을 복제해서 새 행으로 쓴다 - 열 순서(유지보수ID, 설비명,
    // 작업내용, 작업매뉴얼, 담당 AR작업자, 작업상태, 작업예정일시, 작업완료일시)는 실제 씬에서
    // 확인한 자식 순서 그대로다.
    void AddRow(string id, string equipment, string workContent, string manual, string technician,
        string status, string scheduledAt, string completedAt)
    {
        if (rowContainer.childCount == 0)
        {
            Debug.LogWarning("[MaintenanceRegisterController] rowContainer에 복제할 기존 행이 없습니다.");
            return;
        }

        var template = rowContainer.GetChild(rowContainer.childCount - 1).gameObject;
        var newRow = Instantiate(template, rowContainer);
        newRow.name = "contentRow(Clone)";

        var texts = newRow.GetComponentsInChildren<TMP_Text>(true);
        string[] values = { id, equipment, workContent, manual, technician, status, scheduledAt, completedAt };
        for (int i = 0; i < texts.Length && i < values.Length; i++)
            texts[i].text = values[i];
    }

    void ClosePopup()
    {
        // PopupCard와 반투명 배경(dimBackground)만 끄고, 이 스크립트가 붙은 상위 오브젝트
        // 자체(GameObject)는 계속 활성 상태로 둔다 - 꺼버리면 SuccessPanel도 같이 꺼지고,
        // 지금 막 시작한 HideSuccessPanelAfterDelay 코루틴도 즉시 멈춰버려서 토스트가 영원히
        // 안 사라진다.
        popupCard?.SetActive(false);
        if (dimBackground != null) dimBackground.enabled = false;

        if (successPanel == null) return;
        successPanel.SetActive(true);
        StopAllCoroutines(); // 연속으로 빠르게 등록할 때 이전 타이머가 늦게 꺼버리는 걸 방지
        StartCoroutine(HideSuccessPanelAfterDelay(3f));
    }

    System.Collections.IEnumerator HideSuccessPanelAfterDelay(float seconds)
    {
        yield return new WaitForSeconds(seconds);
        successPanel.SetActive(false);
    }
}
