using System.Collections.Generic;
using System.Collections;
using UnityEngine;

/// <summary> The NPCState is used to keep track of the current state of the NPC </summary>
public enum NPCState { Idle, Walking, Dialog }

/// <summary>
/// The NPCController class manages all NPC-related logic
/// </summary>
public class NPCController : MonoBehaviour, Interactable
{
    [SerializeField] private Dialog dialog;
    [SerializeField] private List<Vector2> movementPatterns;
    [SerializeField] private float timeBetweenPattern;

    private NPCState state;
    private float idleTimer = 0;
    private int currentMovementPatternIndex = 0;

    private Character character;

    public void Awake()
    {
        character = GetComponent<Character>();
    }

    public void Update()
    {
        if (state == NPCState.Idle)
        {
            idleTimer += Time.deltaTime;
            if (idleTimer > timeBetweenPattern)
            {
                idleTimer = 0f;
                if (movementPatterns.Count > 0)
                    StartCoroutine(Walk());
            }
        }

        character.HandleUpdate();
    }

    /// <summary>
    /// Interact with the NPC (set the state, look towards the initiator, and start the dialog sequence)
    /// </summary>
    /// <param name="initiator">The transform of the character to interact with </param>
    public IEnumerator Interact(Transform initiator)
    {
        if (state == NPCState.Idle)
        {
            state = NPCState.Dialog;
            character.LookTowards(initiator.position);

            yield return DialogManager.Instance.ShowDialog(dialog);

            idleTimer = 0f;
            state = NPCState.Idle;
        }
    }

    private IEnumerator Walk()
    {
        state = NPCState.Walking;

        Vector3 oldPos = transform.position;

        yield return character.Move(movementPatterns[currentMovementPatternIndex]);

        if (transform.position != oldPos)
            currentMovementPatternIndex = (currentMovementPatternIndex + 1) % movementPatterns.Count;

        state = NPCState.Idle;
    }
}
