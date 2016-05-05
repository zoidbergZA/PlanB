using System.Collections.Generic;
using UnityEngine;
using System.Collections;
//waiitng state - removed code is present in 107 revision, commented out

public class WaveManager : MonoBehaviour
{
    public float inProgressTime = 30f;

    [SerializeField] private WaveInfo waveInfo;
    [SerializeField] private AudioClip waveStartSound;
    [SerializeField] private int forcewallDisableAt = 4;
    [SerializeField] private float spawnTime = 3f;
    [SerializeField] private float inProgressIncrements = 6f;
    [SerializeField] private float shopTime = 15.0f;
    [SerializeField] private float kickOffTime = 5.0f;
    [SerializeField] private bool applyDroneBonusses;
    [SerializeField] private float slopeCoeff = 2f;
    [SerializeField] private float HPBonusCoeff = 1.0f;
    [SerializeField] private float DMGBonusCoeff = 1.0f;
    [SerializeField] private float BLTSPDBonusCoeff = 1.0f;
    [SerializeField] private float BLTRNGBonusCoeff = 1.0f;
    [SerializeField] private float SPDBonusCoeff = 1.0f;
    [SerializeField] private float RGNRTNBonusCoeff = 1.0f;
 
    private float spawnTimer;

    public enum State
    {
        SPAWNING,
        INPROGRESS,
        SHOP,
        KICKOFF
    }

//    private int dronesToSpawn;
    private int currentSpawner = 0;
    public List<Drone> SpawnedList { get; set; }
    public int CurrentWave { get; private set; }
    public int CurrentDrone { get; private set; }
    public float InProgressTimer { get; private set; }
    public float ShoppingTimer { get; private set; }
    public float KickOffTimer { get; private set; }
    public State WaveState { get; private set; }

    void Awake()
    {
        SpawnedList = new List<Drone>();
    }

    void Start()
    {
        InProgressTimer = inProgressTime;
        ShoppingTimer = shopTime;
        KickOffTimer = kickOffTime;
        
        GotoState(State.KICKOFF);
    }

    void Update()
    {
        UpdateWaveStates();
    }

    private void UpdateWaveStates()
    {
        switch (WaveState)
        {
            case State.KICKOFF:
                HandleKickOff();
                break;

            case State.SPAWNING:
                HandleSpawning();
                break;

            case State.INPROGRESS:
                HandleInProgress();
                break;

            case State.SHOP:
                HandleShopping();
                break;
        }
    }

    private void HandleKickOff()
    {
        if(KickOffTimer <= 0)
        {
            GameManager.Instance.HomeTower.RoundStarted();
            GotoState(State.SHOP);
        }

        UpdateWaveTimers();
    }

    private void HandleSpawning()
    {
        if (spawnTimer <= 0)
        {
            SpawnDroneWithinWave();
            ProcessToNextSpawner();
        }

        UpdateWaveTimers();
    }

    private void HandleInProgress()
    {
//        Debug.Log(InProgressTimer);

        if (SpawnedList.Count == 0 || InProgressTimer < 0)
        {
            if (CurrentWave < waveInfo.droneWaves.Length - 1)
            {
                GameManager.Instance.Shop.SpawnUpgrade(waveInfo.droneWaves[CurrentWave].upgrade);
                CurrentWave++;
            }
            else
            {
                ImpactExtraWavesBy(slopeCoeff);
            }

            GotoState(State.SHOP);
        }

        UpdateWaveTimers();
    }

    private void HandleShopping()
    {
        if (ShoppingTimer <= 0)
        {
            GotoState(State.SPAWNING);
            return;
        }

        UpdateWaveTimers();
    }

    private void GotoState(State newState)
    {
        switch (newState)
        {
                case State.KICKOFF:
                    WaveState = State.KICKOFF;
                    KickOffTimer = kickOffTime;
                    GameManager.Instance.Player.ToggleInputEnabled(false);
                    GameManager.Instance.Hud.ShowToast("DEFEND THE ESCAPE ROCKET");
                    break;

            case State.SHOP:
                ShoppingTimer = shopTime;
                GameManager.Instance.Hud.ShowMinimap(false);
                WaveState = State.SHOP;
                GameManager.Instance.Shop.SpawnBooster();
                GameManager.Instance.Player.ToggleInputEnabled(true);
                break;

            case State.SPAWNING:
                //this section was in waiting phase
                SpawnedList = new List<Drone>();
                CurrentDrone = 0;

                //this section is initially shopping part
//                GameManager.Instance.Shop.Close();
//                GameManager.Instance.Hud.PlaySound(1f, waveStartSound);
                GameManager.Instance.Hud.ShowToast("Incoming drones!", 2.3f, -1, NewHud.AlertType.WARNING);
                GameManager.Instance.Hud.ShowMinimap(true);
                WaveState = State.SPAWNING;

                //if Current wave is 3, disable energy barriers
                if (CurrentWave == forcewallDisableAt)
                    GameManager.Instance.DisableForceWalls();

                //increment in progress time for the next wave
                inProgressTime += inProgressIncrements;
                break;

            case State.INPROGRESS:
                InProgressTimer = inProgressTime;
                WaveState = State.INPROGRESS;
                break;
        }
    }

