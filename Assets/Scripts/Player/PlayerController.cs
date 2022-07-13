using System;
using System.Collections;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
	public float moveSpeed;

	public LayerMask solidObjectsLayer;
	public LayerMask grassLayer;

	public event Action OnEncounter;

	private bool isMoving;
	private Vector2 input;

	private Animator animator;

	public void Awake()
	{
		animator = GetComponent<Animator>();
	}

	public void HandleUpdate()
	{
		if (!isMoving)
		{
			input.x = Input.GetAxisRaw("Horizontal");
			input.y = Input.GetAxisRaw("Vertical");

			// Removes diagonal movement
			if (input.x != 0) input.y = 0;

			if (input != Vector2.zero)
			{
				animator.SetFloat("moveX", input.x);
				animator.SetFloat("moveY", input.y);

				Vector3 targetPos = transform.position;
				targetPos.x += input.x;
				targetPos.y += input.y;

				if (IsWalkable(new Vector3(targetPos.x, targetPos.y - 0.5f))) StartCoroutine(Move(targetPos));
			}
		}

		animator.SetBool("isMoving", isMoving);
	}

	private IEnumerator Move(Vector3 targetPos)
	{
		isMoving = true;

		// While we are still moving to the target position
		while ((targetPos - transform.position).sqrMagnitude > Mathf.Epsilon)
		{
			// Smoothly move the player in small increments to the target position
			transform.position = Vector3.MoveTowards(transform.position, targetPos, moveSpeed * Time.deltaTime);
			yield return null;
		}

		transform.position = targetPos;

		isMoving = false;

		CheckForEncounters();
	}

	private void CheckForEncounters()
	{
		// If touching a sprite with the grass layer
		if (Physics2D.OverlapCircle(transform.position, 0.2f, grassLayer) != null)
		{
			// 10% chance to encounter a wild pokemon
			if (UnityEngine.Random.Range(1, 101) <= 10)
			{
				animator.SetBool("isMoving", false);
				OnEncounter();
			}
		}
	}

	private bool IsWalkable(Vector3 targetPos)
	{
		// If touching a sprite with the solidObjects layer
		if (Physics2D.OverlapCircle(targetPos, 0.2f, solidObjectsLayer) != null)
		{
			return false;
		}
		else
		{
			return true;
		}
	}
}
