using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using UnityEditor;
using System;
using System.Xml;
using UnityEngine.SceneManagement;

public class GameController : MonoBehaviour
{

    public GameObject mGameOverPanel;
    public GameObject mCamera;
    public bool mGameIsOver;
    public Text mScoreText;
    public Button mPlayButton;
	public Text mHighestScoreText;
	public Text mLastScoreText;

    private int mScore;
    private float mScreenHeight;
    private float mScreenWidth;
    private bool mGameIsPaused;
    private ScoreModel mHighestScore;
    private XmlDocument mScoreHistoryDB;
    private GameObject mLeftWallBase;
    private GameObject mRightWallBase;
    private Vector3 mWallBoundsSize;
    private GameObject mCloneObstacle;
    private Vector2[] mDirections = { new Vector2(1, 0), new Vector2(-1, 0), new Vector2(0, 1), new Vector2(0, -1), new Vector2(1, 1), new Vector2(-1, -1), new Vector2(1, -1), new Vector2(-1, 1) };

    private void Awake()
    {
        mCloneObstacle = GameObject.FindGameObjectWithTag("ClonaObstacol");
        mLeftWallBase = GameObject.FindGameObjectWithTag("PereteStangaBase");
        mRightWallBase = GameObject.FindGameObjectWithTag("PereteDreaptaBase");

        mWallBoundsSize = mLeftWallBase.GetComponent<BoxCollider2D>().bounds.size;  // .x = width , .y = height, .z = depth of the gameobject

        mScoreHistoryDB = new XmlDocument ();
		mScoreHistoryDB.Load ("Assets/Scripts/XML/ScoreHistory.xml");	// Get the xml file content


		XmlNodeList xmlElements = mScoreHistoryDB.GetElementsByTagName ("value");
        int maxim = Int32.Parse(xmlElements[xmlElements.Count - 1].InnerText);
        
		mHighestScore = new ScoreModel (maxim, "dontcare");

        Time.timeScale = 1f;
        mPlayButton.onClick.AddListener(RestartGame);
        mGameIsPaused = false;
        mScoreText.text = "Score: " + 0;
        mGameIsOver = false;
        mGameOverPanel.SetActive(false);

        // Se calculeaza dimensiunile ecranului
        Vector2 topRightCorner = new Vector2(1, 1);
        Vector2 edgeVector = Camera.main.ViewportToWorldPoint(topRightCorner);  // Contine coordonatele coltului dreapta-sus al ecranului in starea initiala
        mScreenHeight = edgeVector.y * 2;
        mScreenWidth = edgeVector.x * 2;

    }
    public void SpawnWalls(GameObject cloneWalls, float size)
    {
        Instantiate(cloneWalls, cloneWalls.transform.position + new Vector3(0, size, 0), cloneWalls.transform.rotation);
    }

