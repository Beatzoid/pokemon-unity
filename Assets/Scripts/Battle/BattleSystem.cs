using System;
using System.Collections;
using UnityEngine;

public enum BattleState
{
    Start,
    PlayerAction,
    PlayerMove,
    EnemyMove,
    Busy,
    PartyScreen
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
    int currentActionIndex;
    int currentMoveIndex;
    int currentPartyMemberIndex;

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
        else if (state == BattleState.PartyScreen)
        {
            HandlePartyScreenSelection();
        }
    }

    void HandlePartyScreenSelection()
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
            PlayerAction();
        }
    }

    void HandleMoveSelection()
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
            PlayerAction();
        }
    }

    void HandleActionSelection()
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
                PlayerMove();
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

    void OpenPartyScreen()
    {
        state = BattleState.PartyScreen;
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
        currentMoveIndex = 0;
        playerUnit.Setup(playerParty.GetHealthyPokemon());
        enemyUnit.Setup(wildPokemon);

        playerHud.SetData(playerUnit.Pokemon);
        enemyHud.SetData(enemyUnit.Pokemon);

        partyScreen.Init();

        dialogBox.SetMoveNames(playerUnit.Pokemon.Moves);

        yield return dialogBox.TypeDialog($"A wild {enemyUnit.Pokemon.Base.Name} appeared");

        PlayerAction();
    }

    IEnumerator SwitchPokemon(Pokemon newPokemon)
    {
        if (playerUnit.Pokemon.HP > 0)
        {
            yield return dialogBox.TypeDialog($"Come back {playerUnit.Pokemon.Base.name}");
            playerUnit.PlayFaintAnimation();
            yield return new WaitForSeconds(1.5f);
        }

        currentMoveIndex = 0;

        playerUnit.Setup(newPokemon);
        playerHud.SetData(newPokemon);

        dialogBox.SetMoveNames(newPokemon.Moves);

        yield return dialogBox.TypeDialog($"Go {newPokemon.Base.Name}!");

        StartCoroutine(PerformEnemyMove());
    }

    IEnumerator PerformPlayerMove()
    {
        state = BattleState.Busy;

        Move move = playerUnit.Pokemon.Moves[currentMoveIndex];
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
                OpenPartyScreen();
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
