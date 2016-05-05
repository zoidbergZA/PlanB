using System;
using UnityEngine;
using System.Collections;

public class GameManager : MonoBehaviour 
{
    private static GameManager _instance;
    public static KeyCode pauseButton;

    public bool showHealthText;

    //global presets
    public FloatingText HealthTextPrefab;
    public FloatingText GoldTextPrefab;
    public FloatingText CritTextPrefab;
    public Healthbar HealthbarPrefab;
    public UpgradeCanvas upgradeCanvasPrefab;
    public BoosterCanvas boosterCanvasPrefab;
    public AudioClip[] CollectUpgradeSound;
    public GameObject StunParticles;

    public DroneSpawner[] DroneSpawners { get; private set; }
    public ForceWall[] ForceWalls { get; private set; }
    public bool ForcewallsDown { get; private set; }
    public bool GamePaused { get; private set; }
    public HomeTower HomeTower { get; private set; }
    public Player Player { get; private set; }
    public WaveManager WaveManager { get; private set; }
    public Shop Shop { get; set; }
    public NewHud Hud { get; private set; }
    public TracerHelper TracerHelper { get; private set; }
    public ParticlesHelper ParticlesHelper { get; private set; }
    public PhysicsHelper PhysicsHelper { get; private set; }
    public float RoundStartedAt { get; set; }
    public float RoundFinishedAt { get; set; }
    public float PerfectScore { get; set; }
    public float RocketReadyAt { get; set; }

    public delegate void PauseEvent(bool isPaused);
    public static event PauseEvent pauseEvent;

    static public GameManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType(typeof(GameManager)) as GameManager;

                if (_instance == null)
                {
                    throw new Exception("no GameManager!");
                }
            }
            return _instance;
        }
    }

    void Awake()
    {
        Hud = FindObjectOfType<NewHud>();
        Player = FindObjectOfType<Player>();
        WaveManager = FindObjectOfType<WaveManager>();
        HomeTower = FindObjectOfType<HomeTower>();
        Shop = FindObjectOfType<Shop>();
        DroneSpawners = FindObjectsOfType<DroneSpawner>();
        TracerHelper = FindObjectOfType<TracerHelper>();
        ParticlesHelper = FindObjectOfType<ParticlesHelper>();
        PhysicsHelper = FindObjectOfType<PhysicsHelper>();
    }

    void Start()
    {
        //PlayerPrefs.DeleteAll();
        //PlayerPrefs.SetString("controls", "KEYBOARD");

    }

    void Update()
    {
//        //debug inputs

        if (Input.GetKeyDown(KeyCode.F1))
        {
            Player.abilityManager.ResetAbilities(true);
        }

//        if (Input.GetKeyDown(KeyCode.Alpha1))
//        {
//            foreach (DroneSpawner droneSpawner in DroneSpawners)
//            {
//                droneSpawner.Die();
//            }
//        }

        UpdatePauseStatus();
    }

//    public void GameOver(bool success)
//    {
//        Hud.ShowGameOver(success);
//    }

    public void GameVictory()
    {
        RoundFinishedAt = Time.time;
        Player.ToggleInputEnabled(false);
        Player.Invulnerable = true;
        HomeTower.Invulnerable = true;
        Player.gameObject.SetActive(false);
//        Hud.Minimap.gameObject.SetActive(false);
//        Hud.gameObject.SetActive(false);
        Hud.ShowGameOver(true);
        Debug.Log("you made it to victory!! highscore: " + (RoundFinishedAt - RocketReadyAt));

        //pass score object to next scene
        GameObject scoreGameObject = new GameObject();
        ScoreObject scoreObject = scoreGameObject.AddComponent<ScoreObject>();
        scoreObject.score = RoundFinishedAt - RocketReadyAt;
        DontDestroyOnLoad(scoreGameObject);
        Application.LoadLevel("JuanRecording");
    }

    public void GameDefeat()
    {
        Player.ToggleInputEnabled(false);
        Player.Invulnerable = true;
        HomeTower.Invulnerable = true;
        Player.gameObject.SetActive(false);
        //        Hud.Minimap.gameObject.SetActive(false);
        //        Hud.gameObject.SetActive(false);
        Hud.ShowGameOver(false);

        Debug.Log("you lose, escape rocket destroyed!" + Time.time);
    }

    public void CheckAllSpawners()
    {
        int aliveSpawners = 0;
        
        foreach (DroneSpawner droneSpawner in DroneSpawners)
        {
            if (!droneSpawner.IsDead)
                aliveSpawners++;
        }
        
        if (aliveSpawners == 0)
            OpenGate();
    }

    public void DisableForceWalls()
    {
        ForceWall[] forceWalls = FindObjectsOfType<ForceWall>();

        foreach (ForceWall forceWall in forceWalls)
        {
            forceWall.gameObject.SetActive(false);
        }

        ForcewallsDown = true;
        Hud.ShowToast("SPAWNER DEFENCES DOWN!", 2.3f, 3, NewHud.AlertType.GOOD);
    }

    public void PauseGame(bool isPaused)
    {
        if (pauseEvent != null)
        {
            if (isPaused)
            {
                Time.timeScale = 0.0f;
                GamePaused = true;
                pauseEvent(true);
            }
            else
            {
                Time.timeScale = 1.0f;
                GamePaused = false;
                pauseEvent(false);
            }
        }
    }

//    public void CheckGateStatus()
//    {
//        int aliveSpawners = 0;
//
//        foreach (DroneSpawner droneSpawner in DroneSpawners)
//        {
//            if (!droneSpawner.IsDead)
//                aliveSpawners++;
//        }
//
//        if (aliveSpawners == 0)
//            OpenGate();
//        else
//            CloseGate();
//    }

    private void UpdatePauseStatus()
    {
        if (Input.GetKeyDown(pauseButton) || Input.GetKeyDown(KeyCode.Escape))
        {
            PauseGame(!GamePaused);
        }
    }

    private void OpenGate()
    {
        HomeTower.SetHitpoints(HomeTower.MaxHP - 5);
    }

    private void CloseGate()
    {
        //Debug.Log("close the gate! " + Time.time);
    }
}
