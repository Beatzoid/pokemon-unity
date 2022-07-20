using System.Collections;
using UnityEngine;

public class TrainerController : MonoBehaviour
{
    [SerializeField] private Dialog dialog;
    [SerializeField] private GameObject exclamation;
    [SerializeField] private GameObject fov;

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

        StartCoroutine(DialogManager.Instance.ShowDialog(dialog, () =>
        {
            Debug.Log("Starting trainer battle");
        }));
    }

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
}
