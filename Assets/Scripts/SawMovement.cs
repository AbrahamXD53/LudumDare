using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SawMovement : MonoBehaviour {

    private float pos;

    // Use this for initialization
    void Start () {
        pos = Random.Range(0.0f, 2.0f);
    }
	
	// Update is called once per frame
	void Update () {
        pos += Time.deltaTime;
        transform.position = new Vector2 (transform.position.x, Mathf.PingPong(pos, 1.0f) - 1.5f);
    }
}
