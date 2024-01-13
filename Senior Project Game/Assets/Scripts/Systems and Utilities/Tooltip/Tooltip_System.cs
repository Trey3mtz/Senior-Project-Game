using UnityEngine;

public class Tooltip_System : MonoBehaviour
{
    private static Tooltip_System _currentTooltipSystem;
    [SerializeField] Tooltip tooltip;


    public void Awake()
    {   _currentTooltipSystem = this;    }
        

    public static void Show(string content, string header = "")
    {   _currentTooltipSystem.tooltip.SetText(content, header);
        _currentTooltipSystem.tooltip.gameObject.SetActive(true);    }


    public static void Hide()
    {   _currentTooltipSystem.tooltip.gameObject.SetActive(false);    }    
}
