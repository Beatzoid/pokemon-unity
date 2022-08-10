using System.Collections;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public enum DestinationIdentifier { A, B, C, D, E, F, G, H, I, J, K, L }

public class Portal : MonoBehaviour, IPlayerTriggerable
{
    [SerializeField] private int sceneToLoad = -1;
    [SerializeField] private Transform spawnPoint;
    [SerializeField] private DestinationIdentifier destinationPortal;

    private PlayerController player;

    public void OnPlayerTriggered(PlayerController player)
    {
        this.player = player;
        Debug.Log("Player entered the portal");
        StartCoroutine(SwitchScene());
    }

    private IEnumerator SwitchScene()
    {
        DontDestroyOnLoad(gameObject);
        GameController.instance.PauseGame(true);
        yield return SceneManager.LoadSceneAsync(sceneToLoad);

        Portal destPortal = FindObjectsOfType<Portal>().First(x => x != this && x.destinationPortal == this.destinationPortal);
        player.Character.SetPositionAndSnapToTile(destPortal.SpawnPoint.position);

        GameController.instance.PauseGame(false);

        Destroy(gameObject);
    }

    public Transform SpawnPoint => spawnPoint;
}
