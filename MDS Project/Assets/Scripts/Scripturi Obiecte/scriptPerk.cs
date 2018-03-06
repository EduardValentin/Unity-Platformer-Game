using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class scriptPerk : scriptObiect {

	void Update () {
        doMovement();
	}

    protected override void collisionAction(Collision2D col)
    {
        Debug.Log("You just earned a perk yay.");
    }

    public void OnCollisionEnter2D(Collision2D collision)
    {
        collisionAction(collision);
    }
}
