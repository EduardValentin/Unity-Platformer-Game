using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour {
    private GameObject mCharacter;
   // private Vector3 mOffset;
    public float mSmoothTime = 0.3F;
    //private Vector3 mVelocity = Vector3.zero;

    // --
    public float mFollowFactor;
    public Vector3 mOffset;
    private Vector3 mLowerLimit;
    public float mDificultyScale = 1;

    // Use this for initialization
    void Start () {

        mCharacter = GameObject.FindGameObjectWithTag("Player");
    }

    void Update()
    {
        /*
        Vector3 targetPosition = mCharacterTransform.TransformPoint(new Vector3(0, 5, -10));
        targetPosition.x = 0;
        targetPosition.z = transform.position.z;
        transform.position = Vector3.SmoothDamp(transform.position, targetPosition, ref mVelocity, mSmoothTime);*/
        if(mCharacter.GetComponent<CharacterController>().mJumpedOnce)
        {
            mLowerLimit = transform.position + mOffset;

            float difference = mCharacter.transform.position.y - mLowerLimit.y;

            float factor = 1;
            if (difference > 0) 
                factor = (Mathf.Pow(2.14f,(difference / 2.3f)));
            transform.position += Vector3.up * Time.deltaTime * factor * mDificultyScale;
        }

    }
}
