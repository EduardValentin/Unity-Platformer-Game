using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackgroundFollow : MonoBehaviour {

    private GameObject mCharacter;
    private Vector3 mOffset;

    // Use this for initialization
    void Start()
    {
        mCharacter = GameObject.FindGameObjectWithTag("Player");
        mOffset = transform.position - mCharacter.transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = new Vector3(0, mCharacter.transform.position.y + mOffset.y, 0);
    }
}
