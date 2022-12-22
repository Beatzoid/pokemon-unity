using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using System.Collections;

/// <summary>
/// The BattleUnit class manages a unit (pokemon) in the battle scene
/// </summary>
public class BattleUnit : MonoBehaviour
{
    [SerializeField] private bool isPlayerUnit;

    [SerializeField] private BattleHud hud;

    /// <summary> The pokemon attached to the BattleUnit </summary>
    public Pokemon Pokemon { get; set; }

    /// <summary> Whether or not this unit is a player unit </summary>
    public bool IsPlayerUnit
    {
        get { return isPlayerUnit; }
    }

    /// <summary> The HUD of this unit </summary>
    public BattleHud Hud
    {
        get { return hud; }
    }

    private Image image;
    private Vector3 originalPos;
    private Color originalColor;

    public void Awake()
    {
        image = GetComponent<Image>();

        originalPos = image.transform.localPosition;
        originalColor = image.color;
    }

    /// <summary>
    /// Setup the battle unit
    /// </summary>
    public void Setup(Pokemon pokemon)
    {
        image = GetComponent<Image>();
        Pokemon = pokemon;

        if (isPlayerUnit)
            image.sprite = Pokemon.Base.BackSprite;
        else
            image.sprite = Pokemon.Base.FrontSprite;

        hud.gameObject.SetActive(true);
        hud.SetData(pokemon);

        transform.localScale = new Vector3(1, 1, 1);
        image.color = originalColor;

        PlayEnterAnimation();
    }

    /// <summary>
    /// Play the enter animation (moves the unit from offscreen into the scene smoothly animated using DoTween)
    /// </summary>
    public void PlayEnterAnimation()
    {
        if (isPlayerUnit)
            image.transform.localPosition = new Vector3(-500f, originalPos.y);
        else
            image.transform.localPosition = new Vector3(500f, originalPos.y);

        image.transform.DOLocalMove(originalPos, 1f);
    }

    /// <summary>
    /// Play the attack animation (moves the unit forward and back a small amount smoothly animated suing DoTween)
    /// </summary>
    public void PlayAttackAnimation()
    {
        Sequence sequence = DOTween.Sequence();

        if (isPlayerUnit)
            sequence.Append(image.transform.DOLocalMoveX(originalPos.x + 50f, 0.25f));
        else
            sequence.Append(image.transform.DOLocalMoveX(originalPos.x - 50f, 0.25f));

        sequence.Append(image.transform.DOLocalMoveX(originalPos.x, 0.25f));
    }

    /// <summary>
    /// Play the hit animation (moves the unit back and forward a small amount smoothly animated suing DoTween)
    /// </summary>
    public void PlayHitAnimation()
    {
        Sequence sequence = DOTween.Sequence();
        // Color color = new(255, 93, 71);

        sequence.Append(image.DOColor(Color.gray, 0.1f));
        sequence.Append(image.transform.DOShakePosition(0.5f, 7f));
        sequence.Append(image.DOColor(originalColor, 0.1f));
    }

    /// <summary>
    /// Play the faint animation (fades the unit and moves them down smoothly animated suing DoTween)
    /// </summary>
    public void PlayFaintAnimation()
    {
        Sequence sequence = DOTween.Sequence();

        sequence.Append(image.transform.DOLocalMoveY(originalPos.y - 150f, 2f));
        sequence.Join(image.DOFade(0f, 1f));
    }

    /// <summary>
    /// Play the capture animation (smoothly animated using DoTween)
    /// </summary>
    public IEnumerator PlayCaptureAnimation()
    {
        Sequence sequence = DOTween.Sequence();

        sequence.Append(image.DOFade(0, 0.5f));
        sequence.Join(transform.DOLocalMoveY(originalPos.y + 50f, 0.5f));
        sequence.Join(transform.DOScale(new Vector3(0.3f, 0.3f, 1f), 0.5f));

        yield return sequence.WaitForCompletion();
    }

    /// <summary>
    /// Play the breakout animation (smoothly animated using DoTween)
    /// </summary>
    public IEnumerator PlayBreakoutAnimation()
    {
        Sequence sequence = DOTween.Sequence();

        sequence.Append(image.DOFade(1, 0.5f));
        sequence.Join(transform.DOLocalMoveY(originalPos.y, 0.5f));
        sequence.Join(transform.DOScale(new Vector3(1f, 1f, 1f), 0.5f));

        yield return sequence.WaitForCompletion();
    }

    /// <summary>
    /// Clear the BattleUnit (disables the HUD of the unit)
    /// </summary>
    public void Clear()
    {
        hud.gameObject.SetActive(false);
    }
}
