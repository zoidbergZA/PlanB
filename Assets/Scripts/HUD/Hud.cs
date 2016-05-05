using UnityEngine;
using System.Collections;
using UnityEngine.UI;

[RequireComponent(typeof(AudioSource))]
public class Hud : MonoBehaviour
{
    [SerializeField] private AbilityDisplay abilityDisplayPrefab;
    [SerializeField] private GameObject abilitiesPanel;
    [SerializeField] private Text waveState;
	[SerializeField] private Text timeleft;

	[SerializeField] private Text money;
	
	[SerializeField] private RectTransform health;

	[SerializeField] private GameObject pauseScreen;
	[SerializeField] private GameObject panelScreen;
	[SerializeField] private GameObject waveScreen;
	[SerializeField] private GameObject gameOverScreen;

	[SerializeField] private Text endMessage;

	[SerializeField] private Sprite kb;
	[SerializeField] private Sprite js;
	[SerializeField] private Sprite c;

	[SerializeField] private UnityEngine.UI.Image playerScreen;

    private AudioSource myAudioSource;
    private AbilityDisplay[] abilityDisplays; 

	private string inprogTimer;
	private string shopingTimer;
	private string waitingTimer;
    private string kickoffTimer;

	void Awake()
	{
	    myAudioSource = GetComponent<AudioSource>();
		pauseScreen.SetActive (false);
		gameOverScreen.SetActive (false);
	}

	void Start()
	{
        InitAbilityDisplays();

		UpdateInputMethod ();
	}

	void Update () 
    {

		inprogTimer = GameManager.Instance.WaveManager.InProgressTimer.ToString ("00.0");
		shopingTimer = GameManager.Instance.WaveManager.ShoppingTimer.ToString ("00.0");
        kickoffTimer = GameManager.Instance.WaveManager.KickOffTimer.ToString("00.0");

		UpdateWaveStates ();

		UpdateGold ();
		ShowAbillity ();
		UpdateHealth ();
    }

	void UpdateInputMethod()
	{
		if(PlayerPrefs.GetString("controls") == "JOYSTICK")
		{
			playerScreen.sprite = c;
		}
		if(PlayerPrefs.GetString("controls") == "SIPANDPUFF")
		{
			playerScreen.sprite = js;
		}
		if(PlayerPrefs.GetString("controls") == "KEYBOARD")
		{
			playerScreen.sprite = kb;
		}
	}

    public void PlaySound(AudioClip sound)
    {
        myAudioSource.PlayOneShot(sound);
    }

    public void ShowGameOver(bool success)
    {
        if(success){
			panelScreen.SetActive (false);
			waveScreen.SetActive (false);
			abilitiesPanel.SetActive (false);
			gameOverScreen.SetActive (true);
			endMessage.text = "YOU WIN";
		} else {
			panelScreen.SetActive (false);
			waveScreen.SetActive (false);
			abilitiesPanel.SetActive (false);
			gameOverScreen.SetActive (true);
			endMessage.text = "YOU LOOSE";
		}
    }

    private void OnEnable()
    {
        GameManager.pauseEvent += handlePauseFunction;
    }

    private void OnDisable()
    {
        GameManager.pauseEvent -= handlePauseFunction;
    }

    private void InitAbilityDisplays()
    {
        int abilityCount = GameManager.Instance.Player.abilityManager.Abilities.Length;

        abilityDisplays = new AbilityDisplay[abilityCount];

        for (int i = 0; i < abilityCount; i++)
        {
            AbilityDisplay display = (AbilityDisplay)Instantiate(abilityDisplayPrefab, Vector3.zero, Quaternion.identity);
            abilityDisplays[i] = display;

            display.transform.SetParent(abilitiesPanel.transform);
            display.GetComponent<RectTransform>().anchoredPosition = new Vector2(i * 100, 0);
        }
    }

    private void handlePauseFunction(bool IsPaused)
    {
        if (IsPaused == true)
        {
            pauseScreen.SetActive(true);
        }
        else
        {
            pauseScreen.SetActive(false);
        }
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

	private void UpdateGold()
	{
		money.text = "" + Mathf.RoundToInt(GameManager.Instance.Player.Gold);
	}

	private void ShowAbillity()
	{
	    Ability[] playerAbilities = GameManager.Instance.Player.abilityManager.Abilities;

	    for (int i = 0; i < abilityDisplays.Length; i++)
	    {
	        if (playerAbilities[i].IsActivated)
	        {
	            abilityDisplays[i].gameObject.SetActive(true);

//	            abilityDisplays[i].icon.sprite = playerAbilities[i].Icon;
//	            abilityDisplays[i].AmmoText.text = playerAbilities[i].Ammo.ToString();
	        }
	        else
	        {
                abilityDisplays[i].gameObject.SetActive(false);
	        }

	    }

//		if (GameManager.Instance.Player.DriverAbilities [0].IsActivated) {
//			IDriverAbillity1.enabled = true;
//			TDriverAmmo1.enabled = true;
//			TDriverAmmo1.text = GameManager.Instance.Player.DriverAbilities[0].Ammo.ToString();
//		}
//		if (GameManager.Instance.Player.DriverAbilities [1].IsActivated) {
//			IDriverAbillity2.enabled = true;
//			TDriverAmmo2.enabled = true;
//            TDriverAmmo2.text = GameManager.Instance.Player.DriverAbilities[1].Ammo.ToString();
//		}
//		if (GameManager.Instance.Player.GunnerAbilities [0].IsActivated) {
//			IGunnerAbillity1.enabled = true;
//			TGunnerAmmo1.enabled = true;
//            TGunnerAmmo1.text = GameManager.Instance.Player.GunnerAbilities[0].Ammo.ToString();
//		}
//		if (GameManager.Instance.Player.GunnerAbilities [1].IsActivated) {
//			IGunnerAbillity2.enabled = true;
//			TGunnerAmmo2.enabled = true;
//            TGunnerAmmo2.text = GameManager.Instance.Player.GunnerAbilities[1].Ammo.ToString();
//		}
	}

	private void UpdateHealth()
	{
//		health.sizeDelta = new Vector2(100, GameManager.Instance.Player.Hitpoints);
	    float healthFrac = GameManager.Instance.Player.Hitpoints/GameManager.Instance.Player.MaxHP;
	    health.localScale = new Vector3(1, healthFrac);
    }
    
	public void onContinueClick()
	{
        GameManager.Instance.PauseGame(false);
	}

	public void onMenuClick()
	{
		Application.LoadLevel ("MainMenu");
        GameManager.Instance.PauseGame(false);
	}

	public void onRestartClick()
	{
		Application.LoadLevel (Application.loadedLevel);
        GameManager.Instance.PauseGame(false);
	}



}
