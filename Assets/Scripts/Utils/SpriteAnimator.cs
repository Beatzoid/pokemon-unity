using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// The SpriteAnimator class manages all sprite animation logic
public class SpriteAnimator
{
    public List<Sprite> Frames { get; }

    private SpriteRenderer spriteRenderer;
    private float frameRate;
    private int currentFrame;
    private float timer;

    /// <summary>
    /// The SpriteAnimator class manages all sprite animation logic
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

    public void Start()
    {
        currentFrame = 1;
        timer = 0f;
        spriteRenderer.sprite = Frames[1];
    }

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
