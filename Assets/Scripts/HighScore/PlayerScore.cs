using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PlayerScore : MonoBehaviour 
{

	public void AddScore(string name, int score)
	{
		int newScore = score;
		string newName = name;
		int oldScore;
		string oldName;

		for(int i=0; i<10; i++){
			if(PlayerPrefs.HasKey(i+"HScore")){
				if(PlayerPrefs.GetInt(i+"HScore")<newScore){ 
					oldScore = PlayerPrefs.GetInt(i+"HScore");
					oldName = PlayerPrefs.GetString(i+"HScoreName");
					PlayerPrefs.SetInt(i+"HScore",newScore);
					PlayerPrefs.SetString(i+"HScoreName",newName);
					newScore = oldScore;
					newName = oldName;
				}
			}else{
				PlayerPrefs.SetInt(i+"HScore",newScore);
				PlayerPrefs.SetString(i+"HScoreName",newName);
				newScore = 0;
				newName = "";
			}
		}
	}












}
