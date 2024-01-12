using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    [SerializeField] private int MaxHP = 10;
    [SerializeField] private int _hp;
    [SerializeField] private float iFrames = 0.5f;

    [Header("This section is for following healthbars only")]
    [SerializeField] SpriteRenderer border;
    [SerializeField] SpriteRenderer spriteFill;

    private Transform _fillTransform;
    Coroutine hideHP;

    [Header("This section is for Canvas UI healthbars only")]
    [SerializeField] Slider slider;
    [SerializeField] Gradient gradient;
    [SerializeField] Image imageFill;
    
    
    private bool isPlayer = false;
    private bool justGotHit = false;
    private bool isInvincible = false;

    void Awake()
    {
        if(this.gameObject.CompareTag("Player"))
            isPlayer = true;  
    }

    void Start()
    {   // Failsafe incase something starts out with 0 hp.
        if(_hp <= 0)
            _hp = MaxHP;

        _fillTransform = transform.GetChild(0);
        justGotHit = false;

        HideHealth();
        if(isPlayer)
        {
            slider.maxValue = MaxHP;
            VisualizePlayerHealth();
        } 
    }

    // Pass in positive values to heal, and negative values to damage health
    // If its negative, activate iFrames feature
    public void ChangeHealth(int amountChanged)
    {   
        if(amountChanged < 0)
        {
            if(isInvincible)
                return;
            else  
                StartCoroutine(InvincibilityFrames());   
        }


        if(amountChanged < 0)
            justGotHit = true;
        
        // We don't want to overkill/overheal past our health, so we clamp it between 0 and our Max healthpool
        _hp = Mathf.Clamp(_hp + amountChanged, 0, MaxHP);

        if(isPlayer)
            VisualizePlayerHealth();
        else
            VisualizeHealth();
    

        if(CurrentHP() <= 0)
        {   HideHealth();   } 
    }

    public void SetHealth(int value)
    {   _hp = value;    }

    public int CurrentHP()
    {   return _hp;     }

    public bool WasHit(){
        if(justGotHit){
            justGotHit = false;
            return true;
        }
        return false;
    }

    IEnumerator InvincibilityFrames()
    {
        isInvincible = true;        
        yield return new WaitForSeconds(iFrames);
        isInvincible = false;
    }

    IEnumerator HideTimer()
    {
        yield return new WaitForSeconds(4);
        HideHealth();
    }

    // For healthbars that are using Spriterender's and not some Canvas UI
    public void VisualizeHealth()
    {
        _fillTransform.localScale = new Vector3((float)_hp/MaxHP, _fillTransform.localScale.y, _fillTransform.localScale.z);

        border.enabled = true;
        spriteFill.enabled = true;

        // Resets the countdown to hide health
        if(hideHP != null)
            StopCoroutine(hideHP);
        
        hideHP = StartCoroutine(HideTimer());
    }

    public void HideHealth()
    {
        border.enabled = false;
        spriteFill.enabled = false;
    }

    // For the Player's HUD, or any Canvas based Healthbar
    public void VisualizePlayerHealth()
    {   
            slider.value = _hp;
            spriteFill.color = gradient.Evaluate(slider.normalizedValue);       
    }
}
