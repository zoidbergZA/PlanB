using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class MenuButtons : MonoBehaviour
{

	Animator _animator;
	CanvasGroup _canvasGroup; 

	public bool IsOpen
    {
		get { return _animator.GetBool("IsOpen"); }
		set { _animator.SetBool("IsOpen", value); }
	}

	public void Awake()
    {
		_animator = GetComponent<Animator>();
		_canvasGroup = GetComponent<CanvasGroup>();

		var rect = GetComponent<RectTransform>();
		rect.offsetMax = rect.offsetMin = new Vector2 (0, 0);
	}
	
	// Update is called once per frame
	void Update()
    {
		if (!_animator.GetCurrentAnimatorStateInfo (0).IsName ("Open"))
        {
			_canvasGroup.blocksRaycasts = _canvasGroup.interactable = false;
		}
        else
        {
			_canvasGroup.blocksRaycasts = _canvasGroup.interactable = true;
		}
	}

	public void onJoystickSelect()
    {
        if (true) PlayerPrefs.SetString("controls", "SIPANDPUFF");
        //else return; //unreachable code thus commented out
	}

	public void onControllerSelect()
    {
        if (true) PlayerPrefs.SetString("controls", "JOYSTICK");
        //else return; //unreachable code thus commented out
	}

	public void onKeyboardSelect()
    {
        if (true) PlayerPrefs.SetString("controls", "KEYBOARD");
        //else return; //unreachable code thus commented out
	}

	public void onExitClick()
    {
		Application.Quit ();
	}

	public void onStartClick()
    {
        Application.LoadLevel("Loading");
	}


}
