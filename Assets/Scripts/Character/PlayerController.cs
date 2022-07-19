using System;
using UnityEngine;

/// <summary>
/// The PlayerController manages player logic
/// </summary>
public class PlayerController : MonoBehaviour
{
    public event Action OnEncounter;

    private Vector2 input;

    private Character character;

    public void Awake()
    {
        character = GetComponent<Character>();
    }

    public void HandleUpdate()
    {
        if (!character.IsMoving)
        {
            input.x = Input.GetAxisRaw("Horizontal");
            input.y = Input.GetAxisRaw("Vertical");

            // Removes diagonal movement
            if (input.x != 0) input.y = 0;

            if (input != Vector2.zero)
            {
                StartCoroutine(character.Move(input, CheckForEncounters));
            }
        }

        character.HandleUpdate();

        if (Input.GetKeyDown(KeyCode.Return)) Interact();
    }

    private void CheckForEncounters()
    {
        // If touching a sprite with the grass layer
        if (Physics2D.OverlapCircle(transform.position, 0.2f, GameLayers.L.GrassLayer) != null)
        {
            // 10% chance to encounter a wild pokemon
            if (UnityEngine.Random.Range(1, 101) <= 10)
            {
                character.Animator.IsMoving = false;
                OnEncounter();
            }
        }
    }

    private void Interact()
    {
        Vector3 facingDir = new Vector3(character.Animator.MoveX, character.Animator.MoveY);
        // Position of the tile that the player is facing
        Vector3 interactPos = transform.position + facingDir;

        // Debug.DrawLine(transform.position, interactPos, Color.red, 0.5f);

        Collider2D collider = Physics2D.OverlapCircle(interactPos, 0.3f, GameLayers.L.InteractablesLayer);
        if (collider != null)
        {
            collider.GetComponent<Interactable>()?.Interact(transform);
        }
    }
}
