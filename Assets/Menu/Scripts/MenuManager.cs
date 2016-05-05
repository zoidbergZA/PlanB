using System;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class MenuManager : MonoBehaviour
{

	public MenuButtons currentMenu;
	
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

	[SerializeField] private Image Title;

	// Use this for initialization
	void Start ()
    {
		ShowMenu (currentMenu);

		StartCoroutine (Logo());

        //force show cursor
	    Cursor.visible = true;
    }

	public void ShowMenu(MenuButtons menu)
    {
		if (currentMenu != null)
        {
			currentMenu.IsOpen = false;
		}
		currentMenu = menu;
		currentMenu.IsOpen = true;
	}

	public void onHighScoreClick(){
		GetHighScores ();
	}
	
	private void GetHighScores()
	{
		//		for(int i = 0; i < 10; i++)
		//		{
		//			Debug.Log(PlayerPrefs.GetString(i + "HScoreName") + " has a score of: " +  PlayerPrefs.GetInt(i + "HScore"));
		//		}
		
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

	private IEnumerator Logo()
	{
		yield return new WaitForSeconds(5);
		Title.gameObject.SetActive (false);
	}









}
