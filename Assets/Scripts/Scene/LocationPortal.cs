using System.Collections;
using System.Linq;
using UnityEngine;

/// <summary>
/// The LocationPortal manages all logic for the location portals
/// </summary>
public class LocationPortal : MonoBehaviour, IPlayerTriggerable
{
    [SerializeField] private Transform spawnPoint;
    [SerializeField] private DestinationIdentifier destinationPortal;

    public Transform SpawnPoint => spawnPoint;

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
        GameController.Instance.PauseGame(true);
        yield return fader.FadeIn(0.5f);

        // TODO: Re-add "x != this"
        LocationPortal destPortal = FindObjectsOfType<LocationPortal>().First(x => x != this && x.destinationPortal == destinationPortal);
        player.Character.SetPositionAndSnapToTile(destPortal.SpawnPoint.position);

        yield return fader.FadeOut(0.5f);
        GameController.Instance.PauseGame(false);
    }
}
