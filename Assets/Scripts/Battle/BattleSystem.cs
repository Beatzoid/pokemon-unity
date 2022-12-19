using System;
using System.Collections;
using System.Linq;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public enum BattleState
{
    Start,
    ActionSelection,
    MoveSelection,
    RunningTurn,
    Busy,
    PartyScreen,
    BattleOver,
    AboutToUse,
    MoveToForget,
    Bag
}

public enum BattleAction
{
    Move,
    SwitchPokemon,
    UseItem,
    Run
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
    [SerializeField] private Image playerImage;
    [SerializeField] private Image trainerImage;
    [SerializeField] private GameObject pokeball;
    [SerializeField] private MoveSelectionUI moveSelectionUI;
    [SerializeField] private InventoryUI inventoryUI;

    public event Action<bool> OnBattleOver;

    private BattleState state;

    private int currentActionIndex;
    private int currentMoveIndex;
    private bool aboutToUseChoice = true;
    private int escapeAttempts;

    private PokemonParty playerParty;
    private PokemonParty trainerParty;
    private Pokemon wildPokemon;

    private bool isTrainerBattle = false;
    private PlayerController player;
    private TrainerController trainer;

    private MoveBase moveToLearn;

    #region Setup
    /// <summary>
    /// Start a battle
    /// </summary>
    /// <param name="playerParty">The players PokemonParty </param>
    /// <param name="wildPokemon">The enemy's pokemon </param>
    public void StartBattle(PokemonParty playerParty, Pokemon wildPokemon)
    {
        this.playerParty = playerParty;
        this.wildPokemon = wildPokemon;
        player = playerParty.GetComponent<PlayerController>();
        escapeAttempts = 0;
        isTrainerBattle = false;

        StartCoroutine(SetupBattle());
    }

    /// <summary>
    /// Start a trainer battle
    /// </summary>
    /// <param name="playerParty">The players PokemonParty </param>
    /// <param name="wildPokemon">The enemy's pokemon </param>
    public void StartTrainerBattle(PokemonParty playerParty, PokemonParty trainerParty)
    {
        this.playerParty = playerParty;
        this.trainerParty = trainerParty;

        isTrainerBattle = true;

        player = playerParty.GetComponent<PlayerController>();
        trainer = trainerParty.GetComponent<TrainerController>();

        StartCoroutine(SetupBattle());
    }

    /// <summary>
    /// Setup the battle by setting up the player and enemy units, setting the HUD stats, and
    /// showing a message on the dialog box
    /// </summary>
    public IEnumerator SetupBattle()
    {
        playerUnit.Clear();
        enemyUnit.Clear();
        currentMoveIndex = 0;

        if (!isTrainerBattle)
        {
            // Wild battle
            playerUnit.Setup(playerParty.GetHealthyPokemon());
            enemyUnit.Setup(wildPokemon);

            dialogBox.SetMoveNames(playerUnit.Pokemon.Moves);
            yield return dialogBox.TypeDialog($"A wild {enemyUnit.Pokemon.Base.Name} appeared");
        }
        else
        {
            // Trainer battle
            playerUnit.gameObject.SetActive(false);
            enemyUnit.gameObject.SetActive(false);

            playerImage.gameObject.SetActive(true);
            trainerImage.gameObject.SetActive(true);

            playerImage.sprite = player.Sprite;
            trainerImage.sprite = trainer.Sprite;

            yield return dialogBox.TypeDialog($"{trainer.Name} wants to battle!");

            // Send out trainer pokemon
            trainerImage.gameObject.SetActive(false);
            enemyUnit.gameObject.SetActive(true);

            Pokemon enemyPokemon = trainerParty.GetHealthyPokemon();
            enemyUnit.Setup(enemyPokemon);
            yield return dialogBox.TypeDialog($"{trainer.Name} sent out {enemyPokemon.Base.Name}!");

            // Send out player pokemon
            playerImage.gameObject.SetActive(false);
            playerUnit.gameObject.SetActive(true);

            Pokemon playerPokemon = playerParty.GetHealthyPokemon();
            playerUnit.Setup(playerPokemon);
            yield return dialogBox.TypeDialog($"Go {playerPokemon.Base.Name}!");

            dialogBox.SetMoveNames(playerUnit.Pokemon.Moves);
        }

        partyScreen.Init();
        ActionSelection();
    }

    #endregion

