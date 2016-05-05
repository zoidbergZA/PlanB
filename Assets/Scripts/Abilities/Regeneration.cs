using UnityEngine;
using System.Collections;

public class Regeneration : PassiveAbility 
{
    [SerializeField] private float bonusRegen = 2f;

    protected override void Use()
    {
        //passive
    }

    public override void ApplyEffect()
    {
        GameManager.Instance.Player.IncreaseRegeneration(bonusRegen);
    }

    public override void RemoveEffect()
    {
        GameManager.Instance.Player.IncreaseRegeneration(-bonusRegen);
    }
}
