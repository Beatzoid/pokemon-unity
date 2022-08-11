using System.Collections;
using System.Linq;
using UnityEngine;

// Teleports the player to a different position without switching scenes
public class LocationPortal : MonoBehaviour, IPlayerTriggerable
{
    [SerializeField] private Transform spawnPoint;
    [SerializeField] private DestinationIdentifier destinationPortal;

    private PlayerController player;
    private Fader fader;

    public void Start()
    {
        fader = FindObjectOfType<Fader>();
    }

    public void OnPlayerTriggered(PlayerController player)
    {
        player.Character.Animator.IsMoving = false;
        this.player = player;
        Debug.Log("Player entered the portal");
        StartCoroutine(Teleport());
    }

    private IEnumerator Teleport()
    {
        GameController.instance.PauseGame(true);
        yield return fader.FadeIn(0.5f);

        LocationPortal destPortal = FindObjectsOfType<LocationPortal>().First(x => x != this && x.destinationPortal == destinationPortal);
        player.Character.SetPositionAndSnapToTile(destPortal.SpawnPoint.position);

        yield return fader.FadeOut(0.5f);
        GameController.instance.PauseGame(false);
    }

    public Transform SpawnPoint => spawnPoint;
}
