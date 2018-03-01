using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class scriptMiscareVerticala : scriptObiect {

	
	// Update is called once per frame
	void Update () {
        doMovement();
	}

    void doMovement()
    {

        transform.position = new Vector3(transform.position.x,transform.position.y + mDirectie * mViteza * Time.deltaTime);
        float traversed = Vector2.Distance(mPozitieStart, transform.position);
        if (traversed >= mDistance)
            mDirectie *= -1;

    }
}
