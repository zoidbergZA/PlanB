using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;
using System.Collections;

public class PlayerInput : MonoBehaviour {

	public enum PlayerInputType{

		JOYSTICK,
		SIPANDPUFF,
		KEYBOARD
	}

    [SerializeField] private LayerMask mouseMask;

	private float moveXaxis;
	private float moveZaxis;
	private float mouseXaxis;
	private float mouseYaxis;
    private string controls;

	private Player player;
    private AbilityManager abilityManager;

    public string Controls { get { return controls; } }

	public float MoveXaxis{
		get {
			return moveXaxis;
		} private set {
			moveXaxis = value;
		}
	}

	public float MoveZaxis{
		get {
			return moveZaxis;
		} private set {
			moveZaxis = value;
		}
	}

	public float MouseXaxis{
		get {
			return mouseXaxis;
		} private set {
			mouseXaxis = value;
		}
	}

	public float MouseYaxis{
		get {
			return mouseYaxis;
		} private set {
			mouseYaxis = value;
		}
	}

	void Awake()
    {
		player = gameObject.GetComponent<Player>();
        abilityManager = player.abilityManager;
        if(PlayerPrefs.HasKey("controls"))
        {
            PlayerPrefs.GetString("controls");
        }
        else
        {
            PlayerPrefs.SetString("controls", "SIPANDPUFF");
        }

        controls = PlayerPrefs.GetString("controls");
	}

    void Start()
    {
        SetAbilityKeys();
    }

	void Update ()
    {
		UpdateInputMethod ();
	}

    private void SetAbilityKeys()
    {
        if (controls == "JOYSTICK")
        {
            GameManager.pauseButton = KeyCode.JoystickButton4;
            player.myInputType = PlayerInputType.JOYSTICK;
            abilityManager.abilityKeys[0] = KeyCode.JoystickButton0; //flamethrower
            abilityManager.abilityKeys[1] = KeyCode.JoystickButton1; //landminer
            abilityManager.abilityKeys[2] = KeyCode.JoystickButton2; //sawblade
            abilityManager.abilityKeys[3] = KeyCode.JoystickButton3; //guidedrocket
        }
        if (controls == "SIPANDPUFF")
        {
            GameManager.pauseButton = KeyCode.JoystickButton9;
            player.myInputType = PlayerInputType.SIPANDPUFF;
            abilityManager.abilityKeys[0] = KeyCode.JoystickButton5;    //flamethrower R1 red
            abilityManager.abilityKeys[1] = KeyCode.JoystickButton10;   //landminer L3 green
            abilityManager.abilityKeys[2] = KeyCode.JoystickButton4;    //sawblade L1 red
            abilityManager.abilityKeys[3] = KeyCode.JoystickButton11;   //guidedrocket R3 green
        }
        if (controls == "KEYBOARD")
        {
            GameManager.pauseButton = KeyCode.P;
            player.myInputType = PlayerInputType.KEYBOARD;
            abilityManager.abilityKeys[0] = KeyCode.Alpha1; //flamethrower
            abilityManager.abilityKeys[1] = KeyCode.Alpha2; //landminer
            abilityManager.abilityKeys[2] = KeyCode.Alpha3; //sawblade
            abilityManager.abilityKeys[3] = KeyCode.Alpha4; //guidedrocket
        }
    }

	void UpdateInputMethod()
    {
        if (controls == "JOYSTICK")
        {
            UpdateJoystickInput();
		}
        if (controls == "SIPANDPUFF")
        {
			UpdateSipAndPuffInput();
		}
        if (controls == "KEYBOARD")
        {
			UpdateKeyboardInput();
		}
	}

	void UpdateJoystickInput()
    {
		moveXaxis = Input.GetAxis("Horizontal");
		moveZaxis = Input.GetAxis ("Vertical");
        mouseXaxis = Input.GetAxis("RightJoyStickX");
        mouseYaxis = Input.GetAxis("RightJoyStickY");
	}

	void UpdateSipAndPuffInput()
    {
        moveXaxis = Input.GetAxis("Horizontal");
        moveZaxis = Input.GetAxis("Vertical");
        mouseXaxis = Input.GetAxis("RightStickX");
        mouseYaxis = Input.GetAxis("RightStickY");
	}

	void UpdateKeyboardInput()
	{
	    RaycastHit hit;
	    Vector3 delta = Vector3.zero;

        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out hit, mouseMask))
        {
            delta = hit.point - transform.position;
            delta.y = 0;
            delta.Normalize();
        }

        moveXaxis = Input.GetAxis("HorizontalK");
        moveZaxis = Input.GetAxis("VerticalK");
        mouseXaxis = delta.x;
        mouseYaxis = delta.z;
	}

}
