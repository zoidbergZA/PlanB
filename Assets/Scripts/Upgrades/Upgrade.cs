using UnityEngine;
using System.Collections;
using UnityEngine.UI;

[RequireComponent(typeof(Collider))] 
public class Upgrade : MonoBehaviour
{
    public string upgradeName;
    public int abilitySlot;
//    public float goldCost = 10;

    [SerializeField] private GameObject model;
    [SerializeField] private string headerText;
    [SerializeField] private Sprite icon;
    [SerializeField] private Transform myDisplayTransform;

    private UpgradeCanvas upgradeCanvas;

    public bool IsCollected { get; private set; }
    
    void Awake()
    {
//        // delay pickup abilityDisplay //todo: FIX
//        GetComponent<Collider>().enabled = false;
//        StartCoroutine(DelayedCollider(2f));
    }

    void Start()
    {
//        UpdateShopWindowStatus();
        InitDisplay();
    }

//    void Update()
//    {
//        UpdateShopWindowStatus();
//    }

    private void UpdateShopWindowStatus()
    {
        if (upgradeCanvas)
        {
            if (GameManager.Instance.Player.Gold >= GameManager.Instance.Player.abilityManager.Abilities[abilitySlot].Cost)
            {
                upgradeCanvas.Icon.color = Color.white;
            }
            else
            {
                upgradeCanvas.Icon.color = Color.red;
            }
        }
    }

    private void InitDisplay()
    {
        upgradeCanvas = (UpgradeCanvas)Instantiate(GameManager.Instance.upgradeCanvasPrefab, myDisplayTransform.position, myDisplayTransform.rotation);
        upgradeCanvas.transform.SetParent(transform);

        //populate canvas
        upgradeCanvas.HeaderText.text = headerText;
        upgradeCanvas.Icon.sprite = icon;
        upgradeCanvas.NameText.text = upgradeName;
//        upgradeCanvas.CostText.text = GameManager.Instance.Player.abilityManager.Abilities[abilitySlot].Cost.ToString();
    }

    public void Reset()
    {
        model.SetActive(true);
        IsCollected = false;
    }

    private IEnumerator DelayedCollider(float delay)
    {
        yield return new WaitForSeconds(delay);

        GetComponent<Collider>().enabled = true;
    }

    void OnTriggerEnter(Collider other)
    {
        if (IsCollected)
            return;

        Player player = other.transform.root.GetComponent<Player>();

        if (player)
        {
//            if (player.Gold < GameManager.Instance.Player.abilityManager.Abilities[abilitySlot].Cost)
//                return;

//            player.Gold -= GameManager.Instance.Player.abilityManager.Abilities[abilitySlot].Cost;

            GameManager.Instance.Hud.PlaySound(1f, GameManager.Instance.CollectUpgradeSound[Random.Range(0, GameManager.Instance.CollectUpgradeSound.Length)]);

            player.CollectUpgrade(abilitySlot);
            IsCollected = true;
            model.SetActive(false);

            Destroy(gameObject);
        }
    }
}
