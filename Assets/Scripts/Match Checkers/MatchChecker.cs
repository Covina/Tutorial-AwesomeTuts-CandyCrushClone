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





}
