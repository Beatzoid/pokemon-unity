using UnityEngine;

/// <summary>
/// The TrainerFOV class manages the FOV of all trainers
/// </summary>
public class TrainerFOV : MonoBehaviour, IPlayerTriggerable
{
    /// <summary>
    /// Called whenever the player enters the trainers FOV
    /// </summary>
    public void OnPlayerTriggered(PlayerController player)
    {
        player.Character.Animator.IsMoving = false;
        GameController.Instance.OnEnterTrainersView(GetComponentInParent<TrainerController>());
    }
}
