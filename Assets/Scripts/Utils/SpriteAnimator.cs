using System.Collections.Generic;
using UnityEngine;

public class SpriteAnimator
{
    private SpriteRenderer spriteRenderer;
    private List<Sprite> frames;
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
        this.frames = frames;
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
        spriteRenderer.sprite = frames[1];
    }

    /// <summary>
    /// Ran every frame
    /// </summary>
    public void HandleUpdate()
    {
        timer += Time.deltaTime;

        if (timer > frameRate)
        {
            currentFrame = (currentFrame + 1) % frames.Count;
            spriteRenderer.sprite = frames[currentFrame];
            timer -= frameRate;
        }
    }

    public List<Sprite> Frames
    {
        get { return frames; }
    }
}
