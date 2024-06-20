using UnityEditor;
using UnityEngine;
using TMPro;

[InitializeOnLoad]
public class TMProPerformanceHelper
{
    [MenuItem("Tools/Performance/TMPro/Disable all RaycastTarget")]
    private static void DisableRaycastTarget()
    {
        TextMeshProUGUI[] textMeshPros = Resources.FindObjectsOfTypeAll<TextMeshProUGUI>();
        foreach (TextMeshProUGUI textMeshPro in textMeshPros)
        {
            if (textMeshPro.raycastTarget)
            {
                Debug.Log(textMeshPro.text);
                textMeshPro.raycastTarget = false;
            }
        }

        Debug.Log("RaycastTarget disabled for all TextMeshProUGUI components in the scene.");
    }
    [MenuItem("Tools/Performance/TMPro/Disable all RichText")]
    private static void DisableRichText()
    {
        TextMeshProUGUI[] textMeshPros = Resources.FindObjectsOfTypeAll<TextMeshProUGUI>();
        foreach (TextMeshProUGUI textMeshPro in textMeshPros)
        {
            if (textMeshPro.richText)
            {
                Debug.Log(textMeshPro.text);
                textMeshPro.richText = false;
            }
        }

        Debug.Log("RichText disabled for all TextMeshProUGUI components in the scene.");
    }
    [MenuItem("Tools/Performance/TMPro/Enable all IsScaleStatic")]
    private static void EnableIsStaticScale()
    {
        TextMeshProUGUI[] textMeshPros = Resources.FindObjectsOfTypeAll<TextMeshProUGUI>();
        foreach (TextMeshProUGUI textMeshPro in textMeshPros)
        {
            if (!textMeshPro.isTextObjectScaleStatic)
            {
                Debug.Log(textMeshPro.text);
                textMeshPro.isTextObjectScaleStatic = true;
            }
        }

        Debug.Log("IsScaleStatic enabled for all TextMeshProUGUI components in the scene.");
    }
}