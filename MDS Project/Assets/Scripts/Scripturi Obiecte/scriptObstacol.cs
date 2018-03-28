using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class scriptObstacol : scriptObiect {

    public float mRotationalSpeed;

    public override void collisionAction(Collision2D col)
    {
        Debug.Log("Game Over my friend.");
    }
    void Update()
    {
        // Rotate the object around its local Z axis
        transform.Rotate(new Vector3(0,0,1) * Time.deltaTime * mRotationalSpeed);
    }
}
