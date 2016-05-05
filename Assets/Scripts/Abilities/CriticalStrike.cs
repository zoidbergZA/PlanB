using UnityEngine;
using System.Collections;

public class CriticalStrike : PassiveAbility
{
    [SerializeField] private float critChance = 0.2f;
    [SerializeField] private float critMultiplier = 2.2f;

    protected override void Use()
    {
        //do nothing
    }

    public override void ApplyEffect()
    {
        //todo: increase player crit chance and set crit multiplier
//        Debug.Log("add crits to player " + Time.time);

		GameManager.Instance.Player.CritChance = critChance;
		GameManager.Instance.Player.CritMultiplier = critMultiplier;
    }

    public override void RemoveEffect()
    {
        //do nothing
    }
}
