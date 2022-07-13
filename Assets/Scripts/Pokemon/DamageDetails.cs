/// <summary>
/// The DamageDetails class holds data about a pokemon's move result
/// (whether the enemy fainted, if it was a critical, and the effectiveness in terms of pokemon types)
/// </summary>
public class DamageDetails
{
    public bool Fainted { get; set; }
    public float Critical { get; set; }
    public float TypeEffectiveness { get; set; }
}
