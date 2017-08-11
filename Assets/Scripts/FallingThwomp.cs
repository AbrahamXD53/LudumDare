using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FallingThwomp : MonoBehaviour {

    public float positionY;
    public float positionX;

    private GameObject bomb = null;

    // Use this for initialization
    void Start () {
        positionY = transform.position.y;
        positionX = transform.position.x;
	}
	
	// Update is called once per frame
	void FixedUpdate () {

	}

    void OnCollisionEnter2D(Collision2D col)
    {
        if (col.gameObject)
        {
            //regresa el enemigo a su posicion original
            transform.position = new Vector2(positionX, positionY);
        }
        if(col.gameObject.tag == "bomb")
        {
            if(bomb != null)
            {
                float width = GetComponent<SpriteRenderer>().size.x;
                if (GetComponent<CharacterMovementController>().facingRight)
                {
                    bomb.transform.position = new Vector3(transform.position.x + width, transform.position.y, 0.0f);
                }
                else
                {
                    bomb.transform.position = new Vector3(transform.position.x - width, transform.position.y, 0.0f);
                }
                bomb.GetComponent<Rigidbody2D>().gravityScale = 1.0f;
                bomb.GetComponent<BombTimer>().owner = null;
                bomb = null;
            }
        } else if(col.gameObject.tag == "Player")
        {
            CharacterMovementController cmc = col.gameObject.GetComponent<CharacterMovementController>();
            if (cmc != null)
            {
                cmc.Reverse();
            }
        }
    }
}