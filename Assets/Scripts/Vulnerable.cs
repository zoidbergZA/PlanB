using UnityEngine;
using System.Collections;

[RequireComponent(typeof(AudioSource))]
public abstract class Vulnerable : MonoBehaviour
{
    public Transform floatingText_ref;
    public AudioSource myAudioSource;

    [SerializeField] private GameObject minimapIconPrefab;
    [SerializeField] private GameObject deathPrefab;
    [SerializeField] private GameObject takeDamagePrefab;
    [SerializeField] private float MaxHitpoints = 100f;
    public float regenerationRate = 0f;
    [SerializeField] private AudioClip[] vulnerableTakeDmg;


    public bool Invulnerable { get; set; }
    public bool IsDead { get; protected set; }
    public float Hitpoints { get; protected set; }
    public float MaxHP { get { return MaxHitpoints; } }
    public Transform FloatingTextRef { get { return floatingText_ref; } }
    private bool IsHealing
    {
        get
        {
            if(stunnedParticles != null) return true;
            else return false;
        }
    }
    
    private Healthbar healthbar;
    private Vulnerable vulnerable;
    private GameObject stunnedParticles;

    public virtual void Awake()
    {
        myAudioSource = GetComponent<AudioSource>();
        Hitpoints = MaxHitpoints;

        if (floatingText_ref)
        { 
            healthbar = Instantiate(GameManager.Instance.HealthbarPrefab, floatingText_ref.position, floatingText_ref.rotation) as Healthbar;
            healthbar.transform.SetParent(floatingText_ref);
            healthbar.Target = this;
        }

        if (minimapIconPrefab)
        {
            GameObject minimapIcon = Instantiate(minimapIconPrefab, transform.position, Quaternion.identity) as GameObject;
            minimapIcon.transform.SetParent(this.transform);
        }
    }

    public virtual void Update()
    {
        UpdateRegeneration();
        UpdateMaxHPAmount();
        UpdateStunnedParticlesPosition();

        //can be optimized
        if (healthbar)
        {
            healthbar.gameObject.SetActive(Hitpoints < MaxHP);
        }
    }

    private void UpdateRegeneration()
    {
        if (IsDead)
            return;

        Hitpoints += regenerationRate * Time.deltaTime;
    }

    private void UpdateMaxHPAmount()
    {
        if (Hitpoints > MaxHP)
            Hitpoints = MaxHitpoints;
    }

    private void UpdateStunnedParticlesPosition()
    {
        if(stunnedParticles && vulnerable)
        {
            stunnedParticles.transform.position = vulnerable.transform.position;
        }
    }


    public void IncreaseHitpoints(float amount, bool addToCurrentHP = true)
    {
        MaxHitpoints += amount;

        if (addToCurrentHP)
            Hitpoints += amount;
    }

    public void IncreaseRegeneration(float amount)
    {
        regenerationRate += amount;
    }

    public virtual void TakeDamage(float amount, Vulnerable attacker, Vector3 point)
    {
		if (IsDead || amount <= 0)
            return;

        if (Invulnerable && floatingText_ref)
        {
            FloatingText immuneText = Instantiate(GameManager.Instance.HealthTextPrefab, floatingText_ref.position, floatingText_ref.rotation) as FloatingText;

            immuneText.textObject.text = "immune!";
//            immuneText.textObject.fontSize = 10;
            immuneText.textObject.color = Color.white;

            return;
        }

        Hitpoints -= amount;

        if (takeDamagePrefab)
            Instantiate(takeDamagePrefab, point, transform.rotation);

        if (Hitpoints <= 0 && !IsDead)
        {
            Hitpoints = 0;
            IsDead = true;
            if (healthbar)
            {
                healthbar.gameObject.SetActive(false);
            }

            if (deathPrefab)
                Instantiate(deathPrefab, transform.position, transform.rotation);

            Die();
        }

        if (vulnerableTakeDmg.Length > 0 && !IsDead)
        {
            myAudioSource.PlayOneShot(vulnerableTakeDmg[Random.Range(0, vulnerableTakeDmg.Length)]);
//            Debug.Log(vulnerableTakeDmg[0].name);
        }

        if (floatingText_ref)
        {
            FloatingText damageText = Instantiate(GameManager.Instance.HealthTextPrefab, floatingText_ref.position, floatingText_ref.rotation) as FloatingText;

            damageText.textObject.text = "-" + Mathf.RoundToInt(amount);
            damageText.textObject.color = Color.red;
        }
    }

    public virtual void HealVulnerable(float amount, Vulnerable target)
    {
        vulnerable = target;
        if (IsDead || amount <= 0 || Invulnerable) return;
        if (target.Hitpoints >= target.MaxHP) return;
        target.Hitpoints += amount;
        if (target.Hitpoints >= target.MaxHP) target.Hitpoints = target.MaxHP;
        //myAudioSource.PlayOneShot(takeDamageSound);

        if (target.Hitpoints <= target.MaxHP)
        {
            stunnedParticles = Instantiate(GameManager.Instance.StunParticles, target.transform.position, Quaternion.identity) as GameObject;
            GameManager.Instance.TracerHelper.ShowHealTracer(this.transform.position, vulnerable.transform.position);
        }

        /*
        FloatingText damageText = Instantiate(GameManager.Instance.HealthTextPrefab, target.floatingText_ref.position, target.floatingText_ref.rotation) as FloatingText;
        damageText.textObject.text = "+" + Mathf.RoundToInt(amount);
        damageText.textObject.color = Color.green;
        */
    }

    public void Revive()
    {
        IsDead = false;
        Hitpoints = MaxHitpoints;
        healthbar.gameObject.SetActive(true);
    }

    public abstract void Die();
}