    #region Handlers

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
        else if (state == BattleState.AboutToUse)
        {
            HandleAboutToUse();
        }
        else if (state == BattleState.MoveToForget)
        {
            Action<int> onMoveSelection = (moveIndex) =>
            {
                moveSelectionUI.gameObject.SetActive(false);

                if (moveIndex == PokemonBase.MaxNumOfMoves)
                {
                    StartCoroutine(dialogBox.TypeDialog($"{playerUnit.Pokemon.Base.Name} did not learn {moveToLearn.MoveName}"));
                }
                else
                {
                    MoveBase selectedMove = playerUnit.Pokemon.Moves[moveIndex].Base;
                    StartCoroutine(dialogBox.TypeDialog(
                        $"{playerUnit.Pokemon.Base.Name} forgot {selectedMove.MoveName} and learned {moveToLearn.MoveName}"
                    ));

                    playerUnit.Pokemon.Moves[moveIndex] = new Move(moveToLearn);
                }

                moveToLearn = null;
                state = BattleState.RunningTurn;
            };

            moveSelectionUI.HandleMoveSelection(onMoveSelection);
        }
        else if (state == BattleState.Bag)
        {
            void onBack()
            {
                inventoryUI.gameObject.SetActive(false);
                state = BattleState.ActionSelection;
            }

            void onItemUsed()
            {
                state = BattleState.Busy;
                inventoryUI.gameObject.SetActive(false);
                StartCoroutine(RunTurns(BattleAction.UseItem));
            }

            inventoryUI.HandleUpdate(onBack, onItemUsed);
        }
    }

    private void HandleAboutToUse()
    {
        if (Input.GetKeyDown(KeyCode.DownArrow) || Input.GetKeyDown(KeyCode.UpArrow))
            aboutToUseChoice = !aboutToUseChoice;

        dialogBox.UpdateChoiceBox(aboutToUseChoice);

        if (Input.GetKeyDown(KeyCode.Return))
        {
            dialogBox.SetChoiceBoxActive(false);
            if (aboutToUseChoice)
            {
                // Yes
                OpenPartyScreen();
            }
            else
            {
                // No
                StartCoroutine(SendNextTrainerPokemon());
            }
        }
    }

    private void HandlePartyScreenSelection()
    {
        void OnSelected()
        {
            Pokemon selectedPartyMember = partyScreen.SelectedMember;

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

            if (partyScreen.CalledFrom == BattleState.ActionSelection)
            {
                StartCoroutine(RunTurns(BattleAction.SwitchPokemon));
            }
            else
            {
                state = BattleState.Busy;
                bool isTrainerAboutToUse = partyScreen.CalledFrom == BattleState.AboutToUse;
                StartCoroutine(SwitchPokemon(selectedPartyMember, isTrainerAboutToUse));
            }

            partyScreen.CalledFrom = null;
        }

        void OnBack()
        {
            if (playerUnit.Pokemon.HP <= 0)
            {
                partyScreen.SetMessageText("You have to choose a pokemon to continue!");
                return;
            }

            partyScreen.gameObject.SetActive(false);

            if (partyScreen.CalledFrom == BattleState.AboutToUse)
            {
                StartCoroutine(SendNextTrainerPokemon());
            }
            else
                ActionSelection();

            partyScreen.CalledFrom = null;
        }

        partyScreen.HandleUpdate(OnSelected, OnBack);
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
            Move move = playerUnit.Pokemon.Moves[currentMoveIndex];
            if (move.PP == 0) return;

            dialogBox.SetMoveSelectorActive(false);
            dialogBox.SetDialogTextActive(true);
            StartCoroutine(RunTurns(BattleAction.Move));
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
                OpenBag();
            }
            else if (currentActionIndex == 2)
            {
                // Choose Pokemon
                OpenPartyScreen();
            }
            else if (currentActionIndex == 3)
            {
                StartCoroutine(RunTurns(BattleAction.Run));
            }
        }
    }

    #endregion

    #region Selection

    private void OpenBag()
    {
        state = BattleState.Bag;
        inventoryUI.gameObject.SetActive(true);
    }

    private void OpenPartyScreen()
    {
        partyScreen.CalledFrom = state;
        state = BattleState.PartyScreen;
        partyScreen.gameObject.SetActive(true);
    }

    private void ActionSelection()
    {
        state = BattleState.ActionSelection;
        dialogBox.SetDialog("Choose an action");
        dialogBox.SetActionSelectorActive(true);
    }

    private void MoveSelection()
    {
        state = BattleState.MoveSelection;
        dialogBox.SetActionSelectorActive(false);
        dialogBox.SetDialogTextActive(false);
        dialogBox.SetMoveSelectorActive(true);
    }

    private IEnumerator ChooseMoveToForget(Pokemon pokemon, MoveBase newMove)
    {
        state = BattleState.Busy;
        yield return dialogBox.TypeDialog($"Choose a move you want to forget");
        moveToLearn = newMove;

        moveSelectionUI.gameObject.SetActive(true);
        moveSelectionUI.SetMoveData(pokemon.Moves.Select(x => x.Base).ToList(), newMove);

        state = BattleState.MoveToForget;
    }

    #endregion

    #region Events

    private IEnumerator AboutToUse(Pokemon newPokemon)
    {
        state = BattleState.Busy;
        yield return dialogBox.TypeDialog($"{trainer.Name} is about to use {newPokemon.Base.Name}. Do you want to switch pokemon?");

        state = BattleState.AboutToUse;
        dialogBox.SetChoiceBoxActive(true);
    }

    #endregion

    #region Checks

    private void BattleOver(bool won)
    {
        state = BattleState.BattleOver;
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
        {
            if (!isTrainerBattle)
            {
                BattleOver(true);
            }
            else
            {
                Pokemon nextPokemon = trainerParty.GetHealthyPokemon();
                if (nextPokemon != null)
                {
                    StartCoroutine(AboutToUse(nextPokemon));
                }
                else
                {
                    BattleOver(true);
                }
            }
        }
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

    #endregion

    #region UI

    private IEnumerator ShowStatusChanges(Pokemon pokemon)
    {
        while (pokemon.StatusChanges.Count > 0)
        {
            string message = pokemon.StatusChanges.Dequeue();
            yield return dialogBox.TypeDialog(message);
        }
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

    #endregion

    #region Runners

    private IEnumerator RunMove(BattleUnit source, BattleUnit target, Move move)
    {
        bool canRunMove = source.Pokemon.OnBeforeTurn();
        if (!canRunMove)
        {
            yield return ShowStatusChanges(source.Pokemon);
            yield return source.Hud.WaitForHPUpdate();
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
                yield return target.Hud.WaitForHPUpdate();
                yield return ShowDamageDetails(damageDetails);
            }

            if (target.Pokemon.HP <= 0)
            {
                yield return HandlePokemonFainted(target);
            }
        }
        else
        {
            yield return dialogBox.TypeDialog($"{source.Pokemon.Base.Name}'s attack missed!");
        }

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
    }

    private IEnumerator RunAfterTurn(BattleUnit source)
    {
        if (state == BattleState.BattleOver) yield break;
        yield return new WaitUntil(() => state == BattleState.RunningTurn);

        source.Pokemon.OnAfterTurn();
        yield return ShowStatusChanges(source.Pokemon);
        yield return source.Hud.WaitForHPUpdate();

        // Since statuses like poison and burn effect the HP of the pokemon
        // it has the potential to faint because of the effect
        // so this if statement checks if that is the case and handles it accordingly
        if (source.Pokemon.HP <= 0)
        {
            yield return HandlePokemonFainted(source);
            yield return new WaitUntil(() => state == BattleState.RunningTurn);
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

    private IEnumerator RunTurns(BattleAction playerAction)
    {
        state = BattleState.RunningTurn;

        if (playerAction == BattleAction.Move)
        {
            playerUnit.Pokemon.CurrentMove = playerUnit.Pokemon.Moves[currentMoveIndex];
            enemyUnit.Pokemon.CurrentMove = enemyUnit.Pokemon.GetRandomMove();

            int playerMovePriority = playerUnit.Pokemon.CurrentMove.Base.Priority;
            int enemyMovePriority = enemyUnit.Pokemon.CurrentMove.Base.Priority;

            // Check to see who goes first

            bool playerGoesFirst = true;

            if (enemyMovePriority > playerMovePriority)
                playerGoesFirst = false;
            else if (enemyMovePriority == playerMovePriority)
                playerGoesFirst = playerUnit.Pokemon.Speed >= enemyUnit.Pokemon.Speed;

            BattleUnit firstUnit = playerGoesFirst ? playerUnit : enemyUnit;
            BattleUnit secondUnit = playerGoesFirst ? enemyUnit : playerUnit;

            Pokemon secondPokemon = secondUnit.Pokemon;

            // First turn
            yield return RunMove(firstUnit, secondUnit, firstUnit.Pokemon.CurrentMove);
            yield return RunAfterTurn(firstUnit);
            if (state == BattleState.BattleOver) yield break;

            if (secondPokemon.HP > 0)
            {
                // Second turn
                yield return RunMove(secondUnit, firstUnit, secondUnit.Pokemon.CurrentMove);
                yield return RunAfterTurn(secondUnit);
                if (state == BattleState.BattleOver) yield break;
            }
        }
        else
        {
            switch (playerAction)
            {
                case BattleAction.SwitchPokemon:
                    Pokemon selectedPokemon = partyScreen.SelectedMember;
                    state = BattleState.Busy;
                    yield return SwitchPokemon(selectedPokemon);
                    break;
                case BattleAction.UseItem:
                    // This is handled from the item screen, so do nothing and skip to the enemy's turn
                    dialogBox.SetActionSelectorActive(false);
                    break;
                case BattleAction.Run:
                    yield return TryToEscape();
                    break;
            }

            // Enemy move
            Move enemyMove = enemyUnit.Pokemon.GetRandomMove();

            yield return RunMove(enemyUnit, playerUnit, enemyMove);
            yield return RunAfterTurn(enemyUnit);

            if (state == BattleState.BattleOver) yield break;
        }

        if (state != BattleState.BattleOver) ActionSelection();
    }

    #endregion

    #region Pokemon

    private IEnumerator HandlePokemonFainted(BattleUnit faintedUnit)
    {
        yield return dialogBox.TypeDialog($"{faintedUnit.Pokemon.Base.Name} Fainted");
        faintedUnit.PlayFaintAnimation();

        yield return new WaitForSeconds(1.5f);

        if (!faintedUnit.IsPlayerUnit)
        {
            int expYield = faintedUnit.Pokemon.Base.ExpYield;
            int enemyLevel = faintedUnit.Pokemon.Level;
            float trainerBonus = isTrainerBattle ? 1.5f : 1f;

            int expGain = Mathf.FloorToInt(expYield * enemyLevel * trainerBonus / 7);
            playerUnit.Pokemon.Exp += expGain;

            yield return dialogBox.TypeDialog($"{playerUnit.Pokemon.Base.Name} gained {expGain} exp!");
            yield return playerUnit.Hud.SetExpSmooth();

            yield return new WaitForSeconds(1f);

            // "while" not "if" to handle multiple level ups at once
            while (playerUnit.Pokemon.CheckForLevelUp())
            {
                playerUnit.Hud.SetLevel();
                yield return dialogBox.TypeDialog($"{playerUnit.Pokemon.Base.Name} grew to level {playerUnit.Pokemon.Level}!");

                // Try to learn new move
                LearnableMove newMove = playerUnit.Pokemon.GetLearnableMoveAtCurrentLevel();

                if (newMove != null)
                {
                    if (playerUnit.Pokemon.Moves.Count < PokemonBase.MaxNumOfMoves)
                    {
                        playerUnit.Pokemon.LearnMove(newMove);
                        yield return dialogBox.TypeDialog($"{playerUnit.Pokemon.Base.Name} learned {newMove.MoveBase.MoveName}!");
                        dialogBox.SetMoveNames(playerUnit.Pokemon.Moves);
                    }
                    else
                    {
                        yield return dialogBox.TypeDialog($"{playerUnit.Pokemon.Base.Name} is trying to learn {newMove.MoveBase.MoveName}");
                        yield return dialogBox.TypeDialog($"But it cannot learn more than {PokemonBase.MaxNumOfMoves} moves!");
                        yield return ChooseMoveToForget(playerUnit.Pokemon, newMove.MoveBase);
                        yield return new WaitUntil(() => state != BattleState.MoveToForget);
                        yield return new WaitForSeconds(2f);
                    }
                }

                yield return playerUnit.Hud.SetExpSmooth(true);
            }

            yield return new WaitForSeconds(1f);
        }

        CheckForBattleOver(faintedUnit);
    }
    private IEnumerator ThrowPokeball()
    {
        state = BattleState.Busy;

        if (isTrainerBattle)
        {

            yield return dialogBox.TypeDialog("You can't catch the trainer's pokemon!");
            state = BattleState.RunningTurn;
            yield break;
        }

        yield return dialogBox.TypeDialog($"{player.Name} used Pokeball!");

        GameObject pokeballObject = Instantiate(pokeball, playerUnit.transform.position - new Vector3(2, 0), Quaternion.identity);
        SpriteRenderer pokeballSprite = pokeballObject.GetComponent<SpriteRenderer>();

        // Animations

        yield return pokeballSprite.transform.DOJump(enemyUnit.transform.position + new Vector3(0, 2), 2f, 1, 1f).WaitForCompletion();
        yield return enemyUnit.PlayCaptureAnimation();
        yield return pokeballSprite.transform.DOMoveY(enemyUnit.transform.position.y - 1.3f, 0.5f).WaitForCompletion();

        // Shake
        int shakeCount = GetShakeCount(enemyUnit.Pokemon);

        for (int i = 0; i < Mathf.Min(shakeCount, 3f); ++i)
        {
            yield return new WaitForSeconds(0.5f);
            yield return pokeballSprite.transform.DOPunchRotation(new Vector3(0f, 0f, 10f), 0.8f).WaitForCompletion();
        }

        if (shakeCount == 4)
        {
            // Pokemon caught
            yield return dialogBox.TypeDialog($"{enemyUnit.Pokemon.Base.Name} was caught!");
            yield return new WaitForSeconds(0.5f);

            yield return pokeballSprite.DOFade(0f, 1.5f).WaitForCompletion();
            Destroy(pokeballObject);

            yield return dialogBox.TypeDialog($"{enemyUnit.Pokemon.Base.Name} has been added to your party!");
            yield return new WaitForSeconds(0.5f);

            playerParty.AddPokemon(enemyUnit.Pokemon);
            BattleOver(true);
        }
        else
        {
            // Pokemon broke out
            yield return new WaitForSeconds(1f);
            pokeballSprite.DOFade(0f, 0.2f);
            yield return enemyUnit.PlayBreakoutAnimation();

            if (shakeCount < 2)
                yield return dialogBox.TypeDialog($"{enemyUnit.Pokemon.Base.Name} broke free!");
            else
                yield return dialogBox.TypeDialog($"{enemyUnit.Pokemon.Base.Name} was almost caught!");

            Destroy(pokeballObject);
            state = BattleState.RunningTurn;
        }
    }

    private int GetShakeCount(Pokemon pokemon)
    {
        float a = ((3 * pokemon.MaxHp) - (2 * pokemon.HP)) * pokemon.Base.CatchRate * ConditionsDB.GetStatusBonus(pokemon.Status) / (3 * pokemon.MaxHp);

        if (a >= 255)
            // Pokemon caught
            return 4;

        float b = 1048560 / Mathf.Sqrt(Mathf.Sqrt(16711680 / a));

        int shakeCount = 0;

        while (shakeCount < 4)
        {
            if (UnityEngine.Random.Range(0, 65535) >= b) break;

            ++shakeCount;
        }

        return shakeCount;
    }

    private IEnumerator SwitchPokemon(Pokemon newPokemon, bool isTrainerAboutToUse = false)
    {
        if (playerUnit.Pokemon.HP > 0)
        {
            yield return dialogBox.TypeDialog($"Come back {playerUnit.Pokemon.Base.name}");
            playerUnit.PlayFaintAnimation();
            yield return new WaitForSeconds(1.5f);
        }

        // Makes sure that we don't get an "OutOfRangeIndex" error
        currentMoveIndex = 0;

        playerUnit.Setup(newPokemon);

        dialogBox.SetMoveNames(newPokemon.Moves);

        yield return dialogBox.TypeDialog($"Go {newPokemon.Base.Name}!");

        if (isTrainerAboutToUse)
            StartCoroutine(SendNextTrainerPokemon());
        else
            state = BattleState.RunningTurn;
    }

    private IEnumerator SendNextTrainerPokemon()
    {
        state = BattleState.Busy;

        Pokemon nextPokemon = trainerParty.GetHealthyPokemon();

        enemyUnit.Setup(nextPokemon);
        yield return dialogBox.TypeDialog($"{trainer.Name} send out {nextPokemon.Base.Name}");

        state = BattleState.RunningTurn;
    }

    private IEnumerator TryToEscape()
    {
        state = BattleState.Busy;
        escapeAttempts++;

        if (isTrainerBattle)
        {
            yield return dialogBox.TypeDialog("You can't run from trainer battles!");
            state = BattleState.RunningTurn;
            yield break;
        }

        int playerSpeed = playerUnit.Pokemon.Speed;
        int enemySpeed = enemyUnit.Pokemon.Speed;

        if (enemySpeed < playerSpeed)
        {
            yield return dialogBox.TypeDialog("Safely ran away!");

            playerUnit.PlayFaintAnimation();
            yield return new WaitForSeconds(1.5f);

            BattleOver(true);
        }
        else
        {
            int f = ((playerSpeed * 128 / enemySpeed) + 30) * escapeAttempts;
            f %= 256;

            if (UnityEngine.Random.Range(0, 256) < f)
            {
                yield return dialogBox.TypeDialog("Safely ran away!");
                playerUnit.PlayFaintAnimation();
                BattleOver(true);
            }
            else
            {
                yield return dialogBox.TypeDialog("Failed to escape!");
                state = BattleState.RunningTurn;
            }
        }
    }

    #endregion
}
