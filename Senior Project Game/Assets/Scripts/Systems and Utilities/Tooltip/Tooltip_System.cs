using System.Collections;
using DG.Tweening;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

public class Tooltip_System : MonoBehaviour
{


        private static Tooltip_System _currentTooltipSystem;
        public static Tooltip_System Instance 
        { 
            get
            {   
                if(!_currentTooltipSystem)
                {  
                    // NOTE: Read docs to see directory requirements for Resources.Load! 
                    // https://docs.unity3d.com/ScriptReference/Resources.Load.html
                    var prefab = Resources.Load<GameObject>("Tooltip_System");
                    // Create the prefab in the scene
                    var inScene = Instantiate<GameObject>(prefab);
                    // Try to find the instance inside the prefab
                    _currentTooltipSystem = inScene.GetComponentInChildren<Tooltip_System>();
                    // Guess there isn't one, add one
                    if (!_currentTooltipSystem) _currentTooltipSystem = inScene.AddComponent<Tooltip_System>();
                    // Mark root as DontDestroyOnLoad();
                    DontDestroyOnLoad(_currentTooltipSystem.transform.root.gameObject);                    
                }
             return _currentTooltipSystem;
            }
        }



    [SerializeField] Tooltip tooltip;
    [SerializeField] float fadeTime = .1f;
    private bool isBeingShown;
    private Tooltip_Trigger lastKnownTrigger;
    private CanvasGroup canvasGroup;
    private bool canBeShown = true;

    public void Awake()
    {   if(_currentTooltipSystem != null && _currentTooltipSystem != this)
        {   Destroy(gameObject);    }
        else
            _currentTooltipSystem = this;
        canvasGroup = GetComponent<CanvasGroup>();  
        tooltip = GetComponentInChildren<Tooltip>();}


    public void Show(string content, string header = "")
    {   
        if(!_currentTooltipSystem.canBeShown)
            return;

        _currentTooltipSystem.canvasGroup.DOFade(1, _currentTooltipSystem.fadeTime).SetUpdate(true);  
        _currentTooltipSystem.tooltip.SetText(content, header);
        _currentTooltipSystem.tooltip.gameObject.SetActive(true);    
        _currentTooltipSystem.isBeingShown = true;  
    }

    public void Hide()
    {   
        _currentTooltipSystem.canvasGroup.DOFade(0, _currentTooltipSystem.fadeTime * 0.75f).SetUpdate(true);  
        _currentTooltipSystem.StartCoroutine(_currentTooltipSystem.Wait());
    }    

    private IEnumerator Wait()
    {
        yield return new WaitForSecondsRealtime(fadeTime);
        _currentTooltipSystem.isBeingShown = false;
    }

    public bool IsShown()
    {   return _currentTooltipSystem.isBeingShown;              }

    public void ToggleVisibilityOn()
    {   _currentTooltipSystem.canBeShown = true;                }
       
    public void ToggleVisibilityOff()
    {   _currentTooltipSystem.canBeShown = false;   Hide();     }

    public void SetLastKnownTrigger(Tooltip_Trigger lastTrigger)
    {   _currentTooltipSystem.lastKnownTrigger = lastTrigger;   }

    public Tooltip_Trigger GetLastKnownTrigger()
    {   return _currentTooltipSystem.lastKnownTrigger;          }
     
}
