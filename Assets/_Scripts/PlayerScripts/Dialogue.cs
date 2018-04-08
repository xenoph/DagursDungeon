using UnityEngine;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

/// <summary>
/// This script was intended to handle the output of the dialogue given to the Player during the game.
/// It was extended to also handle saving the commands into the GUI.
/// -BV
/// </summary>
public class Dialogue : MonoBehaviour {
    private InterfaceScript _uScript;
    public List<string> CommandHistoryList;
    public TextAsset DialogueTexts;

    //public int CommandsPool = 4;
    public int CommandsUsed = 0;
    public int InitialCommands = 4;

    private int _errorTextCounter;

    void Awake() {
        //If this is the tutorial, so the very first level, set the number of commands available to 4
        //Otherwise the Player should receive whatever they had left from the previous level.
        if (SceneManager.GetActiveScene().name == "Tutorial") {
            PlayerPrefs.SetInt("CommandsPool", 4);
        }
        GameObject userInterface = GameObject.FindGameObjectWithTag("UI");
        _uScript = userInterface.GetComponent<InterfaceScript>();
        CommandHistoryList = new List<string>();
    }

    /// <summary>
    /// Saves the command into a list
    /// </summary>
    /// <param name="command"></param>
    public void SaveIntoCommandLog(string command) {
        //Check that the Player has more free slots to save into, if not give feedback and return
        if (PlayerPrefs.GetInt("CommandsPool") == 0) {
            ErrorText("ooc");
            return;
        }
        //Add the command to the list
        CommandHistoryList.Add(command);
        //Add the command to the saved command box in the gui
        _uScript.CommandPoolTexts[_uScript.CommandPoolTextNumber].text = command;
        //Add to the counter variables
        _uScript.CommandPoolTextNumber ++;
        CommandsUsed ++;
        //Set the playerprefs variables
        PlayerPrefs.SetInt("CommandsPool", PlayerPrefs.GetInt("CommandsPool") - 1);
        //Check how many commands the Player has left and inform them.
        if (PlayerPrefs.GetInt("CommandsPool") != 0) {
            MovementText("You plan to do " + command + ". You can do more.");
        }
        else {
            MovementText("You plan to do " + command + ". You can't do more.");
        }
    }

    /// <summary>
    /// Updates the command pool. It will check if the Player has any remaining commands and add them to
    /// the next turn - up to a max of 6 (initial is 4).
    /// </summary>
    public void UpdateCommandsPool() {
        CommandHistoryList.Clear();
        int leftOverCommands = InitialCommands - CommandsUsed;
        PlayerPrefs.SetInt("CommandsPool", 4 + leftOverCommands);
        //If the Player have too many free commands that would take them above 6, set it to 6.
        if (PlayerPrefs.GetInt("CommandsPool") > 6) {
            PlayerPrefs.SetInt("CommandsPool", 6);
        }
        CommandsUsed = 0;
        InitialCommands = PlayerPrefs.GetInt("CommandsPool");
    }

    /// <summary>
    /// Removes the last command the Player saved into the pool. Called from the Interface script when the Player uses "cancel".
    /// </summary>
    public void RemoveLastCommand() {
        CommandHistoryList.RemoveAt(CommandHistoryList.Count - 1);
        _uScript.CommandPoolTexts[CommandHistoryList.Count].text = "FREE COMMAND";
        _uScript.CommandPoolTextNumber--;
        CommandsUsed--;
        PlayerPrefs.SetInt("CommandsPool", PlayerPrefs.GetInt("CommandsPool") + 1);
    }

    /// <summary>
    /// Prints out the current number of free commands. Not used much. Will check if it is plural or not.
    /// </summary>
    public void ShowNumberOfCommands() {
        if (PlayerPrefs.GetInt("CommandsPool") < 2) {
            GameInformationText("There is " + PlayerPrefs.GetInt("CommandsPool") + " free command slot left.");
        }
        else {
            GameInformationText("There are " + PlayerPrefs.GetInt("CommandsPool") + " free command slots left.");
        }
    }

    // These five functions will print out text to the GUI.
    //The colour of the text varies based on what information is to be given. 
    public void GameInformationText(string text) {
        _uScript.PlaceText(text, Color.white);
    }

    public void CombatTextEnemy(string text) {
		if (PlayerPrefs.GetInt("colorblind") == 1){_uScript.PlaceText(text, new Color(0, 175, 255));}
		else {_uScript.PlaceText(text, new Color(1, 0.15f, 0.15f));}
    }

    public void CombatTextPlayer(string text) {
		if (PlayerPrefs.GetInt("colorblind") == 1){_uScript.PlaceText(text, new Color(100, 164, 255));}
		else {_uScript.PlaceText(text, new Color(0.3f, 0.8f, 0.3f));}
    }

    public void ErrorText(string text) {
        _errorTextCounter += 1;       
        var myData = DialogueTexts.text.Split("\n"[0]);
        if (text == "invalid") {
            _uScript.PlaceText(myData[Random.Range(1, 5)], Color.yellow);
            ManyErrors();
            return;
        }
        if (text == "ooc") {
            _uScript.PlaceText(myData[Random.Range(6, 9)], Color.yellow);
            ManyErrors();
            return;
        }

        _uScript.PlaceText(text, Color.yellow);
        ManyErrors();
    }

    public void MovementText(string text) {
        _uScript.PlaceText(text, Color.cyan);
    }

    //This counts the errors and will display some messages based on how many errors the Player gets.
    //It is meant to be expanded further so it will actually keep a proper count that can decrease
    // if the Player is getting things right. Idea is also to create a random string to display.
	//CHANGED on the eve of delivery. Will now give a hint to the player about using the help functions if they are stuck.
    private void ManyErrors() {
        if (_errorTextCounter == 5) {
            ErrorText("Remember to use the Help function if you are a bit unsure.");
            _errorTextCounter -= 1;
        }
        if (_errorTextCounter == 10) {
            ErrorText("Have you tried listing the available commands?");
            _errorTextCounter -= 1;
        }
        if (_errorTextCounter == 20) {
            ErrorText("I can help you with commands, just ask me to \"Help\" you with \"command\".");
            _errorTextCounter -= 1;
        }
    }
}