    private Vector2 WhereToSpawnObstacle(Vector2 withDirection,Vector2 atCameraOrigin,ref int[] choseFromPositionsHorz,ref int[] choseFromPositionsVert)
    {
        /*
            choseFromPositions este un vector cu 3 elemente, din care unele pot fi ne setate(cu valoarea 0) sau setate(disponibile cu valoarea 1)
            valoarea v[0] = sus / dreapta
            valoarea v[1] = centru
            valoarea v[2] = jos / stanga
         */
        float offsetFromWalls = mCloneObstacle.GetComponent<CircleCollider2D>().bounds.size.x / 2f + 0.5f;
        float thirdPercentWidth = 0.33f * (mScreenWidth - mWallBoundsSize.x);
        float thirdPercentHeight = 0.33f * mScreenHeight;

        float xpos = -999, ypos = -999;

        int[] availableHorz = new int[3];
        int indexHorz = 0;
        int[] availableVert = new int[3];
        int indexVert = 0;

        for(int i=0;i<3;i++)
        {
            if(choseFromPositionsHorz[i] == 1)
            {
                availableHorz[indexHorz] = i;
                indexHorz++;
            }

            if (choseFromPositionsVert[i] == 1)
            {
                availableVert[indexVert] = i;
                indexVert++;
            }
        }

        if (indexHorz < 3)
            availableHorz[indexHorz] = -1;
        if (indexVert < 3)
            availableVert[indexVert] = -1;

        int choseAxis;  // 1 = vertical, 2 = orizontal
        float screenBottomMarginY = atCameraOrigin.y - mScreenHeight / 2f;
        float screenTopMarginY = atCameraOrigin.y + mScreenHeight / 2f;
        float screenLeftMarginX = (atCameraOrigin.x - mScreenWidth / 2f) + mWallBoundsSize.x/2 + offsetFromWalls; 
        float screenRightMarginX = (atCameraOrigin.x + mScreenWidth / 2f) - mWallBoundsSize.x/2 - offsetFromWalls;

        /*print("left = " + (screenLeftMarginX - offsetFromWalls));
        print("right = " + (screenRightMarginX + offsetFromWalls));
        print("third p width = " + thirdPercentWidth);*/

        if (withDirection == new Vector2(0,1))
        {
            // se duce in sus

            choseAxis = 2;
            ypos = UnityEngine.Random.Range(screenBottomMarginY , screenBottomMarginY + thirdPercentHeight);

        } else if (withDirection == new Vector2(0,-1))
        {
            choseAxis = 2;
            ypos = UnityEngine.Random.Range(screenTopMarginY, screenTopMarginY + thirdPercentHeight);
        } else if (withDirection == new Vector2(1,0))
        {
            // se duce in dreapta
            choseAxis = 1;
            xpos = UnityEngine.Random.Range(screenLeftMarginX, screenLeftMarginX + thirdPercentWidth);
        } else if(withDirection == new Vector2(-1,0))
        {
            // se duce in stanga
            choseAxis = 1;
            xpos = UnityEngine.Random.Range(screenRightMarginX , screenRightMarginX - thirdPercentWidth);

        } else if(withDirection == new Vector2(1,1))
        {
            // dreapta sus

            choseAxis = 0;
            xpos = UnityEngine.Random.Range(screenLeftMarginX, screenLeftMarginX + thirdPercentWidth);
            ypos = UnityEngine.Random.Range(screenBottomMarginY, screenBottomMarginY + thirdPercentHeight);

        }
        else if(withDirection == new Vector2(1, -1))
        {
            // dreapta jos

            choseAxis = 0;
            xpos = UnityEngine.Random.Range(screenLeftMarginX, screenLeftMarginX + thirdPercentWidth);
            ypos = UnityEngine.Random.Range(screenTopMarginY, screenTopMarginY + thirdPercentHeight);

        } else if(withDirection == new Vector2(-1,-1))
        {
            // stanga jos

            choseAxis = 0;
            xpos = UnityEngine.Random.Range(screenRightMarginX, screenRightMarginX - thirdPercentWidth);
            ypos = UnityEngine.Random.Range(screenTopMarginY, screenTopMarginY + thirdPercentHeight);

        } else if(withDirection == new Vector2(-1, 1))
        {
            // spre stanga sus
            choseAxis = 0;
            xpos = UnityEngine.Random.Range(screenRightMarginX, screenRightMarginX - thirdPercentWidth);
            ypos = UnityEngine.Random.Range(screenBottomMarginY, screenBottomMarginY + thirdPercentHeight);
        } else
        {
            throw new InvalidOperationException();
        }

        if (choseAxis == 1)
        {
            int randIndex = UnityEngine.Random.Range(0, availableVert.Length);
            int chosenVertPos = availableVert[randIndex];

            if (chosenVertPos == -1)
            {
                while (chosenVertPos == -1 && randIndex >= 0)
                {
                    randIndex--;
                    chosenVertPos = availableVert[randIndex];
                }
            }
            choseFromPositionsVert[chosenVertPos] = 0;
            switch (chosenVertPos)
            {
                case 0:
                    ypos = UnityEngine.Random.Range(screenTopMarginY, screenTopMarginY + thirdPercentHeight);
                    break;
                case 1:
                    ypos = UnityEngine.Random.Range(screenTopMarginY + thirdPercentHeight, screenTopMarginY + 2 * thirdPercentHeight);
                    break;

                case 2:
                    ypos = UnityEngine.Random.Range(screenTopMarginY + 2 * thirdPercentHeight, screenTopMarginY + 3 * thirdPercentHeight);
                    break;
                default:
                    Debug.Log("Error in available vertical positions.");
                    break;
            }
        }
        else if (choseAxis == 2)
        {
            int randIndex = UnityEngine.Random.Range(0, availableHorz.Length);
            int chosenHorzPos = availableHorz[randIndex];

            if(chosenHorzPos == -1)
            {
                while(chosenHorzPos == -1 && randIndex >= 0)
                {
                    randIndex--;
                    chosenHorzPos = availableHorz[randIndex];
                }
            }

            choseFromPositionsHorz[chosenHorzPos] = 0;
            switch (chosenHorzPos)
            {
                case 0:
                    xpos = UnityEngine.Random.Range(screenLeftMarginX, screenLeftMarginX + thirdPercentWidth);
                    break;
                case 1:
                    xpos = UnityEngine.Random.Range(screenLeftMarginX + thirdPercentWidth, screenLeftMarginX + 2 * thirdPercentWidth);
                    break;

                case 2:
                    xpos = UnityEngine.Random.Range(screenLeftMarginX + 2 * thirdPercentWidth, screenLeftMarginX + 3 * thirdPercentWidth);
                    break;
                default:
                    Debug.Log("Error in available horizontal positions.");
                    break;
            }
        }

        print("spawning at: " + new Vector2(xpos, ypos));
        return new Vector2(xpos, ypos);
    }

