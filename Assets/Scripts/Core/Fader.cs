using System.Collections;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// The Fader class manages the fading effects in the game
/// </summary>
public class Fader : MonoBehaviour
{
    private Image image;

    public void Awake()
    {
        image = GetComponent<Image>();
    }

    public IEnumerator FadeIn(float time)
    {
        yield return image.DOFade(1f, time).WaitForCompletion();
    }

    public IEnumerator FadeOut(float time)
    {
        yield return image.DOFade(0f, time).WaitForCompletion();
    }
}
