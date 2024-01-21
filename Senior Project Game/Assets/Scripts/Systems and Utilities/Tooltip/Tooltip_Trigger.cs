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
       Tooltip_System.Instance.SetLastKnownTrigger(this);
        tween = DOVirtual.DelayedCall(0.75f, () =>
        {
            Tooltip_System.Instance.Show(content, header);
        });
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        tween.Kill();
        Tooltip_System.Instance.Hide();
    }
    
    bool isQuiting = false;
    // If this tooltip is being disabled before OnPointerExit is called, hide this tool tip.
    private void OnDisable()
    {   if(!isQuiting && Tooltip_System.Instance.GetLastKnownTrigger() )
           Tooltip_System.Instance.Hide();    
    }
    
    void OnApplicationQuit()
    {
        isQuiting=true;
    }
}