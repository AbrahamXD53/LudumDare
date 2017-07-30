using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BombTimer : MonoBehaviour {
    private static float MAX_TIMER = 3.0f;
    private static float TIMER_STEP = 1.0f / 60f;
    private bool isTicking = false;
    private float time;
    public GameObject owner;

	// Use this for initialization
	void Start () {
        time = MAX_TIMER;
        owner = null;
    }
	
	// Update is called once per frame
	void FixedUpdate () {
        if (owner != null)
        {
            isTicking = true;
        }
        else
        {
            isTicking = false;
            time = MAX_TIMER;
        }
		if (isTicking)
        {
            time -= TIMER_STEP;
            if (time <= 0.0f)
            {
                // BOOM
                GetComponent<SpriteRenderer>().color = Color.red;
            }
        }
	}

    private void Update()
    {
        if (owner != null)
        {
            float height = owner.GetComponent<SpriteRenderer>().size.y * 2.5f;
            transform.position = new Vector3(owner.transform.position.x, owner.transform.position.y + height, 0.0f);
        }
    }
}
