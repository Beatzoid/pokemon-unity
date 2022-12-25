using System;
using System.Collections;
using UnityEngine;
using TMPro;

/// <summary>
/// The DialogManager class manages all the dialog-related logic
/// </summary>
public class DialogManager : MonoBehaviour
{
    [SerializeField] private GameObject dialogBox;
    [SerializeField] private TextMeshProUGUI dialogText;
    [SerializeField] private int lettersPerSecond;

    public static DialogManager Instance { get; private set; }
    public event Action OnShowDialog;
    public event Action OnCloseDialog;
    public bool IsShowing { get; private set; }

    public void Awake()
    {
        Instance = this;
    }

    public void HandleUpdate()
    {

    }

    /// <summary>
    /// Show dialog in the dialog box
    /// </summary>
    /// <param name="dialog">The dialog to show </param>
    public IEnumerator ShowDialog(Dialog dialog)
    {
        yield return new WaitForEndOfFrame();

        OnShowDialog.Invoke();

        IsShowing = true;

        dialogBox.SetActive(true);

        foreach (string line in dialog.Lines)
        {
            yield return TypeDialog(line);
            yield return new WaitUntil(() => Input.GetKeyDown(KeyCode.Return));
        }

        dialogBox.SetActive(false);
        IsShowing = false;
        OnCloseDialog?.Invoke();
    }

    /// <summary>
    /// Show the text on the dialog
    /// </summary>
    /// <param name="text">The text to show </param>
    /// <param name="waitForInput">Whether or not to wait for an input before clearing the dialog box </param>
    /// <param name="autoClose"> Whether or not to close the dialog after it's finished </param>
    public IEnumerator ShowDialogText(string text, bool waitForInput = true, bool autoClose = true)
    {
        IsShowing = true;
        dialogBox.SetActive(true);

        yield return TypeDialog(text);
        if (waitForInput)
            yield return new WaitUntil(() => Input.GetKeyDown(KeyCode.Return));

        if (autoClose)
            CloseDialog();
    }

    /// <summary>
    /// Smoothly type a line on the dialog box
    /// </summary>
    /// <param name="line">The line to type </param>
    public IEnumerator TypeDialog(string line)
    {
        dialogText.text = "";

        foreach (char letter in line.ToCharArray())
        {
            dialogText.text += letter;
            yield return new WaitForSeconds(1f / lettersPerSecond);
        }
    }

    public void CloseDialog()
    {
        dialogBox.SetActive(false);
        IsShowing = false;
    }
}
