using UnityEngine;
using System.Collections;
using UnityEngine.UI;

[RequireComponent(typeof(AudioSource))]
public class NewHud : MonoBehaviour 
{
    public enum AlertType
    {
        WARNING,
        NEUTRAL,
        GOOD
    }

    [SerializeField] private AbilityDisplay abilityDisplayPrefab;
    [SerializeField] private AudioClip[] voiceAlerts;
    [SerializeField] private Text countdownText;
    [SerializeField] private Text countdownChangedText;
    [SerializeField] private Image countdownBackground;
    [SerializeField] private Sprite countdownGood;
    [SerializeField] private Sprite countdownBad;
    [SerializeField] private Image playerDamageImage;
    [SerializeField] private Image boosterAttack;
    [SerializeField] private Image boosterSpeed;
    [SerializeField] private Image boosterHealth;
    [SerializeField] private Text boosterAttackStack;
    [SerializeField] private Text boosterSpeedStack;
    [SerializeField] private Text boosterHealthStack;
    [SerializeField] private AudioClip warningAlert;
    [SerializeField] private AudioClip neutralAlert;
    [SerializeField] private AudioClip goodAlert;
    [SerializeField] private Text waveState;
	[SerializeField] private Text timeleft;
    [SerializeField] private Text endMessage;
    [SerializeField] private GameObject toastPanel;
    [SerializeField] private Text toastText;
    [SerializeField] private GameObject abilitiesPanel;
    [SerializeField] private GameObject countdownPanel;
    [SerializeField] private GameObject highscorePanel;
    [SerializeField] private Text highscoreText;
    [SerializeField] private GameObject pauseScreen;
    [SerializeField] private GameObject gameOverScreen;
	[SerializeField] private InputField enterName;
	[SerializeField] private Button send;
    [SerializeField] private Camera miniMap;

    private AudioSource myAudioSource;
    private AbilityDisplay[] abilityDisplays;
    private string inprogTimer;
    private string shopingTimer;
    private string waitingTimer;
    private string kickoffTimer;
    private bool showingToast;
    private float hideToastAt;
    private HomeTower homeTower;
    private string timerFormatted;
    private float timerChangedAt;
    private float lastTimerChange;
    private float lastVoiceMsgAt;
    private int healthBoosters;
    private int attackBoosters;
    private int speedBoosters;
    //private float abilityCooldownTimeleft; //Chiggy

	private string name;
	private int time;
    private float playerDamageFade;

	[SerializeField] private Text nr1;
	[SerializeField] private Text nr2;
	[SerializeField] private Text nr3;
	[SerializeField] private Text nr4;
	[SerializeField] private Text nr5;
	[SerializeField] private Text nr6;
	[SerializeField] private Text nr7;
	[SerializeField] private Text nr8;
	[SerializeField] private Text nr9;
	[SerializeField] private Text nr10;

	[SerializeField] private Text topScore;

    public Camera Minimap { get { return miniMap; } }

    void Awake()
    {
        myAudioSource = GetComponent<AudioSource>();
        pauseScreen.SetActive(false);
        gameOverScreen.SetActive(false);
		enterName.gameObject.SetActive (false);
        HideToastPanel();

//        countdownChangedText.gameObject.SetActive(false);
        timerChangedAt = -100f;
        lastVoiceMsgAt = -100f;
    }

    void Start()
    {
        homeTower = GameManager.Instance.HomeTower;
        InitAbilityDisplays();
        InitBoosterDisplays();
        playerDamageImage.color = Color.clear;
        ShowMinimap(false);
//        InitAbitlityButtons(); todo

        countdownBackground.enabled = false;
        countdownChangedText.enabled = false;
        ShowToast("DEFEND THE ESCAPE ROCKET!", 2.3F, 4, AlertType.GOOD);

        // if not keyboard + mouse, hide cursor
        if (GameManager.Instance.Player.PlayerInput.Controls != "KEYBOARD")
        {
            Cursor.visible = false;
        }
    }

