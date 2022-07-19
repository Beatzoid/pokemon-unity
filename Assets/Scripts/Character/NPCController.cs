using System.Collections.Generic;
using System.Collections;
using UnityEngine;

public enum NPCState { Idle, Walking };

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
        if (DialogManager.Instance.IsShowing) return;

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

    public void Interact()
    {
        if (state == NPCState.Idle)
            StartCoroutine(DialogManager.Instance.ShowDialog(dialog));
    }

    private IEnumerator Walk()
    {
        state = NPCState.Walking;

        yield return character.Move(movementPatterns[currentMovementPatternIndex]);
        currentMovementPatternIndex = (currentMovementPatternIndex + 1) % movementPatterns.Count;

        state = NPCState.Idle;
    }
}
