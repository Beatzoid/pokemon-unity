using System;
using System.Collections;
using UnityEngine;

public enum BattleState
{
    Start,
    ActionSelection,
    MoveSelection,
    PerformMove,
    Busy,
    PartyScreen,
    Battleover
}

/// <summary>
/// The BattleSystem class manages the core battle scene logic
/// </summary>
public class BattleSystem : MonoBehaviour
{
    [SerializeField] private BattleUnit playerUnit;
    [SerializeField] private BattleUnit enemyUnit;
    [SerializeField] private BattleDialogBox dialogBox;
    [SerializeField] private PartyScreen partyScreen;

    public event Action<bool> OnBattleOver;

    private BattleState state;
    private int currentActionIndex;
    private int currentMoveIndex;
    private int currentPartyMemberIndex;

    private PokemonParty playerParty;
    private Pokemon wildPokemon;

    /// <summary>
    /// Start a battle
    /// </summary>
    /// <param name="playerParty">The players PokemonParty </param>
    /// <param name="wildPokemon">The enemy's pokemon </param>
    public void StartBattle(PokemonParty playerParty, Pokemon wildPokemon)
    {
        this.playerParty = playerParty;
        this.wildPokemon = wildPokemon;

        StartCoroutine(SetupBattle());
    }

    /// <summary>
    /// Update the battle system
    /// </summary>
    public void HandleUpdate()
    {
        if (state == BattleState.ActionSelection)
        {
            HandleActionSelection();
        }
        else if (state == BattleState.MoveSelection)
        {
            HandleMoveSelection();
        }
        else if (state == BattleState.PartyScreen)
        {
            HandlePartyScreenSelection();
        }
    }

    /// <summary>
    /// Setup the battle by setting up the player and enemy units, setting the HUD stats, and
    /// showing a message on the dialog box
    /// </summary>
    public IEnumerator SetupBattle()
    {
        currentMoveIndex = 0;
        playerUnit.Setup(playerParty.GetHealthyPokemon());
        enemyUnit.Setup(wildPokemon);

        partyScreen.Init();

        dialogBox.SetMoveNames(playerUnit.Pokemon.Moves);

        yield return dialogBox.TypeDialog($"A wild {enemyUnit.Pokemon.Base.Name} appeared");

        ChooseFirstTurn();
    }

    private void HandlePartyScreenSelection()
    {
        if (Input.GetKeyDown(KeyCode.RightArrow))
            ++currentPartyMemberIndex;
        else if (Input.GetKeyDown(KeyCode.LeftArrow))
            --currentPartyMemberIndex;
        else if (Input.GetKeyDown(KeyCode.DownArrow))
            currentPartyMemberIndex += 2;
        else if (Input.GetKeyDown(KeyCode.UpArrow))
            currentPartyMemberIndex -= 2;

        currentPartyMemberIndex = Mathf.Clamp(currentPartyMemberIndex, 0, playerParty.Pokemon.Count);

        partyScreen.UpdateMemberSelection(currentPartyMemberIndex);

        if (Input.GetKeyDown(KeyCode.Return))
        {
            Pokemon selectedPartyMember = playerParty.Pokemon[currentPartyMemberIndex];

            if (selectedPartyMember.HP <= 0)
            {
                partyScreen.SetMessageText("You can't send out a fainted pokemon");
                return;
            }

            if (selectedPartyMember == playerUnit.Pokemon)
            {
                partyScreen.SetMessageText("You can't switch with the currently active pokemon");
                return;
            }

            partyScreen.gameObject.SetActive(false);
            state = BattleState.Busy;
            StartCoroutine(SwitchPokemon(selectedPartyMember));
        }
        else if (Input.GetKeyDown(KeyCode.X))
        {
            partyScreen.gameObject.SetActive(false);
            ActionSelection();
        }
    }

    private void HandleMoveSelection()
    {
        if (Input.GetKeyDown(KeyCode.RightArrow))
            ++currentMoveIndex;
        else if (Input.GetKeyDown(KeyCode.LeftArrow))
            --currentMoveIndex;
        else if (Input.GetKeyDown(KeyCode.DownArrow))
            currentMoveIndex += 2;
        else if (Input.GetKeyDown(KeyCode.UpArrow))
            currentMoveIndex -= 2;

        currentMoveIndex = Mathf.Clamp(currentMoveIndex, 0, playerUnit.Pokemon.Moves.Count - 1);

        dialogBox.UpdateMoveSelection(currentMoveIndex, playerUnit.Pokemon.Moves[currentMoveIndex]);

        if (Input.GetKeyDown(KeyCode.Return))
        {
            dialogBox.SetMoveSelectorActive(false);
            dialogBox.SetDialogTextActive(true);
            StartCoroutine(PerformPlayerMove());
        }
        else if (Input.GetKeyDown(KeyCode.X))
        {
            dialogBox.SetMoveSelectorActive(false);
            dialogBox.SetDialogTextActive(true);
            ActionSelection();
        }
    }

