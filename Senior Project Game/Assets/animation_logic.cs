using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class animation_logic : MonoBehaviour
{
    [SerializeField] PlayerController playerController;
    [SerializeField] AnimatorController animatorController;

    // Start is called before the first frame update
    void Start()
    {
        if(playerController == null)
            playerController = this.gameObject.GetComponentInParent<PlayerController>();

        animatorController = this.gameObject.GetComponent<AnimatorController>();
    }

    // Update is called once per frame
    void Update()
    {
        // Faces the correct direction on the x-axis, and animates moving
        if(playerController.playerControls.Player.Move.IsInProgress()){
            animatorController.OrientateBody(playerController.playerControls.Player.Move.ReadValue<Vector2>().x);

            if(playerController.playerControls.Player.Move.ReadValue<Vector2>().y > 0)
                animatorController.PlayIntegerAnimation("Movestate",2);
            else
                animatorController.PlayIntegerAnimation("Movestate",1);
        }
        else
          animatorController.PlayIntegerAnimation("Movestate",0);
            

    //if(playerController.playerControls.Player.Move.IsInProgress())
    //     Debug.Log("Moving right now");
    // else
    //     Debug.Log("Not moving");


    }
}
