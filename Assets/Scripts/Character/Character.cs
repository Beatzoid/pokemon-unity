using System;
using System.Collections;
using UnityEngine;

/// <summary>
/// The character class manages all character-related logic
/// </summary>
public class Character : MonoBehaviour
{
    public CharacterAnimator Animator { get; private set; }
    public float moveSpeed;
    public bool IsMoving { get; private set; }

    public void Awake()
    {
        Animator = GetComponent<CharacterAnimator>();
    }

    /// <summary>
    /// Move the character
    /// </summary>
    /// <param name="moveVector">The position to move to </param>
    /// <param name="OnMoveOver">A function to call after the character is done moving </param>
    public IEnumerator Move(Vector2 moveVector, Action OnMoveOver = null)
    {
        Animator.MoveX = Mathf.Clamp(moveVector.x, -1f, 1f);
        Animator.MoveY = Mathf.Clamp(moveVector.y, -1f, 1f);

        Vector3 targetPos = transform.position;
        targetPos.x += moveVector.x;
        targetPos.y += moveVector.y;

        if (!IsPathClear(targetPos)) yield break;

        IsMoving = true;

        // While we are still moving to the target position
        while ((targetPos - transform.position).sqrMagnitude > Mathf.Epsilon)
        {
            // Smoothly move the player in small increments to the target position
            transform.position = Vector3.MoveTowards(transform.position, targetPos, moveSpeed * Time.deltaTime);
            yield return null;
        }

        transform.position = targetPos;

        IsMoving = false;

        OnMoveOver?.Invoke();
    }

    public void HandleUpdate()
    {
        Animator.IsMoving = IsMoving;
    }

    /// <summary>
    /// Make the character look towards the target position
    /// </summary>
    /// <param name="targetPos">The position to look towards </param>
    public void LookTowards(Vector3 targetPos)
    {
        float xDiff = Mathf.Floor(targetPos.x) - Mathf.Floor(transform.position.x);
        float yDiff = Mathf.Floor(targetPos.y) - Mathf.Floor(transform.position.y);

        if (xDiff == 0 || yDiff == 0)
        {
            Animator.MoveX = Mathf.Clamp(xDiff, -1f, 1f);
            Animator.MoveY = Mathf.Clamp(yDiff, -1f, 1f);
        }
        else
            Debug.LogError("Error in LookTowards: You can't ask the character to look diagonally");
    }
    private bool IsPathClear(Vector3 targetPos)
    {
        Vector3 diff = targetPos - transform.position;
        Vector3 direction = diff.normalized;

        if (Physics2D.BoxCast(
            transform.position + direction, // Origin
            new Vector2(0.2f, 0.2f), // Size
            0f, // Angle
            direction,
            diff.magnitude - 1, // Distance
            GameLayers.L.SolidObjectsLayer | GameLayers.L.InteractablesLayer | GameLayers.L.PlayerLayer) == true
        )
            return false;

        return true;
    }

    // private bool IsWalkable(Vector3 targetPos)
    // {
    //     // If touching a sprite with the solidObjects layer
    //     if (Physics2D.OverlapCircle(targetPos, 0.2f, GameLayers.L.SolidObjectsLayer | GameLayers.L.InteractablesLayer) != null)
    //     {
    //         return false;
    //     }
    //     else
    //     {
    //         return true;
    //     }
    // }
}
