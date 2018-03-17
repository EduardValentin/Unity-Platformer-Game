using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterController : MonoBehaviour
{

    private Rigidbody2D mRbody;
    private bool mJumpReady;
    private bool mDirection;
    private int mScore;
    private GameManager mGameManager;

    void Start()
    {
        mGameManager = GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>();
        mRbody = GetComponent<Rigidbody2D>();
        mJumpReady = true;
        mDirection = true;
        mScore = 0;

    }

    void FixedUpdate()
    {

        if (!mJumpReady)
        {
            // Cand ghemotocul e in aer crestem la fiecare frame scorul cu 1
            // Daca ghemotocul stationeaza , scorul nu creste

            mScore += 1;
            if (!mGameManager.mGameIsOver)
                mGameManager.UpdateScore(mScore);
        }

        if (mJumpReady == true && Input.GetKeyDown("space"))
        {
            mJumpReady = false;

            if (this.GetComponent<FixedJoint2D>() != null)
                Destroy(this.GetComponent<FixedJoint2D>());

            if (mDirection)
                mRbody.AddForce(new Vector2(600, 800));
            else
                mRbody.AddForce(new Vector2(-600, 800));

            mDirection = !mDirection;
        }
    }

    public void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Perete")
        {
            mJumpReady = true;

            this.gameObject.AddComponent<FixedJoint2D>();
            this.gameObject.GetComponent<FixedJoint2D>().connectedBody = collision.rigidbody;
        }
        else if (collision.gameObject.tag == "Obstacol")
        {
            // ey game is over
            mGameManager.GameOver();
        }

    }
}
