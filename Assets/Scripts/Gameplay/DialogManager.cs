using System;
using System.Collections;
using UnityEngine;
using TMPro;

public class DialogManager : MonoBehaviour
{
    [SerializeField] private GameObject dialogBox;
    [SerializeField] private TextMeshProUGUI dialogText;
    [SerializeField] private int lettersPerSecond;

    public static DialogManager Instance { get; private set; }
    public event Action OnShowDialog;
    public event Action OnCloseDialog;
    public bool IsShowing { get; private set; }

    private int currentLineIndex = 0;
    private Dialog dialog;
    private bool isTyping;
    private Action OnDialogFinished;

    public void Awake()
    {
        Instance = this;
    }

    public void HandleUpdate()
    {
        if (Input.GetKeyDown(KeyCode.Return) && !isTyping)
        {
            ++currentLineIndex;
            if (currentLineIndex < dialog.Lines.Count)
            {
                StartCoroutine(TypeDialog(dialog.Lines[currentLineIndex]));
            }
            else
            {
                currentLineIndex = 0;
                IsShowing = false;
                dialogBox.SetActive(false);

                OnDialogFinished?.Invoke();
                OnCloseDialog?.Invoke();
            }
        }
    }

    /// <summary>
    /// Show dialog in the dialog box
    /// </summary>
    /// <param name="dialog">The dialog to show </param>
    /// <param name="onFinished">A function to run after the dialog is finished </param>
    public IEnumerator ShowDialog(Dialog dialog, Action onFinished = null)
    {
        yield return new WaitForEndOfFrame();

        OnShowDialog.Invoke();

        IsShowing = true;
        this.dialog = dialog;
        OnDialogFinished = onFinished;

        dialogBox.SetActive(true);
        StartCoroutine(TypeDialog(dialog.Lines[0]));
    }

    public IEnumerator TypeDialog(string line)
    {
        isTyping = true;
        dialogText.text = "";

        foreach (char letter in line.ToCharArray())
        {
            dialogText.text += letter;
            yield return new WaitForSeconds(1f / lettersPerSecond);
        }
        isTyping = false;
    }
}
