using UnityEngine;
using System.Collections;

public class HomeTower : Vulnerable
{
    [SerializeField] private ParticleSystem readyParticleSystem;
    [SerializeField] private Transform doorPivot;
    [SerializeField] private float startHitpoints = 20f;
    [SerializeField] private int hitPenaltyTime = 4;
    [SerializeField] private Transform model;
//    [SerializeField] private Vector3 endPosition;
    [SerializeField] private Collider doorTrigger;
    [SerializeField] private NavMeshObstacle doorNavMeshObstacle;
    [SerializeField] private Light floorLight;
    [SerializeField] private Light mainLight;
    [SerializeField] private float mainLightDark = 0.25f;
    [SerializeField] private float lightFadeSpeed = 0.6f;

//    private Vector3 startPosition;
    private bool ready;
    private float floorlightMax;
    private float blackoutAt;
    private float mainLightNormal;

    public float TimeModifier { get;private set; }
    public bool IsBlackout { get; private set; }

    public override void Awake()
    {
        base.Awake();

        doorTrigger.enabled = false;
    }

    void Start()
    {
        Hitpoints = startHitpoints;
        floorlightMax = floorLight.intensity;
        mainLightNormal = mainLight.intensity;
        floorLight.intensity = 0f;
//        startPosition = model.transform.position;
//        endPosition += model.transform.position;
    }

    public override void Update()
    {
        base.Update();

//        AdjustPosition();

        if (IsBlackout)
        {
            if (TimeModifier <= 0.1f)
            {
                ToggleBlackout(false);
            }
        }
        else if (mainLight.intensity < mainLightNormal)
        {
            //fade mainlight back up
            mainLight.intensity += Time.deltaTime*0.5f;
        }

        if (TimeModifier > 0.1f)
        {
            TimeModifier -= Time.deltaTime;
        }
        else if (TimeModifier < -0.1f)
            TimeModifier += Time.deltaTime;

        if (Hitpoints >= MaxHP && !ready)
        {
            ready = true;
            doorTrigger.enabled = true;
            doorNavMeshObstacle.gameObject.SetActive(true);
            readyParticleSystem.gameObject.SetActive(true);
            doorPivot.localEulerAngles = new Vector3(200, 0, 0);
            GameManager.Instance.WaveManager.inProgressTime = 1f; //reduce time between wave spawns
            GameManager.Instance.RocketReadyAt = Time.time;
            GameManager.Instance.Hud.ShowHighscore();
            GameManager.Instance.Hud.ShowToast("Escape rocket ready!");
        }
        else if (Hitpoints < MaxHP)
        {
            doorTrigger.enabled = false;

            if (ready)
            {
                ready = false;
//                GameManager.Instance.Hud.ShowToast("Escape Rocket ready!");
            }
        }

        if (floorLight.intensity > 0)
        {
            floorLight.intensity -= lightFadeSpeed*Time.deltaTime;
        }
    }

    public void RoundStarted()
    {
        GameManager.Instance.RoundStartedAt = Time.time;
        GameManager.Instance.PerfectScore = (MaxHP - Hitpoints)/regenerationRate;
//        Debug.Log("pefect time is: " + GameManager.Instance.PerfectScore);
    }

    public override void TakeDamage(float amount, Vulnerable attacker, Vector3 point)
    {
        if (ready)
            return;

        //override with 1 second increase in timeleft
        base.TakeDamage(hitPenaltyTime, attacker, point);
        TimeModifier += hitPenaltyTime;

        floorLight.color = Color.red;
        floorLight.intensity = floorlightMax;

        ToggleBlackout(true);

        GameManager.Instance.Hud.TimerChanged(hitPenaltyTime);

        if (!Invulnerable)
            GameManager.Instance.Hud.ShowToast("ESCAPE ROCKET TAKING DAMAGE!", 2.3f, 1, NewHud.AlertType.WARNING);
    }

    public void DecreaseClock(float amount = 1f)
    {
        Hitpoints += amount;
        TimeModifier -= amount;

        floorLight.color = Color.green;
        floorLight.intensity = floorlightMax;

        GameManager.Instance.Hud.TimerChanged(-1f);
//        Debug.Log("time reduced!! " + Time.time);
    }

    public void SetHitpoints(float hp)
    {
        Hitpoints = hp;
    }

    public override void Die()
    {
        Debug.Log("Home Tower DEAD!");
//        GameManager.Instance.GameDefeat();
    }

    private void ToggleBlackout(bool dark)
    {
        if (dark)
        {
            IsBlackout = true;
            blackoutAt = Time.time;
            mainLight.intensity = mainLightDark;
            GameManager.Instance.Player.ToggleFlashLight(true);
        }
        else
        {
            IsBlackout = false;
//            mainLight.intensity = mainLightNormal;
            GameManager.Instance.Player.ToggleFlashLight(false);
        }
    }

//    private void AdjustPosition()
//    {
//        float healthFrac = Hitpoints / MaxHP;
//
//        model.transform.position = Vector3.Lerp(startPosition, endPosition, 1 - healthFrac);
//    }
}
