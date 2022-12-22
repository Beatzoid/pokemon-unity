using UnityEngine;

public enum GameState { FreeRoam, Battle, Dialog, Cutscene, Paused, Menu, PartyScreen, Bag }

/// <summary>
/// The GameController class manages all core game logic
/// </summary>
public class GameController : MonoBehaviour
{
    [SerializeField] private PlayerController playerController;
    [SerializeField] private BattleSystem battleSystem;
    [SerializeField] private Camera worldCamera;
    [SerializeField] private PartyScreen partyScreen;
    [SerializeField] private InventoryUI inventoryUI;

    public static GameController Instance { get; private set; }

    public SceneDetails CurrentScene { get; private set; }
    public SceneDetails PrevScene { get; private set; }

    public GameState State { get; private set; }

    public PlayerController Player => playerController;

    private GameState stateBeforePause;
    private TrainerController trainer;
    private MenuController menuController;

    public void Awake()
    {
        Debug.Log(Application.persistentDataPath);
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        Instance = this;
        menuController = GetComponent<MenuController>();
        PokemonDB.Init();
        MoveDB.Init();
        ConditionsDB.Init();
    }

    public void Start()
    {
        partyScreen.Init();

        battleSystem.OnBattleOver += EndBattle;

        DialogManager.Instance.OnShowDialog += () =>
        {
            State = GameState.Dialog;
        };

        DialogManager.Instance.OnCloseDialog += () =>
        {
            if (State == GameState.Dialog)
                State = GameState.FreeRoam;
        };

        menuController.onBack += () =>
        {
            State = GameState.FreeRoam;
        };

        menuController.onMenuSelected += OnMenuSelected;
    }

    public void Update()
    {
        if (State == GameState.FreeRoam)
        {
            playerController.HandleUpdate();

            if (Input.GetKeyDown(KeyCode.Z))
            {
                menuController.OpenMenu();
                State = GameState.Menu;
            }
        }
        else if (State == GameState.Battle)
        {
            battleSystem.HandleUpdate();
        }
        else if (State == GameState.Dialog)
        {
            DialogManager.Instance.HandleUpdate();
        }
        else if (State == GameState.Menu)
        {
            menuController.HandleUpdate();
        }
        else if (State == GameState.PartyScreen)
        {
            void onSelected()
            {
                // TODO: Go to summary screen
            }

            void onBack()
            {
                partyScreen.gameObject.SetActive(false);
                State = GameState.FreeRoam;
            }

            partyScreen.HandleUpdate(onSelected, onBack);
        }
        else if (State == GameState.Bag)
        {
            void onBack()
            {
                inventoryUI.gameObject.SetActive(false);
                State = GameState.FreeRoam;
            }

            inventoryUI.HandleUpdate(onBack);
        }
    }

    public void PauseGame(bool pause)
    {
        if (pause)
        {
            stateBeforePause = State;
            State = GameState.Paused;
        }
        else
        {
            State = stateBeforePause;
        }
    }

    public void StartTrainerBattle(TrainerController trainer)
    {
        State = GameState.Battle;
        this.trainer = trainer;

        battleSystem.gameObject.SetActive(true);
        worldCamera.gameObject.SetActive(false);

        PokemonParty playerParty = playerController.GetComponent<PokemonParty>();
        PokemonParty trainerParty = trainer.GetComponent<PokemonParty>();

        battleSystem.StartTrainerBattle(playerParty, trainerParty);
    }

    public void StartBattle()
    {
        State = GameState.Battle;

        battleSystem.gameObject.SetActive(true);
        worldCamera.gameObject.SetActive(false);

        PokemonParty playerParty = playerController.GetComponent<PokemonParty>();
        Pokemon wildPokemon = CurrentScene.GetComponent<MapArea>().GetRandomWildPokemon();

        // Prevents us from adding the pokemon in the map area
        // to our party
        Pokemon wildPokemonCopy = new Pokemon(wildPokemon.Base, wildPokemon.Level);

        battleSystem.StartBattle(playerParty, wildPokemonCopy);
    }

    public void OnEnterTrainersView(TrainerController trainer)
    {
        State = GameState.Cutscene;
        StartCoroutine(trainer.TriggerTrainerBattle(playerController));
    }

    public void SetCurrentScene(SceneDetails currScene)
    {
        PrevScene = CurrentScene;
        CurrentScene = currScene;
    }

    private void OnMenuSelected(int selectedItem)
    {
        if (selectedItem == 0)
        {
            partyScreen.gameObject.SetActive(true);
            State = GameState.PartyScreen;
        }
        else if (selectedItem == 1)
        {
            inventoryUI.gameObject.SetActive(true);
            State = GameState.Bag;
        }
        else if (selectedItem == 2)
        {
            SavingSystem.i.Save("saveSlot1");
            State = GameState.FreeRoam;
        }
        else if (selectedItem == 3)
        {
            SavingSystem.i.Load("saveSlot1");
            State = GameState.FreeRoam;
        }
    }

    private void EndBattle(bool won)
    {
        State = GameState.FreeRoam;

        if (trainer != null && won == true)
        {
            trainer.BattleLost();
            trainer = null;
        }

        battleSystem.gameObject.SetActive(false);
        worldCamera.gameObject.SetActive(true);
    }
}
