using System;
using System.Collections;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class InterfaceScript : MonoBehaviour {

    public GameObject ScriptHolder;
    public GameObject Player;
    private Commands _commandScript;
    private Dialogue _dialogueScript;
    private GameTurnController _gameTurnController;
    private DissectInputScript _dissectingScript;

    public TextAsset HelpFilesTextAsset;

    public string PlayerChosenClass;
    public GameObject SavedCommandBox;
    public GameObject ContentPanel;
    public ScrollRect ScrollBoxRext;
    public GameObject TextPrefab;   
    public GameObject DamageText;
    public GameObject XpText;
    public GameObject LevelUpPanel;
    public bool ShowingHelp = true;
    public Text HealthText;
    public Text[] CommandPoolTexts;
    public int CommandPoolTextNumber;

    public Text InfoKeyText;
    public Text InfoLevelText;

    public Text LevelText;
    public Image LevelUpImage;

    public InputField CommandInputField;
    public string PlayerInputString;

    public bool IsGettingLoot = false;

    public string LastCommands;

    public GameObject HelpPanel;
    public GameObject HelpMovePanel;
    public GameObject HelpInteractPanel;
    public GameObject HelpCombatPanel;
    public GameObject HelpCommandPanel;
    public GameObject HelpTutorialPanel;

	public GameObject CommandPoolPanel;
	public GameObject AbilityPanel;

	private bool _shownTutorialHelp;

    void Awake() {
        _dialogueScript = GetComponent<Dialogue>();
        int textCounter = 0;
        LevelUpImage.enabled = false;
        _gameTurnController = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameTurnController>();
        foreach(Text commandPoolText in CommandPoolTexts) {
            commandPoolText.text = textCounter < PlayerPrefs.GetInt("CommandsPool") ? "FREE COMMAND " : "CLOSED COMMAND";
            textCounter++;
        }
        InfoLevelText.text = "Your Level: " + PlayerPrefs.GetInt("Level");
    }

    /// <summary>
    /// Initialises a text prefab that will be placed in the scrollview. Need text content and color provided. Can also take font size.
    /// </summary>
    /// <param name="content">The content.</param>
    /// <param name="color">Font color.</param>
    public void PlaceText(string content, Color color) {
        //Instantiate the text prefab and get the Text component
        GameObject inst = Instantiate(TextPrefab);
        Text textComponent = inst.GetComponent<Text>();
        //Set the color, size and content based on which function called it
        textComponent.color = color;
        textComponent.fontSize = 20;
        textComponent.text = content;
        //Set the parent to the chat windows and scroll it to make sure the new text is shown
        inst.transform.SetParent(ContentPanel.transform, false);
        StartCoroutine(UpdateScrollRect());
    }

    /// <summary>
    /// Initialises a text prefab that will be placed in the scrollview. Need text content, color and font size provided. 
    /// </summary>
    /// <param name="content">The content.</param>
    /// <param name="color">Font color.</param>
    /// <param name="size">Font size.</param>
    public void PlaceText(string content, Color color, int size) {
        GameObject inst = Instantiate(TextPrefab);
        Text textComponent = inst.GetComponent<Text>();

        textComponent.color = color;
        textComponent.fontSize = size;
        textComponent.text = content;

        inst.transform.SetParent(ContentPanel.transform, false);
        StartCoroutine(UpdateScrollRect());
    }

	/// <summary>
	/// Waits until the end of frame and then updates the Scroll Rect.
	/// </summary>
	/// <returns>The scroll rect.</returns>
    IEnumerator UpdateScrollRect() {
        yield return new WaitForEndOfFrame();
        ScrollBoxRext.normalizedPosition = new Vector2(0, 0);
    }

    public void ShowLevelUp() {
        StartCoroutine(ShowingLevelUp());
    }

    IEnumerator ShowingLevelUp() {
        //Enable the image and set a variable for the current scale
        LevelUpImage.enabled = true;
        var currentScale = LevelUpImage.transform.localScale;
        //Iterate through 50 iterations to gradually scale the image larger and create an animation effect
        for (int i = 0; i < 50; i++) {
            LevelUpImage.transform.localScale = new Vector3(LevelUpImage.transform.localScale.x + 0.2f, LevelUpImage.transform.localScale.y + 0.2f, LevelUpImage.transform.localScale.z);
            yield return new WaitForEndOfFrame();
        }
        //Wait 3 seconds, then disable the image and set the scale back to normal
        yield return new WaitForSeconds(3);
        LevelUpImage.enabled = false;
        LevelUpImage.transform.localScale = currentScale;
    }

    /// <summary>
    /// Activates the input.
    /// </summary>
    public void ActivateInput() {
        //Select the inputfield and activate it, then make sure the text is cleared.
        CommandInputField.Select();
        CommandInputField.ActivateInputField();
        CommandInputField.text = "";
    }

    public void ShowHelp(string arg) {
        string[] panelArguments = {"MOVE", "INTERACT", "COMBAT", "COMMAND", "TUTORIAL"};
        
        if (!ShowingHelp && arg == "null") {
			//This should make sure that the special tutorial help is only shown in the Tutorial
			// and only once. 
			if (SceneManager.GetActiveScene().name == "Tutorial" && !_shownTutorialHelp) {
				HelpTutorialPanel.SetActive(true);
				ShowingHelp = true;
				_shownTutorialHelp = true;
				return;
			}
            HelpPanel.SetActive(true);
            ShowingHelp = true;
            return;
        }
        if (ShowingHelp && arg == "null") {
			_commandScript.Close();
            ShowingHelp = false;
            return;
        }
        if (panelArguments.Any(s => s.Equals(arg, StringComparison.CurrentCultureIgnoreCase))) {
			_commandScript.Close();
            switch (arg.ToUpper()) {
                case "MOVE":
                    HelpMovePanel.SetActive(true);
                    break;

				case "TUTORIAL":
					if (SceneManager.GetActiveScene().name == "Tutorial") {
						if (_shownTutorialHelp) {
							_dialogueScript.ErrorText("I believe you have already read the tutorial help.");
							return;
						}
						_dialogueScript.ErrorText("Try using just \"help\".");
						return;
					}
					_dialogueScript.ErrorText("I'm not going to show you any tutorial help here!");
                    break;

                case "INTERACT":
                    HelpInteractPanel.SetActive(true);
                    break;

                case "COMBAT":
                    HelpCombatPanel.SetActive(true);
                    break;

                case "COMMAND":
                    HelpCommandPanel.SetActive(true);
                    break;
            }
        }
        else {
            //Create an array of strings from the HelpFiles text file
            var helpLines = HelpFilesTextAsset.text.Split("\n"[0]);
            //Iterate through the array
            for (int i = 0; i < helpLines.Length; i++) {
                //Trim the string to make sure there's no whitespace
                string trimmedHelpLine = helpLines[i].Trim();
                //Compare input from Player with the line in the text file
                bool result = arg.ToUpper().Equals(trimmedHelpLine, StringComparison.Ordinal);
                //If they are equal
                if(result) {
                    //Print out the next line in the array, which would be the help for that command
                    _dialogueScript.GameInformationText(helpLines[i+1]);
                    if(_gameTurnController.CurrentState == GameTurnController.PlayerState.Combat) {
                        ActivateInput();
                    }
                    return;
                }
            }
            //If there's no help entry for a command, print out this.
            _dialogueScript.ErrorText("No help for that.");
            if(_gameTurnController.CurrentState == GameTurnController.PlayerState.Combat) {
                ActivateInput();
            }
        }       
    }

    public void DeActivateInput() {
        //Make sure that there's a reference to Commands
        if (!_commandScript) {
            _commandScript = GameObject.FindGameObjectWithTag("Player").GetComponent<Commands>();
        }
        //Deactivate the input field and let Commands know
        _commandScript.IsInputActive = false;
        CommandInputField.DeactivateInputField();
    }

    public void PlayerInput() {
        //Make sure that the Interface script have the dissecting script
        if (!_dissectingScript) {
            _dissectingScript = GameObject.FindGameObjectWithTag("Player").GetComponent<DissectInputScript>();
        }
        //Get the string from the input field, also save it into a different string variable to use with the up-arrow option in Commands.cs Update()
        PlayerInputString = CommandInputField.text;
        LastCommands = PlayerInputString;
        //If the state is currently enemycombat, just return without doing anything
        if (_gameTurnController.CurrentState == GameTurnController.PlayerState.EnemyCombatTurn) {
            return;
        }
        //If the Player is currently looting, run a special function and return
        if (IsGettingLoot) {
            GetLoot(PlayerInputString);
            return;
        }
        //Check if the Player is using multiple inputs (using a comma in the input)
        if (IsItMultipleCommands(PlayerInputString)) {
            return; }
        //Make sure the state is PlayerTurn
        if (_gameTurnController.CurrentState == GameTurnController.PlayerState.PlayerTurn) {
            //Check if the Player has used Start or Cancel
            switch (PlayerInputString.ToUpper().Trim()) {
                case "START":
                    //Check if the Player has actually saved some commands, if not activate input and return
                    if (_dialogueScript.CommandHistoryList.Count == 0) {
                        _dialogueScript.ErrorText("You can't do nothing.");
                        ActivateInput();
                        return;
                    }
                    //Else deactivate input, change to commandchain state and start processing through the saved commands
                    DeActivateInput();
                    _gameTurnController.CurrentState = GameTurnController.PlayerState.CommandChain;
                    _commandScript.StartChain();                   
                    return;
                    //If Cancel was used, remove the last command, reactivate the input and return
                case "CANCEL":
                    _dialogueScript.RemoveLastCommand();
                    ActivateInput();
                    return;
            }
            //Otherwise, run the input through the dissect script to check if it's a valid command and be saved
            // then remove the text from the input field, ready to either be deactivated to activated
            _dissectingScript.DissectPlayerInput(PlayerInputString, 1);
            CommandInputField.text = "";
        }
        //If the state is Combat
        else if (_gameTurnController.CurrentState == GameTurnController.PlayerState.Combat) {
            //If they use Start, inform them that it's not to be use din combat, activate input and return
            if (PlayerInputString.ToUpper() == "START") {
                _dialogueScript.ErrorText("You can't use \"Start\" in combat.");
                ActivateInput();
                return;
            }
            //Otherwise run the dissect script with 2 so it will go to the execute function
            _dissectingScript.DissectPlayerInput(PlayerInputString, 2);
        }
    }

    /// <summary>
    /// Only called from PlayerInput(). This is a special function that will check if the Player want to take their loot or not.
    /// </summary>
    /// <param name="arg">The argument.</param>
    void GetLoot(string arg) {
        string trimmedInput = arg.Trim();
        if (trimmedInput.ToUpper() != "YES") {
            _dialogueScript.GameInformationText("You chose to not take advantage of your loot.");
            _gameTurnController.CurrentState = GameTurnController.PlayerState.CommandChain;
            IsGettingLoot = false;
            _commandScript.StartChain();
            return;
        }
        if (_commandScript.LootedPermPotion) {
            _commandScript.Stats(null);
            PlayerPrefs.SetInt(PlayerPrefs.GetString("Buff"), PlayerPrefs.GetInt("Buff"));
            PlayerPrefs.SetInt(PlayerPrefs.GetString("Debuff"), PlayerPrefs.GetInt("Debuff"));
            _commandScript.Stats(null);
            IsGettingLoot = false;
            _commandScript.LootedPermPotion = false;
            _commandScript.StartChain();
        }
        IsGettingLoot = false;
        _commandScript.StartChain();
    }

    //TODO: Entire function should be re-written once I am smart enough to add in all the checks that are needed.
    private bool IsItMultipleCommands(string input) {
        DeActivateInput();
        if (_gameTurnController.CurrentState == GameTurnController.PlayerState.Combat) {
            return false;
        }
        //If the input contains one or more commas
        if (input.ToLower().Contains(',')) {           
            bool breakOut = false;
            //Trim the input, then split it into an array of strings
            string trimmedInput = input.Trim();
            string[] allInputs = trimmedInput.Split(',');
            //Check if the Player has written start anywhere in the input. TODO: Needs improved to use the startlist
            bool hasStart = allInputs.Any(s => string.Equals(s.Trim(), "start", StringComparison.CurrentCultureIgnoreCase));
            //if they have, check that it's the last one
            if (hasStart && allInputs[allInputs.Length-1].Trim() != "start") {
                _dialogueScript.ErrorText("Start has to be at the end.");
                ActivateInput();
                breakOut = true;
                return true;
            }
            //Check if the Player didn't use more than their remaining commands. 
            if ((allInputs.Length > PlayerPrefs.GetInt("CommandsPool")) && !hasStart) {
                _dialogueScript.ErrorText("You don't have that many commands available.");
                ActivateInput();
                breakOut = true;
                return true;
            }
            //If there are more strings than commands and they have start
            if (allInputs.Length > PlayerPrefs.GetInt("CommandsPool") && hasStart) {
                //..and the length - 1 (which is start) is not equal to the commandpool
                if (allInputs.Length - 1 != PlayerPrefs.GetInt("CommandsPool")) {
                    //stop the functtion and notfiy the Player
                    _dialogueScript.ErrorText("That's too many commands.");
                    ActivateInput();
                    breakOut = true;
                    return true;
                }
            }
            //Get the StartList and CommandsList into arrays
            string[] startCommands =
                GameObject.FindGameObjectWithTag("CommandListHolder").GetComponent<CommandList>().StartList.ToArray();
            string[] instantCommands =
                GameObject.FindGameObjectWithTag("CommandListHolder").GetComponent<CommandList>().CommandsList.ToArray();
            string[] tutorialCommands =
                GameObject.FindGameObjectWithTag("CommandListHolder")
                    .GetComponent<CommandList>()
                    .TutorialCommands.ToArray();
            //Iterate through the input array
            foreach (string trimmedString in allInputs.Select(s => s.Trim())) {
                //Make sure there are no instant commands.
                if (instantCommands.Any(t => string.Equals(t, trimmedString, StringComparison.CurrentCultureIgnoreCase)) || tutorialCommands.Any(t => string.Equals(t, trimmedString, StringComparison.CurrentCultureIgnoreCase))) {
                    _dialogueScript.ErrorText("That's an instant or tutorial command. Try again.");
                    _commandScript.ClearCommandPool();
                    ActivateInput();
                    breakOut = true;
                    return true;
                }
                if (!breakOut) {
                    if (
                        startCommands.Any(
                            t => string.Equals(t, trimmedString, StringComparison.CurrentCultureIgnoreCase))) {
                        if (_dialogueScript.CommandHistoryList.Count == 0) {
                            _dialogueScript.ErrorText("You can't do nothing.");
                            ActivateInput();
                            return true;
                        }

                        DeActivateInput();
                        _gameTurnController.CurrentState = GameTurnController.PlayerState.CommandChain;
                        _commandScript.StartChain();                      
                        return true;
                    }
                }
                _dissectingScript.DissectPlayerInput(trimmedString, 1);
            }
            return true;
        }
        //Return false if there are no commas
        return false;
    }

	public void FindPlayerScript() {
		Player = GameObject.FindGameObjectWithTag("Player");
	    _commandScript = Player.GetComponent<Commands>();
	    _dissectingScript = Player.GetComponent<DissectInputScript>();
	}

    public void EnableLevelScript() {
        ScriptHolder.SetActive(true);
    }

    public void ShowDamage(int damage) {
        DamageText.GetComponent<Text>().text = damage.ToString();
        DamageText.SetActive(true);
        Invoke("HideDamage", 2);
    }

    void HideDamage() {
        DamageText.SetActive(false);
    }

    public void ShowXpGain(int xp) {
        HideDamage();
        XpText.GetComponent<Text>().text = xp.ToString() + " XP";
        XpText.SetActive(true);
        Invoke("HideXpGain", 2);
    }

    void HideXpGain() {
        XpText.SetActive(false);
    }

	public void ChangeCommandPool(string change){
		switch (change) {

		case "on":
			AbilityPanel.GetComponent<CanvasRenderer>().SetAlpha(0);
			AbilityPanel.GetComponent<AbilityPanel>().UpdateVisibility();
			CommandPoolPanel.SetActive (true);
			break;

		case "off":				
			AbilityPanel.GetComponent<CanvasRenderer>().SetAlpha(1);
			AbilityPanel.GetComponent<AbilityPanel>().UpdateVisibility();
			CommandPoolPanel.SetActive (false);
			break;

		default:
			break;
		}

	}

}
