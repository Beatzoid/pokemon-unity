using System;

public class Condition
{
    public string Name { get; set; }
    public string Description { get; set; }

    /// <summary>
    /// Message shown when the condition is inflicted on a pokemon
    /// </summary>
    public string StartMessage { get; set; }
    /// <summary>
    /// What to do to the pokemon after they get inflicted with the condition
    /// </summary>
    public Action<Pokemon> OnAfterTurn { get; set; }
}