    public void SpawnMovingObstacle(Vector3 atCameraOrigin)
    {
        float speedRand = UnityEngine.Random.Range(1, 2);
        int chosenDirection = UnityEngine.Random.Range(0, mDirections.Length - 1);
        int[] verticalPosAvailable = { 1, 1, 1 };
        int[] horizontalPosAvailable = { 1, 1, 1 };

        Vector2 spawnOrigin = WhereToSpawnObstacle(mDirections[chosenDirection], atCameraOrigin,ref horizontalPosAvailable,ref verticalPosAvailable);   // !!! Direction must not be normalized
        Vector2 targetPoint = scriptObiect.ComputeTargetPoint(spawnOrigin, mDirections[chosenDirection].normalized, 2);

        GameObject newObstacle = Instantiate(mCloneObstacle, spawnOrigin, mCloneObstacle.transform.rotation);
        newObstacle.GetComponent<scriptObstacol>().SetAndComputeProperties(mDirections[chosenDirection], newObstacle.transform.position, speedRand, 2);
        newObstacle.gameObject.tag = "Obstacol";
    }
    /*
    public void SpawnMovingOneObstacleAtOrigin(Vector3 origin)
    {
        //print("screen origin at" + origin);
        float offsetFromWallOrigin = mWallBoundsSize.x / 2 + 1;
        float offsetFromCameraOrigin = mScreenHeight / 2 - 1;

        float xrand = UnityEngine.Random.Range(mLeftWallBase.transform.position.x + offsetFromWallOrigin, mRightWallBase.transform.position.x - offsetFromWallOrigin);
        float yrand = UnityEngine.Random.Range(origin.y - offsetFromCameraOrigin, origin.y + offsetFromCameraOrigin);
        float speedRand = UnityEngine.Random.Range(1, 2);

        int chosenDirection = UnityEngine.Random.Range(0, mDirections.Length-1);

        Vector2 targetPoint = scriptObiect.ComputeTargetPoint(origin, mDirections[chosenDirection].normalized, 2);
        Vector2 obstacleOrigin = new Vector2(xrand, yrand);

        RaycastHit2D hit = Physics2D.Raycast(obstacleOrigin,mDirections[chosenDirection],2);

      //  Ray horizontalRay = new Ray(obstacleOrigin, mDirections[chosenDirection]);
        if (hit && hit.transform.gameObject.tag == "Perete" && hit.distance < Vector2.Distance(obstacleOrigin, targetPoint))
        {
            print("I will adjust");
            // need to adjust
            float difference = Vector2.Distance(obstacleOrigin, targetPoint) - hit.distance + 2;
            obstacleOrigin = obstacleOrigin - (mDirections[chosenDirection].normalized * difference);
        } 
  
        GameObject newObstacle = Instantiate(mCloneObstacle,obstacleOrigin, mCloneObstacle.transform.rotation);
        newObstacle.GetComponent<scriptObstacol>().SetAndComputeProperties(mDirections[chosenDirection],newObstacle.transform.position,speedRand,2);
        newObstacle.gameObject.tag = "Obstacol";
        //print("spawning new obstacle at " + newObstacle.GetComponent<scriptObstacol>().mPozitieStart );

    }*/

    public void UpdateScoreView()
    {
		mScore +=1;
        mScoreText.text = "Score: " + mScore;
    }

    public void RestartGame()
    {
		SceneManager.LoadScene("gameplay", LoadSceneMode.Single);
    }

    public void GameOver()
    {
		if( mHighestScore.getScore() < mScore)
		{
			mHighestScore = new ScoreModel(mScore,DateTime.Now.ToString("MM/dd/yyyy"));

			// Insert to database

			mScoreHistoryDB = new XmlDocument ();
			mScoreHistoryDB.Load ("Assets/Scripts/XML/ScoreHistory.xml");	// Get the xml file content

			XmlElement xmlbase = mScoreHistoryDB.DocumentElement;
			XmlElement scoreElement =  mScoreHistoryDB.CreateElement ("score");
			XmlElement scoreValue = mScoreHistoryDB.CreateElement ("value");
			scoreValue.InnerText = mHighestScore.getScore().ToString();
			XmlElement scoreDate = mScoreHistoryDB.CreateElement ("date");
			scoreDate.InnerText = mHighestScore.getDate ();
			scoreElement.AppendChild (scoreValue);
			scoreElement.AppendChild (scoreDate);
			xmlbase.AppendChild (scoreElement);
		
			mScoreHistoryDB.Save("Assets/Scripts/XML/ScoreHistory.xml");
		}		

		mHighestScoreText.text = "Highest Score: " + mHighestScore.getScore();
		mLastScoreText.text = "Latest Score: " + mScore;

		PauseGame();
        mGameOverPanel.SetActive(true);
        mGameIsOver = true;

    }
    public bool GameIsPaused()
    {
        return mGameIsPaused;
    }
    public float GetScreenHeight()
    {
        return mScreenHeight;
    }
    public void PauseGame()
    {
        mGameIsPaused = true;
        Time.timeScale = 0;
    }
    public void ResumeGame()
    {
        Time.timeScale = 1;
        mGameIsPaused = false;
    }
}
