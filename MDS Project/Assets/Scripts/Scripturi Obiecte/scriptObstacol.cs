using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class scriptObstacol : scriptObiect {
    
    public override void collisionAction(Collision2D col)
    {
        Debug.Log("Game Over my friend.");
    }

}
