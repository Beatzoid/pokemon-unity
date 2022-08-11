using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// The PlayerController manages player logic
/// </summary>
public class PlayerController : MonoBehaviour, ISavable
{
    [SerializeField] private new string name;
    [SerializeField] private Sprite sprite;
    public Character Character { get; private set; }

    private Vector2 input;

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

    public object CaptureState()
    {
        PlayerSaveData saveData = new PlayerSaveData()
        {
            position = new float[] { transform.position.x, transform.position.y },
            pokemon = GetComponent<PokemonParty>().Pokemon.Select(p => p.GetSaveData()).ToList()
        };

        return saveData;
    }

    public void RestoreState(object state)
    {
        PlayerSaveData saveData = (PlayerSaveData)state;

        // Restore position
        float[] pos = saveData.position;
        transform.position = new Vector3(pos[0], pos[1]);

        // Restore party
        GetComponent<PokemonParty>().Pokemon = saveData.pokemon.Select(p => new Pokemon(p)).ToList();
    }

    private void OnMoveOver()
    {
        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position - new Vector3(0, Character.OffsetY), 0.2f, GameLayers.L.TriggerableLayers);

        foreach (Collider2D collider in colliders)
        {
            IPlayerTriggerable triggerable = collider.GetComponent<IPlayerTriggerable>();

            if (triggerable != null)
            {
                triggerable.OnPlayerTriggered(this);
                break;
            }
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

[System.Serializable]
public class PlayerSaveData
{
    public float[] position;
    public List<PokemonSaveData> pokemon;
}