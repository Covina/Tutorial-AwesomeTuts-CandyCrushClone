using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class CandyArray
{

    // Crate the two-dimensional array
    GameObject[,] candies = new GameObject[GameVariables.Rows, GameVariables.Columns];

    // Copies of the two beans for swapping
    private GameObject backup1;
    private GameObject backup2;

    // INDEXER - Get/Set the item at this row and column
    public GameObject this[int row, int column]
    {
        get
        {
            try
            {
                return candies[row, column];
            }
            catch (Exception e)
            {
                throw;
            }
        }

        set
        {
            candies[row, column] = value;
        }

    }

    /// <summary>
    /// Swap function to change array information and positions of two candies
    /// </summary>
    /// <param name="g1"></param>
    /// <param name="g2"></param>
    public void Swap(GameObject g1, GameObject g2)
    {
        // Store backups
        backup1 = g1;
        backup1 = g2;

        var g1Candy = g1.GetComponent<Candy>();

        var g2Candy = g2.GetComponent<Candy>();

        // store row/column data
        int g1Row = g1Candy.Row;
        int g1Column = g1Candy.Column;

        // store row/column data
        int g2Row = g2Candy.Row;
        int g2Column = g2Candy.Column;



        // Get the game object stored in position
        var temp = candies[g1Row, g1Column];

        // Swap 2 into position 1
        candies[g1Row, g1Column] = candies[g2Row, g2Column];

        // Swap 1 into position 2
        candies[g2Row, g2Column] = temp;

        // Now make the game object swap
        Candy.SwapRowColumn(g1Candy, g2Candy);


    }


    /// <summary>
    /// Undo the swap if necessary
    /// </summary>
    public void UndoSwap()
    {
        // use the same swap function above
        Swap(backup1, backup2);

    }


    /// <summary>
    /// Checks for matches along a row
    /// </summary>
    /// <param name="obj"></param>
    /// <returns></returns>
    private IEnumerable<GameObject> GetMatchesHorizontally(GameObject obj)
    {
        // Create list for candy objects
        List<GameObject> matches = new List<GameObject>();

        // add the passed in objects
        matches.Add(obj);

        // get the Candy class object
        var candy = obj.GetComponent<Candy>();

        if(candy.Column != 0)
        {
            // Scan to the left to find all sequential matches
            for(int column = candy.Column - 1; column >= 0; column--)
            {
                // Check object at that position is same type of candy as this "candy"
                if (candies[candy.Row, column].GetComponent<Candy>().IsSameType(candy))
                {
                    // add it to the match list
                    matches.Add(candies[candy.Row, column]);

                } else
                {
                    // no match, so stop looping
                    break;
                }

            }

        } // search left side


        // search on the right side
        if (candy.Column != GameVariables.Columns - 1)
        {
            // scan to the right to find all sequential matches
            for (int column = candy.Column + 1; column < GameVariables.Columns; column++)
            {
                // Check object at that position is same type of candy as this "candy"
                if (candies[candy.Row, column].GetComponent<Candy>().IsSameType(candy))
                {
                    // add it to the match list
                    matches.Add(candies[candy.Row, column]);

                }
                else
                {
                    // no match, so stop looping
                    break;
                }


            }  // end for loop
        }

        // check for minimum matches required
        if(matches.Count < GameVariables.MinimumMatches)
        {
            // remove all entries if no valid matches
            matches.Clear();
        }

        // return de-duped list
        return matches.Distinct();

    } // GetMatchesHorizontally()


    /// <summary>
    /// Check matches along the rows
    /// </summary>
    /// <param name="obj"></param>
    /// <returns></returns>
    private IEnumerable<GameObject> GetMatchesVertically(GameObject obj)
    {
        // Create a list of matches
        List<GameObject> matches = new List<GameObject>();

        // add the starting object to the list
        matches.Add(obj);

        // get Candy Component
        var candy = obj.GetComponent<Candy>();

        // start checking down rows.
        if (candy.Row != 0)
        {
            // Scan below to find all sequential matches
            for (int row = candy.Row - 1; row >= 0; row--)
            {
                // Check object at that position is same type of candy as this "candy"
                if (candies[row, candy.Column].GetComponent<Candy>().IsSameType(candy))
                {
                    // add it to the match list
                    matches.Add(candies[row, candy.Column]);

                }
                else
                {
                    // first wrong match, so stop looping
                    break;
                }

            }

        } // search down rows

        // start checking UP rows.
        if (candy.Row != GameVariables.Rows - 1)
        {
            // Scan up the rows until we hit max rows
            for (int row = candy.Row + 1; row < GameVariables.Rows; row++)
            {
                // Check object at that position is same type of candy as this "candy"
                if (candies[row, candy.Column].GetComponent<Candy>().IsSameType(candy))
                {
                    // add it to the match list
                    matches.Add(candies[row, candy.Column]);

                }
                else
                {
                    // first wrong match, so stop looping
                    break;
                }

            }

        } // search down rows

        // if not enough matches, clear it out
        if(matches.Count < GameVariables.MinimumMatches)
        {
            matches.Clear();
        }

        // return de-duped (or empty) matches.
        return matches.Distinct();
       

    } // GetMatchesVertically()


    private bool ContainsDestroyWholeRowColumnBonus(IEnumerable<GameObject> matches)
    {
        // did enough matches exist
        if(matches.Count() >= GameVariables.MinimumMatches)
        {
            // Does the list of matches contain a bonus candy that destroys the row and column?
            foreach(var item in matches)
            {
                if(BonusTypeChecker.ContainsDestroyWholeWorColumn(item.GetComponent<Candy>().Bonus))
                {
                    return true;
                }

            }

        }

        return false;
    }

    /// <summary>
    /// Grab all objects for a specific row based on the passed candy location
    /// </summary>
    /// <param name="obj"></param>
    /// <returns></returns>
    private IEnumerable<GameObject> GetEntireRow(GameObject obj)
    {
        // create list to store all beans in a row
        List<GameObject> matches = new List<GameObject>();

        // get the row location
        int row = obj.GetComponent<Candy>().Row;

        // loop through the row from left to right
        for(int column = 0; column < GameVariables.Columns; column++)
        {
            // add the candy object
            matches.Add(candies[row, column]);
        }

        return matches;

    }

    /// <summary>
    /// Get the entire column of beans based on location of passed object
    /// </summary>
    /// <param name="obj"></param>
    /// <returns></returns>
    private IEnumerable<GameObject> GetEntireColumn(GameObject obj)
    {
        // create list to store all beans in a row
        List<GameObject> matches = new List<GameObject>();

        // get the row location
        int column = obj.GetComponent<Candy>().Column;

        // loop through the row from left to right
        for (int row = 0; row < GameVariables.Columns; row++)
        {
            // add the candy object
            matches.Add(candies[row, column]);
        }

        return matches;

    }


    /// <summary>
    /// Remove the item from the array
    /// </summary>
    /// <param name="item"></param>
    public void Remove(GameObject item)
    {
        // Remove the passed bean from the candies array
        candies[item.GetComponent<Candy>().Row, item.GetComponent<Candy>().Column] = null;

    }

    /// <summary>
    /// Find all the candies that are collapsing downward
    /// </summary>
    /// <param name="columns"></param>
    /// <returns></returns>
    public AlteredCandyInfo Collapse(IEnumerable<int> columns)
    {

        AlteredCandyInfo collapseInfo = new AlteredCandyInfo();

        // loop through all columns
        foreach(var column in columns)
        {
            // Loop through each row
            for(int row = 0; row < GameVariables.Rows - 1; row++)
            {

                // Search for a destroyed candy location
                if(candies[row, column] == null)
                {

                    // Now navigate the column upward to find the first non-null candy
                    for(int row2 = row + 1; row2 < GameVariables.Rows; row2++)
                    {

                        // Did we find a non-null candy spot?
                        if(candies[row2, column] != null)
                        {

                            // We found a candy, so set the object at that destroyed position to the first non null object
                            candies[row, column] = candies[row2, column];

                            // nullify the found candy position
                            candies[row2, column] = null;

                            // Update the max distance between the destroyed bean and its first non-null bean above it
                            if(row2 - row > collapseInfo.maxDistance)
                            {
                                collapseInfo.maxDistance = row2 - row;
                            }

                            // Update the candy array positioning information
                            candies[row, column].GetComponent<Candy>().Row = row;
                            candies[row, column].GetComponent<Candy>().Column = column;

                            // Add the candy to the collapsing info
                            collapseInfo.AddCandy(candies[row, column]);

                            // break loop
                            break;

                        }

                    }

                }

            }

        } 

        return collapseInfo;

    } // Collapse()


    /// <summary>
    /// Generate a List of all the empty game object position within a column
    /// </summary>
    /// <param name="column"></param>
    /// <returns></returns>
    public IEnumerable<CandyInfo> GetEmptyItemsOnColumn(int column)
    {
        // Create empty items list to return
        List<CandyInfo> emptyItems = new List<CandyInfo>();

        // Loop through the rows for the passed in column
        for(int row = 0; row < GameVariables.Rows; row++)
        {
            // if we found an empty game object position in this column
            if(candies[row, column] == null)
            {
                // add it to the emptyItems list, leveraging the setter
                emptyItems.Add(new CandyInfo() { Row = row, Column = column });

            }

        }

        return emptyItems;

    } // GetEmptyItemsOnColumn()



    /// <summary>
    /// Get all the matches adjacent to the candy
    /// </summary>
    /// <param name="obj"></param>
    /// <returns></returns>
    public MatchesInfo GetMatches(GameObject obj)
    {

        MatchesInfo matchesInfo = new MatchesInfo();

        // Get any horizontal matches adjacent to the passed object
        var horizontalMatches = GetMatchesHorizontally(obj);

        // Does the match contain a bonus bean that destroys the entire row + column?
        if(ContainsDestroyWholeRowColumnBonus(horizontalMatches))
        {
            // if yes, then get all the objects in the row.
            horizontalMatches = GetEntireRow(obj);

            // if the matches info doesn't have the info about the bonus, make sure it does.
            if(!BonusTypeChecker.ContainsDestroyWholeWorColumn(matchesInfo.BonusesContained))
            {
                matchesInfo.BonusesContained = BonusType.DestroyWholeRowColumn;
            }
        }

        // Add all the row matches to the list
        matchesInfo.AddObjectRange(horizontalMatches);

        // ==== Now do the same for vertical column ====


        // Get any horizontal matches adjacent to the passed object
        var verticalMatches = GetMatchesVertically(obj);

        // Does the match contain a bonus bean that destroys the entire row + column?
        if (ContainsDestroyWholeRowColumnBonus(verticalMatches))
        {
            // if yes, then get all the objects in the column.
            verticalMatches = GetEntireColumn(obj);

            // if the matches info doesn't have the info about the bonus, make sure it does.
            if (!BonusTypeChecker.ContainsDestroyWholeWorColumn(matchesInfo.BonusesContained))
            {
                matchesInfo.BonusesContained = BonusType.DestroyWholeRowColumn;
            }
        }

        // Add all the vertical matches to the list
        matchesInfo.AddObjectRange(verticalMatches);

        // return the list of all matched candies
        return matchesInfo;

    }

    /// <summary>
    /// Look for newly matched candies based on candy repopulations
    /// </summary>
    /// <param name="objs"></param>
    /// <returns></returns>
    public IEnumerable<GameObject> GetRefillMatches( IEnumerable<GameObject> objs)
    {
        // create a list for new matches
        List<GameObject> matches = new List<GameObject>();

        // Loop through the passed list
        foreach(var gameObj in objs)
        {
            matches.AddRange(GetMatches(gameObj).MatchedCandy);
        }


        return matches.Distinct();
        

    }


    

}
