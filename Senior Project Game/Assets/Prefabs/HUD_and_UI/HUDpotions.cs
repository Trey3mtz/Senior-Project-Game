using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class HUDpotions : MonoBehaviour
{
    Animator animator;
    public AnimatorController animatorController;
    [SerializeField] TMP_Text HUD_text;

    private int lastNumber;
   
    private Color original;
    // Start is called before the first frame update
    void Awake()
    {
        animator = GetComponent<Animator>();
        original = HUD_text.color;
        lastNumber = 1;
        
    }

    
    // changes potions text to be a number of how many potions the Player has
    public void changeNumber(int number)
    {  
        HUD_text.CrossFadeColor(Color.white, 0f,true,false);
        HUD_text.color = Color.white;
       
        HUD_text.text = "x" + number.ToString("0");

        if(number > lastNumber)
            animatorController.PlayTriggerAnimation("pickUp");
        else
            animatorController.PlayTriggerAnimation("usePotion");

        HUD_text.CrossFadeColor(original, .4f,true,false);
        lastNumber = number;
    }




}
