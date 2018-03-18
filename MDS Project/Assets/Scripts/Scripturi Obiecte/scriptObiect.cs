using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class scriptObiect : MonoBehaviour {

    public float mDuratieTravers;
    public float mTestDistance;
    public float mWaitAfterTarget;
    public Vector2 mTestDirection;
    private Vector2 mTargetPoint;

    protected Vector2 mPozitieStart;        // Pozitia initiala a obstacolului

    
    protected virtual void Start()
    {
        mTestDirection = mTestDirection.normalized;
        mPozitieStart = transform.position;
        mTargetPoint = mPozitieStart + mTestDirection * mTestDistance;

        StartCoroutine(moveLinear(mWaitAfterTarget));   // Incepe miscarea

    }


    protected IEnumerator moveLinear(float waitDuration)
    {
        // Loops each cycles
        while (Application.isPlaying)
        {
            // First step, travel from A to B
            float counter = 0f;
            while (counter < mDuratieTravers)
            {
                transform.position = Vector3.Lerp(mPozitieStart, mTargetPoint, counter / mDuratieTravers);
                counter += Time.deltaTime;
                yield return null;
            }

            // Make sure you're exactly at B, in case the counter 
            // wasn't precisely equal to travelDuration at the end
            transform.position = mTargetPoint;

            // Second step, wait
            yield return new WaitForSeconds(waitDuration);

            // Third step, travel back from B to A
            counter = 0f;
            while (counter < mDuratieTravers)
            {
                transform.position = Vector3.Lerp(mTargetPoint, mPozitieStart, counter / mDuratieTravers);
                counter += Time.deltaTime;
                yield return null;
            }

            transform.position = mPozitieStart;

            // Finally, wait
            yield return new WaitForSeconds(waitDuration);
        }

    }

    abstract public void collisionAction(Collision2D col);         // Trebuie supraincarcata in scripturile speciala pentru miscari
}