    private void HandleActionSelection()
    {
        if (Input.GetKeyDown(KeyCode.RightArrow))
            ++currentActionIndex;
        else if (Input.GetKeyDown(KeyCode.LeftArrow))
            --currentActionIndex;
        else if (Input.GetKeyDown(KeyCode.DownArrow))
            currentActionIndex += 2;
        else if (Input.GetKeyDown(KeyCode.UpArrow))
            currentActionIndex -= 2;

        currentActionIndex = Mathf.Clamp(currentActionIndex, 0, 3);

        dialogBox.UpdateActionSelection(currentActionIndex);

        if (Input.GetKeyDown(KeyCode.Return))
        {
            if (currentActionIndex == 0)
            {
                // Fight
                MoveSelection();
            }
            else if (currentActionIndex == 1)
            {
                // Bag
            }
            else if (currentActionIndex == 2)
            {
                // Choose Pokemon
                OpenPartyScreen();
            }
            else if (currentActionIndex == 3)
            {
                // Run
            }
        }
    }

    private void OpenPartyScreen()
    {
        state = BattleState.PartyScreen;
        partyScreen.SetPartyData(playerParty.Pokemon);
        partyScreen.gameObject.SetActive(true);
    }

    private void ActionSelection()
    {
        state = BattleState.ActionSelection;
        dialogBox.SetDialog("Choose an action");
        dialogBox.SetActionSelectorActive(true);
    }

    private void ChooseFirstTurn()
    {
        if (playerUnit.Pokemon.Speed >= enemyUnit.Pokemon.Speed)
            ActionSelection();
        else
            StartCoroutine(PerformEnemyMove());
    }

    private void MoveSelection()
    {
        state = BattleState.MoveSelection;
        dialogBox.SetActionSelectorActive(false);
        dialogBox.SetDialogTextActive(false);
        dialogBox.SetMoveSelectorActive(true);
    }

    private void BattleOver(bool won)
    {
        state = BattleState.Battleover;
        playerParty.Pokemon.ForEach(p => p.OnBattleOver());
        OnBattleOver(won);
    }

    private void CheckForBattleOver(BattleUnit faintedUnit)
    {
        if (faintedUnit.IsPlayerUnit)
        {
            Pokemon nextPokemon = playerParty.GetHealthyPokemon();

            if (nextPokemon != null)
                OpenPartyScreen();
            else
                BattleOver(false);
        }
        else
            BattleOver(true);
    }

    private bool CheckIfMoveHits(Move move, Pokemon source, Pokemon target)
    {
        if (move.Base.AlwaysHits) return true;

        float moveAccuracy = move.Base.Accuracy;

        int accuracy = source.StatBoosts[Stat.Accuracy];
        int evasion = target.StatBoosts[Stat.Evasion];

        float[] boostValues = new float[] { 1f, 4f / 3f, 5f / 3f, 2f, 7f / 3f, 8f / 3f, 3f };

        if (accuracy > 0)
            moveAccuracy *= boostValues[accuracy];
        else
            moveAccuracy /= boostValues[-accuracy];

        if (evasion > 0)
            moveAccuracy /= boostValues[evasion];
        else
            moveAccuracy *= boostValues[-evasion];


        return UnityEngine.Random.Range(1, 101) <= moveAccuracy;
    }
    private IEnumerator ShowStatusChanges(Pokemon pokemon)
    {
        while (pokemon.StatusChanges.Count > 0)
        {
            string message = pokemon.StatusChanges.Dequeue();
            yield return dialogBox.TypeDialog(message);
        }
    }

