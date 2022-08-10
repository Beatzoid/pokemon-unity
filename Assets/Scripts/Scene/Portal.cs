using System.Collections;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Portal : MonoBehaviour, IPlayerTriggerable
{
    [SerializeField] private int sceneToLoad = -1;
    [SerializeField] private Transform spawnPoint;

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
        yield return SceneManager.LoadSceneAsync(sceneToLoad);

        Portal destPortal = FindObjectsOfType<Portal>().First(x => x != this);
        player.Character.SetPositionAndSnapToTile(destPortal.SpawnPoint.position);

        Destroy(gameObject);
    }

    public Transform SpawnPoint => spawnPoint;
}
