using UnityEngine;

public enum GameState { FreeRoam, Battle }

/// <summary>
/// The GameController class manages all core game logic
/// </summary>
public class GameController : MonoBehaviour
{
    [SerializeField] private PlayerController playerController;
    [SerializeField] private BattleSystem battleSystem;
    [SerializeField] private Camera worldCamera;

    private GameState state;

    public void Start()
    {
        playerController.OnEncounter += StartBattle;
        battleSystem.OnBattleOver += EndBattle;
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
    }

    private void EndBattle(bool won)
    {
        state = GameState.FreeRoam;

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
