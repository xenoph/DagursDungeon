using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Will take the input from the Player and dissect it. It goes through various checks, and it will call
///  the validation script to validate the input - then also execute the functions.
/// -BV
/// TODO: Need to be tidied up a bit more as some functions might be deprecated after the new validation script.
/// </summary>
public class DissectInputScript : MonoBehaviour {

    //Create references to other scripts
    private GameTurnController _gameTurnController;
    private Dialogue _dialogueScript;
    private InterfaceScript _interfaceScript;
    private CommandList _commandListScript;
    private Commands _commandsScript;

    private string _className;
    private Type _type;
    private object _obj;

    //Lists which contains valid commands
    private List<string> _tutorialCommands;
    private List<string> _instantCommands;
    private List<string> _nillOneTwoArgumentCommands;

    void Awake() {
        //Get the other scripts
        _gameTurnController = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameTurnController>();
        _commandListScript = GameObject.FindGameObjectWithTag("CommandListHolder").GetComponent<CommandList>();
        _dialogueScript = GameObject.FindGameObjectWithTag("UI").GetComponent<Dialogue>();
        _commandsScript = GetComponent<Commands>();
        _interfaceScript = GameObject.FindGameObjectWithTag("UI").GetComponent<InterfaceScript>();

        //Fill up the command lists
        _tutorialCommands = new List<string> { "NEXT", "FINISHED", "PREVIOUS", "RESPAWN", "INFO" };
        _instantCommands = new List<string> {"ALIAS", "UNALIAS", "LEVEL", "RESTART", "HELP", "LIST", "EXPERIENCE", "STATS", "REST", "GAINLEVEL", "GETKEY", "CLOSE"};
        _nillOneTwoArgumentCommands = new List<string>() { "KICK", "WEAR", "LICK", "JUMP", "HIDE", "LOOK", "PUSH", "PULL", "GO", "HELP" };

        //Get the player class
        _className = PlayerPrefs.GetString("Class");
    }

    /// <summary>
    /// Dissects the player input.
    /// </summary>
    /// <param name="input">The input.</param>
    /// <param name="version">The version.</param>
    public void DissectPlayerInput(string input, int version) {
        //Run the input through a function to remove any additional whitespace. This removes both start and end whitespace, and also between words.
        string trimmedInput = TrimInput(input);
        //Split the input into an array of strings
        string[] fullInput = trimmedInput.Split(null);
		//Since "use" is such a common commando to use in text-based games, notify the player as soon as possible about how it won't work here.
		if (fullInput[0].Equals("use", StringComparison.CurrentCultureIgnoreCase)){
			_dialogueScript.ErrorText("I'm terribly sorry, but \"use\" is not a valid commando here.");
			_interfaceScript.ActivateInput();
			return;
		}
        //Iterate through the various command lists and see if the Player is using a variation of the actual function
        foreach (List<string> list in _commandListScript.ValidCommandsList) {       
            for (int i = 0; i < list.Count; i++) {
                if(string.Equals(list[i], fullInput[0], StringComparison.CurrentCultureIgnoreCase) && list[i] != list[0]) {
                    fullInput[0] = list[0];
                }
            }
        }
        //Get the type and object to look for the function in. Also check if it's a special tutorial function
        bool ifTutorial = GetCommandTypeObject(fullInput[0]);
        //Find the function in the selected script
        MethodInfo function = _type.GetMethod(fullInput[0],
            BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase | BindingFlags.NonPublic);
        //Make sure a function has been found
        if(function == null) {
            _dialogueScript.ErrorText("invalid");
            _interfaceScript.ActivateInput();
            return;
        }
        if (_commandListScript.CombatCommands.Contains(trimmedInput.ToUpper()) &&
            _gameTurnController.CurrentState != GameTurnController.PlayerState.Combat) {
            _dialogueScript.ErrorText("You're not in combat right now.");
            _interfaceScript.ActivateInput();
            return;
        }
        //After finding the function, if it was a tutorial or instant one, execute it and return
        if ((ifTutorial || _instantCommands.Contains(fullInput[0].ToUpper())) && version == 1) {
            _commandsScript.StartInstant(trimmedInput);
            return;
        }
        //Check if the Player has given a valid function and arguments
        bool foundFunction = GameObject.Find("ScriptHolder").GetComponent<InputValidation>().ValidFunctionInput(fullInput);
        //If 1 is given, the input should be validated and saved
        if (version == 1) {
            
            //If false is returned, feedback is given via the function, so just return
            if (!foundFunction) {
                _interfaceScript.ActivateInput();
                return;
            }
            //Otherwise, save the input into the command log and inform the Player
            _dialogueScript.SaveIntoCommandLog(trimmedInput);
            //Also reactivate the input.
            _interfaceScript.ActivateInput();
        }
        //If 2 is given, execute the function
        if (version == 2) {
            if (!foundFunction) {
                _interfaceScript.ActivateInput();
                return;
            }
            ExecuteFunction(function, fullInput);
        }
    }

