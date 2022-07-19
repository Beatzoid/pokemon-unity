using UnityEngine;

public class GameLayers : MonoBehaviour
{
    [SerializeField] private LayerMask solidObjectsLayer;
    [SerializeField] private LayerMask interactablesLayer;
    [SerializeField] private LayerMask grassLayer;

    public static GameLayers L { get; set; }

    public void Awake()
    {
        L = this;
    }

    public LayerMask SolidObjectsLayer
    {
        get => solidObjectsLayer;
    }

    public LayerMask InteractablesLayer
    {
        get => interactablesLayer;
    }

    public LayerMask GrassLayer
    {
        get => grassLayer;
    }
}