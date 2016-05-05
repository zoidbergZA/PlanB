using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using System.Collections;

public class Shop : MonoBehaviour
{
    [SerializeField] private List<Upgrade> upgrades;
    [SerializeField] private int upradeInterval = 3;
    [SerializeField] private float boosterChance = 0.6f;
    [SerializeField] private Booster[] boosterPrefabs;
    [SerializeField] private Transform[] upgrade_refs;
    [SerializeField] private Transform[] booster_refs;

    private int nextUpgrade;

    void Start()
    {
        nextUpgrade = upradeInterval;
    }

    public void SpawnUpgrade(Upgrade upgradePrefab)
    {
//        if (upgradePrefab == null)
//            return;

        nextUpgrade--;

        if (nextUpgrade > 0)
            return;

        nextUpgrade = upradeInterval;

        if (upgrades.Count > 0)
        {
            int r = Random.Range(0, upgrades.Count);

            int rand = Random.Range(0, upgrade_refs.Length);
            Upgrade upgrade = Instantiate(upgrades[r], upgrade_refs[rand].position, upgrade_refs[rand].rotation) as Upgrade;
            GameManager.Instance.Hud.ShowToast(upgrade.upgradeName + " Available!", 2.3f, 2, NewHud.AlertType.GOOD);

            upgrades.RemoveAt(r);
        }

//        if (upgradePrefab == null) //remove this
//        {
//            int abilityCount = GameManager.Instance.Player.abilityManager.Abilities.Count();
//            int r = Random.Range(0, abilityCount);
//            int triesLeft = abilityCount;
//
//            Debug.Log("abilityCount: " + abilityCount + ", random: " + r);
//
////            if (!GameManager.Instance.Player.abilityManager.Abilities[r].IsActivated)
////            {
////                unlocked = true;
////                int rand = Random.Range(0, upgrade_refs.Length);
////                Upgrade upgrade = Instantiate(upgradePrefab, upgrade_refs[rand].position, upgrade_refs[rand].rotation) as Upgrade;
////                GameManager.Instance.Hud.ShowToast(upgrade.upgradeName + " Available!", 2.3f, 2, NewHud.AlertType.GOOD);
////            }
//
//            while (triesLeft > 0)
//            {
//                if (!GameManager.Instance.Player.abilityManager.Abilities[r].IsActivated)
//                {
//                    int rand = Random.Range(0, upgrade_refs.Length);
//                    Upgrade upgrade = Instantiate(upgradePrefab, upgrade_refs[rand].position, upgrade_refs[rand].rotation) as Upgrade;
//                    GameManager.Instance.Hud.ShowToast(upgrade.upgradeName + " Available!", 2.3f, 2,NewHud.AlertType.GOOD);
//                }
//
//                else
//                {
//                    r++;
//                    if (r >= abilityCount) r = 0;
//                    triesLeft--;
//                }
////
//            }
//        }

//        int rand = Random.Range(0, upgrade_refs.Length);
//
//        Upgrade upgrade = Instantiate(upgradePrefab, upgrade_refs[rand].position, upgrade_refs[rand].rotation) as Upgrade;

//        GameManager.Instance.Hud.ShowToast(upgrade.upgradeName + " Available!", 2.3f, 2, NewHud.AlertType.GOOD);
    }

    public void SpawnBooster()
    {
        if (Random.value > boosterChance)
            return;

        int randLoc = Random.Range(0, booster_refs.Length);
        int randBoost = Random.Range(0, boosterPrefabs.Length);

        Instantiate(boosterPrefabs[randBoost], booster_refs[randLoc].position, booster_refs[randLoc].rotation);
    }
}
