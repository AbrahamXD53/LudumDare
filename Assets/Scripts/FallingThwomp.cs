using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FallingThwomp : MonoBehaviour {

    public float positionY;
    public float positionX;
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
        if(col.gameObject.tag == "Player")
        {
            //destruye la isntancia del player
            //Destroy(col.gameObject);
            Debug.Log("ALV el men");
        }
    }
}