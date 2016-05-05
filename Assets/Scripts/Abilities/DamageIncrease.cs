using UnityEngine;
using System.Collections;

public class DamageIncrease : PassiveAbility
{
    [SerializeField] private float bonusDamage = 15f;

    protected override void Use()
    {
        //passive
    }

    public override void ApplyEffect()
    {
        GameManager.Instance.Player.ModifyDamage(bonusDamage);
    }

    public override void RemoveEffect()
    {
        GameManager.Instance.Player.ModifyDamage(-bonusDamage);
    }
}
