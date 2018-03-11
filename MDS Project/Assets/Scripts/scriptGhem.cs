using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class scriptGhem : MonoBehaviour {

    // Use this for initialization
    Rigidbody2D mRbody;             // Pt iulian, m-ul din fata e o conventie ca datele membru sa fie cu m , te pup :*  . 
    Animator mAnimator;
    bool mJumpReady;
    bool mDirection;

    void Start()
    {
        mRbody = GetComponent<Rigidbody2D>();
        mAnimator = GetComponent<Animator>();
        mJumpReady = true;
        mDirection = true;
    }

    // Update is called once per frame
    void Update()
    {

        if (mJumpReady == true && Input.GetKeyDown("space")) 
        {
            mJumpReady = false;
            mAnimator.SetBool("jumping", true);

            if (this.GetComponent<FixedJoint2D>() != null)
                Destroy(this.GetComponent<FixedJoint2D>());

			if(mDirection)
            	mRbody.AddForce(new Vector2(600, 800));
			else
				mRbody.AddForce(new Vector2(-600, 800));

            mDirection = !mDirection;
        }
    }

    public void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.gameObject.tag == "Obstacol")
        {
            // game over
            print("game over my friend.");
        }
        else if(collision.gameObject.tag == "Perete")
        {
            mJumpReady = true;
            mAnimator.SetBool("jumping", false);

            this.gameObject.AddComponent<FixedJoint2D>();
            this.gameObject.GetComponent<FixedJoint2D>().connectedBody = collision.rigidbody;
        }

    }
}
