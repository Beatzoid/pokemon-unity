using System.Collections;
using UnityEngine;

/// <summary>
/// The TrainerController class manages all trainer-related logic
/// </summary>
public class TrainerController : MonoBehaviour, Interactable, ISavable
{
    [SerializeField] private new string name;
    [SerializeField] private Sprite sprite;
    [SerializeField] private Dialog dialog;
    [SerializeField] private Dialog dialogAfterBattle;
    [SerializeField] private GameObject exclamation;
    [SerializeField] private GameObject fov;

    /// <summary> The Sprite of the trainer </summary>
    public Sprite Sprite
    {
        get => sprite;
    }

    /// <summary>The Name of the trainer </summary>
    public string Name
    {
        get => name;
    }

    private bool battleLost = false;

    private Character character;

    public void Awake()
    {
        character = GetComponent<Character>();
    }

    public void Start()
    {
        SetFovRotation(character.Animator.DefaultDirection);
    }

    public void Update()
    {
        character.HandleUpdate();
    }

    /// <summary>
    /// Initiates the trainer battle
    /// </summary>
    /// <param name="Transform">The transform of the object that is initiating the battle </param>
    public IEnumerator Interact(Transform initiator)
    {
        character.LookTowards(initiator.position);

        if (!battleLost)
        {
            yield return DialogManager.Instance.ShowDialog(dialog);
            GameController.Instance.StartTrainerBattle(this);
        }
        else
            yield return DialogManager.Instance.ShowDialog(dialogAfterBattle);
    }

    /// <summary>
    /// Called when the battle is lost
    /// </summary>
    public void BattleLost()
    {
        battleLost = true;
        fov.SetActive(false);
    }

    /// <summary>
    /// Trigger a trainer battle
    /// </summary>
    /// <param name="PlayerController">The player controller </param>
    public IEnumerator TriggerTrainerBattle(PlayerController player)
    {
        // Show exclamation
        exclamation.SetActive(true);
        yield return new WaitForSeconds(0.5f);
        exclamation.SetActive(false);

        // Walk towards the player
        Vector3 diff = player.transform.position - transform.position;

        Vector3 moveVec = diff - diff.normalized;
        moveVec = new Vector2(Mathf.Round(moveVec.x), Mathf.Round(moveVec.y));

        yield return character.Move(moveVec);
        player.Character.LookTowards(transform.position);

        yield return DialogManager.Instance.ShowDialog(dialog);
        GameController.Instance.StartTrainerBattle(this);
    }

    /// <summary>
    /// Set the fov collider rotation
    /// </summary>
    /// <param name="FacingDirection">The direction to face the collider in </param>
    public void SetFovRotation(FacingDirection dir)
    {
        float angle = 0f;

        switch (dir)
        {
            case FacingDirection.Right:
                angle = 90f;
                break;
            case FacingDirection.Left:
                angle = 270f;
                break;
            case FacingDirection.Up:
                angle = 180f;
                break;
        }

        fov.transform.eulerAngles = new Vector3(0f, 0f, angle);
    }

    public object CaptureState()
    {
        return battleLost;
    }

    public void RestoreState(object state)
    {
        battleLost = (bool)state;

        if (battleLost)
            fov.SetActive(false);
    }
}
