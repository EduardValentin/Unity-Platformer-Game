using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class scriptObstacol : scriptObiect {

	void Update () {
        doMovement();
	}

    protected override void collisionAction(Collision2D col)
    {
        Debug.Log("Game Over my friend.");
    }

    public void OnCollisionEnter2D (Collision2D collision)
    {
        collisionAction(collision);
    }
}
