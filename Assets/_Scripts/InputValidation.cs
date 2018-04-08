using System;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

/// <summary>
/// The previous validation of commands was severely lacking, and most of the time the Player wouldn't receive any information
/// before they actually executed the command. This system is a bit more hand-holding, and provides the Player with 
/// instant, and varied, feedback.
/// The script became a little bit messy, and there could be some cleanup done since a lot of the returns can be combined.
/// </summary>
public class InputValidation : MonoBehaviour {

    private Dialogue _dialogueScript;

    private List<string> _tutorialCommands;
    private List<string> _combatCommands;
    private List<string> _noArgumentCommands;
    private List<string> _oneArgumentCommands;
    private List<string> _twoArgumentCommands;
    private List<string> _emoteCommands;
    private List<string> _cheatCommands;

    void Awake() {

        _dialogueScript = GameObject.FindGameObjectWithTag("UI").GetComponent<Dialogue>();

        _tutorialCommands = new List<string> { "NEXT", "FINISHED", "PREVIOUS", "RESPAWN", "CLOSE", "INFO" };
        _combatCommands = new List<string>() { "SLASH", "THRUST", "SHIELD", "SLAM", "CLEAVE", "COUNTERATTACK", "STRAINSLASH", "EXECUTE", "REKT" };
        _noArgumentCommands = new List<string>() {
            "ATTACK", "FORWARD", "LEFT", "RIGHT", "BACK", "ENTER", "REST", "START", "EXPERIENCE", "STATS", "LOOK", "HELP", "CLOSE"
        };
        _oneArgumentCommands = new List<string>() {
            "FORWARD", "LEFT", "RIGHT", "BACK", "OPEN", "GET", "LIGHT", "EXAMINE", "HELP",
            "UNALIAS", "HELP", "LIST", "PUSH", "PULL", "GO", "TURN", "ATTACK", "GET", "UNALIAS", "CLIMB"
        };
        _twoArgumentCommands = new List<string>() {"GO", "PUSH", "PULL", "LOOK", "GET", "HELP", "ALIAS"};
        _emoteCommands = new List<string>() {"KICK", "WEAR", "LICK", "JUMP", "HIDE", "CHEER", "CLAP", "DANCE", "WHINE", "STRUT", "FLEX"};
        _cheatCommands = new List<string>() {"RESTART", "GAINLEVEL", "GETKEY", "LEVEL", "CLEARCOOLDOWN"};
    }

    /// <summary>
    /// Valids the function input.
    /// </summary>
    /// <param name="inputStrings">The input strings.</param>
    /// <returns></returns>
    /// <exception cref="MissingMethodException">
    /// Function does not exist or is not in the correct List
    /// </exception>
    public bool ValidFunctionInput(string[] inputStrings) {
        //If it is an emote, cheat or a tutorial command, return true regardless (right now).
        if (_emoteCommands.Any(s => s.Equals(inputStrings[0], StringComparison.CurrentCultureIgnoreCase)) ||
            _tutorialCommands.Any(s => s.Equals(inputStrings[0], StringComparison.CurrentCultureIgnoreCase)) ||
            _cheatCommands.Any(s => s.Equals(inputStrings[0], StringComparison.CurrentCultureIgnoreCase))) {
            return true;
        }
        if (_combatCommands.Any(s => s.Equals(inputStrings[0], StringComparison.CurrentCultureIgnoreCase))) {
            return CheckCombatCommands(inputStrings);
        }

        //Check the length of the input
        switch (inputStrings.Length) {
            case 1:
                if (
                    _noArgumentCommands.Any(
                        s => string.Equals(s, inputStrings[0], StringComparison.CurrentCultureIgnoreCase))) {
                    return true;
                }
                _dialogueScript.ErrorText("You need to add something to that command.");
                return false;

            case 2:
                if (
                    _oneArgumentCommands.Any(
                        s => string.Equals(s, inputStrings[0], StringComparison.CurrentCultureIgnoreCase))) {
                    return CheckOneArgumentCommands(inputStrings);
                }
                _dialogueScript.ErrorText("Consider adding or removing a word...");
                return false;

            case 3:
                if (
                    _twoArgumentCommands.Any(
                        s => string.Equals(s, inputStrings[0], StringComparison.CurrentCultureIgnoreCase))) {
                    return CheckTwoArgumentCommands(inputStrings);
                }
                _dialogueScript.ErrorText("There might be a word too many there.");
                return false;

             //If it's more than 3 (function argument argument) it's not a valid command currently
			default:
				_dialogueScript.ErrorText("Try using fewer words.");
				return false;
        }
        return true;
    }

