using UnityEngine;

[CreateAssetMenu(menuName = "Items/Create new recovery item")]
public class RecoveryItem : ItemBase
{
    [Header("Hp")]
    [SerializeField] private int hpAmount;
    [SerializeField] private bool restoreMaxHP;

    [Header("PP")]
    [SerializeField] private int ppAmount;
    [SerializeField] private bool restoreMaxPP;

    [Header("Status Conditions")]
    [SerializeField] private ConditionID status;
    [SerializeField] private bool recoverAllStatus;

    [Header("Revive")]
    [SerializeField] private bool revive;
    [SerializeField] private bool maxRevive;

    public override bool Use(Pokemon pokemon)
    {
        if (hpAmount > 0)
        {
            if (pokemon.HP >= pokemon.MaxHp)
                return false;

            pokemon.IncreaseHP(hpAmount);
        }

        return true;
    }
}
