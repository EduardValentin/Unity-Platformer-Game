using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class script : MonoBehaviour {

    // Use this for initialization
    Rigidbody2D rbody;

	void Start () {
        rbody = GetComponent<Rigidbody2D>();	
	}
	
	// Update is called once per frame
	void Update () {
        if( Input.GetKeyDown("space") && rbody.velocity == new Vector2(0,0))
        {
            rbody.AddForce(new Vector2(300, 800));
        }
		
	}
}
