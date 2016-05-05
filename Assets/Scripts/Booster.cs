using UnityEngine;
using System.Collections;

public class Booster : MonoBehaviour 
{
    public enum Type
    {
        HEALTH,
        ATTACK,
        MOVESPEED
    }

    [SerializeField] private Type type;
    [SerializeField] private Transform myDisplayTransform;

    private BoosterCanvas boosterCanvas;

    public Type BoosterType { get { return type; } }

    void Start()
    {
        InitDisplay();
    }

    private void Collect(Player player)
    {
        switch (type)
        {
            case Type.ATTACK:
                player.ReduceCooldown(0.02f);
                player.CritChance += 0.07f;
		        player.CritMultiplier = 3.2f;
                break;

            case Type.MOVESPEED:
                player.IncreaseMovespeed(1f);
                break;

            case Type.HEALTH:
                player.IncreaseHitpoints(15f);
                player.IncreaseRegeneration(0.3f);
                break;
        }

        GameManager.Instance.Hud.BoosterCollected(this);
    }

    private void InitDisplay()
    {
        boosterCanvas = (BoosterCanvas)Instantiate(GameManager.Instance.boosterCanvasPrefab, myDisplayTransform.position, myDisplayTransform.rotation);
        boosterCanvas.transform.SetParent(transform);

        string bName = "";

        //populate canvas
        switch (type)
        {
            case Type.ATTACK:
                bName = "+Attack Speed";
                break;

            case Type.HEALTH:
                bName = "+Health";
                break;

            case Type.MOVESPEED:
                bName = "+Mobility";
                break;
        }

        boosterCanvas.NameText.text = bName;
        //        upgradeCanvas.CostText.text = GameManager.Instance.Player.abilityManager.Abilities[abilitySlot].Cost.ToString();
    }

    void OnTriggerEnter(Collider other)
    {
        Player player = other.transform.root.GetComponent<Player>();

        if (player)
        {
            Collect(player);

            GameManager.Instance.Hud.PlaySound(1f, GameManager.Instance.CollectUpgradeSound[Random.Range(0, GameManager.Instance.CollectUpgradeSound.Length)]);

            Destroy(gameObject);
        }
    }
}
