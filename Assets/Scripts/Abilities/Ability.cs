using UnityEngine;
using System.Collections;

public abstract class Ability : MonoBehaviour
{
    public float Cost = 40f;
    public GameObject model;
    public Sprite[] Icons; // pc/joystick/action remote
//    public int abilitySlot;

    [SerializeField] private AudioClip useSound;
    [SerializeField] public float Cooldown = 5f;
//    [SerializeField] private int maxAmmo;

    private Player player;
    private float lastUseAt;

    public bool IsActivated { get; private set; }
    public bool IsOnCooldown
    {
        get
        {
            if (Time.time < lastUseAt + Cooldown)
                return true;
            return false;
        }
    }

    public float CooldownProgress
    {
        get
        {
            if (!IsOnCooldown)
                return 1;

            return (Time.time - lastUseAt)/Cooldown;
        }
    }
    public Player Player { get { return player; } }

    public virtual void Awake()
    {
        player = transform.root.GetComponent<Player>();
        lastUseAt = -999f;
//        Reset();
    }

    public virtual void Update()
    {
//        ActualizeStatus();
    }

    public void TryUse()
    {
        if (player.IsDead)
            return;

        if (IsOnCooldown)
            return;

        player.AudioSource[0].PlayOneShot(useSound);
        lastUseAt = Time.time;
        Use();
    }

    public virtual void Activate()
    {
        if (IsActivated)
            return;

        IsActivated = true;
//        Reset();

        if (model)
        {
            model.SetActive(true);
        }
    }
    
    public virtual void DeActivate()
    {
        IsActivated = false;

        if (model)
        {
            model.SetActive(false);
        }
    }

//    public void Reset()
//    {
//        
//    }

    protected abstract void Use();
}
