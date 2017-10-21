using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MatchChecker {

    /// <summary>
    /// Fade the opacity in/out to show a potential match
    /// </summary>
    /// <param name="potentialMatches"></param>
    /// <returns></returns>
    public static IEnumerator AnimatePotentialMatches(IEnumerable<GameObject> potentialMatches)
    {

        // loop through to reduce alpha from 1.0 to 0.3 as part of the animation
        for (float i = 1.0f; i >= 0.3f; i -= 0.1f) {

            // for each of the beans
            foreach(var item in potentialMatches) {

                // get the color
                Color c = item.GetComponent<SpriteRenderer>().color;

                // update the alpha value
                c.a = i;

                // set new alpha value
                item.GetComponent<SpriteRenderer>().color = c;

            }

            // Length to wait for the opacity animation
            yield return new WaitForSeconds(GameVariables.OpactiyAnimationDelay);

        }


        // loop through to increase alpha from 0.3 to 1.0 as part of the animation
        for (float i = 0.3f; i <= 1.0f; i += 0.1f)
        {

            // for each of the beans
            foreach (var item in potentialMatches)
            {

                // get the color
                Color c = item.GetComponent<SpriteRenderer>().color;

                // update the alpha value
                c.a = i;

                // set new alpha value
                item.GetComponent<SpriteRenderer>().color = c;

            }

            // Length to wait for the opacity animation
            yield return new WaitForSeconds(GameVariables.OpactiyAnimationDelay);

        }

    } // AnimatePotentialMatches()


    /// <summary>
    /// Checks if the two candies are next to each other.
    /// </summary>
    /// <param name="c1"></param>
    /// <param name="c2"></param>
    /// <returns></returns>
    public static bool AreHorizontalOrVericalNeighbors(Candy c1, Candy c2)
    {

        // Check if the two candies are 1) In the same row or column and 2) Are adjacent to each other
        return (c1.Column == c2.Column || c1.Row == c2.Row) && Mathf.Abs(c1.Column - c2.Column) <= 1 && Mathf.Abs(c1.Row - c2.Row) <= 1;

    }



    public static IEnumerable<GameObject> GetPotentialMatches(CandyArray candies)
    {

        // create a list of Lists
        List< List<GameObject>> matches = new List<List<GameObject>>();

        // loop through each candy and check for matches
        for(int row = 0; row < GameVariables.Rows; row++)
        {

            for(int column = 0; column < GameVariables.Columns; column++)
            {
                // check horizontal matches
                var matches1 = CheckHorizontal1(row, column, candies);
                var matches2 = CheckHorizontal2(row, column, candies);
                var matches3 = CheckHorizontal3(row, column, candies);

                // Check vertical matches
                var matches4 = CheckVertical1(row, column, candies);
                var matches5 = CheckVertical2(row, column, candies);
                var matches6 = CheckVertical3(row, column, candies);

                // Now verify if they had anything, and if so, add it to the total matches list
                if (matches1 != null) matches.Add(matches1);
                if (matches2 != null) matches.Add(matches2);
                if (matches3 != null) matches.Add(matches3);
                if (matches4 != null) matches.Add(matches4);
                if (matches5 != null) matches.Add(matches5);
                if (matches6 != null) matches.Add(matches6);

                // if we have more than three matches, return a random one
                if(matches.Count >= 3)
                {
                    return matches[Random.Range(0, matches.Count - 1)];
                }

                // if we're in middle of the loop and we have one or two, return it.
                if(row > GameVariables.Rows / 2 && matches.Count > 0 && matches.Count <= 2)
                {
                    return matches[Random.Range(0, matches.Count - 1)];
                }

            }


        }


        return null;

    }

    // checking to the left of the specific candy
    public static List<GameObject> CheckHorizontal1(int row, int column, CandyArray candies)
    {

        /*  EXAMPLE *\
         
         * * * * *
         * * * * *
         * * * * *
         * * @ & *
         * & * * * 
         
         \*         */

        // Prep to look at the column one place to the left
        if(column <= GameVariables.Columns - 2)
        {

            // check if the candy to the right is the same Type
            if( candies[row, column].GetComponent<Candy>().IsSameType( candies[row, column + 1].GetComponent<Candy>() ) )
            {
                // If we are at least in the 2nd row (starts numbering at zero)
                if(row >= 1 && column >= 1)
                {
                    // check one left, one down
                    if (candies[row, column].GetComponent<Candy>().IsSameType(candies[row - 1, column - 1].GetComponent<Candy>())) {

                        return new List<GameObject>
                        {
                            candies[row, column],
                            candies[row, column + 1],
                            candies[row - 1, column - 1]
                        };

                        /*  EXAMPLE *\

                         * * * * *
                         * * * * *
                         * & * * *
                         * * @ & *
                         * * * * * 

                         \*         */

                        
                    } else if (row <= GameVariables.Rows - 1 && column > 1) {

                        // check one left, one up
                        if(candies[row, column].GetComponent<Candy>().IsSameType( candies[row + 1, column - 1].GetComponent<Candy>()) ) {

                            return new List<GameObject>
                            {
                                candies[row, column],
                                candies[row, column + 1],
                                candies[row + 1, column - 1]
                            };

                        }

                    }
                 

                }

            }


        } // end if(column <= GameVariables.Columns - 2)


        // catch all
        return null;


    } // CheckHorizontal1



    // checking to the right of the specific candy
    public static List<GameObject> CheckHorizontal2(int row, int column, CandyArray candies)
    {

        /*  EXAMPLE *\
         
         * * * * *
         * * * * *
         * * * * *
         * @ & * *
         * * * & * 
         
         \*         */

        // Prep to look at the column two places to the right
        if (column <= GameVariables.Columns - 3)
        {

            // check if the candy to the right is the same Type
            if (candies[row, column].GetComponent<Candy>().IsSameType(candies[row, column + 1].GetComponent<Candy>()))
            {
                // If we are at least in the 2nd row (starts numbering at zero), and if we are at least 3 columns from end on the right
                if (row >= 1 && column <= GameVariables.Columns - 3)
                {
                    // check two right, one down
                    if (candies[row, column].GetComponent<Candy>().IsSameType(candies[row - 1, column + 2].GetComponent<Candy>()))
                    {

                        return new List<GameObject>
                        {
                            candies[row, column],
                            candies[row, column + 1],
                            candies[row - 1, column + 2]
                        };

                    }
                    else if (row <= GameVariables.Rows - 2 && column <= GameVariables.Columns - 3)
                    {
                        /*  EXAMPLE *\

                         * * * * *
                         * * * * * 
                         * * * & * 
                         * @ & * *
                         * * * * *  

                         \*         */

                        // check two right, one up
                        if (candies[row, column].GetComponent<Candy>().IsSameType(candies[row + 1, column + 2].GetComponent<Candy>()))
                        {

                            return new List<GameObject>
                            {
                                candies[row, column],
                                candies[row, column + 1],
                                candies[row + 1, column + 2]
                            };

                        }

                    }


                }

            }


        } 


        // catch all
        return null;


    } // CheckHorizontal2



    // checking to the right of the specific candy, skipping one in the row
    public static List<GameObject> CheckHorizontal3(int row, int column, CandyArray candies)
    {

        /*  EXAMPLE *\
         
         * * * * *
         * * * * *
         * * * * *
         * @ & * &
         * * * * * 
         
         \*         */

        // Prep to look at the column two places to the right
        if (column <= GameVariables.Columns - 4)
        {

            // check if the candy to the right is the same Type and check if the candy +3 to the right
            if (
                candies[row, column].GetComponent<Candy>().IsSameType(candies[row, column + 1].GetComponent<Candy>())
                &&
                candies[row, column].GetComponent<Candy>().IsSameType(candies[row, column + 3].GetComponent<Candy>())
                )
                {

                    return new List<GameObject>
                        {
                            candies[row, column],
                            candies[row, column + 1],
                            candies[row, column + 3]
                        };
                }

        }


        /*  EXAMPLE *\
         
         * * * * *
         * * * * *
         * * * * *
         * & * @ &
         * * * * * 
         
         \*         */

        // Prep to look at the column two places to the left
        if (column >= 2 && column <= GameVariables.Columns - 2)
        {
            // check if the candy to the right is the same Type and check if the candy -2 to the left
            if (
                candies[row, column].GetComponent<Candy>().IsSameType(candies[row, column + 1].GetComponent<Candy>())
                &&
                candies[row, column].GetComponent<Candy>().IsSameType(candies[row, column - 2].GetComponent<Candy>())
                )
                {

                    return new List<GameObject>
                        {
                            candies[row, column],
                            candies[row, column + 1],
                            candies[row, column - 2]
                        };
                }
        }

        // catch all
        return null;

    } // CheckHorizontal3



    // First function to help us find based on column
    public static List<GameObject> CheckVertical1(int row, int column, CandyArray candies)
    {

        /*  EXAMPLE *\

         * * * * *      // Row 4
         * * * * *      // Row 3
         * & * * *      // Row 2
         * @ * * *      // Row 1
         & * * * *      // Row 0

         \*         */

        // if there is at least one row above
        if(row <= GameVariables.Rows - 2)
        {
            // look at candy immediately above it
            if(candies[row, column].GetComponent<Candy>().IsSameType( candies[row + 1, column].GetComponent<Candy>()))
            {
                // Now confirm we're not on the first row or first column, so we can check "below" and "left"
                if(column >= 1 && row >= 1)
                {
                    // compare the two
                    if (candies[row, column].GetComponent<Candy>().IsSameType(candies[row - 1, column - 1].GetComponent<Candy>()))
                    {
                        return new List<GameObject>
                        {
                            candies[row, column],
                            candies[row + 1, column],
                            candies[row - 1, column - 1]
                        };
                    }

                } else if (column <= GameVariables.Columns - 2 && row >= 1)
                {
                    // Confirm we're not on the first row or last column, so we can check "below" and "right"

                    /*  EXAMPLE *\

                      * * * * *      // Row 4
                      * * * * *      // Row 3
                      * & * * *      // Row 2
                      * @ * * *      // Row 1
                      * * & * *      // Row 0

                      \*         */


                    // compare the two
                    if (candies[row, column].GetComponent<Candy>().IsSameType(candies[row - 1, column + 1].GetComponent<Candy>()))
                    {
                        return new List<GameObject>
                        {
                            candies[row, column],
                            candies[row + 1, column],
                            candies[row - 1, column + 1]
                        };
                    }

                }

            }

        }


        return null;

    } // CheckVertical1



    // TODO
    public static List<GameObject> CheckVertical2(int row, int column, CandyArray candies)
    {

        /*  EXAMPLE *\

          * * * * *      // Row 4
          & * * * *      // Row 3
          * & * * *      // Row 2
          * @ * * *      // Row 1
          * * * * *      // Row 0

          \*         */

        if(row <= GameVariables.Rows - 3)
        {

            if(candies[row, column].GetComponent<Candy>().IsSameType( candies[row + 1, column].GetComponent<Candy>() ) ) {

                if ( column >= 1)
                {

                    if(candies[row, column].GetComponent<Candy>().IsSameType( candies[row +2, column - 1].GetComponent<Candy>() ))
                    
                        return new List<GameObject>
                        {
                            candies[row, column],
                            candies[row + 1, column],
                            candies[row + 2, column - 1]
                        };
                    }

                } else if ( column <= GameVariables.Columns - 2)
                {
                    /*  EXAMPLE *\

                      * * * * *      // Row 4
                      * * & * *      // Row 3
                      * & * * *      // Row 2
                      * @ * * *      // Row 1
                      * * * * *      // Row 0

                      \*         */

                if(candies[row, column].GetComponent<Candy>().IsSameType( candies[row + 2, column +1].GetComponent<Candy>() ))
                    {

                        return new List<GameObject>
                        {
                            candies[row, column],
                            candies[row + 1, column],
                            candies[row + 2, column + 1]
                        };

                    }


                }

        }

        return null;
    }



    public static List<GameObject> CheckVertical3(int row, int column, CandyArray candies)
    {
        /*  EXAMPLE *\

          * & * * *      // Row 4
          * * * * *      // Row 3
          * & * * *      // Row 2
          * @ * * *      // Row 1
          * * * * *      // Row 0

          \*         */

        if (row <= GameVariables.Rows - 4)
        {

            if (
                candies[row, column].GetComponent<Candy>().IsSameType(candies[row + 1, column].GetComponent<Candy>())
                &&
                candies[row, column].GetComponent<Candy>().IsSameType(candies[row + 3, column].GetComponent<Candy>())
                )
            {
                return new List<GameObject>
                    {
                        candies[row, column],
                        candies[row + 1, column],
                        candies[row + 3, column]
                    };
            }
        }


        /*  EXAMPLE *\

          * * * * *      // Row 4
          * & * * *      // Row 3
          * @ * * *      // Row 2
          * * * * *      // Row 1
          * & * * *      // Row 0

          \*         */

        if( row >= 2 && row <= GameVariables.Rows - 2) {

            if(
                candies[row, column].GetComponent<Candy>().IsSameType( candies[row + 1, column].GetComponent<Candy>())
                &&
                candies[row - 2, column].GetComponent<Candy>().IsSameType( candies[row - 2, column].GetComponent<Candy>())
                )
            {
                return new List<GameObject>
                    {
                        candies[row, column],
                        candies[row - 1, column],
                        candies[row + 2, column]
                    };

            }

        }


        return null;

    } // CheckVertical3

}
