using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SawMovement : MonoBehaviour {

    // Use this for initialization
    void Start () {
    }
	
	// Update is called once per frame
	void Update () {
        transform.position = new Vector3 (transform.position.x, Mathf.PingPong(transform.position.y, 2.0f), transform.position.z);
    }
}
