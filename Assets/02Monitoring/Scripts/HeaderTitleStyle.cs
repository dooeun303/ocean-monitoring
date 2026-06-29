using UnityEngine;
using TMPro;

[ExecuteAlways]
[RequireComponent(typeof(TMP_Text))]
public class HeaderTitleStyle : MonoBehaviour
{
    private TMP_Text tmpText;
    private bool isApplying = false;

    private readonly Color32 colorLeft = new Color32(0x73, 0xCF, 0x79, 0xFF);
    private readonly Color32 colorMiddle = new Color32(0x50, 0xB8, 0xB8, 0xFF);
    private readonly Color32 colorRight = new Color32(0x39, 0x78, 0xB8, 0xFF);

    void OnEnable()
    {
        TMPro_EventManager.TEXT_CHANGED_EVENT.Add(OnTextChanged);
        ApplyStyle();
    }

    void OnDisable()
    {
        TMPro_EventManager.TEXT_CHANGED_EVENT.Remove(OnTextChanged);
    }

#if UNITY_EDITOR
    void OnValidate()
    {
        UnityEditor.EditorApplication.delayCall += () =>
        {
            if (this != null) ApplyStyle();
        };
    }
#endif

    private void OnTextChanged(Object obj)
    {
        if (isApplying) return;
        if (tmpText == null) tmpText = GetComponent<TMP_Text>();
        if (obj == tmpText) ApplyStyle();
    }

    public void ApplyStyle()
    {
        if (tmpText == null) tmpText = GetComponent<TMP_Text>();
        if (tmpText == null) return;

        isApplying = true;
        try
        {
            tmpText.fontSize = 22f;
            tmpText.fontWeight = FontWeight.SemiBold;
            tmpText.characterSpacing = -2f;
            tmpText.enableVertexGradient = false;
            tmpText.ForceMeshUpdate();

            if (tmpText.textInfo != null && tmpText.textInfo.characterCount > 0)
                ApplyContinuousGradient();
        }
        finally
        {
            isApplying = false;
        }
    }

    private void ApplyContinuousGradient()
    {
        tmpText.ForceMeshUpdate();
        TMP_TextInfo textInfo = tmpText.textInfo;
        if (textInfo == null) return;

        int characterCount = textInfo.characterCount;
        if (characterCount == 0) return;

        float minX = float.MaxValue;
        float maxX = float.MinValue;

        for (int i = 0; i < characterCount; i++)
        {
            TMP_CharacterInfo charInfo = textInfo.characterInfo[i];
            if (!charInfo.isVisible) continue;

            int materialIndex = charInfo.materialReferenceIndex;
            int vertexIndex = charInfo.vertexIndex;

            if (textInfo.meshInfo == null || materialIndex < 0 || materialIndex >= textInfo.meshInfo.Length) continue;
            var meshInfo = textInfo.meshInfo[materialIndex];
            if (meshInfo.vertices == null || vertexIndex + 3 >= meshInfo.vertices.Length) continue;

            for (int j = 0; j < 4; j++)
            {
                float x = meshInfo.vertices[vertexIndex + j].x;
                if (x < minX) minX = x;
                if (x > maxX) maxX = x;
            }
        }

        if (minX == float.MaxValue || maxX == float.MinValue || Mathf.Approximately(minX, maxX)) return;

        for (int i = 0; i < characterCount; i++)
        {
            TMP_CharacterInfo charInfo = textInfo.characterInfo[i];
            if (!charInfo.isVisible) continue;

            int materialIndex = charInfo.materialReferenceIndex;
            int vertexIndex = charInfo.vertexIndex;

            if (textInfo.meshInfo == null || materialIndex < 0 || materialIndex >= textInfo.meshInfo.Length) continue;
            var meshInfo = textInfo.meshInfo[materialIndex];
            if (meshInfo.vertices == null || vertexIndex + 3 >= meshInfo.vertices.Length) continue;
            if (meshInfo.colors32 == null || vertexIndex + 3 >= meshInfo.colors32.Length) continue;

            for (int j = 0; j < 4; j++)
            {
                float x = meshInfo.vertices[vertexIndex + j].x;
                float t = Mathf.Clamp01((x - minX) / (maxX - minX));
                meshInfo.colors32[vertexIndex + j] = EvaluateThreeColorGradient(t);
            }
        }

        tmpText.UpdateVertexData(TMP_VertexDataUpdateFlags.Colors32);
    }

    private Color32 EvaluateThreeColorGradient(float t)
    {
        if (t < 0.5f)
            return Color32.Lerp(colorLeft, colorMiddle, t / 0.5f);
        else
            return Color32.Lerp(colorMiddle, colorRight, (t - 0.5f) / 0.5f);
    }
}