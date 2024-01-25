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


    /*
               CAUTION: Below, I had originally thought disabling UI stuff needed to hide the UI elements, however,
                        This caused issues with my Single due to it being OnDisable.

                        The fix I did was in the Tooltip's Update, to check if the point was over any UI.
                        Making the below code unnecesary. For now.

                        If I wanted tooltips above non-UI things, and they became destroyed, it might cause another tooltip bug.
                        Avoid using OnDestroy/OnDisable with this singleton for knowing if what you hovered is gone.

                        I am leave this code below as an example and cautionary tale.
    */

    
   // bool isQuiting = false;
    // If this tooltip is being disabled before OnPointerExit is called, hide this tool tip.
    private void OnDisable()
    {   //if(!isQuiting && Tooltip_System.Instance.GetLastKnownTrigger() )
           //Tooltip_System.Instance.Hide();    
    }
    
    void OnApplicationQuit()
    {
       //isQuiting=true;
    }
}