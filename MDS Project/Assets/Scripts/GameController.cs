using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

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

    private void Start()
    {

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
        /*
        GameObject[] arrWalls = GameObject.FindGameObjectsWithTag("Perete");
        for (int i = 0; i < arrWalls.Length; i++)
        {
            arrWalls[i].transform.localScale = new Vector3(mWallSize, mScreenHeight);
        }
        print(mScreenHeight);
        */


    }
    public void SpawnWalls(GameObject cloneWalls, float size)
    {
        Instantiate(cloneWalls, cloneWalls.transform.position + new Vector3(0, size, 0), cloneWalls.transform.rotation);
    }
    public void SpawnObstacles(GameObject cloneObstacle, int numberOfObstacles,Vector2 origin)
    {
        // Spawn a set of obstacles at the screen with origin given
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
