using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Candy : MonoBehaviour {

    // Bonus type
    public BonusType Bonus { get; set; }

    // Row Location
    public int Row { get; set; }

    // Column Location
    public int Column { get; set; }

    // Type of candy
    public string Type { get; set; }

    // Constructor
    public Candy()
    {
        // set default bonus type when candy is created
        Bonus = BonusType.None;
    }

    //
    public bool IsSameType(Candy otherCandy)
    {
        // Compare if this candy and the other are the same type
        return string.Compare(this.Type, otherCandy.Type) == 0;

    }


    public void Initialize(string type, int row, int column)
    {

        // Set variables
        Column = column;
        Row = row;
        Type = type;


    }

    // change location of two candies
    public static void SwapRowColumn(Candy c1, Candy c2)
    {
        // hold original row
        int temp = c1.Row;

        // set c1 row to c2
        c1.Row = c2.Row;

        // set c2 to what was c1
        c2.Row = temp;


        // Perform same swap info but with columns
        temp = c1.Column;
        c1.Column = c2.Column;
        c2.Column = temp;



    }

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

} // Candy
