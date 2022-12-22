using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// The Pokemon class manages all logic for pokemon
/// </summary>
[System.Serializable]
public class Pokemon
{
    [SerializeField] private PokemonBase _base;
    [SerializeField] private int level;

    public PokemonBase Base { get { return _base; } }
    public int Level { get { return level; } }
    public int HP { get; set; }
    public int Exp { get; set; }

    public List<Move> Moves { get; set; }
    public Move CurrentMove { get; set; }
    public Dictionary<Stat, int> Stats { get; private set; }
    public Dictionary<Stat, int> StatBoosts { get; private set; }

    public Condition Status { get; private set; }
    public Condition VolatileStatus { get; private set; }
    public int StatusTime { get; set; }
    public int VolatileStatusTime { get; set; }
    public Queue<string> StatusChanges { get; private set; }

    public event System.Action OnStatusChanged;
    public event System.Action OnHPChanged;

    public int Attack
    {
        get { return GetStat(Stat.Attack); }
    }

    public int Defense
    {
        get { return GetStat(Stat.Defense); }
    }

    public int SpecialAttack
    {
        get { return GetStat(Stat.SpecialAttack); }
    }

    public int SpecialDefense
    {
        get { return GetStat(Stat.SpecialDefense); }
    }

    public int Speed
    {
        get { return GetStat(Stat.Speed); }
    }

    public int MaxHp { get; private set; }

    public Pokemon(PokemonBase pBase, int pLevel)
    {
        _base = pBase;
        level = pLevel;

        Init();
    }

    public Pokemon(PokemonSaveData saveData)
    {
        _base = PokemonDB.GetPokemonByName(saveData.name);
        HP = saveData.hp;
        level = saveData.level;
        Exp = saveData.exp;

        if (saveData.statusID != null)
            Status = ConditionsDB.Conditions[saveData.statusID.Value];
        else
            Status = null;

        Moves = saveData.moves.Select(m => new Move(m)).ToList();

        CalculateStats();
        StatusChanges = new Queue<string>();
        ResetStatBoosts();
        VolatileStatus = null;
    }

    /// <summary>
    /// Initializes the Pokemon
    /// </summary>
    public void Init()
    {
        Moves = new List<Move>();

        // Generate moves based on pokemon level
        foreach (LearnableMove move in Base.LearnableMoves)
        {
            if (move.Level <= Level)
                Moves.Add(new Move(move.MoveBase));

            if (Moves.Count >= PokemonBase.MaxNumOfMoves)
                break;
        }

        Exp = Base.GetExpForLevel(Level);

        CalculateStats();
        HP = MaxHp;

        ResetStatBoosts();
        Status = null;
        VolatileStatus = null;
        StatusChanges = new Queue<string>();
    }

    public PokemonSaveData GetSaveData()
    {
        PokemonSaveData saveData = new PokemonSaveData()
        {
            name = Base.name,
            hp = HP,
            level = Level,
            exp = Exp,
            statusID = Status?.Id,
            moves = Moves.Select(m => m.GetSaveData()).ToList()
        };

        return saveData;
    }

    #region Battle

    /// <summary>
    /// Causes the specified pokemon to take damage
    /// </summary>
    /// <param name="move">The move to apply to the pokemon </param>
    /// <param name="attacker">The attacking pokemon </param>
    /// <returns>
    /// A <see cref="DamageDetails"> DamageDetails </see> class containing the effectiveness of the attack, whether it was a critical,
    /// and whether or not the pokemon fainted
    /// </returns>
    public DamageDetails TakeDamage(Move move, Pokemon attacker)
    {
        float critical = 1;

        if (Random.value * 100f <= 6.25) critical = 2;

        float effectiveness = TypeChart.GetEffectiveness(move.Base.Type, Base.Type1) * TypeChart.GetEffectiveness(move.Base.Type, Base.Type2);

        DamageDetails damageDetails = new()
        {
            TypeEffectiveness = effectiveness,
            Critical = critical,
            Fainted = false
        };

        bool specialMove = move.Base.Category == MoveCategory.Special;

        float attack = specialMove ? attacker.SpecialAttack : attacker.Attack;
        float defense = specialMove ? attacker.SpecialDefense : attacker.Defense;

        float modifiers = Random.Range(0.85f, 1f) * effectiveness * critical;
        float a = ((2 * attacker.Level) + 10) / 250f;

        float d = (a * move.Base.Power * ((float)attack / defense)) + 2;
        int damage = Mathf.FloorToInt(d * modifiers);

        DecreaseHP(damage);

        return damageDetails;
    }

    /// <summary>
    /// Get a random move from the pokemon's move list
    /// </summary>
    public Move GetRandomMove()
    {
        List<Move> movesWithPP = Moves.Where(move => move.PP > 0).ToList();

        int r = Random.Range(0, movesWithPP.Count);
        return movesWithPP[r];
    }

    /// <summary>
    /// Set the status of the pokemon
    /// </summary>
    /// <param name="ConditionID">The <see cref="ConditionID">ConditionID </see> of the status to set </param>
    public void SetStatus(ConditionID conditionID)
    {
        if (Status != null) return;

        Status = ConditionsDB.Conditions[conditionID];
        Status?.OnStart?.Invoke(this);
        StatusChanges.Enqueue($"{Base.Name} {Status.StartMessage}!");
        OnStatusChanged?.Invoke();
    }

    /// <summary>
    /// Remove all status effects from the pokemon
    /// </summary>
    public void CureStatus()
    {
        Status = null;
        OnStatusChanged?.Invoke();
    }

