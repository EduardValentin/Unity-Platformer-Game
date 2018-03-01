using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class scriptObiect : MonoBehaviour {

    public float mDistance;         // Distanta traversata de obstacol in directie sus-jos
    public float mViteza;           // Viteza cu care se misca
    protected int mDirectie;          // Directia in care se misca obstacolul. 1 = se misca in sus, -1 = se misca in jos
    protected Vector2 mPozitieStart;  // Pozitia initiala a obstacolului

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

    virtual protected void doMovement() { }           // Trebuie supraincarcata in scripturile speciala pentru miscari
}