    /// <summary>
    /// Checks the input if one argument is given
    /// </summary>
    /// <param name="input">The input from the Player in an array</param>
    /// <returns></returns>
    /// <exception cref="MissingMethodException">Function does not exist or is not in a List</exception>
    private bool CheckOneArgumentCommands(string[] input) {
        //Separate the commands into two arrays - one for taking int arguments and one for string.
        string[] intArgumentCommands = {"FORWARD", "LEFT", "BACK", "RIGHT",};
        string[] stringArgumentCommands = {"OPEN", "GET", "LIGHT", "EXAMINE", "UNALIAS", "LIST", "TURN", "ATTACK", "GET", "UNALIAS", "CLIMB"};

        //If the function is in the int array, check if the argument is an int
        if (intArgumentCommands.Any(s => s.Equals(input[0], StringComparison.CurrentCultureIgnoreCase))) {
            if (input[1].Any(char.IsDigit)) {
                if(CheckPositiveNumber(int.Parse(input[1]))) return true;
                _dialogueScript.ErrorText("?...sdrawkcab evom ot hsiw uoY");
                return false;
            }
            _dialogueScript.ErrorText("I don't think you can move " + input[1] + " steps " + input[0] + "!");
            return false;
        }

        //Likewise, if in the string array, make sure it's not an int
        if (stringArgumentCommands.Any(s => s.Equals(input[0], StringComparison.CurrentCultureIgnoreCase))) {
            //Turning can only be done in three different directions
            string[] turnArguments = {"RIGHT", "LEFT", "BACK"};
            if ((input[0].Equals("turn", StringComparison.CurrentCultureIgnoreCase) || 
                input[0].Equals("attack", StringComparison.CurrentCultureIgnoreCase)) && 
                !turnArguments.Any(s => s.Equals(input[1], StringComparison.CurrentCultureIgnoreCase))) {
                _dialogueScript.ErrorText("Think about which ways normal people can turn...");
                return false;
            }
            //List and Examine needed a function on their own. Bad programmer.
            if (!CheckArguments(input)) return false;
            if (!input[0].Any(char.IsDigit)) return true;
            _dialogueScript.ErrorText("Consider the logical flaw in trying to " + input[0] + " " + input[1] + ".");
            return false;
        }

        //Do a switch for the remaining functions as they can take an int or a string.
        //If it's none of these, throw an error.
        switch (input[0].ToUpper()) {
            case "PUSH":
                return CheckPush(input, 1);

            case "PULL":
                return CheckPull(input, 1);

            case "GO":
                return CheckGo(input, 1);

            case "HELP":
                return CheckHelp(input, 1);

            default:
                throw new MissingMethodException("Function does not exist or is not in a List");
        }
    }

    /// <summary>
    /// Check the input if two arguments are given.
    /// </summary>
    /// <param name="input">The input from the Player in an array.</param>
    /// <returns></returns>
    private bool CheckTwoArgumentCommands(string[] input) {

        switch (input[0].ToUpper()) {
            case "PUSH":
                return CheckPush(input, 2);

            case "PULL":
                return CheckPull(input, 2);

            case "GO":
                return CheckGo(input, 2);

            case "LOOK":
                return CheckLook(input);

            case "GET":
                return CheckArguments(input);

            case "HELP":
                return CheckHelp(input, 2);

			case "ALIAS":
				return CheckArguments(input);

            default:
                throw new MissingMethodException("Function does not exist or is not in a List");
        }
    }