    private IEnumerator RunMove(BattleUnit source, BattleUnit target, Move move)
    {
        bool canRunMove = source.Pokemon.OnBeforeTurn();
        if (!canRunMove)
        {
            yield return ShowStatusChanges(source.Pokemon);
            yield return source.Hud.UpdateHP();
            yield break;
        }

        yield return ShowStatusChanges(source.Pokemon);

        move.PP--;

        yield return dialogBox.TypeDialog($"{source.Pokemon.Base.Name} used {move.Base.MoveName}");

        if (CheckIfMoveHits(move, source.Pokemon, target.Pokemon))
        {

            source.PlayAttackAnimation();
            yield return new WaitForSeconds(1f);

            target.PlayHitAnimation();

            if (move.Base.Category == MoveCategory.Status)
            {
                yield return RunMoveEffects(move.Base.Effects, source.Pokemon, target.Pokemon, move.Base.Target);
            }
            else
            {
                DamageDetails damageDetails = target.Pokemon.TakeDamage(move, source.Pokemon);
                yield return target.Hud.UpdateHP();
                yield return ShowDamageDetails(damageDetails);
            }

            if (target.Pokemon.HP <= 0)
            {
                yield return dialogBox.TypeDialog($"{target.Pokemon.Base.Name} Fainted");
                target.PlayFaintAnimation();

                yield return new WaitForSeconds(1.5f);

                CheckForBattleOver(target);
            }
        }
        else
        {
            yield return dialogBox.TypeDialog($"{source.Pokemon.Base.Name}'s attack missed!");
        }

        source.Pokemon.OnAfterTurn();
        yield return ShowStatusChanges(source.Pokemon);
        yield return source.Hud.UpdateHP();

        if (move.Base.SecondaryEffects != null && move.Base.SecondaryEffects.Count > 0 && target.Pokemon.HP > 0)
        {
            foreach (SecondaryEffects secondaryEffect in move.Base.SecondaryEffects)
            {
                int random = UnityEngine.Random.Range(1, 101);

                if (random <= secondaryEffect.Chance)
                {
                    yield return RunMoveEffects(secondaryEffect, source.Pokemon, target.Pokemon, secondaryEffect.Target);
                }
            }
        }

        // Since statuses like poison and burn effect the HP of the pokemon
        // it has the potential to faint because of the effect
        // so this if statement checks if that is the case and handles it accordingly
        if (source.Pokemon.HP <= 0)
        {
            yield return dialogBox.TypeDialog($"{source.Pokemon.Base.Name} Fainted");
            source.PlayFaintAnimation();

            yield return new WaitForSeconds(1.5f);

            CheckForBattleOver(source);
        }
    }

    private IEnumerator RunMoveEffects(MoveEffects effects, Pokemon source, Pokemon target, MoveTarget moveTarget)
    {
        // Stat boosting/deboosting
        if (effects.Boosts != null)
        {
            if (moveTarget == MoveTarget.self)
                source.ApplyBoosts(effects.Boosts);
            else
                target.ApplyBoosts(effects.Boosts);
        }

        // Status Conditions/Effects
        if (effects.Status != ConditionID.none)
        {
            target.SetStatus(effects.Status);
        }

        // Volatile Status Conditions/Effects
        if (effects.VolatileStatus != ConditionID.none)
        {
            target.SetVolatileStatus(effects.VolatileStatus);
        }

        yield return ShowStatusChanges(source);
        yield return ShowStatusChanges(target);
    }

    private IEnumerator SwitchPokemon(Pokemon newPokemon)
    {
        bool currentPokemonFainted = true;

        if (playerUnit.Pokemon.HP > 0)
        {
            currentPokemonFainted = false;

            yield return dialogBox.TypeDialog($"Come back {playerUnit.Pokemon.Base.name}");
            playerUnit.PlayFaintAnimation();
            yield return new WaitForSeconds(1.5f);
        }

        // Makes sure that we don't get an "OutOfRangeIndex" error
        currentMoveIndex = 0;

        playerUnit.Setup(newPokemon);

        dialogBox.SetMoveNames(newPokemon.Moves);

        yield return dialogBox.TypeDialog($"Go {newPokemon.Base.Name}!");

        if (currentPokemonFainted)
            ChooseFirstTurn();
        else
            StartCoroutine(PerformEnemyMove());

        StartCoroutine(PerformEnemyMove());
    }

    private IEnumerator PerformPlayerMove()
    {
        state = BattleState.PerformMove;

        Move move = playerUnit.Pokemon.Moves[currentMoveIndex];

        yield return RunMove(playerUnit, enemyUnit, move);

        if (state == BattleState.PerformMove)
            StartCoroutine(PerformEnemyMove());
    }

    private IEnumerator PerformEnemyMove()
    {
        state = BattleState.PerformMove;

        Move move = enemyUnit.Pokemon.GetRandomMove();

        yield return RunMove(enemyUnit, playerUnit, move);

        if (state == BattleState.PerformMove)
            ActionSelection();
    }

    private IEnumerator ShowDamageDetails(DamageDetails damageDetails)
    {
        if (damageDetails.Critical > 1f)
            yield return dialogBox.TypeDialog("A critical hit!");

        if (damageDetails.TypeEffectiveness > 1f)
            yield return dialogBox.TypeDialog("It's super effective!");
        else if (damageDetails.TypeEffectiveness < 1f)
            yield return dialogBox.TypeDialog("Its not very effective...");
    }
}
