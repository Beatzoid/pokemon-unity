using System.Collections.Generic;
using UnityEngine;

public class SpriteAnimator
{
    public List<Sprite> Frames { get; }

    private SpriteRenderer spriteRenderer;
    private float frameRate;
    private int currentFrame;
    private float timer;

    /// <summary>
    /// The SpriteAnimator class handles all sprite animations
    /// </summary>
    /// <param name="frames">The frames of the animation </param>
    /// <param name="spriteRenderer">The SpriteRenderer to render to </param>
    /// <param name="frameRate">The framerate of the animation </param>
    public SpriteAnimator(List<Sprite> frames, SpriteRenderer spriteRenderer, float frameRate = 0.16f)
    {
        Frames = frames;
        this.spriteRenderer = spriteRenderer;
        this.frameRate = frameRate;
    }

    /// <summary>
    /// Start the animation
    /// </summary>
    public void Start()
    {
        currentFrame = 1;
        timer = 0f;
        spriteRenderer.sprite = Frames[1];
    }

    /// <summary>
    /// Ran every frame
    /// </summary>
    public void HandleUpdate()
    {
        timer += Time.deltaTime;

        if (timer > frameRate)
        {
            currentFrame = (currentFrame + 1) % Frames.Count;
            spriteRenderer.sprite = Frames[currentFrame];
            timer -= frameRate;
        }
    }

}
