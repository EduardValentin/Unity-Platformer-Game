using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour {
    private Transform mCharacterTransform;
    private Vector3 mOffset;
    public float mSmoothTime = 0.3F;
    private Vector3 mVelocity = Vector3.zero;
    private GameManager mGameManager;

    // Use this for initialization
    void Start () {
        mGameManager = GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>();
        mCharacterTransform = GameObject.FindGameObjectWithTag("Player").transform;
    }

    void FixedUpdate()
    {
        Vector3 targetPosition = mCharacterTransform.TransformPoint(new Vector3(0, 5, -10));
        targetPosition.x = 0;
        targetPosition.z = transform.position.z;
        transform.position = Vector3.SmoothDamp(transform.position, targetPosition, ref mVelocity, mSmoothTime);
    }
}
