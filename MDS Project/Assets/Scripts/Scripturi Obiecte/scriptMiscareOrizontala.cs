using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class scriptMiscareOrizontala : scriptObiect {

	
	// Update is called once per frame
	void Update () {
        doMovement();
	}

    void doMovement()
    {

        transform.position = new Vector3(transform.position.x + mDirectie * mViteza * Time.deltaTime, transform.position.y);
        float traversed = Vector2.Distance(mPozitieStart, transform.position);
        if (traversed >= mDistance)
            mDirectie *= -1;

    }
}
