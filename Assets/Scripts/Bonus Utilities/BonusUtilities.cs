using System;


[Flags]
public enum BonusType
{
    None,
    DestroyWholeRowColumn
}


public enum GameState
{
    None,
    SelectionStarted,
    Animating
}


public static class BonusTypeChecker
{
    // single function
    public static bool ContainsDestroyWholeWorColumn (BonusType bt)
    {
        // Single & same as && when comparing enums
        return (bt & BonusType.DestroyWholeRowColumn) == BonusType.DestroyWholeRowColumn;
    }
}


