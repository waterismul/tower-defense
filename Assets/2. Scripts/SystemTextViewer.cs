using TMPro;
using UnityEngine;

public enum SystemType{Money,Build}
public class SystemTextViewer : MonoBehaviour
{
    private TextMeshProUGUI textSystem;
    private TMPAlpha tmpAlpha;

    private void Awake()
    {
        textSystem = GetComponent<TextMeshProUGUI>();
        tmpAlpha = GetComponent<TMPAlpha>();
    }

    public void PrintText(SystemType type)
    {
        switch (type)
        {
            case SystemType.Money:
                textSystem.text = "System: No Money";
                break;
            case SystemType.Build:
                textSystem.text = "System: No Build";
                break;
        }
        
        tmpAlpha.FadeOut();
    }
}