    /// <summary>
    /// Checks the arguments given with the Push command.
    /// </summary>
    /// <param name="input">Input string array.</param>
    /// <param name="arguments">Number of arguments to check.</param>
    /// <returns></returns>
    /// <exception cref="ArgumentException">Function only take 1 or 2 as integer argument.</exception>
    private bool CheckPush(string[] input, int arguments) {
        switch (arguments) {
            //If only one argument is used with Push, it should (currently) be button or wall.
            //Give a specified error if the Player gives an int as an argument.
            case 1:
                if (input[1].Equals("button", StringComparison.CurrentCultureIgnoreCase) ||
                    input[1].Equals("wall", StringComparison.CurrentCultureIgnoreCase)) {
                    return true;
                }
                if (input[1].Any(char.IsDigit)) {
                    _dialogueScript.ErrorText("Push what for " + int.Parse(input[1]) + " meters?");
                    return false;
                }
                _dialogueScript.ErrorText("I don't think you can push a " + input[1] + ".");
                return false;

            case 2:
                //The most obvious check - Player wants to push a wall - second argument should be an integer
                if (input[1].Equals("wall", StringComparison.CurrentCultureIgnoreCase)) {
                    if (input[2].Any(char.IsDigit)) {
                        if (CheckPositiveNumber(int.Parse(input[2]))) return true;
                        _dialogueScript.ErrorText("...that'd be the same as pulling?");
                        return false;
                    }
                    _dialogueScript.ErrorText("Push a wall with " + input[2] + "?");
                    return false;
                }
                //If the smart Player uses button with another argument
                if (input[1].Equals("button", StringComparison.CurrentCultureIgnoreCase)) {
					if (input[2].Any(char.IsDigit)) {
						_dialogueScript.ErrorText("Ahum. Push how many buttons?");
					} 
					else {
						_dialogueScript.ErrorText("Honestly, did you think pushing a button with a " + input[2] + " would work?");
					}
                    return false;
                }
                //Player messes things up
                if (input[1].Any(char.IsDigit)) {
                    if (input[2].Equals("wall", StringComparison.CurrentCultureIgnoreCase)) {
                        _dialogueScript.ErrorText("Think long and hard about how you place words...");
                    }
					if (input[2].Any(char.IsDigit)) {
						_dialogueScript.ErrorText("Fairly certain pusing a number with a number won't work.");
					} 
					else {
						_dialogueScript.ErrorText("Pushing a number with a " + input[2] + " will not work.");
					}
                    return false;
                }
                break;

            default:
                throw new ArgumentException("Function only take 1 or 2 as integer argument.");
        }
        return true;
    }

    /// <summary>
    /// Checks the arguments given with the Pull command.
    /// </summary>
    /// <param name="input">Input string array.</param>
    /// <param name="arguments">Number of arguments to check.</param>
    /// <returns></returns>
    /// <exception cref="ArgumentException">Function only take 1 or 2 as integer argument.</exception>
    private bool CheckPull(string[] input, int arguments) {
        //Currently the only valid argument for Pull is wall, so this is an easy check.
        switch (arguments) {
            case 1:
                if (input[1].Equals("wall", StringComparison.CurrentCultureIgnoreCase)) {
                    return true;
                }
                if (input[1].Any(char.IsDigit)) {
                    _dialogueScript.ErrorText("It'd be nice if you specified what you want to pull.");
                    return false;
                }
                _dialogueScript.ErrorText("No matter how much you strain, I don't believe you can pull " + input[1] + "!");
                return false;
            //Same for two arguments - only wall and a number is allowed
            case 2:
                if (input[1].Equals("wall", StringComparison.CurrentCultureIgnoreCase)) {
                    if (input[2].Any(char.IsDigit)) {
                        if (CheckPositiveNumber(int.Parse(input[2]))) return true;
                        _dialogueScript.ErrorText("Pull negatively? Perhaps try pushing.");
                        return false;
                    }
                    _dialogueScript.ErrorText("How does it work to pull " + input[1] + " " + input[2] + " meters?");
                    return false;
                }
                //Stupid Players everywhere...
                if (input[1].Any(char.IsDigit)) {
                    if (input[2].Equals("wall", StringComparison.CurrentCultureIgnoreCase)) {
                        _dialogueScript.ErrorText("You might be upside down.");
                        return false;
                    }
                    _dialogueScript.ErrorText("Pull how far what now?");
                    return false;
                }
                _dialogueScript.ErrorText("Likelyhood if you pulling " + input[1] + " equals zero.");
                return false;

            default:
                throw new ArgumentException("Function only take 1 or 2 as integer argument.");
        }
    }

