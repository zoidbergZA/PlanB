using UnityEngine;
using System.Collections;

public class AbilityManager : MonoBehaviour
{
    public Ability[] Abilities;

    public KeyCode[] abilityKeys;

    void Start()
    {
        ResetAbilities(false); //cheat code set true
    }

    public void ResetAbilities(bool enabled)
    {
        foreach (Ability ability in Abilities)
        {
            if (enabled)
                ability.Activate();
            else
                ability.DeActivate();
        }
    }

    void Update()
    {
        for (int i = 0; i < abilityKeys.Length; i++)
        {
            if (Abilities[i].IsActivated)
            {
                if (Input.GetKeyDown(abilityKeys[i]))
                {
                    Abilities[i].TryUse();
                }
            }
        }
    }
}
