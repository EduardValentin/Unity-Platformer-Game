using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallGenerator : MonoBehaviour {
    public GameObject mGameController;
    private Vector2 mCurrentScreenOrigin;
    private GameController mGameControllerScript;
    private void Start()
    {
        mCurrentScreenOrigin = transform.parent.Find("PereteStanga").transform.position;
        mGameControllerScript = mGameController.GetComponent<GameController>();
       
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.name == "Spawner")
        {
            mGameControllerScript.SpawnWalls(this.gameObject.transform.parent.gameObject, mGameControllerScript.GetScreenHeight());   // Spawn pereti noi
            //mGameControllerScript.SpawnWalls()
        }
    }
}
