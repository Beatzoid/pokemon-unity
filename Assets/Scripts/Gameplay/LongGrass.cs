using UnityEngine;

public class LongGrass : MonoBehaviour, IPlayerTriggerable
{
    public void OnPlayerTriggered(PlayerController player)
    {
        // 10% chance to encounter a wild pokemon
        if (Random.Range(1, 101) <= 10)
        {
            player.Character.Animator.IsMoving = false;
            GameController.instance.StartBattle();
        }
    }
}
