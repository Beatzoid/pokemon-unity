using UnityEngine;

public enum GameState { FreeRoam, Battle, Dialog, Cutscene }

/// <summary>
/// The GameController class manages all core game logic
/// </summary>
public class GameController : MonoBehaviour
{
    [SerializeField] private PlayerController playerController;
    [SerializeField] private BattleSystem battleSystem;
    [SerializeField] private Camera worldCamera;

    public static GameController instance { get; private set; }

    private GameState state;
    private TrainerController trainer;

    public void Awake()
    {
        instance = this;
        ConditionsDB.Init();
    }

    public void Start()
    {
        playerController.OnEncounter += StartBattle;
        battleSystem.OnBattleOver += EndBattle;

        playerController.OnEnterTrainersView += (Collider2D fovCollider) =>
        {
            TrainerController trainer = fovCollider.GetComponentInParent<TrainerController>();

            if (trainer != null)
            {
                state = GameState.Cutscene;

                StartCoroutine(trainer.TriggerTrainerBattle(playerController));
            }
        };

        DialogManager.Instance.OnShowDialog += () =>
        {
            state = GameState.Dialog;
        };

        DialogManager.Instance.OnCloseDialog += () =>
        {
            if (state == GameState.Dialog)
                state = GameState.FreeRoam;
        };
    }

    public void Update()
    {
        if (state == GameState.FreeRoam)
        {
            playerController.HandleUpdate();
        }
        else if (state == GameState.Battle)
        {
            battleSystem.HandleUpdate();
        }
        else if (state == GameState.Dialog)
        {
            DialogManager.Instance.HandleUpdate();
        }
    }

    public void StartTrainerBattle(TrainerController trainer)
    {
        state = GameState.Battle;
        this.trainer = trainer;

        battleSystem.gameObject.SetActive(true);
        worldCamera.gameObject.SetActive(false);

        PokemonParty playerParty = playerController.GetComponent<PokemonParty>();
        PokemonParty trainerParty = trainer.GetComponent<PokemonParty>();

        battleSystem.StartTrainerBattle(playerParty, trainerParty);
    }

    private void EndBattle(bool won)
    {
        state = GameState.FreeRoam;

        if (trainer != null && won == true)
        {
            trainer.BattleLost();
            trainer = null;
        }

        battleSystem.gameObject.SetActive(false);
        worldCamera.gameObject.SetActive(true);
    }

    private void StartBattle()
    {
        state = GameState.Battle;

        battleSystem.gameObject.SetActive(true);
        worldCamera.gameObject.SetActive(false);

        PokemonParty playerParty = playerController.GetComponent<PokemonParty>();
        Pokemon wildPokemon = FindObjectOfType<MapArea>().GetComponent<MapArea>().GetRandomWildPokemon();

        battleSystem.StartBattle(playerParty, wildPokemon);
    }
}
