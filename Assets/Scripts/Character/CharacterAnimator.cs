using System.Collections.Generic;
using UnityEngine;

public enum FacingDirection { Up, Down, Left, Right }

/// <summary>
/// The CharacterAnimator class manages all character animations
/// </summary>
public class CharacterAnimator : MonoBehaviour
{
    [SerializeField] private List<Sprite> walkDownSprites;
    [SerializeField] private List<Sprite> walkUpSprites;
    [SerializeField] private List<Sprite> walkLeftSprites;
    [SerializeField] private List<Sprite> walkRightSprites;
    [SerializeField] private FacingDirection defaultDirection = FacingDirection.Down;

    /// <summary> The current moveX of this animator </summary>
    public float MoveX { get; set; }
    /// <summary> The current moveY of this animator </summary>
    public float MoveY { get; set; }
    /// <summary> Whether or not the animator is currently in the moving state </summary>
    public bool IsMoving { get; set; }

    /// <summary> The default direction of the animator </summary>
    public FacingDirection DefaultDirection
    {
        get => defaultDirection;
    }

    private SpriteAnimator walkDownAnim;
    private SpriteAnimator walkUpAnim;
    private SpriteAnimator walkRightAnim;
    private SpriteAnimator walkLeftAnim;

    private SpriteRenderer spriteRenderer;
    private SpriteAnimator currentAnim;
    private bool wasPrevMoving;

    public void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();

        walkDownAnim = new SpriteAnimator(walkDownSprites, spriteRenderer);
        walkUpAnim = new SpriteAnimator(walkUpSprites, spriteRenderer);
        walkRightAnim = new SpriteAnimator(walkRightSprites, spriteRenderer);
        walkLeftAnim = new SpriteAnimator(walkLeftSprites, spriteRenderer);

        SetFacingDirection(defaultDirection);
        currentAnim = walkDownAnim;
    }

    public void Update()
    {
        SpriteAnimator prevAnim = currentAnim;

        if (MoveX == 1)
            currentAnim = walkRightAnim;
        else if (MoveX == -1)
            currentAnim = walkLeftAnim;
        else if (MoveY == 1)
            currentAnim = walkUpAnim;
        else if (MoveY == -1)
            currentAnim = walkDownAnim;

        if (currentAnim != prevAnim || IsMoving != wasPrevMoving) currentAnim.Start();

        if (IsMoving)
            currentAnim.HandleUpdate();
        else
            spriteRenderer.sprite = currentAnim.Frames[0];

        wasPrevMoving = IsMoving;
    }

    /// <summary>
    /// Set the facing direction for the animator
    /// </summary>
    /// <param name="dir">The direction to face </param>
    public void SetFacingDirection(FacingDirection dir)
    {
        switch (dir)
        {
            case FacingDirection.Right:
                MoveX = 1;
                break;
            case FacingDirection.Left:
                MoveX = -1;
                break;
            case FacingDirection.Down:
                MoveY = 1;
                break;
            case FacingDirection.Up:
                MoveY = 1;
                break;
        }
    }
}
