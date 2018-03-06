using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class scriptObiect : MonoBehaviour {

    public float mDistance;           // Distanta traversata de obstacol in directie sus-jos
    public float mViteza;             // Viteza cu care se misca
    public Miscari mTipMiscare;

    protected int mDirectie;          // Directia in care se misca obstacolul. 1 = se misca in sus, -1 = se misca in jos
    protected Vector2 mPozitieStart;  // Pozitia initiala a obstacolului

    public enum Miscari {
        Vertical,
        Orizontal,
        DiagonalaPrincipala,
        DiagonalaSecundara,
        Circular
    };

    
    void Start()
    {
        mPozitieStart = transform.position;
        int[] directii = new int[2] { -1, 1 };
        int index = Random.Range(0, 1);
        mDirectie = directii[index];
    }

    public void schimbaDirectie()
    {
        mDirectie *= -1;
    }

    protected void doMovement() {

        switch (mTipMiscare)
        {
            case Miscari.Vertical:
            {
                transform.position = new Vector3(transform.position.x, transform.position.y + mDirectie * mViteza * Time.deltaTime);
                float traversed = Vector2.Distance(mPozitieStart, transform.position);
                if (traversed >= mDistance)
                    mDirectie *= -1;
                break;
            }
            case Miscari.Orizontal:
            {

                    transform.position = new Vector3(transform.position.x + mDirectie * mViteza * Time.deltaTime, transform.position.y);
                    float traversed = Vector2.Distance(mPozitieStart, transform.position);
                    if (traversed >= mDistance)
                        mDirectie *= -1;
                    break;
            }
            default:
            {
                    break;
            }
        }
    }

    abstract protected void collisionAction(Collision2D col);         // Trebuie supraincarcata in scripturile speciala pentru miscari

    public void OnCollisionEnter2D(Collision2D collision)
    {
        collisionAction(collision);
    }
}