    /// <summary>
    /// Checks arguments given with the Go command.
    /// </summary>
    /// <param name="input">Input string array.</param>
    /// <param name="arguments">Number of arguments to check.</param>
    /// <returns></returns>
    /// <exception cref="ArgumentException">Function only take 1 or 2 as integer argument.</exception>
    private bool CheckGo(string[] input, int arguments) {
        //Create an array of valid string arguments. These are valid both on their own or with an additional integer
        string[] validStringArguments = { "FORWARD", "UP", "LEFT", "RIGHT", "BACK" };
        //One argument check is easy enough - argument must either be a valid string or an int.
        switch (arguments) {
            case 1:
                if (validStringArguments.Any(s => s.Equals(input[1], StringComparison.CurrentCultureIgnoreCase))) {
                    return true;
                }
                if (input[1].Any(char.IsDigit)) {
                    if(CheckPositiveNumber(int.Parse(input[1]))) return true;
                    _dialogueScript.ErrorText("It's possible you wish to go back?");
                    return false;
                }
                _dialogueScript.ErrorText("Can't really go " + input[1] + " now, can you?");
                return false;

            case 2:
                //Again check the first argument for a valid string
                if (validStringArguments.Any(s => s.Equals(input[1], StringComparison.CurrentCultureIgnoreCase))) {
                    //Change here being that "up" is only to be used on its own, so give two different error messages
                    //based on if the Player adds an integer or string after it.
                    if (input[1].Equals("up", StringComparison.CurrentCultureIgnoreCase)) {
                        if (input[2].Any(char.IsDigit)) {
							if (CheckPositiveNumber (int.Parse (input [2]))) {
								_dialogueScript.ErrorText ("I don't think you need to specify how far to go up");
							} else {
								_dialogueScript.ErrorText ("Wouldn't that be going down into the floor?");
							}
                            return false;
                        }
                        _dialogueScript.ErrorText("Up " + input[2] + "? Really?");
                        return false;
                    }
                    //Otherwise just check if the second argument is a positive integer.
				if (input [2].Any (char.IsDigit)) {
					if (CheckPositiveNumber (int.Parse (input [2]))) return true;
					_dialogueScript.ErrorText ("Try being a little bit less... negative.");
				} else {
					_dialogueScript.ErrorText ("Saying how far instead of " + input [2] + " might help.");
				}
                    return false;
                }
                //Since the Player is stupid - check if they are switching things around
                //and if the switching is done with a valid argument or not
                if (input[1].Any(char.IsDigit)) {
                    if (validStringArguments.Any(s => s.Equals(input[2], StringComparison.CurrentCultureIgnoreCase))) {
                        _dialogueScript.ErrorText(
                            "The words and numbers are there... perhaps next try.");
                        return false;
                    }
                    _dialogueScript.ErrorText("Not sure what random numbers in front of random words should mean.");
                    return false;
                }
                //If all else fails, the Players first argument makes no sense
                _dialogueScript.ErrorText("I'm afraid that Go and " + input[1] + " don't play well together.");
                return false;

            default:
                throw new ArgumentException("Function only take 1 or 2 as integer argument.");
        }
    }

    /// <summary>
    /// Check what the Player is looking at. Same as Examine command, but can also take "at" as second input.
    /// </summary>
    /// <param name="input">Object Player attempts to look at.</param>
    /// <returns></returns>
    private bool CheckLook(string[] input) {
        //Too tired to come up with anything amusing for this. Need to test the script before bed.
        if (input[1].Any(char.IsDigit)) {
            _dialogueScript.ErrorText("Are you Scrooge in disguise?");
            return false;
        }
        //First argument must be "at", and the second can not be a number.
        if (input[1].Equals("at", StringComparison.CurrentCultureIgnoreCase)) {
            if (!input[2].Any(char.IsDigit)) return true;
            _dialogueScript.ErrorText("No more counting numbers, we'll be counting stars.");
            return false;
        }
        //For the time being, the Player can only use "at", and not "in". Might change though.
        if (input[1].Equals("in", StringComparison.CurrentCultureIgnoreCase)) {
            _dialogueScript.ErrorText("Unless you are staring into a soul, that won't work.");
            return false;
        }
        _dialogueScript.ErrorText("Have a second thought about what you want to look at.");
        return false;
    }

