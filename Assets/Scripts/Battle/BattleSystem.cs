using System;
using System.Collections;
using UnityEngine;

public enum BattleState
{
    Start,
    PlayerAction,
    PlayerMove,
    EnemyMove,
    Busy
}

public class BattleSystem : MonoBehaviour
{
    [SerializeField] BattleUnit playerUnit;
    [SerializeField] BattleUnit enemyUnit;
    [SerializeField] BattleHud playerHud;
    [SerializeField] BattleHud enemyHud;
    [SerializeField] BattleDialogBox dialogBox;
    [SerializeField] PartyScreen partyScreen;

    public event Action<bool> OnBattleOver;

    BattleState state;
    int currentAction;
    int currentMove;

    PokemonParty playerParty;
    Pokemon wildPokemon;

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

    public void HandleUpdate()
    {
        if (state == BattleState.PlayerAction)
        {
            HandleActionSelection();
        }
        else if (state == BattleState.PlayerMove)
        {
            HandleMoveSelection();
        }
    }

    void HandleMoveSelection()
    {
        if (Input.GetKeyDown(KeyCode.RightArrow))
            ++currentMove;
        else if (Input.GetKeyDown(KeyCode.LeftArrow))
            --currentMove;
        else if (Input.GetKeyDown(KeyCode.DownArrow))
            currentMove += 2;
        else if (Input.GetKeyDown(KeyCode.UpArrow))
            currentMove -= 2;

        currentMove = Mathf.Clamp(currentMove, 0, playerUnit.Pokemon.Moves.Count - 1);

        dialogBox.UpdateMoveSelection(currentMove, playerUnit.Pokemon.Moves[currentMove]);

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
            PlayerAction();
        }
    }

    void HandleActionSelection()
    {
        if (Input.GetKeyDown(KeyCode.RightArrow))
            ++currentAction;
        else if (Input.GetKeyDown(KeyCode.LeftArrow))
            --currentAction;
        else if (Input.GetKeyDown(KeyCode.DownArrow))
            currentAction += 2;
        else if (Input.GetKeyDown(KeyCode.UpArrow))
            currentAction -= 2;

        currentAction = Mathf.Clamp(currentAction, 0, 3);

        dialogBox.UpdateActionSelection(currentAction);

        if (Input.GetKeyDown(KeyCode.Return))
        {
            if (currentAction == 0)
            {
                // Fight
                PlayerMove();
            }
            else if (currentAction == 1)
            {
                // Bag
            }
            else if (currentAction == 2)
            {
                // Choose Pokemon
                OpenPartyScreen();
            }
            else if (currentAction == 3)
            {
                // Run
            }
        }
    }

    void OpenPartyScreen()
    {
        partyScreen.SetPartyData(playerParty.Pokemon);
        partyScreen.gameObject.SetActive(true);
    }

    void PlayerAction()
    {
        state = BattleState.PlayerAction;
        dialogBox.SetDialog("Choose an action");
        dialogBox.SetActionSelectorActive(true);
    }

    void PlayerMove()
    {
        state = BattleState.PlayerMove;
        dialogBox.SetActionSelectorActive(false);
        dialogBox.SetDialogTextActive(false);
        dialogBox.SetMoveSelectorActive(true);
    }

    /// <summary>
    /// Setup the battle by setting up the player and enemy units, setting the HUD stats, and
    /// showing a message on the dialog box
    /// </summary>
    public IEnumerator SetupBattle()
    {
        currentMove = 0;
        playerUnit.Setup(playerParty.GetHealthyPokemon());
        enemyUnit.Setup(wildPokemon);

        playerHud.SetData(playerUnit.Pokemon);
        enemyHud.SetData(enemyUnit.Pokemon);

        partyScreen.Init();

        dialogBox.SetMoveNames(playerUnit.Pokemon.Moves);

        yield return dialogBox.TypeDialog($"A wild {enemyUnit.Pokemon.Base.Name} appeared");

        PlayerAction();
    }

    IEnumerator PerformPlayerMove()
    {
        state = BattleState.Busy;

        Move move = playerUnit.Pokemon.Moves[currentMove];
        move.PP--;

        yield return dialogBox.TypeDialog($"{playerUnit.Pokemon.Base.Name} used {move.Base.MoveName}");

        playerUnit.PlayAttackAnimation();
        yield return new WaitForSeconds(1f);

        enemyUnit.PlayHitAnimation();

        DamageDetails damageDetails = enemyUnit.Pokemon.TakeDamage(move, playerUnit.Pokemon);
        yield return enemyHud.UpdateHP();
        yield return ShowDamageDetails(damageDetails);

        if (damageDetails.Fainted)
        {
            yield return dialogBox.TypeDialog($"{enemyUnit.Pokemon.Base.Name} Fainted");
            enemyUnit.PlayFaintAnimation();

            yield return new WaitForSeconds(1.5f);
            OnBattleOver(true);
        }
        else
        {
            StartCoroutine(PerformEnemyMove());
        }
    }

    IEnumerator PerformEnemyMove()
    {
        state = BattleState.EnemyMove;

        Move move = enemyUnit.Pokemon.GetRandomMove();
        move.PP--;

        yield return dialogBox.TypeDialog($"{enemyUnit.Pokemon.Base.Name} used {move.Base.MoveName}");

        enemyUnit.PlayAttackAnimation();
        yield return new WaitForSeconds(1f);

        playerUnit.PlayHitAnimation();

        DamageDetails damageDetails = playerUnit.Pokemon.TakeDamage(move, enemyUnit.Pokemon);

        yield return playerHud.UpdateHP();
        yield return ShowDamageDetails(damageDetails);

        if (damageDetails.Fainted)
        {
            yield return dialogBox.TypeDialog($"{playerUnit.Pokemon.Base.Name} Fainted");
            playerUnit.PlayFaintAnimation();

            yield return new WaitForSeconds(1.5f);

            Pokemon nextPokemon = playerParty.GetHealthyPokemon();

            if (nextPokemon != null)
            {
                currentMove = 0;

                playerUnit.Setup(nextPokemon);
                playerHud.SetData(nextPokemon);

                dialogBox.SetMoveNames(nextPokemon.Moves);

                yield return dialogBox.TypeDialog($"Go {nextPokemon.Base.Name}!");

                PlayerAction();
            }
            else
            {

                OnBattleOver(false);
            }
        }
        else
        {
            PlayerAction();
        }
    }

    IEnumerator ShowDamageDetails(DamageDetails damageDetails)
    {
        if (damageDetails.Critical > 1f)
            yield return dialogBox.TypeDialog("A critical hit!");

        if (damageDetails.TypeEffectiveness > 1f)
            yield return dialogBox.TypeDialog("It's super effective!");
        else if (damageDetails.TypeEffectiveness < 1f)
            yield return dialogBox.TypeDialog("Its not very effective...");
    }
}
