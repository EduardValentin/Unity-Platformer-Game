using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallGenerator : MonoBehaviour {
    public GameObject mGameController;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.name == "Spawner")
        {
            mGameController.GetComponent<GameController>().SpawnWalls(this.gameObject.transform.parent.gameObject,
                this.transform.parent.
                Find("PereteStanga").
                GetComponent<BoxCollider2D>().
                bounds.size.y);
        }
    }
}
