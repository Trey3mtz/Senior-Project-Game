
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHealthBar : MonoBehaviour
{

	public Slider slider;
	public Gradient gradient;
	public Image fill;

    public int MaxHP;
    private int _hp;
    private bool justGotHit;

    public AnimatorController animatorController;
    
    [SerializeField] GameObject PlayerHP;
    private HealthBar pHP;
    

    void Awake()
    {
        // Grabs Players HealthBar, Sets its maxHP to it's Max, and sets visual HP at that amount
        PlayerHP = GameObject.Find("PlayerHealth");
        pHP = PlayerHP.GetComponent<HealthBar>();
        //MaxHP = pHP.MaxHP;

        justGotHit = false;

        SetMaxHealth(MaxHP);
    }

    void Start()
    {
        //_hp = pHP._hp;    
        ChangedHealthByAmount(_hp);
    }

    public void ChangeHealth(int amt)
    {   
        if(_hp > Mathf.Clamp(_hp + amt, 0, MaxHP))
        {
            justGotHit = true;
  
        }     

        _hp = Mathf.Clamp(_hp + amt, 0, MaxHP);

        ChangedHealthByAmount(_hp);
    }

    // New Method for Health HUD
	private void SetMaxHealth(int health)
	{
		slider.maxValue = MaxHP;
		slider.value = health;

		fill.color = gradient.Evaluate(1f);
	}

    // New Method to adjust HUD health based on current HP
    public void ChangedHealthByAmount(int health)
	{
        _hp = health;
        if(health <= 0)
        {
            StartCoroutine(deathTiming());
        }
        else if(health > slider.value){
            animatorController.CrossFade("GainHealth");
        }
        else if(health < slider.value){
            animatorController.CrossFade("HitHealth");
        }
        

		slider.value = health;
		fill.color = gradient.Evaluate(slider.normalizedValue);
	}

        public int currentHP()
    {
        return _hp;
    }

    public bool wasHit()
    {
        if(justGotHit)
        {
            justGotHit = false;
            return true;
        }
        else
            return false;
            
    }

    IEnumerator deathTiming()
    {
        yield return new WaitForSeconds(2f);
        animatorController.CrossFade("Death");
    }

}