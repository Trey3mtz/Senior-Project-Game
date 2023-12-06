using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthBar : MonoBehaviour
{
    public int MaxHP = 10;
    public int _hp;
    private bool justGotHit;
    private Transform _fill;

    [SerializeField] SpriteRenderer border;
    [SerializeField] SpriteRenderer fill;

    PlayerHealthBar playerHP;
    private bool yesPlayer = false;
    // Start is called before the first frame update
    void Awake()
    {
        if(this.gameObject.CompareTag("Player")){
            playerHP =  GameObject.Find("PlayerHealthBar").GetComponent<PlayerHealthBar>();
            yesPlayer = true;
        }
           
    }

    void Start()
    {
        _hp = MaxHP;
        _fill = transform.GetChild(0);
        justGotHit = false;

        HideHealth();
    }

    public void ChangeHealth(int amt){
        if(_hp > Mathf.Clamp(_hp + amt, 0, MaxHP)){
            justGotHit = true;
        }
        _hp = Mathf.Clamp(_hp + amt, 0, MaxHP);
        _fill.localScale = new Vector3((float)(_hp)/MaxHP, _fill.localScale.y, _fill.localScale.z);


        if(yesPlayer)
            playerHP.ChangeHealth(amt);
        else
            VisualizeHealth();
    

        if(currentHP() <= 0){
            HideHealth();
        }
           
    }

    public int currentHP(){
        return _hp;
    }

    public bool wasHit(){
        if(justGotHit){
            justGotHit = false;
            return true;
        }
        return false;
    }

    IEnumerator HideTimer()
    {
        yield return new WaitForSeconds(4);
        HideHealth();
    }

    public void VisualizeHealth()
    {
        border.enabled = true;
        fill.enabled = true;

        StartCoroutine(HideTimer());
    }

    public void HideHealth()
    {
        border.enabled = false;
        fill.enabled = false;
    }
}
