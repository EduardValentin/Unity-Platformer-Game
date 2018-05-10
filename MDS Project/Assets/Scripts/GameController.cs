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
	private struct Axis {
		public  const int Horizontal = 1;
		public  const int Vertical = 2;
	};

	private struct VerticalPositions {
		public const int Top = 0;
		public const int Center = 1;
		public const int Bot = 2;
	};

	private struct HorizontalPositions {
		public const int Left = 0;
		public const int Center = 1;
		public const int Right = 2;
	};

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
    private Vector3 mWallBoundsSize;
    private GameObject mCloneObstacle;
    private Vector2[] mDirections = { 
		new Vector2(1, 0),
		new Vector2(-1, 0),
		new Vector2(0, 1),
		new Vector2(0, -1), 
		new Vector2(1, 1), 
		new Vector2(-1, -1),
		new Vector2(1, -1), 
		new Vector2(-1, 1) 
	};

	private float mThirdPercentHeight;
	private float mThirdPercentWidth;
	private float mOffsetFromWalls;
	private float mScreenLeftMarginX; 
	private float mScreenRightMarginX;


    private void Awake()
    {
        mCloneObstacle = GameObject.FindGameObjectWithTag("ClonaObstacol");
       
		// Se updateaza scorul maxim
        mScoreHistoryDB = new XmlDocument ();
		mScoreHistoryDB.Load ("Assets/Scripts/XML/ScoreHistory.xml");	// Get the xml file content
		XmlNodeList xmlElements = mScoreHistoryDB.GetElementsByTagName ("value");
        int maxim = Int32.Parse(xmlElements[xmlElements.Count - 1].InnerText);
		mHighestScore = new ScoreModel (maxim, "dontcare");

		// Configuratie initiala
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

		// Se calculeaza marginile impreuna cu alte dimensiuni pentru calcule ulterioare
		mWallBoundsSize = GameObject.FindGameObjectWithTag("PereteDreaptaBase").GetComponent<BoxCollider2D>().bounds.size;  // .x = width , .y = height, .z = depth of the gameobject
		mThirdPercentHeight = 0.33f * mScreenHeight;
		mThirdPercentWidth = 0.33f * (mScreenWidth - 2*mWallBoundsSize.x);
		mOffsetFromWalls = mCloneObstacle.transform.Find("HorizontalColider").GetComponent<BoxCollider2D>().bounds.size.x / 2f + 0.3f; //!!!!!!!
		mScreenLeftMarginX = (mCamera.transform.position.x - (mScreenWidth - 2*mWallBoundsSize.x) / 2f); 
		mScreenRightMarginX = (mCamera.transform.position.x + (mScreenWidth - 2*mWallBoundsSize.x) / 2f);

    }
    public void SpawnWalls(GameObject cloneWalls, float size)
    {
        Instantiate(cloneWalls, cloneWalls.transform.position + new Vector3(0, size, 0), cloneWalls.transform.rotation);
    }

	private int ChoseRandomHorizontalPosition( int[] fromAvailable , out float xpos) 
	{
		int randIndex = UnityEngine.Random.Range(0, fromAvailable.Length-1);
		int chosenHorzPos = fromAvailable[randIndex];

		if(chosenHorzPos == -1)
		{
			while(chosenHorzPos == -1 && randIndex > 0)
			{
				randIndex--;
				chosenHorzPos = fromAvailable[randIndex];
			}
		}
			
		switch (chosenHorzPos)
		{
		case HorizontalPositions.Left:
			xpos = UnityEngine.Random.Range(mScreenLeftMarginX+ mOffsetFromWalls, mScreenLeftMarginX + mThirdPercentWidth - mOffsetFromWalls) ;
			break;
		case HorizontalPositions.Center:
			xpos = UnityEngine.Random.Range(mScreenLeftMarginX + mThirdPercentWidth + mOffsetFromWalls, mScreenLeftMarginX + 2 * mThirdPercentWidth - mOffsetFromWalls);
			break;
		case HorizontalPositions.Right:
			xpos = UnityEngine.Random.Range(mScreenLeftMarginX + 2 * mThirdPercentWidth +mOffsetFromWalls, mScreenLeftMarginX + 3 * mThirdPercentWidth - mOffsetFromWalls);
			break;
		default:
			Debug.Log ("Error in available horizontal positions.");
			xpos = -9999;
			break;
		}

		return chosenHorzPos;
	}

	private int ChoseRandomVerticalPosition(int[] fromAvailable, float withScreenTopMarginY, out float ypos)
	{
	
		int randIndex = UnityEngine.Random.Range(0, fromAvailable.Length-1);
		int chosenVertPos = fromAvailable[randIndex];

		if (chosenVertPos == -1)	// daca s-a ales o pozitie goala se cauta prima pozitie disponibila catre stanga
		{
			while (chosenVertPos == -1 && randIndex > 0)
			{
				randIndex--;
				chosenVertPos = fromAvailable[randIndex];
			}
		}

		switch (chosenVertPos)
		{
		case VerticalPositions.Top:
			ypos = UnityEngine.Random.Range(withScreenTopMarginY - mOffsetFromWalls, withScreenTopMarginY - mThirdPercentHeight + mOffsetFromWalls);
			break;
		case VerticalPositions.Center:
			ypos = UnityEngine.Random.Range(withScreenTopMarginY - mThirdPercentHeight - mOffsetFromWalls, withScreenTopMarginY - 2 * mThirdPercentHeight + mOffsetFromWalls);
			break;

		case VerticalPositions.Bot:
			ypos = UnityEngine.Random.Range(withScreenTopMarginY - 2 * mThirdPercentHeight - mOffsetFromWalls, withScreenTopMarginY - 3 * mThirdPercentHeight + mOffsetFromWalls);
			break;
		default:
			Debug.Log ("Error in available vertical positions.");
			ypos = -9999;
			break;
		}
		return chosenVertPos;
	}

    private Vector2 WhereToSpawnObstacle(Vector2 withDirection,Vector2 atCameraOrigin,ref int[] choseFromPositionsHorz,ref int[] choseFromPositionsVert)
    {     
        float xpos = -999, ypos = -999;

        int[] availableHorz = new int[3];
		int[] availableVert = new int[3];

		int indexHorz = 0;
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

		if (indexHorz < 3)	// daca nu s-au umplut toate pozitiile se pune un capat de vector (i.e. -1)
            availableHorz[indexHorz] = -1;
        if (indexVert < 3)
            availableVert[indexVert] = -1;

        int choseAxis;	// in functie de directia pe care se merge, se va alege o coordonata, urmand ca cealalta sa fie aleasa la final pentru a evita alegerea ei pe fiecare branch si duplicarea codului

        float screenBottomMarginY = atCameraOrigin.y - mScreenHeight / 2f;
        float screenTopMarginY = atCameraOrigin.y + mScreenHeight / 2f;
        

        //print(screenLeftMarginX + 3 * thirdPercentWidth);
        if (withDirection == new Vector2(0,1))
        {
            // se duce in sus
			choseAxis = Axis.Horizontal;
            ypos = UnityEngine.Random.Range(screenBottomMarginY , screenBottomMarginY + mThirdPercentHeight);
        } else if (withDirection == new Vector2(0,-1))
        {
			choseAxis = Axis.Horizontal;
            ypos = UnityEngine.Random.Range(screenTopMarginY, screenTopMarginY - mThirdPercentHeight);
        } else if (withDirection == new Vector2(1,0))
        {
            // se duce in dreapta
			choseAxis = Axis.Vertical;
			xpos = UnityEngine.Random.Range(mScreenLeftMarginX, mScreenLeftMarginX + mThirdPercentWidth);
        } else if(withDirection == new Vector2(-1,0))
        {
            // se duce in stanga
			choseAxis = Axis.Vertical;
			xpos = UnityEngine.Random.Range(mScreenRightMarginX , mScreenRightMarginX - mThirdPercentWidth);

        } else if(withDirection == new Vector2(1,1))
        {
            // dreapta sus
            choseAxis = 0;  // nu mai ramane nimic de ales
			xpos = UnityEngine.Random.Range(mScreenLeftMarginX, mScreenLeftMarginX + mThirdPercentWidth);    
			int chosenPosition = ChoseRandomVerticalPosition (availableVert, screenTopMarginY, out ypos);	// chose a vertical position and set in ypos
			choseFromPositionsVert[chosenPosition] = 0;	// unset the position
        }
        else if(withDirection == new Vector2(1, -1))
        {
            // dreapta jos

            choseAxis = 0;	// nu mai ramane nimic de ales la final			 
			xpos = UnityEngine.Random.Range(mScreenLeftMarginX, mScreenLeftMarginX + mThirdPercentWidth);
			int chosenPosition = ChoseRandomVerticalPosition (availableVert, screenTopMarginY, out ypos);	// chose a vertical position and set in ypos
			choseFromPositionsVert[chosenPosition] = 0;	// unset the position

        } else if(withDirection == new Vector2(-1,-1))
        {
            // stanga jos
            choseAxis = 0;
            xpos = UnityEngine.Random.Range(mScreenRightMarginX, mScreenRightMarginX - mThirdPercentWidth);
			int chosenPosition = ChoseRandomVerticalPosition (availableVert, screenTopMarginY, out ypos);	
			choseFromPositionsVert[chosenPosition] = 0;	// unset the position

        } else if(withDirection == new Vector2(-1, 1))
        {
            // spre stanga sus
            choseAxis = 0;
            xpos = UnityEngine.Random.Range(mScreenRightMarginX, mScreenRightMarginX - mThirdPercentWidth);
			int chosenPosition = ChoseRandomVerticalPosition (availableVert, screenTopMarginY, out ypos);	
			choseFromPositionsVert[chosenPosition] = 0;	// unset the position

        } else
        {
			print ("wrong direction");
            throw new InvalidOperationException();
        }

		// ramane de vazut ce coordonate au ramas pentru a fi fixate random

		if (choseAxis == Axis.Vertical)
        {
			int chosenPosition = ChoseRandomVerticalPosition (availableVert, screenTopMarginY, out ypos);	// chose a vertical position and set in ypos
			choseFromPositionsVert[chosenPosition] = 0;	// unset the position
        }
		else if (choseAxis == Axis.Horizontal)
        {
			int chosenPosition = ChoseRandomHorizontalPosition (availableHorz, out xpos);	// chose a horizontal position and set in ypos
			choseFromPositionsHorz[chosenPosition] = 0;	// unset the position
        }

        return new Vector2(xpos, ypos);
    }

    public void SpawnMovingObstacle(Vector3 atCameraOrigin,int numberOfObjects)
    {
        if (numberOfObjects > 3)
        {
            numberOfObjects = 3;
        }

        int chosenDirection = UnityEngine.Random.Range(0, mDirections.Length - 1);
		Vector2 direction = mDirections[chosenDirection];

        int[] verticalPosAvailable = { 1, 1, 1 };
        int[] horizontalPosAvailable = { 1, 1, 1 };

		// daca directia aleasa e pe diagonala, ne asiguram ca obstacolul nu va iesi din ecranul sau

        if (mDirections[chosenDirection] == new Vector2(1,1))
        {
            verticalPosAvailable[0] = 0;
		} else if (direction == new Vector2(1, -1))
        {
            verticalPosAvailable[2] = 0;
		} else if(direction == new Vector2(-1, -1))
        {
            verticalPosAvailable[2] = 0;
		} else if (direction == new Vector2(-1, 1))
        {
            verticalPosAvailable[0] = 0;
        }

		bool weReverse;


		// se spawneaza obiectele 
        for (int i=0;i<numberOfObjects;i++)
        {
			if (UnityEngine.Random.Range (0.0f, 1.0f) > 0.5f) {
				weReverse = true;
				print (UnityEngine.Random.Range (0f, 1f));
			} else {
				weReverse = false;
				print (UnityEngine.Random.Range (0f, 1f));

			}

			if (weReverse == true) {
				direction *= -1;
			}
            float speedRand = UnityEngine.Random.Range(1, 2);
			Vector2 spawnOrigin = WhereToSpawnObstacle(direction, atCameraOrigin,ref horizontalPosAvailable,ref verticalPosAvailable);   // !!! Direction must not be normalized
			Vector2 targetPoint = scriptObiect.ComputeTargetPoint(spawnOrigin, direction.normalized, 2);

            GameObject newObstacle = Instantiate(mCloneObstacle, spawnOrigin, mCloneObstacle.transform.rotation);
			newObstacle.GetComponent<scriptObstacol>().SetAndComputeProperties(direction, newObstacle.transform.position, speedRand, 2);
            newObstacle.gameObject.tag = "Obstacol";

			if (weReverse == true) {
				direction *= -1;
			}
		}
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
		mHighestScoreText.text = "Highscore: " + mHighestScore.getScore();
		mLastScoreText.text = "" + mScore;

		// new
		if (mScore > mHighestScore.getScore ()) {
			mLastScoreText.fontSize = 70;
			mLastScoreText.color = Color.red;
		}
		else
			mLastScoreText.fontSize = 60;

		PauseGame();
        mGameOverPanel.SetActive(true);
        mGameIsOver = true;

        if (mHighestScore.getScore() < mScore)
        {
            mHighestScore = new ScoreModel(mScore, DateTime.Now.ToString("MM/dd/yyyy"));

            // Insert to database

            mScoreHistoryDB = new XmlDocument();
            mScoreHistoryDB.Load("Assets/Scripts/XML/ScoreHistory.xml");    // Get the xml file content

            XmlElement xmlbase = mScoreHistoryDB.DocumentElement;
            XmlElement scoreElement = mScoreHistoryDB.CreateElement("score");
            XmlElement scoreValue = mScoreHistoryDB.CreateElement("value");
            scoreValue.InnerText = mHighestScore.getScore().ToString();
            XmlElement scoreDate = mScoreHistoryDB.CreateElement("date");
            scoreDate.InnerText = mHighestScore.getDate();
            scoreElement.AppendChild(scoreValue);
            scoreElement.AppendChild(scoreDate);
            xmlbase.AppendChild(scoreElement);

            mScoreHistoryDB.Save("Assets/Scripts/XML/ScoreHistory.xml");
        }
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