    private void SpawnBooster()
    {
        
    }

    private void UpdateWaveTimers()
    {
        switch (WaveState)
        {
            case State.INPROGRESS:
                InProgressTimer -= Time.deltaTime;
                break;

            case State.SHOP:
                ShoppingTimer -= Time.deltaTime;
                break;

            case State.SPAWNING:
                spawnTimer -= Time.deltaTime;
                break;

            case State.KICKOFF:
                KickOffTimer -= Time.deltaTime;
                break;
        }
    }

    private void SpawnDroneWithinWave()
    {
        if (CurrentDrone == waveInfo.droneWaves[CurrentWave].drones.Length)
        {
            GotoState(State.INPROGRESS);
            return;
        }

        if(!GameManager.Instance.DroneSpawners[currentSpawner].IsDead)
        {
            Drone nextDrone = waveInfo.droneWaves[CurrentWave].drones[CurrentDrone];
            Drone newDrone = GameManager.Instance.DroneSpawners[currentSpawner].SpawnDrone(nextDrone, GetDroneValue());

            if (applyDroneBonusses)
                AddDroneBonuses(newDrone);

            SpawnedList.Add(newDrone);
            spawnTimer = spawnTime;
        }

        else
        {
            //if spawner is dead, give player the drone value gold
            GameManager.Instance.Player.Gold += waveInfo.droneWaves[CurrentWave].drones[CurrentDrone].GoldValue;
        }

        CurrentDrone++;
    }

    private void ProcessToNextSpawner()
    {
        if (currentSpawner < GameManager.Instance.DroneSpawners.Length - 1)
        {
            currentSpawner++;
        }
        else
        {
            currentSpawner = 0;
        }
    }

    private void AddDroneBonuses(Drone drone)
    {
        drone.IncreaseHitpoints(waveInfo.droneWaves[CurrentWave].bonusHitPoints);
        drone.IncreaseDamage(waveInfo.droneWaves[CurrentWave].bonusDamage);
        drone.IncreaseSpeed(waveInfo.droneWaves[CurrentWave].bonusSpeed);
        drone.IncreaseBulletRange(waveInfo.droneWaves[CurrentWave].bonusBulletRange);
        drone.IncreaseBulletSpeed(waveInfo.droneWaves[CurrentWave].bonusBulletSpeed);
        drone.IncreaseRegeneration(waveInfo.droneWaves[CurrentWave].bonusRegeneration);
    }

    private float GetDroneValue()
    {
        return waveInfo.droneWaves[CurrentWave].totalGoldValue / (float)waveInfo.droneWaves[CurrentWave].drones.Length;
    }

    private void ImpactExtraWavesBy(float impactValue)
    {
        waveInfo.droneWaves[CurrentWave].bonusHitPoints *= HPBonusCoeff;
        waveInfo.droneWaves[CurrentWave].bonusDamage *= DMGBonusCoeff;
        waveInfo.droneWaves[CurrentWave].bonusBulletRange *= BLTRNGBonusCoeff;
        waveInfo.droneWaves[CurrentWave].bonusBulletSpeed *= BLTSPDBonusCoeff;
        waveInfo.droneWaves[CurrentWave].bonusSpeed *= SPDBonusCoeff;
        waveInfo.droneWaves[CurrentWave].bonusRegeneration *= RGNRTNBonusCoeff;
        HPBonusCoeff *= impactValue;
        DMGBonusCoeff *= impactValue;
        BLTRNGBonusCoeff *= impactValue;
        BLTSPDBonusCoeff *= impactValue;
        SPDBonusCoeff *= impactValue;
        RGNRTNBonusCoeff *= impactValue;
    }
}
