using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class AlteredCandyInfo {

    //
    private List<GameObject> newCandy;

    //
    public int maxDistance { get; set; }

    /// <summary>
    /// Constructor
    /// </summary>
    public AlteredCandyInfo()
    {
        // create a list
        newCandy = new List<GameObject>();

    }

    /// <summary>
    /// Helper to return a distinct list of candies
    /// </summary>
    public IEnumerable<GameObject> AlteredCandy
    {
        get
        {
            return newCandy.Distinct();
        }

    }

    /// <summary>
    /// Add a candy to the List
    /// </summary>
    /// <param name="obj"></param>
    public void AddCandy(GameObject obj)
    {
        // check if it already exists
        if(!newCandy.Contains(obj))
        {
            // if it does not already exist, then add it
            newCandy.Add(obj);
        }
    }

}
