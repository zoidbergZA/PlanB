using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class Outtro : MonoBehaviour
{
    public GameObject[] DeleteObjects;

    [SerializeField] private Text highscoreText;
    [SerializeField] private InputField enterName;
    [SerializeField] private Button send;
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

    private ScoreObject scoreObject;
    private string name;
    private int time;

    void Start()
    {
        Cursor.visible = true;
        scoreObject = FindObjectOfType<ScoreObject>();

        PlayerScore ps = enterName.gameObject.GetComponent<PlayerScore>();

        if (scoreObject)
            highscoreText.text = "highscore: " + (int)scoreObject.score;
        
        GetHighScores();

        StartCoroutine(DeleteGameObjects());
    }

    public void OnSendClick()
    {
        if (!scoreObject)
            return;

        PlayerScore ps = enterName.gameObject.GetComponent<PlayerScore>();

        float timePlayed = scoreObject.score;
        name = enterName.text;
        time = Mathf.RoundToInt(timePlayed);
        ps.AddScore(name, time);
        send.enabled = false;
    }

    public void OnClickRetry()
    {
        Application.LoadLevel("Showcase");
    }

    public void OnClickMenu()
    {
        Application.LoadLevel("MainMenu");
    }

    private IEnumerator DeleteGameObjects()
    {
        yield return new WaitForSeconds(9);

        foreach (GameObject deleteObject in DeleteObjects)
        {
            Destroy(deleteObject);
        }
    }

    private void GetHighScores()
    {
        //		for(int i = 0; i < 10; i++)
        //		{
        //			Debug.Log(PlayerPrefs.GetString(i + "HScoreName") + " has a score of: " +  PlayerPrefs.GetInt(i + "HScore"));
        //		}
        
        nr1.text = "NR 1: " + PlayerPrefs.GetString(0 + "HScoreName") + " has a score of: " + PlayerPrefs.GetInt(0 + "HScore");
        nr2.text = "NR 2: " + PlayerPrefs.GetString(1 + "HScoreName") + " has a score of: " + PlayerPrefs.GetInt(1 + "HScore");
        nr3.text = "NR 3: " + PlayerPrefs.GetString(2 + "HScoreName") + " has a score of: " + PlayerPrefs.GetInt(2 + "HScore");
        nr4.text = "NR 4: " + PlayerPrefs.GetString(3 + "HScoreName") + " has a score of: " + PlayerPrefs.GetInt(3 + "HScore");
        nr5.text = "NR 5: " + PlayerPrefs.GetString(4 + "HScoreName") + " has a score of: " + PlayerPrefs.GetInt(4 + "HScore");
        nr6.text = "NR 6: " + PlayerPrefs.GetString(5 + "HScoreName") + " has a score of: " + PlayerPrefs.GetInt(5 + "HScore");
        nr7.text = "NR 7: " + PlayerPrefs.GetString(6 + "HScoreName") + " has a score of: " + PlayerPrefs.GetInt(6 + "HScore");
        nr8.text = "NR 8: " + PlayerPrefs.GetString(7 + "HScoreName") + " has a score of: " + PlayerPrefs.GetInt(7 + "HScore");
        nr9.text = "NR 9: " + PlayerPrefs.GetString(8 + "HScoreName") + " has a score of: " + PlayerPrefs.GetInt(8 + "HScore");
        nr10.text = "NR 10: " + PlayerPrefs.GetString(9 + "HScoreName") + " has a score of: " + PlayerPrefs.GetInt(9 + "HScore");
    }
}