    /// <summary>
    /// Checks for valid arguments given to List, Examine and Get commands. It seems to be expanding rapidly...
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    private bool CheckArguments(string[] input) {
        switch (input[0].ToUpper()) {
            case "LIST":
                string[] listArguments = {"ALIAS", "COMMANDS", "EMOTES"};
                if (listArguments.Any(s => s.Equals(input[1], StringComparison.CurrentCultureIgnoreCase))) {
                    return true;
                }
                _dialogueScript.ErrorText("Try listing something else.");
                return false;

            case "EXAMINE":
                if (GameObject.Find("ScriptHolder").GetComponent<ItemNameList>().AllItemNamesList.Any(list => list.Any(
                    t => t.Equals(input[1], StringComparison.CurrentCultureIgnoreCase)))) {
                    return true;
                }
                _dialogueScript.ErrorText("You can't examine a made-up word");
                return false;

            case "OPEN":
                if (GameObject.Find("ScriptHolder").GetComponent<ItemNameList>().ObjectsToOpenList.Any(
                    s => s.Equals(input[1], StringComparison.CurrentCultureIgnoreCase))) {
                    return true;
                }
                _dialogueScript.ErrorText("Not sure how easy it is to open a " + input[1] + "?");
                return false;

            case "GET":
                string[] getArguments = {"KEY", "STONE", "ROCK", "TABLET", "PIECE"};

                if (input.Length == 2 || input[2].Equals("the", StringComparison.CurrentCultureIgnoreCase)) {
                    if (getArguments.Any(s => s.Equals(input[1], StringComparison.CurrentCultureIgnoreCase)))
                        return true;
                    _dialogueScript.ErrorText("You might not be able to get that.");
                    return false;
                }
                _dialogueScript.ErrorText("Get the what now?");
                return false;
                
            case "CLIMB":
                string[] climbArguments = {"LADDER", "UP"};
                
                if (climbArguments.Any(s => s.Equals(input[1], StringComparison.CurrentCultureIgnoreCase))) return true;
                _dialogueScript.ErrorText("Not sure you can climb that");
                return false;    
        }
        return true;
    }

    /// <summary>
    /// Checks the Help command for valid additional input.
    /// </summary>
    /// <param name="inputStrings"></param>
    /// <param name="arguments"></param>
    /// <returns></returns>
    private bool CheckHelp(string[] inputStrings, int arguments) {
        string[] validHelpStrings =
            GameObject.Find("CommandListHolder").GetComponent<CommandList>().AllCommandsList.ToArray();
        switch (arguments) {
            case 1:
                string[] panelNames = {"MOVE", "TUTORIAL", "INTERACT", "COMMAND", "COMBAT"};
                if (GetComponent<ItemNameList>().AllItemNamesList.Any(list =>
                    list.Any(s => s.Equals(inputStrings[1], StringComparison.CurrentCultureIgnoreCase)))) {
                    _dialogueScript.ErrorText("I'm sorry, but I can't tell you anything about that item.");
                    return false;
                }
                if (validHelpStrings.Any(s => s.Equals(inputStrings[1], StringComparison.CurrentCultureIgnoreCase)) ||
                    panelNames.Any(s => s.Equals(inputStrings[1], StringComparison.CurrentCultureIgnoreCase))) {
                    return true;
                }
                _dialogueScript.ErrorText("I can't help you with that.");
                return false;

            case 2:
                if (inputStrings[1].Equals("with", StringComparison.CurrentCultureIgnoreCase)) {
                    print("bob");
                    if (!validHelpStrings.Any(s => s.Equals(inputStrings[2], StringComparison.CurrentCultureIgnoreCase))) {
                        _dialogueScript.ErrorText("Help with what?");
                        return false;
                    }
                    if (!GetComponent<ItemNameList>().AllItemNamesList.Any(list =>
                        list.Any(s => s.Equals(inputStrings[2], StringComparison.CurrentCultureIgnoreCase))))
                        return true;
                    _dialogueScript.ErrorText("I'm sorry, but I can't tell you anything about that item.");
                    return false;
                }
                _dialogueScript.ErrorText("I don't quite understand what you want help with.");
                return false;
        }
        return false;
    }

    /// <summary>
    /// During combat, the only validation that is needed is "shield" as that can take the "block" argument.
    /// </summary>
    /// <param name="inputStrings"></param>
    /// <returns></returns>
    private bool CheckCombatCommands(string[] inputStrings) {
        switch (inputStrings[0].ToUpper()) {
            case "SHIELD":
                if (inputStrings[1].Equals("block")) return true;
                _dialogueScript.ErrorText("You might want to block something with your shield instead.");
                return false;
        }
        return true;
    }

    /// <summary>
    /// Checks if the given number is positive or not.
    /// </summary>
    /// <param name="number">Number to check.</param>
    /// <returns></returns>
    private bool CheckPositiveNumber(int number) {
        bool positive = number > 0;
        if (positive) return true;
        return false;
    }
}
