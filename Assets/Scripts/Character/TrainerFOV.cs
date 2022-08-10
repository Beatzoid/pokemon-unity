using UnityEngine;

public class TrainerFOV : MonoBehaviour, IPlayerTriggerable
{
    public void OnPlayerTriggered(PlayerController player)
    {
        GameController.instance.OnEnterTrainersView(GetComponentInParent<TrainerController>());
    }
}
