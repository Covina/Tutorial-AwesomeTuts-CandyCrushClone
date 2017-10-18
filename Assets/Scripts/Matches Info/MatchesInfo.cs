using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class MatchesInfo {

    //
    private List<GameObject> matches;

    //
    public BonusType BonusesContained { get; set; }


    /// <summary>
    /// Constructor
    /// </summary>
    public MatchesInfo()
    {
        // init the list
        matches = new List<GameObject>();

        // init that there is no bonus
        BonusesContained = BonusType.None;
    }

    // List of de-duped items
    public IEnumerable<GameObject> MatchedCandy
    {
        get
        {
            return matches.Distinct();
        }
    }


    public void AddObject(GameObject obj)
    {

        // If the item doesn't already exist in the list, then add it
        if(!matches.Contains(obj))
        {
            matches.Add(obj);
        }

    }

    // Pass in the list and loop through to add them all
    public void AddObjectRange(IEnumerable<GameObject> objs)
    {

        foreach(var item in objs)
        {
            AddObject(item);
        }
    }


} // MatchesInfo
