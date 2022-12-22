using System;

/// <summary>
/// The Condition class manages all pokemon conditions
/// </summary>
public class Condition
{
    public ConditionID Id { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }

    /// <summary>
    /// Message shown when the condition is inflicted on a pokemon
    /// </summary>
    public string StartMessage { get; set; }
    /// <summary>
    /// Run an action before OnBeforeTurn
    /// </summary>
    public Action<Pokemon> OnStart { get; set; }
    /// <summary>
    /// Run an action after the pokemon's turn (eg. burn, poison)
    /// </summary>
    public Action<Pokemon> OnAfterTurn { get; set; }
    /// <summary>
    /// Run an action before the pokemon's turn (eg. paralyze)
    /// </summary>
    public Func<Pokemon, bool> OnBeforeMove { get; set; }
}