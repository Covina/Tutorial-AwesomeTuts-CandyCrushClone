using System;

public class GameVariables {

    // rows of beans
    public static int Rows = 12;

    // columns of beans
    public static int Columns = 8;

    // Animation duration delay when moving the beans
    public static float AnimationDuration = 0.2f;

    // Delay in movement
    public static float MoveAnimationDuration = 0.05f;

    // How logn the explosion animation should take
    public static float ExplosionAnimationDuration = 0.3f;

    // how long until we check for matches
    public static float WaitBeforePotentialMatchesCheck = 2.0f;

    // Alter opacity for suggested matches
    public static float OpactiyAnimationDelay = 0.5f;

    // Must have 3 in a row to trigger destruction
    public static int MinimumMatches = 3;

    // Minimum Matches to get bonus type bean
    public static int MinimumMatchesForBonus = 4;

    // points for a match 3
    public static int Match3Score = 100;

    // bonus for 
    public static int SubsequelMatchScore = 1000;


}
