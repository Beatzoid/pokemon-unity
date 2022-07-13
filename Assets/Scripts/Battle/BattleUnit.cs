using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class BattleUnit : MonoBehaviour
{
    [SerializeField] private bool isPlayerUnit;

    [SerializeField] private BattleHud hud;


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

        hud.SetData(pokemon);

        image.color = originalColor;

        PlayEnterAnimation();
    }

    /// <summary>
    /// Play the enter animation
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
    /// Play the attack animation
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
    /// Play the hit animation
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
    /// Play the faint animation
    /// </summary>
    public void PlayFaintAnimation()
    {
        Sequence sequence = DOTween.Sequence();

        sequence.Append(image.transform.DOLocalMoveY(originalPos.y - 150f, 2f));
        sequence.Join(image.DOFade(0f, 1f));
    }

    public Pokemon Pokemon { get; set; }

    public bool IsPlayerUnit
    {
        get { return isPlayerUnit; }
    }

    public BattleHud Hud
    {
        get { return hud; }
    }
}
