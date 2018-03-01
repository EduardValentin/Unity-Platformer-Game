using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class scriptPerete : MonoBehaviour {

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Obstacol")
        {
            collision.gameObject.GetComponent<scriptObiect>().schimbaDirectie();
        }
    }
}
