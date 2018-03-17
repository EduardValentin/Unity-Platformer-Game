using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallGenerator : MonoBehaviour {
    public GameObject mGameManager;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.name == "Spawner")
        {
            mGameManager.GetComponent<GameManager>().SpawnWalls(this.gameObject.transform.parent.gameObject,
                this.transform.parent.
                Find("PereteStanga").
                GetComponent<BoxCollider2D>().
                bounds.size.y);
        }
    }
}
