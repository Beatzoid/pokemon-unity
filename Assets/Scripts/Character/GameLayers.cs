using UnityEngine;

/// <summary>
/// The game layers class manages all the layers in the game
/// </summary>
public class GameLayers : MonoBehaviour
{
    [SerializeField] private LayerMask solidObjectsLayer;
    [SerializeField] private LayerMask interactablesLayer;
    [SerializeField] private LayerMask grassLayer;
    [SerializeField] private LayerMask playerLayer;
    [SerializeField] private LayerMask fovLayer;
    [SerializeField] private LayerMask portalLayer;

    public static GameLayers Instance { get; set; }

    public void Awake()
    {
        Instance = this;
    }

    public LayerMask SolidObjectsLayer => solidObjectsLayer;

    public LayerMask InteractablesLayer => interactablesLayer;

    public LayerMask GrassLayer => grassLayer;

    public LayerMask PlayerLayer => playerLayer;

    public LayerMask FovLayer => fovLayer;

    public LayerMask PortalLayer => portalLayer;

    public LayerMask TriggerableLayers
    {
        get => grassLayer | fovLayer | portalLayer;
    }
}