    /// <summary>
    /// Executes the function.
    /// </summary>
    /// <param name="function">The function.</param>
    /// <param name="inputStrings">The input strings.</param>
    private void ExecuteFunction(MethodInfo function, string[] inputStrings) {
        //TODO: This -might- need to be re-done a little bit after the new validation script!
        //Execute the function based on the number of inputs.
        //TODO: Can this be simplified?
        if (_nillOneTwoArgumentCommands.Contains(inputStrings[0].ToUpper())) {
            switch (inputStrings.Length) {
                case 1:
                    function.Invoke(_obj, new object[] { null, null });
                    return;
                case 2:
                    function.Invoke(_obj, new object[] { inputStrings[1], null });
                    return;

                case 3:                    
                    function.Invoke(_obj, new object[] { inputStrings[1], inputStrings[2] });
                    return;
            }
        }
        if (function.GetParameters().Length < 1) {
            function.Invoke(_obj, new object[] {});
            return;
        }
        if (function.GetParameters().Length > 0 && inputStrings.Length == 1) {
            function.Invoke(_obj, new object[] {null});
            return;
        }
        if (inputStrings.Length == 2) {
            
            function.Invoke(_obj, new object[] {inputStrings[1]});
            return;
        }
        if (inputStrings.Length == 3) {
            function.Invoke(_obj, new object[] {inputStrings[1], inputStrings[2]});
            return;
        }
        _dialogueScript.ErrorText("invalid");
    }

    /// <summary>
    /// Gets the type object.
    /// </summary>
    /// <returns></returns>
    private Type GetClassTypeObject() {
        //TODO: Is this needed now that we only have one class?
        switch(_className) {
            case "WarriorClass":
                _type = typeof(WarriorClass);
                break;

            case "DruidClass":
                _type = typeof(DruidClass);
                break;

            case "MageClass":
                _type = typeof(MageClass);
                break;

            case "ThiefClass":
                _type = typeof(ThiefClass);
                break;
        }
        return _type;
    }

    /// <summary>
    /// Gets the command type object.
    /// </summary>
    /// <param name="arg">The argument.</param>
    private bool GetCommandTypeObject(string arg) {
        if(_gameTurnController.CurrentState == GameTurnController.PlayerState.Combat) {
            _type = GetClassTypeObject();
            _obj = GetComponent(_className);
            return false;
        }
        if (_tutorialCommands.Contains(arg.ToUpper()) && SceneManager.GetActiveScene().name == "Tutorial") {
            _type = typeof (Tutorial);
            _obj = GameObject.FindGameObjectWithTag("ScriptHolder").GetComponent<Tutorial>();
            return true;
        }
        _type = typeof (Commands);
        _obj = GetComponent<Commands>();
        return false;
    }

    /// <summary>
    /// Checks the turn.
    /// </summary>
    /// <param name="arg">The argument.</param>
    /// <returns></returns>
    private bool CheckTurn(string arg) {
        if(arg != "LEFT" && arg != "RIGHT" && arg != "BACK") {
            return false;
        }
        return true;
    }

    /// <summary>
    /// Checks the movement.
    /// </summary>
    /// <param name="arg">The argument.</param>
    /// <returns></returns>
    private bool CheckMovement(string arg) {
        int number = 0;
        bool isInt = int.TryParse(arg, out number);
        bool positive = number > 0;
        if(isInt && positive) {
            return true;
        }
        return false;
    }

    /// <summary>
    /// Trims the input.
    /// </summary>
    /// <param name="input">The input.</param>
    /// <returns></returns>
    private string TrimInput(string input) {
        string firstTrim = input.Trim();
        string pattern = "\\s+";
        string replacement = " ";
        Regex rgx = new Regex(pattern);
        string result = rgx.Replace(firstTrim, replacement);

        return result;
    }
}
