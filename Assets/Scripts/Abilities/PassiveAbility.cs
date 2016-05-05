using UnityEngine;
using System.Collections;

public abstract class PassiveAbility : Ability
{
    private bool effectApplied = false;

    public override void Activate()
    {
        base.Activate();

        if (!effectApplied)
        {
            effectApplied = true;
            ApplyEffect();
        }
    }

    public override void DeActivate()
    {
        base.DeActivate();

        if (effectApplied)
        {
            effectApplied = false;
            RemoveEffect();
        }
    }

    public abstract void ApplyEffect();

    public abstract void RemoveEffect();
}
