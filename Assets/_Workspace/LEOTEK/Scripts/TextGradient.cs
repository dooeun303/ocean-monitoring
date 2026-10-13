using UnityEngine;
using TMPro;

[ExecuteAlways]
[RequireComponent(typeof(TMP_Text))]
public class TextGradient : MonoBehaviour
{
    private TMP_Text tmpText;

    private readonly Color32 colorLeft = new Color32(0x73, 0xCF, 0x79, 0xFF); // #73CF79
    private readonly Color32 colorRight = new Color32(0x39, 0x78, 0xB8, 0xFF); // #3978B8

    void OnEnable()
    {
        Apply();
    }

    void Apply()
    {
        if (tmpText == null) tmpText = GetComponent<TMP_Text>();

        tmpText.colorGradientPreset = null;
        tmpText.enableWordWrapping = tmpText.enableWordWrapping;
        tmpText.colorGradient = new VertexGradient(
            colorLeft,   // ÁÂ»ó
            colorRight,  // ¿ì»ó
            colorLeft,   // ÁÂÇÏ
            colorRight   // ¿ìÇÏ
        );
    }
}