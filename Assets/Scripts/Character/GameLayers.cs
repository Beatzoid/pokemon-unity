using UnityEngine;

public class GameLayers : MonoBehaviour
{
    [SerializeField] private LayerMask solidObjectsLayer;
    [SerializeField] private LayerMask interactablesLayer;
    [SerializeField] private LayerMask grassLayer;
    [SerializeField] private LayerMask playerLayer;
    [SerializeField] private LayerMask fovLayer;
    [SerializeField] private LayerMask portalLayer;

    public static GameLayers L { get; set; }

    public void Awake()
    {
        L = this;
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
