using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WinConditions : MonoBehaviour {

    private Animator anim;
    int wonLeft = Animator.StringToHash("TotemActive");
    int wonRight = Animator.StringToHash("TotemActive");
    // Use this for initialization
    void Start () {
        anim = GetComponent<Animator>();
    }
	
	// Update is called once per frame
	void Update () {
		
	}

    void OnCollisionEnter2D(Collision2D col)
    {
        
        if (col.gameObject.tag == "bomb" && name == "totem_Stand_Left")
        {
            print("Left Team won");
        }
        if (col.gameObject.tag == "bomb" && name == "totem_Stand_Right")
        {
            //anim.SetTrigger(wonRight);
            print("Right Team won");
        }
    }
}