    void Update()
    {
        inprogTimer = GameManager.Instance.WaveManager.InProgressTimer.ToString("00.0");
        shopingTimer = GameManager.Instance.WaveManager.ShoppingTimer.ToString("00.0");
        kickoffTimer = GameManager.Instance.WaveManager.KickOffTimer.ToString("00.0");

        UpdateWaveStates();
        ShowAbillities();
        ShowCountdown();

//        if (GameManager.Instance.HomeTower.TimeModifier != 0)
//        {
//            Debug.Log(GameManager.Instance.HomeTower.TimeModifier);
//        }

        if (playerDamageFade > 0)
        {
            playerDamageImage.color = Color.Lerp(Color.white, Color.clear, 1f - playerDamageFade);

            const float fadeTime = 1f;
            playerDamageFade -= Time.deltaTime / fadeTime;
        }
        
        if (Mathf.Abs(homeTower.TimeModifier) >= 0.1f)
        {
            countdownBackground.enabled = true;
            countdownChangedText.enabled = true;

            string changedText = "";
            
            if (homeTower.TimeModifier < 0f)
            {
                countdownBackground.sprite = countdownGood;
            }
            else
            {
                countdownBackground.sprite = countdownBad;
                changedText = "+";
            }

            changedText += homeTower.TimeModifier.ToString("00.00");
            
            countdownChangedText.text = changedText;

        }

        else
        {
            countdownBackground.enabled = false;
            countdownChangedText.enabled = false;
        }

//        if (Time.time <= timerChangedAt + 2f)
//        {
//            countdownBackground.enabled = true;
//            countdownChangedText.enabled = true;
//            
//            string changedText = "";
//
//            if (lastTimerChange < 0)
//            {
//                countdownBackground.sprite = countdownGood;
////                countdownChangedText.color = Color.green;
//            }
//            else
//            {
//                countdownBackground.sprite = countdownBad;
////                countdownChangedText.color = Color.red;
//                changedText = "+";
//            }
//
//            changedText += lastTimerChange;
//
//            countdownChangedText.text = changedText;
//        }
//
//        else
//        {
//            countdownBackground.enabled = false;
//            countdownChangedText.enabled = false;
//        }

        if (showingToast)
        {
            if (Time.time >= hideToastAt)
                HideToastPanel();
        }

        if (highscorePanel.activeSelf)
        {
            highscoreText.text = "highscore: " + (Time.time - GameManager.Instance.RocketReadyAt).ToString("00.0");
        }
    }

    public void ShowPlayerDamage()
    {
        playerDamageFade = 1f;
        Camera.main.SendMessage("Shake");
    }

    public void ShowHighscore()
    {
        countdownPanel.SetActive(false);
        highscorePanel.SetActive(true);
    }

    public void ShowToast(string message, float duration = 2.3f, int voiceMessage = -1, AlertType alertType = AlertType.NEUTRAL)
    {
        showingToast = true;
        toastPanel.SetActive(true);
        toastText.text = message;
        hideToastAt = Time.time + duration;

        switch (alertType)
        {
            case AlertType.GOOD:
                PlaySound(0.7f, goodAlert);
                break;

            case AlertType.NEUTRAL:
                PlaySound(0.7f, neutralAlert);
                break;

            case AlertType.WARNING:
                PlaySound(0.1f, warningAlert);
                break;
        }

        if (voiceMessage >= 0 && Time.time >= lastVoiceMsgAt + 3f)
        {
            lastVoiceMsgAt = Time.time;
            PlaySound(1f, voiceAlerts[voiceMessage]);
        }
    }

    public void BoosterCollected(Booster booster)
    {
        int stackCount = 0;

        switch (booster.BoosterType)
        {
            case Booster.Type.ATTACK:
                attackBoosters++;
                boosterAttack.gameObject.SetActive(true);
                boosterAttackStack.text = "x" + attackBoosters;
                break;

            case Booster.Type.HEALTH:
                healthBoosters++;
                boosterHealth.gameObject.SetActive(true);
                boosterHealthStack.text = "x" + healthBoosters;
                break;

            case Booster.Type.MOVESPEED:
                speedBoosters++;
                boosterSpeed.gameObject.SetActive(true);
                boosterSpeedStack.text = "x" + speedBoosters;
                break;
        }
    }

    public void ShowGameOver(bool success)
    {
        Cursor.visible = true;
        abilitiesPanel.SetActive(false);
        gameOverScreen.SetActive(true);
        toastPanel.SetActive(false);

		PlayerScore ps = enterName.gameObject.GetComponent<PlayerScore>();

        if (success)
        {
            endMessage.text = "YOU WIN";
			enterName.gameObject.SetActive(true);
			GetHighScores();
        }
        else
        {
//            panelScreen.SetActive(false);
//            waveScreen.SetActive(false);
            
            endMessage.text = "YOU LOSE";
			GetHighScores();
        }
    }

