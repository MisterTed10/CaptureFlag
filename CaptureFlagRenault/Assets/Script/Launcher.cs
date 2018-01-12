using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Launcher : MonoBehaviour {

    // Use this for initialization
    private Core.Game m_game;
	void Start () {
        
	}
	
	// Update is called once per frame
	void Update () {
		
	}
    void OnGUI()
    {
        if (m_game == null || !m_game.Running) return;
        string info = "Score:\n";
        foreach (KeyValuePair<Core.Team, int> entry in m_game.Score)
        {
            info += "Team " + entry.Key.Id + " has " + entry.Value + ".\n";
        }
        GUI.Label(new Rect(25, 25, 100, 300), info);
        GUI.Label(new Rect(25, 325, 200, 600), m_game.Log);
    }
    public void StartGame()
    {
        m_game = new Core.Game();

        GameObject button           = GameObject.Find("StartButton");

        GameObject TextPointToWin   = GameObject.Find("TextPointToWin");
        GameObject TextKnightLife   = GameObject.Find("TextKnightLife");
        GameObject TextSamouraiLife = GameObject.Find("TextSamouraiLife");
        GameObject TextNbOfKnight   = GameObject.Find("TextNbOfKnight");
        GameObject TextNbOfSamourai = GameObject.Find("TextNbOfSamourai");

        InputField inputfPointToWin     = TextPointToWin.GetComponentInChildren<InputField>();
        InputField inputfKnightLife     = TextKnightLife.GetComponentInChildren<InputField>();
        InputField inputfSamouraiLife   = TextSamouraiLife.GetComponentInChildren<InputField>();
        InputField inputfNbOfKnight     = TextNbOfKnight.GetComponentInChildren<InputField>();
        InputField inputfNbOfSamourai   = TextNbOfSamourai.GetComponentInChildren<InputField>();

        m_game.GameConfiguration.SetItemValue("PointToWin", inputfPointToWin.text);
        m_game.GameConfiguration.SetItemValue("KnightStamina", inputfKnightLife.text);
        m_game.GameConfiguration.SetItemValue("SamouraiStamina", inputfSamouraiLife.text);
        m_game.GameConfiguration.SetItemValue("Team[0].KnightCount", inputfNbOfKnight.text);
        m_game.GameConfiguration.SetItemValue("Team[1].SamouraiCount", inputfNbOfSamourai.text);


        m_game.Initialize();

        button.SetActive(false);
        TextPointToWin.SetActive(false);
        TextKnightLife.SetActive(false);
        TextSamouraiLife.SetActive(false);
        TextNbOfKnight.SetActive(false);
        TextNbOfSamourai.SetActive(false);
    }
}
