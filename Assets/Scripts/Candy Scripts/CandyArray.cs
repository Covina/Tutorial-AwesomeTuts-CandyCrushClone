using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class CandyArray {

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
            } catch(Exception e)
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


}
