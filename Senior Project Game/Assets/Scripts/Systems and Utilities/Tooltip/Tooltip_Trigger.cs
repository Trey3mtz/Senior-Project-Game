using System;
using DG.Tweening;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

[Serializable]
public class Tooltip_Trigger : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    private Tween tween;
    public string header;
 
    [Multiline()]
    public string content;



    public void OnPointerEnter(PointerEventData eventData)
    {  
        Tooltip_System.SetLastKnownTrigger(this);
        tween = DOVirtual.DelayedCall(0.5f, () =>
        {
            Tooltip_System.Show(content, header);
        });
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        tween.Kill();
        Tooltip_System.Hide();
    }
    
    // If this tooltip is being disabled before OnPointerExit is called, hide this tool tip.
    private void OnDisable()
    {   if(Tooltip_System.GetLastKnownTrigger() == this)
            Tooltip_System.Hide();    }

    // If I want to give non-UI objects tooltips
    private void OnDestroy()
    {   
        //Tooltip_System.Hide();
    }

    // If I want to give non-UI objects tooltips
    public void OnMouseEnter()
    {Debug.Log("tooltip mouse enter");
        tween = DOVirtual.DelayedCall(0.5f, () =>
        {
            Tooltip_System.Show(content, header);
        });
    }

    public void OnMouseExit()
    {   Debug.Log("tooltip mouse exit");
        tween.Kill();
        Tooltip_System.Hide();
    }
}