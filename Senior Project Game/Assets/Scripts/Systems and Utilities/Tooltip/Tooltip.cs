using TMPro;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using UnityEngine.EventSystems;
using System.Runtime.CompilerServices;

public class Tooltip : MonoBehaviour
{
    public bool tooltipFlipEnabled = true;

    public TextMeshProUGUI headerField;
    public TextMeshProUGUI contentField;
    public LayoutElement layoutElement;
    public RectTransform rectTransform;
    public int charWrapLimit;

    [SerializeField] Sprite leftLeaningSprite;
    [SerializeField] Sprite rightLeaningSprite;

    private int headerLength;
    private int contentLength;
    
    private void Awake()
    {  
        rectTransform = GetComponent<RectTransform>();
    }

    public void SetText(string content = "", string header = "")
    {
        if(string.IsNullOrEmpty(header))
            headerField.gameObject.SetActive(false);
        else
            headerField.gameObject.SetActive(true);

        if(string.IsNullOrEmpty(content))
            contentField.gameObject.SetActive(false);
        else
            contentField.gameObject.SetActive(true);
        
        headerField.text = header;
        contentField.text = content;

        headerLength = headerField.text.Length;
        contentLength = contentField.text.Length;

        layoutElement.enabled = Mathf.Max(headerField.preferredWidth, contentField.preferredWidth) >= layoutElement.preferredWidth;     

        transform.position = Input.mousePosition;
    }

    Sequence moveSequence;
    Vector2 endPosition;

    private void Update()
    {   
        if(!Tooltip_System.Instance.IsShown())
           return;
        if(!EventSystem.current.IsPointerOverGameObject())
         {
            SetText("", "");
            Tooltip_System.Instance.Hide();
            return;
         }   

        endPosition = Input.mousePosition;
           
        float pivotX = endPosition.x / Screen.width;
        float pivotY = endPosition.y / Screen.height;
        float finalPivotX;
        float finalPivotY;

        //  If mouse on left of screen move tooltip to right of cursor and vice vera
        //  If tooltip flipping is enabled, it will flip on towards the screenspace with most room
        if (pivotX < 0.5) 
        {
             finalPivotX = -0.1f;
             if(tooltipFlipEnabled)
             {
                gameObject.GetComponent<Image>().sprite = leftLeaningSprite;
                gameObject.GetComponent<VerticalLayoutGroup>().childAlignment = TextAnchor.MiddleLeft;
                headerField.alignment = TextAlignmentOptions.Left;
                contentField.alignment = TextAlignmentOptions.Left;                
             }
        }
        else
        {
            finalPivotX = 1.01f;
            if(tooltipFlipEnabled)
            {
                gameObject.GetComponent<Image>().sprite = rightLeaningSprite;
                gameObject.GetComponent<VerticalLayoutGroup>().childAlignment = TextAnchor.MiddleRight;
                headerField.alignment = TextAlignmentOptions.Right;
                contentField.alignment = TextAlignmentOptions.Right;                
            }
        }
            
        // If mouse on lower half of screen move tooltip above cursor and vice versa
        if (pivotY < 0.5) 
            finalPivotY = 0;
        else
            finalPivotY = 1;
        
        Vector2 finalPivot = new Vector2 (finalPivotX, finalPivotY);

        
        if (rectTransform.pivot != finalPivot)
        {
                moveSequence.Kill();

                moveSequence = DOTween.Sequence()
                .Join(DOTween.To(() => rectTransform.pivot, x => rectTransform.pivot = x, new Vector2(finalPivotX, finalPivotY), .5f))
                .Join(DOTween.To(() => rectTransform.pivot, y => rectTransform.pivot = y, new Vector2(finalPivotX, finalPivotY), 1f))
                .SetRelative(false)
                .Play();
        }


        transform.DOMove(endPosition, 0.15f);
    }
}
