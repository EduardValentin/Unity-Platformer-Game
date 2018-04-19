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
    //public GameObject mBackground;
    //public GameObject mCharacter;
    public GameObject mGameOverPanel;
    public GameObject mCamera;
    public bool mGameIsOver;
    public Text mScoreText;
    public Button mPlayButton;
	public Text mHighestScoreText;
	public Text mLastScoreText;

    private int mScore;
    private float mScreenHeight;
    private bool mGameIsPaused;
    private ScoreModel mHighestScore;
    private XmlDocument mScoreHistoryDB;
    private GameObject mLeftWallBase;
    private GameObject mRightWallBase;
    private Vector3 mWallBoundsSize;
    private GameObject mCloneObstacle;
    private Vector2[] mDirections = { new Vector2(1, 0), new Vector2(-1, 0), new Vector2(0, 1), new Vector2(0, -1), new Vector2(1, 1), new Vector2(-1, -1), new Vector2(1, -1), new Vector2(-1, 1) };

    private void Start()
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

        // Se calculeaza inaltimea ecranului
        Vector2 topRightCorner = new Vector2(1, 1);
        Vector2 edgeVector = Camera.main.ViewportToWorldPoint(topRightCorner);  // Contine coordonatele coltului dreapta-sus al ecranului in starea initiala
        mScreenHeight = edgeVector.y * 2;

    }
    public void SpawnWalls(GameObject cloneWalls, float size)
    {
        Instantiate(cloneWalls, cloneWalls.transform.position + new Vector3(0, size, 0), cloneWalls.transform.rotation);
    }
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

    }
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
