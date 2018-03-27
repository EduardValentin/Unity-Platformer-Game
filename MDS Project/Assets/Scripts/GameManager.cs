using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class GameManager : MonoBehaviour {
    //public GameObject mBackground;
    //public GameObject mCharacter;
    public GameObject mGameOverPanel;
    public bool mGameIsOver;
    public Text mScoreText;
    private bool mGameIsPaused;
    public Button mPlayButton;
    private void Start() { 
        Time.timeScale = 1f;
        mPlayButton.onClick.AddListener(RestartGame);
        mGameIsPaused = false;
        mScoreText.text = "Score: " + 0;
        mGameIsOver = false;
        mGameOverPanel.SetActive(false);
    }
    public void SpawnWalls(GameObject rwalls,float size)
    {
        Instantiate(rwalls, rwalls.transform.position + new Vector3(0, size, 0), rwalls.transform.rotation);
    }

    private void Update()
    {
        
    }
    public void UpdateScore(int val)
    {
        mScoreText.text = "Score: " + val;
    }

    public void RestartGame()
    {
        Application.LoadLevel(Application.loadedLevel);
    }

    public void GameOver()
    {
        // do some score saving
        PauseGame();
        mGameOverPanel.SetActive(true);
        mGameIsOver = true;
    }
    public bool GameIsPaused()
    {
        return mGameIsPaused;
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
