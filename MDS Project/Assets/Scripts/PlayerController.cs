using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private GameController mGameController;
    public GameObject mGameControllerObj;
    public float mStickiness;
    private Rigidbody2D mRbody;
    private bool mJumpReady;
    private int mDirection;
    private int mScore;
    [HideInInspector]
    public bool mJumpedOnce;
	public int mHighestPoint;

    void Start()
    {
		mHighestPoint = (int)transform.position.y;
        mJumpedOnce = false;
        mRbody = GetComponent<Rigidbody2D>();
        mJumpReady = true;
        mDirection = 1;
        mGameController = mGameControllerObj.GetComponent<GameController>();
    }

    void Update()
    {
        if (!mJumpReady)
        {
            // Cand ghemotocul e in aer crestem la fiecare frame scorul cu 1
            // Daca ghemotocul stationeaza , scorul nu creste
			if ((int)transform.position.y + 0.5 > mHighestPoint && !mGameController.mGameIsOver)
			{
                mGameController.UpdateScoreView();
			
				mHighestPoint = (int)transform.position.y;
			}
		}
        else
        {
            
            if (mJumpedOnce)
            {
                // Daca playerul a sarit macar o data si se afla pe perete

                Vector2 direction = new Vector2(mDirection * -1, 0);
                mRbody.AddForce(direction * mStickiness);
            }
        }

        if (mJumpReady == true && Input.GetKeyDown("space"))
        {

            mJumpReady = false;
            mRbody.AddForce(new Vector2(1100 * mDirection, 950));

            if (mDirection == 1)
                mDirection = -1;
            else
                mDirection = 1;
        }
    }


    public bool isInAir()
    {
        if (!mJumpReady)
        {
            return true;
        }
        return false;
    }

    public void OnCollisionEnter2D(Collision2D collision)
    {

        if (collision.gameObject.tag == "PereteStangaBase" || collision.gameObject.tag == "Perete" || collision.gameObject.tag == "PereteDreaptaBase")
        {
            mJumpedOnce = true;
            mJumpReady = true;

        }
        else if (collision.gameObject.tag == "ObstacolColider" || collision.gameObject.tag == "GarbageCollector")
        {
            // ey game is over
            mGameController.GameOver();
        }

    }
}