    /// <summary>
    /// Apply stat boosts to the pokemon
    /// </summary>
    /// <param name="statBoosts">The boosts to apply </param>
    public void ApplyBoosts(List<StatBoost> statBoosts)
    {
        foreach (StatBoost statBoost in statBoosts)
        {
            Stat stat = statBoost.stat;
            int boost = statBoost.boost;

            StatBoosts[stat] = Mathf.Clamp(StatBoosts[stat] + boost, -6, 6);

            if (boost > 0)
                StatusChanges.Enqueue($"{Base.Name}'s {stat} increased!");
            else
                StatusChanges.Enqueue($"{Base.Name}'s {stat} decreased!");

            Debug.Log($"{stat} has been boosted to {StatBoosts[stat]}");
        }
    }

    /// <summary>
    /// Set the volatile status of the pokemon
    /// </summary>
    /// <param name="ConditionID">The <see cref="ConditionID">ConditionID </see> of the volatile status to set </param>
    public void SetVolatileStatus(ConditionID conditionID)
    {
        if (VolatileStatus != null) return;

        VolatileStatus = ConditionsDB.Conditions[conditionID];
        VolatileStatus?.OnStart?.Invoke(this);
        StatusChanges.Enqueue($"{Base.Name} {VolatileStatus.StartMessage}!");
    }

    /// <summary>
    /// Remove all volatile status effects from the pokemon
    /// </summary>
    public void CureVolatileStatus()
    {
        VolatileStatus = null;
    }

    /// <summary>
    /// Decrease the HP of the pokemon
    /// </summary>
    /// <param name="damage">How much HP to decrease by </param>
    public void DecreaseHP(int damage)
    {
        HP = Mathf.Clamp(HP - damage, 0, MaxHp);
        OnHPChanged?.Invoke();
    }

    public void IncreaseHP(int amt)
    {
        HP = Mathf.Clamp(HP + amt, 0, MaxHp);
        OnHPChanged?.Invoke();
    }

    #endregion

    #region Events

    /// <summary>
    /// Ran after a pokemon's turn
    /// </summary>
    public void OnAfterTurn()
    {
        Status?.OnAfterTurn?.Invoke(this);
        VolatileStatus?.OnAfterTurn?.Invoke(this);
    }

    /// <summary>
    /// Ran before a pokemon's turn
    /// </summary>
    /// <returns>A bool representing whether or not the pokemon can perform their move </returns>
    public bool OnBeforeTurn()
    {
        bool canPerformMove = true;

        if (Status?.OnBeforeMove != null)
            if (!Status.OnBeforeMove(this))
                canPerformMove = false;

        if (VolatileStatus?.OnBeforeMove != null)
            if (!VolatileStatus.OnBeforeMove(this))
                canPerformMove = false;

        return canPerformMove;
    }

    /// <summary>
    /// Resets pokemon
    /// </summary>
    public void OnBattleOver()
    {
        ResetStatBoosts();
        CureVolatileStatus();
    }

    #endregion

    #region Stats

    /// <summary>
    /// Check if the pokemon should level up
    /// and if it should, then level it up
    /// </summary>
    /// <returns>
    /// Whether the pokemon leveled up
    /// </returns>
    public bool CheckForLevelUp()
    {
        if (Exp > Base.GetExpForLevel(level + 1))
        {
            ++level;
            return true;
        }

        return false;
    }

    public LearnableMove GetLearnableMoveAtCurrentLevel()
    {
        return Base.LearnableMoves.Where((move) => move.Level == level).FirstOrDefault();
    }

    public void LearnMove(LearnableMove moveToLearn)
    {
        if (Moves.Count > PokemonBase.MaxNumOfMoves) return;

        Moves.Add(new Move(moveToLearn.MoveBase));
    }

    private void ResetStatBoosts()
    {
        StatBoosts = new Dictionary<Stat, int>()
        {
            {Stat.Attack, 0},
            {Stat.Defense, 0},
            {Stat.SpecialAttack, 0},
            {Stat.SpecialDefense, 0},
            {Stat.Speed, 0},
            {Stat.Accuracy, 0},
            {Stat.Evasion, 0},
        };
    }

    private void CalculateStats()
    {
        Stats = new Dictionary<Stat, int>
        {
            { Stat.Attack, Mathf.FloorToInt(Base.Attack * Level / 100f) + 5 },
            { Stat.Defense, Mathf.FloorToInt(Base.Defense * Level / 100f) + 5 },
            { Stat.SpecialAttack, Mathf.FloorToInt(Base.SpecialAttack * Level / 100f) + 5 },
            { Stat.SpecialDefense, Mathf.FloorToInt(Base.SpecialDefense * Level / 100f) + 5 },
            { Stat.Speed, Mathf.FloorToInt(Base.Speed * Level / 100f) + 5 }
        };

        MaxHp = Mathf.FloorToInt(Base.MaxHp * Level / 100f) + 10 + Level;
    }

    private int GetStat(Stat stat)
    {
        int statVal = Stats[stat];

        int boost = StatBoosts[stat];
        float[] boostValues = new float[] { 1f, 1.5f, 2f, 2.5f, 3f, 3.5f, 4f };

        if (boost >= 0)
            statVal = Mathf.FloorToInt(statVal * boostValues[boost]);
        else
            statVal = Mathf.FloorToInt(statVal / boostValues[-boost]);

        return statVal;
    }

    #endregion
}

[System.Serializable]
public class PokemonSaveData
{
    public string name;
    public int hp;
    public int level;
    public int exp;
    public ConditionID? statusID;
    public List<MoveSaveData> moves;
}