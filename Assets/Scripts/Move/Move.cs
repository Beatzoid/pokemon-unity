/// <summary>
/// The Move class manages all moves for pokemon 
/// </summary>
public class Move
{
    public MoveBase Base { get; set; }
    public int PP { get; set; }

    /// <summary>
    /// The Move class stores info about a pokemon's move
    /// </summary>
    /// <param name="pBase">The move base </param>
    public Move(MoveBase pBase)
    {
        Base = pBase;
        PP = pBase.PP;
    }

    public Move(MoveSaveData saveData)
    {
        Base = MoveDB.GetMoveByName(saveData.name);
        PP = saveData.pp;
    }

    public MoveSaveData GetSaveData()
    {
        MoveSaveData saveData = new MoveSaveData()
        {

            name = Base.MoveName,
            pp = PP,
        };

        return saveData;
    }
}

[System.Serializable]
public class MoveSaveData
{
    public string name;
    public int pp;
}