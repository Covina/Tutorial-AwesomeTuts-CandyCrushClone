using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CandyManager : MonoBehaviour {

    // The candy array
    public CandyArray candies;

    // The score text
    public Text scoretext;

    // Player's score
    private int score;

    private Vector2 bottomRight = new Vector2(-2.37f, -4.27f);

    private Vector2 candySize = new Vector2(0.7f, 0.7f);

    private GameState state = GameState.None;

    private GameObject hitGo = null;

    private Vector2[] spawnPositions;


    //
    private GameObject[] candyPrefabs;

    private GameObject[] bonusPrefabs;

    private GameObject[] explosionPrefabs;




    private IEnumerator CheckPotentialMatchesCoroutine;

    private IEnumerator AnimatePotentialMatchesCoroutine;


    //
    IEnumerable<GameObject> potentialMatches;

    private SoundManager soundManager;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    /// <summary>
    /// Set starting values for variables
    /// </summary>
    private void InitializeVariables()
    {
        score = 0;
        ShowScore();
    }

    /// <summary>
    /// Update the score to display the current value
    /// </summary>
    private void ShowScore()
    {

        scoretext.text = "Score " + score.ToString();

    }

    /// <summary>
    /// Increase the score by the passed amount and then display it
    /// </summary>
    /// <param name="amount"></param>
    private void IncreaseScore(int amount)
    {
        // add to score
        score += amount;

        // update the visual display of the score
        ShowScore();

    }

    /// <summary>
    /// Destroy all the candies
    /// </summary>
    private void DestroyAllCandy()
    {

        for(int row = 0; row < GameVariables.Rows; row++)
        {

            for(int column = 0; column <= GameVariables.Columns; column++)
            {
                // Destroy the game object
                Destroy(candies[row, column]);

            }

        }


    }




    /// <summary>
    /// 
    /// </summary>
    private void InitializeTypesOnPrefabShapesAndBonuses()
    {

        // Fot the Candy and Bonus Prefabs, we need to init
        foreach(var item in candyPrefabs)
        {

            // set the item type value to be its name
            item.GetComponent<Candy>().Type = item.name;

        }


        for(int i = 0; i < bonusPrefabs.Length; i++)
        {

            bonusPrefabs[i].GetComponent<Candy>().Type = candyPrefabs[i].name;

        }


    }


    /// <summary>
    /// Create a new candy
    /// </summary>
    /// <param name="row"></param>
    /// <param name="column"></param>
    /// <param name="newCandy"></param>
    private void InstantiateAndPlaceNewCandy(int row, int column, GameObject newCandy)
    {
        // instantiate the candy game object
        GameObject go = Instantiate(newCandy, bottomRight + new Vector2(column * candySize.x, row * candySize.y), Quaternion.identity) as GameObject;

        // Initialize everything about this candy
        go.GetComponent<Candy>().Initialize( newCandy.GetComponent<Candy>().Type, row, column );

        // Store the instantiated game object
        candies[row, column] = go;


    }



    //
    private void SetupSpawnPositions()
    {

    }

}
