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
        List<List< GameObject>> matches = new List<List<GameObject>>();

        return null;

    }


    public static List<GameObject> CheckHorizontal1(int row, int column, CandyArray candies)
    {

        /*  EXAMPLE *\
         
         * * * * * * *
         * * * * * * *
         * * * * * * *
         * * * @ & * *
         * * & * * * * 
         
         \*         */

        // Prep to look at the column one place to the right
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

                         * * * * * * *
                         * * * * * * *
                         * * & * * * *
                         * * * @ & * *
                         * * * * * * * 

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


    }


}
