using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterController : MonoBehaviour
{

    private Rigidbody2D mRbody;
    private bool mJumpReady;
    private int mDirection;
    private int mScore;
    private GameManager mGameManager;
    [HideInInspector]
    public bool mJumpedOnce;

    void Start()
    {
        mJumpedOnce = false;
        mGameManager = GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>();
        mRbody = GetComponent<Rigidbody2D>();
        mJumpReady = true;
        mDirection = 1;
        mScore = 0;

    }

    void Update()
    {
        if (!mJumpReady)
        {
            // Cand ghemotocul e in aer crestem la fiecare frame scorul cu 1
            // Daca ghemotocul stationeaza , scorul nu creste

            mScore += 1;
            if (!mGameManager.mGameIsOver)
                mGameManager.UpdateScore(mScore);
        }

        print(mJumpReady);
        if (mJumpReady == true && Input.GetKeyDown("space"))
        {
            mJumpReady = false;

            if (this.GetComponent<FixedJoint2D>() != null)
                Destroy(this.GetComponent<FixedJoint2D>());

            mRbody.AddForce(new Vector2(600 * mDirection, 800));

            if (mDirection == 1)
                mDirection = -1;
            else
                mDirection = 1;
        }
    }

    public void OnCollisionEnter2D(Collision2D collision)
    {

        if (collision.gameObject.tag == "Perete")
        {
            mJumpedOnce = true;

            mJumpReady = true;
            if (this.gameObject.GetComponent<FixedJoint2D>() == null)
            {
                this.gameObject.AddComponent<FixedJoint2D>();
                this.gameObject.GetComponent<FixedJoint2D>().connectedBody = collision.rigidbody;
            }
        }
        else if (collision.gameObject.tag == "Obstacol" || collision.gameObject.tag == "GarbageCollector")
        {
            // ey game is over
            mGameManager.GameOver();
        }

    }
}
