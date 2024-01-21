using System.Collections;
using DG.Tweening;
using UnityEditor;
using UnityEngine;

public class Tooltip_System : MonoBehaviour
{
    private static Tooltip_System _currentTooltipSystem;
    [SerializeField] Tooltip tooltip;
    [SerializeField] float fadeTime = .1f;
    private bool isBeingShown;
    private Tooltip_Trigger lastKnownTrigger;
    private CanvasGroup canvasGroup;
    private bool canBeShown = true;

    public void Awake()
    {   _currentTooltipSystem = this;    
        canvasGroup = GetComponent<CanvasGroup>();  }
        

    public static void Show(string content, string header = "")
    {   
        if(!_currentTooltipSystem.canBeShown)
            return;

        _currentTooltipSystem.canvasGroup.DOFade(1, _currentTooltipSystem.fadeTime).SetUpdate(true);  
        _currentTooltipSystem.tooltip.SetText(content, header);
        _currentTooltipSystem.tooltip.gameObject.SetActive(true);    
        _currentTooltipSystem.isBeingShown = true;  
    }

    public static void Hide()
    {   
        _currentTooltipSystem.canvasGroup.DOFade(0, _currentTooltipSystem.fadeTime * 0.8f).SetUpdate(true);  
        _currentTooltipSystem.StartCoroutine(_currentTooltipSystem.Wait());
    }    

    private IEnumerator Wait()
    {
        yield return new WaitForSecondsRealtime(fadeTime);
        _currentTooltipSystem.isBeingShown = false;
    }

    public static bool IsShown()
    {   return _currentTooltipSystem.isBeingShown;              }

    public static void ToggleVisibilityOn()
    {   _currentTooltipSystem.canBeShown = true;                }
       
    public static void ToggleVisibilityOff()
    {   _currentTooltipSystem.canBeShown = false;   Hide();     }

    public static void SetLastKnownTrigger(Tooltip_Trigger lastTrigger)
    {   _currentTooltipSystem.lastKnownTrigger = lastTrigger;   }

    public static Tooltip_Trigger GetLastKnownTrigger()
    {   return _currentTooltipSystem.lastKnownTrigger;          }
     
}
