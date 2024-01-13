using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;

public class Tooltip_Trigger : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    private Tween tween;
    public string header;
 
    [Multiline()]
    public string content;

    public void OnPointerEnter(PointerEventData eventData)
    {
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

    public void OnMouseEnter()
    {
        tween = DOVirtual.DelayedCall(0.5f, () =>
        {
            Tooltip_System.Show(content, header);
        });
    }

    public void OnMouseExit()
    {   
        tween.Kill();
        Tooltip_System.Hide();
    }
}