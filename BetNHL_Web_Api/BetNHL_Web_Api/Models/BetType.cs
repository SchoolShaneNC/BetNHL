namespace BetNHL_Web_Api.Models
{
    public enum BetCategory
    {
        Player,
        Team,
        Game
    }

    public enum BetMetric
    {
        Goal,
        Assist,
        Point,
        TotalGoals
    }

    public enum BetCondition
    {
        Anytime,
        Over,
        Under,
        First,
        Multi
    }

    public enum BetContext
    {
        None,
        Powerplay
    }

}


//PlayerAnytimeGoal
//PlayerAssist
//PlayerPoint
//PlayerFirstGoal
//PlayerPowerplayGoal
//PlayerEmptyNetGoal
//PlayerGoalsOver
//GameTotalOver
//GameTotalUnder
//TeamTotalOver
//TeamTotalUnder