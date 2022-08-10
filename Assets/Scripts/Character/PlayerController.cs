using System;
using UnityEngine;

/// <summary>
/// The PlayerController manages player logic
/// </summary>
public class PlayerController : MonoBehaviour
{
    [SerializeField] private new string name;
    [SerializeField] private Sprite sprite;

    public event Action OnEncounter;
    public event Action<Collider2D> OnEnterTrainersView;
    public Character Character { get; private set; }

    private Vector2 input;
    private const float offsetY = 0.3f;

    public void Awake()
    {
        Character = GetComponent<Character>();
    }

    public void HandleUpdate()
    {
        if (!Character.IsMoving)
        {
            input.x = Input.GetAxisRaw("Horizontal");
            input.y = Input.GetAxisRaw("Vertical");

            // Removes diagonal movement
            if (input.x != 0) input.y = 0;

            if (input != Vector2.zero)
            {
                StartCoroutine(Character.Move(input, OnMoveOver));
            }
        }

        Character.HandleUpdate();

        if (Input.GetKeyDown(KeyCode.Return)) Interact();
    }

    private void OnMoveOver()
    {
        CheckForEncounters();
        CheckIfInTrainersView();
    }

    private void CheckForEncounters()
    {
        // If touching a sprite with the grass layer
        if (Physics2D.OverlapCircle(transform.position - new Vector3(0, offsetY), 0.2f, GameLayers.L.GrassLayer) != null)
        {
            // 10% chance to encounter a wild pokemon
            if (UnityEngine.Random.Range(1, 101) <= 10)
            {
                Character.Animator.IsMoving = false;
                OnEncounter();
            }
        }
    }

    private void CheckIfInTrainersView()
    {
        Collider2D fovCollider = Physics2D.OverlapCircle(transform.position - new Vector3(0, offsetY), 0.2f, GameLayers.L.FovLayer);

        if (fovCollider != null)
        {
            Character.Animator.IsMoving = false;
            OnEnterTrainersView?.Invoke(fovCollider);
        }
    }

    private void Interact()
    {
        Vector3 facingDir = new Vector3(Character.Animator.MoveX, Character.Animator.MoveY);
        // Position of the tile that the player is facing
        Vector3 interactPos = transform.position + facingDir;

        // Debug.DrawLine(transform.position, interactPos, Color.red, 0.5f);

        Collider2D collider = Physics2D.OverlapCircle(interactPos, 0.3f, GameLayers.L.InteractablesLayer);
        if (collider != null)
        {
            collider.GetComponent<Interactable>()?.Interact(transform);
        }
    }

    public Sprite Sprite
    {
        get => sprite;
    }

    public string Name
    {
        get => name;
    }
}
