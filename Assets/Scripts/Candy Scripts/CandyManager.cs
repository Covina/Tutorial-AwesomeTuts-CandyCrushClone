using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

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

        StartCheckForPotentialMatches();

    }

    // Update is called once per frame
    void Update() {

        //
        if(state == GameState.None)
        {
            // Touch input
            if(Input.GetMouseButtonDown(0))
            {

                // Figure out what they touched
                var hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);

                if (hit.collider != null)
                {

                    hitGo = hit.collider.gameObject;

                    // update game state to be aware the player is selecting
                    state = GameState.SelectionStarted;
                }

            }

        } else if (state == GameState.SelectionStarted)
        {
            // player has tapped twice
            if (Input.GetMouseButtonDown(0))
            {

                // Figure out what they touched
                var hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);

                // The player touched something and it was not the same item
                if (hit.collider != null && hitGo != hit.collider.gameObject)
                {

                    StopCheckForPotentialMatches();

                    // Compare if the two objects are not neighbors
                    if (!MatchChecker.AreHorizontalOrVericalNeighbors(hitGo.GetComponent<Candy>(), hit.collider.gameObject.GetComponent<Candy>()))
                    {
                        state = GameState.None;
                    } else
                    {
                        // next to each other
                        state = GameState.Animating;

                        // assign sorting layer
                        FixSortingLayer(hitGo, hit.collider.gameObject);

                        // Pass the raycast hit
                        StartCoroutine(FindMatchesAndCollapse(hit));
                    }


                }

            }

        }




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



    /// <summary>
    /// For touching beans, make suer the touched is always on top.
    /// </summary>
    /// <param name="hitGo"></param>
    /// <param name="hitGo2"></param>
    private void FixSortingLayer(GameObject hitGo, GameObject hitGo2)
    {
        // get sprite renderer components
        SpriteRenderer sp1 = hitGo.GetComponent<SpriteRenderer>();
        SpriteRenderer sp2 = hitGo2.GetComponent<SpriteRenderer>();

        // if there is a difference already
        if(sp1.sortingOrder <= sp2.sortingOrder)
        {
            // assign the new sorting order
            sp1.sortingOrder = 1;
            sp2.sortingOrder = 0;
        }


    }


    /// <summary>
    /// Blow up the candy then destroy the explosion animation and the item itself
    /// </summary>
    /// <param name="item"></param>
    private void RemoveFromScene(GameObject item)
    {
        // create the explosion
        var explosion = Instantiate(GetRandomExplosion(), item.transform.position, Quaternion.identity) as GameObject;

        // delay the destruction
        Destroy(explosion, GameVariables.ExplosionAnimationDuration);

        Destroy(item);

    }

    /// <summary>
    /// Get a bonus type
    /// </summary>
    /// <param name="type"></param>
    /// <returns></returns>
    private GameObject GetBonusFromType(string type)
    {
        // Get color by parsing name "bean_green"
        string color = type.Split('_')[1].Trim();

        // Loop through bonus prefabs to see if it has a matching one.
        foreach (var item in bonusPrefabs)
        {
            // check for the matching color bonus
            if(item.GetComponent<Candy>().Type.Contains(color))
            {
                // return the bonus swirl item
                return item;
            }

        }

        throw new System.Exception("You passed the wrong type");
    }


    /// <summary>
    /// Replace a candy with the bonus version of it.
    /// </summary>
    /// <param name="hitGoCache"></param>
    private void CreateBonus(Candy hitGoCache)
    {
        // Create game object
        GameObject Bonus = Instantiate(
                            GetBonusFromType(hitGoCache.Type), 
                            bottomRight + new Vector2(hitGoCache.Column * candySize.x, hitGoCache.Row * candySize.y), 
                            Quaternion.identity
                            ) as GameObject;


        // replace candy with the bonus version we just made
        candies[hitGoCache.Row, hitGoCache.Column] = Bonus;

        // Get ref to candy component
        var bonusCandy = Bonus.GetComponent<Candy>();

        // Initialize the bonus candy
        bonusCandy.Initialize(hitGoCache.Type, hitGoCache.Row, hitGoCache.Column);

        // Specify the bonus type - the behavior when used in the game
        bonusCandy.Bonus = BonusType.DestroyWholeRowColumn;

    }


    /// <summary>
    /// Create new candies in empty spaces within each column
    /// </summary>
    /// <param name="columnsWithMissingCandies"></param>
    /// <returns></returns>
    private AlteredCandyInfo CreateNewCandyInSpecificColumns(IEnumerable<int> columnsWithMissingCandies)
    {

        AlteredCandyInfo newCandyInfo = new AlteredCandyInfo();
        
        // loop through each column to find the empty spaces
        foreach(int column in columnsWithMissingCandies)
        {
            var emptyItems = candies.GetEmptyItemsOnColumn(column);

            // for each missing item, generate a new candy
            foreach(var item in emptyItems)
            {
                // generate a new candy object
                var go = GetRandomCandy();

                // instantiate this candy 
                GameObject newCandy = Instantiate(go, spawnPositions[column], Quaternion.identity) as GameObject;

                // initialize this candy and put it in its position
                newCandy.GetComponent<Candy>().Initialize(go.GetComponent<Candy>().Type, item.Row, item.Column);

                // 
                if(GameVariables.Rows - item.Row > newCandyInfo.maxDistance)
                {
                    // Update the max distance
                    newCandyInfo.maxDistance = GameVariables.Rows - item.Row;

                }

                // add the new candy into the array
                candies[item.Row, item.Column] = newCandy;

                // add the candy to the new candy info
                newCandyInfo.AddCandy(newCandy);

            }

        }




        return newCandyInfo;
    }


    //  
    private void MoveAndAnimate(IEnumerable<GameObject> movedGameObjects, int distance)
    {

        // loop through and animate each object
        foreach(var item in movedGameObjects)
        {
            // use the GoKit plugin to easily animate 
            item.transform.positionTo(GameVariables.MoveAnimationDuration * distance, bottomRight + new Vector2(item.GetComponent<Candy>().Column * candySize.x, item.GetComponent<Candy>().Row * candySize.y));

        }


    }


    private IEnumerator FindMatchesAndCollapse(RaycastHit2D hit2)
    {

        // assign 2nd game object to the one that was touched.
        var hitGo2 = hit2.collider.gameObject;

        // Swap the candy positions
        candies.Swap(hitGo, hitGo2);

        // Move passed item into position 2
        hitGo.transform.positionTo(GameVariables.AnimationDuration, hit2.transform.position);

        // Move item 2 into position 
        hitGo2.transform.positionTo(GameVariables.AnimationDuration, hitGo.transform.position);

        // wait until its over.
        yield return new WaitForSeconds(GameVariables.AnimationDuration);


        var hitGoMatchesInfo = candies.GetMatches(hitGo);

        var hitGo2MatchesInfo = candies.GetMatches(hitGo2);

        // join the first and second lists, de-duplicating, to result in the total matches
        var totalMatches = hitGoMatchesInfo.MatchedCandy.Union(hitGo2MatchesInfo.MatchedCandy).Distinct();


        // if there is not a valid match, such as only being 2 in a row, wrong colors, etc... we need to undo the animated swap.
        if(totalMatches.Count() < GameVariables.MinimumMatches)
        {
            // Move passed item back to position 1
            hitGo.transform.positionTo(GameVariables.AnimationDuration, hitGo2.transform.position);

            // Move item 2 back to its position 
            hitGo2.transform.localPositionTo(GameVariables.AnimationDuration, hitGo.transform.position);

            yield return new WaitForSeconds(GameVariables.AnimationDuration);

            candies.UndoSwap();
        }


        // figure out if we're going to grant a bonus
        bool addBonus = totalMatches.Count() >= GameVariables.MinimumMatchesForBonus
            && !BonusTypeChecker.ContainsDestroyWholeWorColumn(hitGoMatchesInfo.BonusesContained)       // if the items being destroyed doesn't already contain a bonus
            && !BonusTypeChecker.ContainsDestroyWholeWorColumn(hitGo2MatchesInfo.BonusesContained);     // if the items being destroyed doesn't already contain a bonus


        Candy hitGoCache = null;

        if(addBonus == true)
        {
            hitGoCache = new Candy();

            // assign one of the two game objects depending if its the same type
            var sameTypeGo = hitGoMatchesInfo.MatchedCandy.Count() > 0 ? hitGo : hitGo2;

            // get a reference to the Candy component on the SameTypeGo gameobject
            var candy = sameTypeGo.GetComponent<Candy>();

            // initialize the bonus candy
            hitGoCache.Initialize(candy.Type, candy.Row, candy.Column);
        }

        // track the while loop
        int timesRun = 1;

        while(totalMatches.Count() >= GameVariables.MinimumMatches)
        {
            // earn points for each 3-item match
            IncreaseScore(totalMatches.Count() - 2 * GameVariables.Match3Score);

            // bonus if multiples
            if(timesRun >= 2)
            {
                IncreaseScore(GameVariables.SubsequelMatchScore);
            }

            // trigger sound effect
            soundManager.PlaySound();

            // Now remove the individual items
            foreach(var item in totalMatches)
            {
                candies.Remove(item);
                RemoveFromScene(item);
            }

            // If there is a bonus candy to be generated, create it.
            if(addBonus)
            {
                CreateBonus(hitGoCache);
            }

            addBonus = false;


            var columns = totalMatches.Select(go => go.GetComponent<Candy>().Column).Distinct();

            var collapsedCandyInfo = candies.Collapse(columns);

            var newCandyInfo = CreateNewCandyInSpecificColumns(columns);

            int maxDistance = Mathf.Max(collapsedCandyInfo.maxDistance, newCandyInfo.maxDistance);

            MoveAndAnimate(newCandyInfo.AlteredCandy, maxDistance);
            MoveAndAnimate(collapsedCandyInfo.AlteredCandy, maxDistance);

            // wait for time for each movement animation that has to occur
            yield return new WaitForSeconds(GameVariables.MoveAnimationDuration * maxDistance);

            // Checking for new matches in the collapsed columns
            totalMatches = candies.GetRefillMatches(collapsedCandyInfo.AlteredCandy).Union(candies.GetRefillMatches(newCandyInfo.AlteredCandy)).Distinct();

            timesRun++;

        } // end while

        // reset game state
        state = GameState.None;

        // start checking for potential matches again.
        StartCheckForPotentialMatches();

    }


}
