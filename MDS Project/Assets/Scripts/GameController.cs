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
    public bool mGameIsOver;
    public Text mScoreText;
    private bool mGameIsPaused;
    public Button mPlayButton;
    private float mScreenHeight;
    public float mWallSize;
	private ScoreModel mHighestScore;
    private int mScore;
	private XmlDocument mScoreHistoryDB;
	public Text mHighestScoreText;
	public Text mLastScoreText;


    private void Start()
    {
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
    public void SpawnObstacles(GameObject cloneObstacle, int numberOfObstacles,Vector2 origin)
    {
        // Spawn a set of obstacles at the screen with origin given
<<<<<<< HEAD
       
=======
>>>>>>> 1ed070d769562738ba124aa9f6fdf5acacd01357
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
