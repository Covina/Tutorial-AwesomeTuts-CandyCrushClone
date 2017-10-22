using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CandyManager : MonoBehaviour {

    // The candy array
    public CandyArray candies;

    // The score text
    public Text scoretext;

    // Candies
    public GameObject[] candyPrefabs;

    // Bonus Candies
    public GameObject[] bonusPrefabs;

    // Explosion Candies
    public GameObject[] explosionPrefabs;


    // Player's score
    private int score;

    private Vector2 bottomRight = new Vector2(-2.37f, -4.27f);

    // candy size to use for positioning and laying out the grid
    private Vector2 candySize = new Vector2(0.7f, 0.7f);

    // Current game state
    private GameState state = GameState.None;

    private GameObject hitGo = null;

    private Vector2[] spawnPositions;






    private IEnumerator CheckPotentialMatchesCoroutine;

    private IEnumerator AnimatePotentialMatchesCoroutine;


    //
    IEnumerable<GameObject> potentialMatches;

    [SerializeField]
    private SoundManager soundManager;

    // Use this for initialization
    void Start() {

        // figure out what candies are available
        InitializeTypesOnPrefabShapesAndBonuses();

        // Populate the level
        InitializeCandyAndSpawnPositions();

    }

    // Update is called once per frame
    void Update() {

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

        scoretext.text = "Score: " + score.ToString();

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

        for (int row = 0; row < GameVariables.Rows; row++)
        {

            for (int column = 0; column <= GameVariables.Columns; column++)
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

        // For the Candy and Bonus Prefabs, we need to init
        foreach (var item in candyPrefabs)
        {

            // set the item type value to be its name
            item.GetComponent<Candy>().Type = item.name;

        }


        for (int i = 0; i < bonusPrefabs.Length; i++)
        {
            // Set the Bonus Type values to be the object name
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
        go.GetComponent<Candy>().Initialize(newCandy.GetComponent<Candy>().Type, row, column);

        // Store the instantiated game object
        candies[row, column] = go;


    }



    /// <summary>
    /// Set up spawn positions for the candy
    /// </summary>
    private void SetupSpawnPositions()
    {
        // loop through each column
        for (int column = 0; column < GameVariables.Columns; column++)
        {

            spawnPositions[column] = bottomRight + new Vector2(column * candySize.x, GameVariables.Rows * candySize.y);

        }

    }

    /// <summary>
    /// Returns a random candy
    /// </summary>
    /// <returns></returns>
    private GameObject GetRandomCandy()
    {
        return candyPrefabs[Random.Range(0, candyPrefabs.Length)];
    }

    /// <summary>
    /// Returns a random explosion candy
    /// </summary>
    /// <returns></returns>
    private GameObject GetRandomExplosion()
    {
        return explosionPrefabs[Random.Range(0, explosionPrefabs.Length)];
    }

    /// <summary>
    /// Generate candies for level
    /// </summary>
    public void InitializeCandyAndSpawnPositions()
    {
        InitializeVariables();

        // If any candies are left, destroy them
        if(candies != null)
        {
            DestroyAllCandy();
        }

        // create new candy array
        candies = new CandyArray();

        // Define the number of columns in this array of vector 2 positions
        spawnPositions = new Vector2[GameVariables.Columns];

        // loop through row and column count and generate the candies
        for(int row = 0; row < GameVariables.Rows; row++)
        {

            for(int column = 0; column < GameVariables.Columns; column++)
            {

                GameObject newCandy = GetRandomCandy();

                // If we find two adjacent candies horizontally, loop through until we force it to a non-match candy
                while(
                    column >= 2 
                    && 
                    candies[row, column - 1].GetComponent<Candy>().IsSameType( newCandy.GetComponent<Candy>() ) 
                    &&
                    candies[row, column - 2].GetComponent<Candy>().IsSameType( newCandy.GetComponent<Candy>() )
                    ) {

                    newCandy = GetRandomCandy();

                }

                // If two adjacent candies in a vertical line, loop through until we get a non-match candy
                while (
                    row >= 2
                    &&
                    candies[row - 1, column].GetComponent<Candy>().IsSameType(newCandy.GetComponent<Candy>())
                    &&
                    candies[row - 2, column].GetComponent<Candy>().IsSameType(newCandy.GetComponent<Candy>())
                    )
                {
                    // generate the candy
                    newCandy = GetRandomCandy();

                }

                // Place the new candy
                InstantiateAndPlaceNewCandy(row, column, newCandy);

            }

        }


        SetupSpawnPositions();

    }


    /// <summary>
    /// Start the hint detection to help players
    /// </summary>
    private void StartCheckForPotentialMatches()
    {
        // first stop the matches
        StopCheckForPotentialMatches();

        // Set the coroutine reference
        CheckPotentialMatchesCoroutine = CheckPotentialMatches();

        // Start the coroutine
        StartCoroutine(CheckPotentialMatchesCoroutine);


    }

    /// <summary>
    /// Stop the matches hint system
    /// </summary>
    private void StopCheckForPotentialMatches()
    {
        
        if(AnimatePotentialMatchesCoroutine != null)
        {
            // Stop the coroutine.
            StopCoroutine(AnimatePotentialMatchesCoroutine);
        }


        if(CheckPotentialMatchesCoroutine != null)
        {
            // stop the coroutine
            StopCoroutine(CheckPotentialMatchesCoroutine);
        }

        // Restore opacity for all candies in case it stops mid animation
        ResetOpacityOnPotentialMatches();



    }

    /// <summary>
    /// Put opacity back to normal
    /// </summary>
    private void ResetOpacityOnPotentialMatches()
    {
        if(potentialMatches != null)
        {
            foreach(var item in potentialMatches)
            {
                if(item == null)
                {
                    break;
                } else
                {
                    // store current color
                    Color c = item.GetComponent<SpriteRenderer>().color;

                    // set alpha to 1
                    c.a = 1f;

                    // update color
                    item.GetComponent<SpriteRenderer>().color = c;
                }

            }

        }


    }


    private IEnumerator CheckPotentialMatches()
    {
        // Wait delay before showing potential matches
        yield return new WaitForSeconds(GameVariables.WaitBeforePotentialMatchesCheck);

        // get any potential matches
        potentialMatches = MatchChecker.GetPotentialMatches(candies);

        // if we have matches
        if(potentialMatches != null)
        {
            // animate them until the player takes their turn
            while(true)
            {

                AnimatePotentialMatchesCoroutine = MatchChecker.AnimatePotentialMatches(potentialMatches);

                StartCoroutine(AnimatePotentialMatchesCoroutine);

                // Wait delay before showing potential matches
                yield return new WaitForSeconds(GameVariables.WaitBeforePotentialMatchesCheck);
            }

        }

    }







}