	public void OnSendClick(){

		PlayerScore ps = enterName.gameObject.GetComponent<PlayerScore>();

		float timePlayed = GameManager.Instance.RoundFinishedAt - GameManager.Instance.RoundStartedAt;
		name = enterName.text;
		time = Mathf.RoundToInt (timePlayed);
		ps.AddScore(name, time);
		send.enabled = false;
	}

    public void TimerChanged(float amount)
    {
        timerChangedAt = Time.time;
        lastTimerChange = amount;
    }

    private void InitBoosterDisplays()
    {
        boosterAttack.gameObject.SetActive(false);
        boosterSpeed.gameObject.SetActive(false);
        boosterHealth.gameObject.SetActive(false);
    }

    private void ShowCountdown()
    {
        float timeLeft = (homeTower.MaxHP - homeTower.Hitpoints)/homeTower.regenerationRate;

        if (GameManager.Instance.HomeTower.TimeModifier > 0.1f)
            timeLeft -= GameManager.Instance.HomeTower.TimeModifier;
        

        System.TimeSpan t = System.TimeSpan.FromSeconds(timeLeft);
        timerFormatted = string.Format("{0:D2}:{1:D2}", t.Minutes, t.Seconds, t.Milliseconds);

        countdownText.text = timerFormatted;
    }

    private void UpdateAbilityCooldownCountdown(bool isOnCooldown, AbilityDisplay abilityDisplay, Ability ability)
    {
        if(isOnCooldown)
        {
            float cooldownTimer = ability.Cooldown - ability.Cooldown*ability.CooldownProgress;
            abilityDisplay.CooldownText.text = cooldownTimer.ToString("0");
            
        }
        else
        {
            abilityDisplay.CooldownText.text = "";
        }
    }

	private void GetHighScores()
	{
//		for(int i = 0; i < 10; i++)
//		{
//			Debug.Log(PlayerPrefs.GetString(i + "HScoreName") + " has a score of: " +  PlayerPrefs.GetInt(i + "HScore"));
//		}

		topScore.text = "The best score possible is: " + Mathf.RoundToInt(GameManager.Instance.PerfectScore);

		nr1.text = "NR 1: " + PlayerPrefs.GetString (0 + "HScoreName") + " has a score of: " + PlayerPrefs.GetInt (0 + "HScore");
		nr2.text = "NR 2: " + PlayerPrefs.GetString (1 + "HScoreName") + " has a score of: " + PlayerPrefs.GetInt (1 + "HScore");
		nr3.text = "NR 3: " + PlayerPrefs.GetString (2 + "HScoreName") + " has a score of: " + PlayerPrefs.GetInt (2 + "HScore");
		nr4.text = "NR 4: " + PlayerPrefs.GetString (3 + "HScoreName") + " has a score of: " + PlayerPrefs.GetInt (3 + "HScore");
		nr5.text = "NR 5: " + PlayerPrefs.GetString (4 + "HScoreName") + " has a score of: " + PlayerPrefs.GetInt (4 + "HScore");
		nr6.text = "NR 6: " + PlayerPrefs.GetString (5 + "HScoreName") + " has a score of: " + PlayerPrefs.GetInt (5 + "HScore");
		nr7.text = "NR 7: " + PlayerPrefs.GetString (6 + "HScoreName") + " has a score of: " + PlayerPrefs.GetInt (6 + "HScore");
		nr8.text = "NR 8: " + PlayerPrefs.GetString (7 + "HScoreName") + " has a score of: " + PlayerPrefs.GetInt (7 + "HScore");
		nr9.text = "NR 9: " + PlayerPrefs.GetString (8 + "HScoreName") + " has a score of: " + PlayerPrefs.GetInt (8 + "HScore");
		nr10.text = "NR 10: " + PlayerPrefs.GetString (9 + "HScoreName") + " has a score of: " + PlayerPrefs.GetInt (9 + "HScore");
	}

    private void HideToastPanel()
    {
        showingToast = false;
        toastPanel.SetActive(false);
    }

    private void OnEnable()
    {
        GameManager.pauseEvent += OnPause;
    }

    private void OnDisable()
    {
        GameManager.pauseEvent -= OnPause;
    }

    public void PlaySound(float pVolume, AudioClip sound)
    {
        myAudioSource.PlayOneShot(sound, pVolume);
    }

    public void ShowMinimap(bool show)
    {
        if (show)
            miniMap.gameObject.SetActive(true);
        else
            miniMap.gameObject.SetActive(false);
    }

    public void onContinueClick()
    {
        GameManager.Instance.PauseGame(false);
    }

    public void onMenuClick()
    {
        Application.LoadLevel("MainMenu");
        GameManager.Instance.PauseGame(false);
    }

    public void onRestartClick()
    {
        Application.LoadLevel(Application.loadedLevel);
        GameManager.Instance.PauseGame(false);
    }

    private void UpdateWaveStates()
    {
        switch (GameManager.Instance.WaveManager.WaveState)
        {
            case WaveManager.State.SPAWNING:
                waveState.text = "Spawning wave: " + (GameManager.Instance.WaveManager.CurrentWave + 1);
                timeleft.text = "";
                break;

            case WaveManager.State.INPROGRESS:
                waveState.text = "In progress";
                timeleft.text = inprogTimer + "";
                break;

            case WaveManager.State.SHOP:
                waveState.text = "Shop is open";
                timeleft.text = shopingTimer + "";
                break;

            case WaveManager.State.KICKOFF:
                waveState.text = "Get ready!";
                timeleft.text = kickoffTimer + "";
                break;
        }
    }

    private void InitAbilityDisplays()
    {
        int iconSet = 0;

        if (PlayerPrefs.GetString("controls") == "JOYSTICK")
            iconSet = 1;
        if (PlayerPrefs.GetString("controls") == "SIPANDPUFF")
            iconSet = 2;

        int abilityCount = GameManager.Instance.Player.abilityManager.Abilities.Length;

        abilityDisplays = new AbilityDisplay[abilityCount];

        for (int i = 0; i < abilityCount; i++)
        {
            AbilityDisplay display = (AbilityDisplay)Instantiate(abilityDisplayPrefab, Vector3.zero, Quaternion.identity);
            abilityDisplays[i] = display;

            display.transform.SetParent(abilitiesPanel.transform);
            display.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, i * 100);

            display.icon.sprite = GameManager.Instance.Player.abilityManager.Abilities[i].Icons[iconSet];
        }
    }

    private void ShowAbillities()
    {
        Ability[] playerAbilities = GameManager.Instance.Player.abilityManager.Abilities;

        for (int i = 0; i < abilityDisplays.Length; i++)
        {
            if (playerAbilities[i].IsActivated)
            {
                abilityDisplays[i].gameObject.SetActive(true);
                if (playerAbilities[i].IsOnCooldown)
                {
                    UpdateAbilityCooldownCountdown(playerAbilities[i].IsOnCooldown, abilityDisplays[i], playerAbilities[i]);
                    if (playerAbilities[i].CooldownProgress <= 0.5)
                    {
                        abilityDisplays[i].icon.color = Color.Lerp(abilityDisplays[i].icon.color,
                            new Color(abilityDisplays[i].icon.color.r, abilityDisplays[i].icon.color.g, abilityDisplays[i].icon.color.b, 0.00f), Time.deltaTime * 4);
                    }
                    else
                    {
                        abilityDisplays[i].icon.color = Color.Lerp(abilityDisplays[i].icon.color, Color.white, playerAbilities[i].CooldownProgress - 0.7f);
                    }
                }
                else
                {
                    UpdateAbilityCooldownCountdown(playerAbilities[i].IsOnCooldown, abilityDisplays[i], playerAbilities[i]);
                    abilityDisplays[i].icon.color = Color.white;
                }
            }
            else
            {
                abilityDisplays[i].gameObject.SetActive(false);
            }
        }
    }

    private void OnPause(bool isPaused)
    {
        if (isPaused)
        {
            pauseScreen.SetActive(true);
            Cursor.visible = true;
        }
        else
        {
            pauseScreen.SetActive(false);

            if (GameManager.Instance.Player.PlayerInput.Controls != "KEYBOARD")
            {
                Cursor.visible = false;
            }
        }
    }
}
