using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Security.AccessControl;
using System.Text;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Commands : MovementCommands {

	public GameObject troll;
	public GameObject skeleton;

    void Start() {
        CommandListScript.UpdateAllCommandsList();
        InterfaceScript.ActivateInput();
        InterfaceScript.LevelText.text = LevelScript.LevelCounter.ToString();
    }

    void Update() {
        if (Input.GetKeyUp(KeyCode.Return)) {
            if (GameTurnController.CurrentState == GameTurnController.PlayerState.EnemyCombatTurn ||
                GameTurnController.CurrentState == GameTurnController.PlayerState.CommandChain ||
                GameTurnController.CurrentState == GameTurnController.PlayerState.EnemyTurn) {
                DialogueScript.ErrorText("Can't do anything now.");
                InterfaceScript.DeActivateInput();
            }
            else if (GameTurnController.CurrentState == GameTurnController.PlayerState.Combat ||
                     GameTurnController.CurrentState == GameTurnController.PlayerState.PlayerTurn) {
                if (InterfaceScript.CommandInputField.text == "") {
                    DialogueScript.ErrorText("Nothing saved.");
                    InterfaceScript.ActivateInput();
                    return;
                }
                InterfaceScript.PlayerInput();
            }
        }
        //Make sure that if people hit their mouse buttons or escape, the input is still active
        if (Input.GetKeyUp(KeyCode.Mouse0) || Input.GetKeyUp(KeyCode.Mouse1) || Input.GetKeyUp(KeyCode.Escape)) {
            InterfaceScript.ActivateInput();
        }
        //If Player hit the up-arrow, the last thing they wrote will be placed back into the input field
        if (Input.GetKeyUp(KeyCode.UpArrow) && (GameTurnController.CurrentState == GameTurnController.PlayerState.PlayerTurn ||
            GameTurnController.CurrentState == GameTurnController.PlayerState.Combat)) {
            if (InterfaceScript.LastCommands == null) return;
            InterfaceScript.ActivateInput();
            InterfaceScript.CommandInputField.text = InterfaceScript.LastCommands;
            InterfaceScript.CommandInputField.MoveTextEnd(true);
            InterfaceScript.LastCommands = null;
        }
    }

    /// <summary>
    /// Shows the help.
    /// </summary>
    /// <param name="arg"></param>
    /// <param name="arg2"></param>
    public void ShowHelp(string arg, string arg2) {
        //TODO: Currently this is being done both in InputValidation.cs and here. Simplify?
        string[] panelNames = { "MOVE", "TUTORIAL", "INTERACT", "COMMAND", "COMBAT" };
        //Check if the Player gave any additional input
        CheckString("Help", arg);
        //If not, send "null" to the InterfaceScript so that will show the help panel
        if (NoInput) {
            InterfaceScript.ShowHelp("null");
            //InterfaceScript.ActivateInput();
            return;
        }
        //If there are arguments, it should either be "with" + a valid help opion
        // or just a valid help option.
        if (arg.Equals("with", StringComparison.CurrentCultureIgnoreCase) &&
            CommandListScript.AllCommandsList.Any(s => s.Equals(arg2, StringComparison.CurrentCultureIgnoreCase))) {
            InterfaceScript.ShowHelp(arg2);
            return;
        }
        if (CommandListScript.AllCommandsList.Any(s => s.Equals(arg, StringComparison.CurrentCultureIgnoreCase)) ||
            panelNames.Any(s => s.Equals(arg, StringComparison.CurrentCultureIgnoreCase))) {
            InterfaceScript.ShowHelp(arg);
            return;
        }
        //Or if no valid command was given as an argument, let the Player know.
        else {
            DialogueScript.ErrorText("No help for that.");
        }
    }

    /// <summary>
    /// Activates the instant command.
    /// </summary>
    /// <param name="instantCommand">The instant command.</param>
    /// <returns></returns>
    public IEnumerator ActivateInstantCommand(string instantCommand) {
        //Change the state to CommandChain
        if (GameTurnController.CurrentState == GameTurnController.PlayerState.PlayerTurn) {
            GameTurnController.CurrentState = GameTurnController.PlayerState.CommandChain;
        }
        //Deactivate the input and run the command through the dissecting script
        InterfaceScript.DeActivateInput();
        DissectingScript.DissectPlayerInput(instantCommand, 2);
        //For the commands that needs it (like Rest), wait until they are done before moving on
        if (MovementCommands.MovementHappening) {
            while (MovementCommands.MovementHappening) {
                yield return new WaitForEndOfFrame();
            }
        }

        yield return new WaitForSeconds(0.2f);

        //Check if it is dark, and check of rest was used which will take up the entire turn then move to enemyturn
        if(LevelScript.Darkness) {
            LevelScript.DarknessHealthLoss();
        }
        if (instantCommand.ToUpper() == "REST") {
            if (CheckForEnemies() == 0 || SceneManager.GetActiveScene().name == "Level20") {
                DialogueScript.GameInformationText("It's your turn.");
                GameTurnController.CurrentState = GameTurnController.PlayerState.PlayerTurn;
                InterfaceScript.ActivateInput();
                yield break;
            }
            DialogueScript.GameInformationText("Enemies are moving.");
            GameTurnController.CurrentState = GameTurnController.PlayerState.EnemyTurn;
            yield break;
        }
        //Or return to playerturn and activate the input
        if (GameTurnController.CurrentState == GameTurnController.PlayerState.CommandChain) {
            GameTurnController.CurrentState = GameTurnController.PlayerState.PlayerTurn;
        }
        InterfaceScript.ActivateInput();
    }

    /// <summary>
    /// IEnumerator that will go through the command chain until it is done, then reset everything that needs resetting to be ready for next turn.
    /// This is called from the function Start in the in-game commands list
    /// </summary>
    /// <returns></returns>
    public IEnumerator ActivatingCommands() {
        GameTurnController.CurrentState = GameTurnController.PlayerState.CommandChain;
        int textToRemove = 0;
        InterfaceScript.DeActivateInput();
        if (DialogueScript.CommandHistoryList.Count > 0) {
            foreach (string s in DialogueScript.CommandHistoryList) {
                DialogueScript.MovementText("You perform " + s);
                DissectingScript.DissectPlayerInput(s, 2);
                InterfaceScript.CommandPoolTexts[textToRemove].color = Color.green;
                if (MovementCommands.MovementHappening) {
                    while (MovementCommands.MovementHappening) {
                        yield return new WaitForEndOfFrame();
                    }
                }
                if (MetObstacle) {
                    DialogueScript.MovementText("You appear too disoriented to continue.");
                    MetObstacle = false;
                    break;
                }
                if (
                    CommandListScript.EmoteList.Any(
                        x =>
                            string.Equals(InterfaceScript.CommandPoolTexts[textToRemove].text, x,
                                StringComparison.CurrentCultureIgnoreCase))) {
                    InterfaceScript.CommandPoolTexts[textToRemove].text = "";
                    textToRemove++;
                    yield return new WaitForSeconds(1.7f);
                }
                else {
                    InterfaceScript.CommandPoolTexts[textToRemove].text = "";
                    textToRemove++;
                    yield return new WaitForSeconds(0.7f);
                }
            }
        }
        ClearCommandPool();
        if (LevelScript.Darkness) {
            LevelScript.DarknessHealthLoss();
        }
        if (GameTurnController.CurrentState == GameTurnController.PlayerState.Combat ||
            GameTurnController.CurrentState == GameTurnController.PlayerState.EnemyCombatTurn) {
            yield break;
        }
        if (SceneManager.GetActiveScene().name == "Tutorial") {
            GameTurnController.CurrentState = GameTurnController.PlayerState.PlayerTurn;
            InterfaceScript.ActivateInput();
            yield break;
        }
        if (LevelFinished) {
            GameTurnController.CurrentState = GameTurnController.PlayerState.PlayerTurn;
            yield break;
        }
        if (CheckForEnemies() == 0 || SceneManager.GetActiveScene().name == "Level20") {
            DialogueScript.GameInformationText("It's your turn.");
            GameTurnController.CurrentState = GameTurnController.PlayerState.PlayerTurn;
            InterfaceScript.ActivateInput();
            yield break;
        }

        DialogueScript.GameInformationText("Enemies are moving.");
        GameTurnController.CurrentState = GameTurnController.PlayerState.EnemyTurn;
    }

    /// <summary>
    /// Checks for enemies.
    /// </summary>
    /// <returns></returns>
    private int CheckForEnemies() {
        GameObject[] enemiesGameObjects = GameObject.FindGameObjectsWithTag("Enemy");
        return enemiesGameObjects.Length;
    }

    /// <summary>
    /// Starts the chain.
    /// </summary>
    public void StartChain() {
        StartCoroutine("ActivatingCommands");
    }

    /// <summary>
    /// Runs the instant command.
    /// </summary>
    /// <param name="function">The function.</param>
    public void StartInstant(string function) {
        StartCoroutine(ActivateInstantCommand(function));
    }

    /// <summary>
    /// Attack forward
    /// </summary>
    /// <param name="arg">The argument.</param>
    public void Attack(string arg) {
        MovementHappening = true;
        //CheckString("Attack", arg);
        //if (arg != "left" && arg != "right" && arg != null) {
        //    DialogueScript.ErrorText("You can only attack right or left, or use no argument for attacking something right in front of you.");
        //}
        if (arg == "left") {
            StartCoroutine("Attacking");
            Turn("left");
        }
        else if (arg == "right") {
            StartCoroutine("Attacking");
            Turn("right");
        }
        else {
            MovementHappening = false;
            StartCoroutine("Attacking");
        }
    }

    private IEnumerator Attacking() {
        //Make sure that if the player is still moving/rotating, this function won't start until movement is done
        while (MovementHappening) {
            yield return new WaitForEndOfFrame();
        }
        //Get what is in front of the Player, if it's nothing inform and return
        GameObject hit = GetRaycastObject();
        if(hit == null) {
            DialogueScript.ErrorText("There's nothing right in front of you");
            yield break;
        }
        if(hit.transform.tag == "Enemy") {
			GameObject.Find("Canvas").GetComponent<InterfaceScript>().ChangeCommandPool("off");
            //Get the sound from the weapon and do the initial attack
            GameObject.FindGameObjectWithTag("Weapon").GetComponent<AudioSource>().Play();
            int damageDone = PlayerPrefs.GetInt("Level") + PlayerPrefs.GetInt("Strength");
            //Give the damage to the enemy, then show it on the GUI
            hit.transform.GetComponent<EnemyScript>().CheckHealth(damageDone, "initial");
            GameObject.FindGameObjectWithTag("UI").GetComponent<InterfaceScript>().ShowDamage(damageDone);
            //Change to enemy turn
            hit.transform.GetComponent<EnemyScript>().GotAttacked(transform.gameObject);
            GameTurnController.CurrentState = GameTurnController.PlayerState.EnemyCombatTurn;
            CombatCommands.Target = hit.transform.gameObject;
            //Inform the player that they are in combat, and clear any extra commands they have used
            DialogueScript.CombatTextPlayer("You've engaged the foe in front of you!");
            ClearCommandPool();
            //Pause the torch countdown while in combat (and it's not the tutorial level)
            if (SceneManager.GetActiveScene().name != "Tutorial") {
                LevelScript.PauseTorches(true);
            }
        } else {
            DialogueScript.ErrorText("You can't attack that.");
        }
    }

    /// <summary>
    /// Create an alias of a command and place it in a dictionary
    /// </summary>
    /// <param name="alias">The alias.</param>
    /// <param name="function">The function.</param>
    public void Alias(string alias, string function) {
        //First check that the alias is not already in use
        if (CommandListScript.AliasDictionary.ContainsKey(alias)) {
            DialogueScript.ErrorText("You've already used that alias for something.");
            return;
        }
        //Also check that there's not already an alias in place for that function.
        if (CommandListScript.AliasDictionary.ContainsValue(function)) {
            DialogueScript.ErrorText("Alias already exist.");
            return;
        }
        //Iterate through all the lists
        foreach (var list in CommandListScript.ValidCommandsList) {
            //Perform a reverse for loop on each list so that they can be changed
            for (int i = list.Count - 1; i > -1; i--) {
                //If the current string in the list is equal to the function the player want to make an alias from
                if (string.Equals(list[i], function, StringComparison.CurrentCultureIgnoreCase)) {
                    //Add their alias to the list
                    list.Add(alias);
                    //Then add the alias to the aliaslist
                    CommandListScript.AliasDictionary.Add(alias, function);
                    //CommandListScript.AliasesList.Add(alias);
                    //Inform the player that they've saved the command
                    DialogueScript.MovementText(
                        "You create an alias out of that command.");
                    return;
                }
            }
        }
        DialogueScript.ErrorText("I'm not sure that's a thing you know how to do.");
    }

    /// <summary>
    /// Remove an alias from the dict of aliases
    /// </summary>
    /// <param name="alias">The alias.</param>
    public void UnAlias(string alias) {
        //Is the alias saved
        if (CommandListScript.AliasDictionary.ContainsKey(alias)) {
            //Iterate through the lists to find it
            foreach (var list in CommandListScript.ValidCommandsList) {
                //Perform a reverse for loop on each list so that they can be changed
                for (int i = list.Count - 1; i > -1; i--) {
                    //If the current string in the list is equal to the alias
                    if (string.Equals(list[i], alias, StringComparison.CurrentCultureIgnoreCase)) {
                        //Remove the alias from the list
                        list.Remove(alias);
                        //also remove it from the aliasdictionary
                        CommandListScript.AliasDictionary.Remove(alias);
                        //Inform the player that they've removed the alias
                        DialogueScript.MovementText("You forgot how to use that alias.");
                    }
                }
            }
        }
    }

    /// <summary>
    /// Print out a list of argument
    /// </summary>
    /// <param name="arg">The argument.</param>
    new public void List(string arg) {
        CheckString("List", arg);
        if (NoInput) {
            return;
        }
        if (arg.ToUpper() == "ALIAS") {
            //Make sure that the Player has actually saved an alias
            if (CommandListScript.AliasDictionary.Count != 0) {
                //Inform the Player that these are the aliases created
                DialogueScript.GameInformationText(
                    "Here's a list of aliases. ");
                //Iterate through the dictionary and print out each alias alongside the function
                foreach (string s in CommandListScript.AliasDictionary.Keys) {
                    string value = CommandListScript.AliasDictionary[s];
                    DialogueScript.MovementText(s + " : " + value);
                }
            }
            else {
                DialogueScript.ErrorText("No aliases saved.");
            }
        }

        if (arg.ToUpper() == "COMMANDS") {
            StringBuilder builtString = new StringBuilder();
            foreach (var list in CommandListScript.ValidCommandsList) {
                foreach (string s in list) {
                    builtString.Append(s).Append(" | ");
                }
            }
            foreach (string s in CommandListScript.CommandsList) {
                builtString.Append(s).Append(" | ");
            }
            foreach (string s in CommandListScript.CombatCommands) {
                builtString.Append(s).Append(" | ");
            }
            InterfaceScript.PlaceText(builtString.ToString(), new Color(0, 255, 200), 15);
        }

        if (string.Equals(arg, "emotes", StringComparison.CurrentCultureIgnoreCase)) {
            var builtString = new StringBuilder();
            foreach (string s in CommandListScript.EmoteList) {
                builtString.Append(s).Append(" - ");
            }
            InterfaceScript.PlaceText(builtString.ToString(), new Color(0, 255, 200), 15);
        }
        if (GameTurnController.CurrentState == GameTurnController.PlayerState.Combat) {
            InterfaceScript.ActivateInput();
        }
    }

    /// <summary>
    /// Rest for 4x5% of total health. Calls an IEnumerator that in turn calls AddRestingHealth x 4 every 1.5 seconds.
    /// </summary>
    /// <param name="arg">The argument.</param>
    public void Rest(string arg) {
        MovementHappening = true;
        DialogueScript.GameInformationText("You rest to regain health.");   
        ClearCommandPool();    
        StartCoroutine("Resting");       
    }

    private IEnumerator Resting() {       
        int amountHealed = ((PlayerPrefs.GetInt("Constitution")*10)*5)/100;
        AddRestingHealth(amountHealed);
        yield return new WaitForSeconds(1.5f);
        AddRestingHealth(amountHealed);
        yield return new WaitForSeconds(1.5f);
        AddRestingHealth(amountHealed);
        yield return new WaitForSeconds(1.5f);
        AddRestingHealth(amountHealed);
        yield return new WaitForSeconds(1.5f);
        DialogueScript.GameInformationText("You finish resting.");
        DialogueScript.GameInformationText("You regained " + amountHealed*4 + " health.");
        MovementHappening = false;


		RaycastHit hit;

		//FindPositonForEnemy(randPos);
		if (UnityEngine.Random.Range(0,3) == 0)
		{
		GameObject[] spawnPoints = GameObject.FindGameObjectsWithTag("Spawner");
		if (spawnPoints.Length > 0)
			{
				GameObject pickedSpawnpoint = spawnPoints[UnityEngine.Random.Range(0,spawnPoints.Length - 1)];
				if (!Physics.Raycast(pickedSpawnpoint.transform.position, pickedSpawnpoint.transform.up, out hit, 1))
				{
					if (PlayerPrefs.GetInt("Level") > 5)
					{
						Instantiate(troll,pickedSpawnpoint.transform.position,Quaternion.identity);
					}
					else
					{
						Instantiate(skeleton,pickedSpawnpoint.transform.position,Quaternion.identity);
					}
				}
			}
		}
    }

    /// <summary>
    /// Adds the health to PlayerPrefs
    /// </summary>
    /// <param name="amountHealth">The amount of health to add.</param>
    private void AddRestingHealth(int amountHealth) {
        DialogueScript.GameInformationText("Resting....");
        PlayerPrefs.SetInt("Health", PlayerPrefs.GetInt("Health") + amountHealth);
        if (PlayerPrefs.GetInt("Health") > PlayerPrefs.GetInt("Constitution") * 10) {
            PlayerPrefs.SetInt("Health", PlayerPrefs.GetInt("Constitution") * 10);
        }
    }

    /// <summary>
    /// Clears the command pool.
    /// </summary>
    public void ClearCommandPool() {
        int textCounter = 0;       
        DialogueScript.UpdateCommandsPool();
        InterfaceScript.CommandPoolTextNumber = 0;
        foreach(Text commandPoolText in InterfaceScript.CommandPoolTexts) {
            commandPoolText.color = Color.white;
        }
        foreach(Text commandPoolText in InterfaceScript.CommandPoolTexts) {
            commandPoolText.text = textCounter < PlayerPrefs.GetInt("CommandsPool") ? "FREE SLOT" : "CLOSED SLOT";
            textCounter++;
        }
    }

    /// <summary>
    /// Show current XP and how much is needed for the next level.
    /// </summary>
    /// <param name="arg">The argument.</param>
    private void Experience(string arg) {
        DialogueScript.GameInformationText("You currently have " + PlayerPrefs.GetInt("CurrentXP") +
                                           " XP. Next level is at " + GetComponent<LevelUP>().NextLevel() + ".");
        DialogueScript.GameInformationText("Currently you are level " + PlayerPrefs.GetInt("Level") + ".");
    }

    /// <summary>
    /// Prints out the Players current stats
    /// </summary>
    /// <param name="arg">The argument.</param>
    public void Stats(string arg) {
        DialogueScript.GameInformationText("Your stats are :\n" +
                                           "Strength: " + PlayerPrefs.GetInt("Strength") + "\n" +
                                           "Dexterity: " + PlayerPrefs.GetInt("Dexterity") + "\n" +
                                           "Constitution: " + PlayerPrefs.GetInt("Constitution"));
        DialogueScript.GameInformationText("You currently have " + PlayerPrefs.GetInt("Health") + " out of maximum " + PlayerPrefs.GetInt("Constitution")*10 + " health!");
        DialogueScript.GameInformationText("You are carrying " + Keys + " keys.");
        DialogueScript.GameInformationText("You have " + PlayerPrefs.GetInt("CommandsPool") + " free command slots.");
        DialogueScript.GameInformationText("You're currently level " + PlayerPrefs.GetInt("Level") + ".");
    }

    /// <summary>
    /// Look at something. First argument (currently) needs to be "at". Calls the Examine command.
    /// </summary>
    /// <param name="arg">Players first argument, needs to be "at"</param>
    /// <param name="obj">The object to look at.</param>
    public void Look(string arg, string obj) {
        if (arg != null && obj != null) {
            if (arg.ToUpper() != "AT") return;
            MovementHappening = true;
            Examine(obj);
        }
        else {
            DialogueScript.ErrorText("Peering into the distance, you can see shadows.");
        }
    }

    /// <summary>
    /// Examine an object. Distance is up to 9 meters (5 blocks ahead).
    /// </summary>
    /// <param name="arg">Object to examine</param>
    void Examine(string arg) {
        //TODO: Add option to examine items on floor that are not showing, i.e. weapon.
        RaycastHit hit;
        if (Physics.Raycast(PlayerScript.PlayerCamera.transform.position, transform.forward, out hit, 9)) {
            //Find if the Player is using a valid word, then place that list into a new list.
            List<string> nameList = new List<string>();
            foreach (List<string> list in ItemNameListScript.AllItemNamesList) {
                for (int i = 0; i < list.Count; i++) {
                    if (!string.Equals(list[i], arg, StringComparison.CurrentCultureIgnoreCase)) continue;
                    nameList = list;
                }
            }
            //If nothing was placed in the list, i.e. the player didn't use a valid command
            if (nameList.Count == 0) {
                DialogueScript.ErrorText("I'm not sure what you want to examine...");
                return;
            }
            //Go through the now filled up list to make sure that the tag matches a string from it
            if (nameList.Any(s => string.Equals(s, hit.transform.gameObject.tag, StringComparison.CurrentCultureIgnoreCase))) {
                //Round the distance to what the Player is examining up to an int.
                int distance = (int) Math.Ceiling((double) hit.distance);
                DialogueScript.GameInformationText("You attempt to examine what's " + distance + " meters ahead.");
                StartCoroutine(ReturnExamination(hit.transform.gameObject, hit.transform.tag));
            }
            else {
                DialogueScript.ErrorText("I think you have the wrong name of that.");
            }
        }
        else {
            DialogueScript.ErrorText("There nothing ahead of you.");
        }
    }

    /// <summary>
    /// Returns the examination.
    /// </summary>
    /// <param name="hit">GameObject to be examined</param>
    /// <param name="objectTag">Tag of object.</param>
    /// <returns></returns>
    IEnumerator ReturnExamination(GameObject hit, string objectTag) {
        DialogueScript.GameInformationText("...");
        yield return new WaitForSeconds(1.5f);
        DialogueScript.GameInformationText("...");
        yield return new WaitForSeconds(1.5f);
        switch (objectTag) {
            case "Enemy":
                DialogueScript.GameInformationText("There is some sort of foe ahead of you.");
                if(hit.transform.gameObject.GetComponent<EnemyScript>().Level < PlayerPrefs.GetInt("Level")) {
                    DialogueScript.GameInformationText(
                        "By closer examination, it appears to be somewhat weaker than you.");
                } else if(hit.transform.gameObject.GetComponent<EnemyScript>().Level == PlayerPrefs.GetInt("Level")) {
                    DialogueScript.GameInformationText(
                        "You appear to be on equal footing. This might be an even match.");
                } else if(hit.transform.gameObject.GetComponent<EnemyScript>().Level > PlayerPrefs.GetInt("Level")) {
                    DialogueScript.GameInformationText(
                        "You feel that it is stronger than you. You consider your options...");
                }
                break;

			case "Hatch":
				DialogueScript.GameInformationText("Looking closely, you think that it is a trapdoor,");
				if (SceneManager.GetActiveScene().name == "Tutorial") {
					bool isPressed = GameObject.FindGameObjectWithTag("Button").GetComponent<Button>().IsPressed;
					DialogueScript.GameInformationText(isPressed 
						? "and it appears to be open."
						: "and it appears to be closed. There should be something that opens it somewhere.");
					break;
				}
                DialogueScript.GameInformationText(LevelScript.ExitOpen
                    ? "and it appears to be open."
                    : "and it appears to be closed. There should be something that opens it somewhere.");
                break;

            case "Torch":
                DialogueScript.GameInformationText("Judging by what you see, it seems to be a torch.");
                GameObject torchChildLight = hit.transform.FindChild("Point light").gameObject;
                DialogueScript.GameInformationText(torchChildLight.GetComponent<Light>().intensity < 4
                    ? "It doesn't shine as brightly as it perhaps could."
                    : "It's shining brightly just as it should.");
                break;

            case "Chest":
                DialogueScript.GameInformationText("It's golden and has a heavy lid. You logically consider it to be a chest.");
                DialogueScript.GameInformationText(Keys > 0 
                    ? "Feeling your pockets, you notice the key you picked up earlier. Maybe it could be useful..."
                    : "It appears to be locked. Now how do one unlock something locked...");
                break;

            case "Key":
                DialogueScript.GameInformationText("Floating in the air with sparkles seems unnatural, but it also looks like a key.");
                DialogueScript.GameInformationText("You could try to pick it up by walking to it.");
                break;

            case "Wall":
                DialogueScript.GameInformationText("Nothing but an empty wall up front...");
                break;

            case "Button":
                DialogueScript.GameInformationText("It appears to be a button carved into the wall.");
				DialogueScript.GameInformationText(hit.transform.gameObject.GetComponent<Button>().IsPressed
                    ? "You have pressed this button previously."
                    : "It is most likely used to open something in the dungeon...");
                break;

            case "PushableWall":
                DialogueScript.GameInformationText("The wall in this area seems to stand out from the rest.");
                DialogueScript.GameInformationText("Perhaps you'll be able to push it out of the way.");
                break;

            case "Door":
                DialogueScript.GameInformationText("Looks like an old door.");
                DialogueScript.GameInformationText(hit.transform.gameObject.GetComponent<WoodenDoor>().IsLocked
                    ? "You shake the handle only to find that it's locked"
                    : "You gently push the door and it appears open.");
                break;
			
			case "Tablet":
				DialogueScript.GameInformationText("The stone piece in front of you seem to be a part of some sort of tablet.");
				DialogueScript.GameInformationText("There's also some strange language on it. Might be important.");
				break;
        }
        MovementHappening = false;
    }  

    public void GettingLoot(string potionType) {
        StopCoroutine("ActivatingCommands");
        ClearCommandPool();       
        if (LootedPermPotion) {
            DialogueScript.GameInformationText("You looted a " + potionType + " potion!");
            DialogueScript.GameInformationText("If you want to use it, write yes! Otherwise write anything else.");
            InterfaceScript.IsGettingLoot = true;
            InterfaceScript.ActivateInput();
            GameTurnController.CurrentState = GameTurnController.PlayerState.PlayerTurn;
            return;
        }
        StartChain();
    }
    
    private void Climb(string arg){
        Go("up", 0);
    }
    
    /// <summary>
    /// Enables the Player to 'go' back up a level. Player needs to use 'Go up'.
    /// </summary>
    /// <param name="arg">Where to go</param>
    /// <param name="length">How far to go</param>
    private void Go(string arg, int length) {
        //First check if the Player want to use Go with a number to move
        if(arg.All(char.IsDigit)) {
            Forward(arg);
            return;
        }
        //Otherwise check what the argument given was.
        switch (arg.ToUpper()) {
            case "UP":
                if(length != 0) {
                    DialogueScript.ErrorText("I don't think you need to specify how far to go up.");
                    return;
                }
                //Check for stairs
                GameObject hit = GetRaycastObject("Stairs");
                //If found
                if(hit != null) {
                    //Look at the stairs
                    transform.LookAt(hit.transform.position);
                    //Change level to one up
                    GameObject.FindGameObjectWithTag("ScriptHolder").GetComponent<LevelChanger>().GoUp();
                    //And stop the function from continuing since the enemies don't need to move when the Player changes level.
                    StopCoroutine("ActivatingCommands");
                } else {
                    //Otherwise inform the Player that there are no stairs next to them.
                    DialogueScript.ErrorText("There are no stairs here.");
                }
                break;

            case "FORWARD":
                Forward(length.ToString());
                break;

            case "LEFT":
                if (length == 0) {
                    length = 1;
                }
                Left(length.ToString());
                break;

            case "RIGHT":
                if(length == 0) {
                    length = 1;
                }
                Right(length.ToString());
                break;

            case "BACK":
                if (length == 0) {
                    length = 1;
                }
                Back(length.ToString());
                break;
        }
    }

    private void Kick(string arg, string arg2) {
        GameObject hit = GetRaycastObject();
        if (hit == null) {
            if (arg == null) {
                DialogueScript.GameInformationText("You kick out wildly into the air. Not sure that is helping anyone.");
                return;
            }
            DialogueScript.ErrorText("Somehow, you fail to kick anything.");
            return;
        }
        if (arg2 == null) {
            if (string.Equals(hit.transform.gameObject.tag, arg, StringComparison.CurrentCultureIgnoreCase)) {
                DialogueScript.GameInformationText("You kick the " + arg + ". Did it help you?");
                return;
            }
        }
        if (arg.ToUpper() == "THE") {
            if (string.Equals(hit.transform.gameObject.tag, arg, StringComparison.CurrentCultureIgnoreCase)) {
                DialogueScript.GameInformationText("You kick " + arg + " " + arg2 + ". Did it help you?");
                return;
            }
        }
        else {
            DialogueScript.ErrorText("What exactly are you trying to kick?");
        }
    }

    private void Wear(string arg, string arg2) {
        if (arg2 == null) {
            DialogueScript.GameInformationText("You try to wear a " + arg + ". You look really pretty.");
            return;
        }

        if (arg.ToUpper() == "A") {
            DialogueScript.GameInformationText("You try to wear " + arg + " " + arg2 + ". You look really pretty.");
        }
    }

    private void Lick(string arg, string arg2) {
        GameObject hit = GetRaycastObject();
        if(hit == null) {
            if (arg == null) {
                DialogueScript.ErrorText("You lick your lips...");
                return;
            }
            else {
                DialogueScript.ErrorText("There's nothing in front of you, so your tongue licks your nose...");
                return;
            }
        }
        if(arg2 == null) {
            if(string.Equals(hit.transform.gameObject.tag, arg, StringComparison.CurrentCultureIgnoreCase)) {
                DialogueScript.GameInformationText("You lick the " + arg + ". Taste good?");
                return;
            }
        }
        if(arg.ToUpper() == "THE") {
            if(string.Equals(hit.transform.gameObject.tag, arg, StringComparison.CurrentCultureIgnoreCase)) {
                DialogueScript.GameInformationText("You lick " + arg + " " + arg2 + ". Taste good?");
                return;
            }
        } else {
            DialogueScript.ErrorText("You might not understand how licking works...");
        }
    }

    /// <summary>
    /// Emote "Jump". Can take none, one or two arguments.
    /// </summary>
    /// <param name="arg">Players first argument</param>
    /// <param name="arg2">Players second argument</param>
    private void Jump(string arg, string arg2) {
        GameObject stairs = GetRaycastObject("Stairs");
        GameObject hit = GetRaycastObject();
        if (arg == null && arg2 == null) {
            if (stairs != null && hit == null) {
                DialogueScript.GameInformationText("You should be careful jumping around ladders like that.");
                return;
            }
            if (hit != null) {
                DialogueScript.GameInformationText("Are you trying to jump on the " + hit.tag + "?");
                return;
            }
            if (stairs == null && hit == null) {
                DialogueScript.GameInformationText("Pretending to be a balloon, you jump up in the air.");
                return;
            }
        }
        if (arg != null && arg2 == null) {
            if (hit != null) {
                if (string.Equals(hit.tag, arg, StringComparison.CurrentCultureIgnoreCase)) {
                    DialogueScript.GameInformationText("You'd like to jump on the " + arg + "?");
                    return;
                }
                DialogueScript.GameInformationText("That's not a " + arg + "!");
                return;
            }
            if (string.Equals(arg, "up", StringComparison.CurrentCultureIgnoreCase)) {
                DialogueScript.GameInformationText("Barely getting off your feet, you jump up.");
                return;
            }
            if (hit == null) {
                DialogueScript.GameInformationText("There's no " + arg + " that you can jump on.");
                return;
            }
        }
        if (arg != null && arg2 != null) {
            DialogueScript.GameInformationText("I haven't taken two arguments into account yet.");
        }
    }

    /// <summary>
    /// Emote "Flex"
    /// </summary>
    private void Flex() {
        DialogueScript.GameInformationText("You flex your muscles. Oh so strong");
    }

    /// <summary>
    /// Emote "Hide". Can take none, one or two arguments.
    /// </summary>
    /// <param name="arg">Players first argument</param>
    /// <param name="arg2">Players second argument</param>
    private void Hide(string arg, string arg2) {
        if (arg == null && arg2 == null) {
            DialogueScript.GameInformationText("The big bad warrior tries to hide.");
        }
    }

    /// <summary>
    /// Emote "Whine"
    /// </summary>
    private void Whine() {
        DialogueScript.GameInformationText("Muttering to yourself about the situation won't help.");
    }

    /// <summary>
    /// Emote "Dance"
    /// </summary>
    private void Dance() {
        DialogueScript.GameInformationText("What are you, a ballerino?");
    }

    /// <summary>
    /// Emote "Clap"
    /// </summary>
    private void Clap() {
        DialogueScript.GameInformationText("You clap wildly about the situation.");
    }

    /// <summary>
    /// Emote "Cheer"
    /// </summary>
    private void Cheer() {
        DialogueScript.GameInformationText("I'm not sure that you have anything to cheer about.");
    }

    /// <summary>
    /// Emote "Strut"
    /// </summary>
    private void Strut() {
        DialogueScript.GameInformationText("Is appearing arrogant going to scare your enemies?");
    }

    /// <summary>
    /// Closes the currently active help panel
    /// </summary>
    public void Close() {
        GameObject panelToClose = GameObject.FindGameObjectWithTag("TutorialPanel");
        if(panelToClose) {
            panelToClose.SetActive(false);
				if  (InterfaceScript.ShowingHelp) {
					InterfaceScript.ShowingHelp = false;
				}
        }
    }

    /// <summary>
    /// Resets the turn.
    /// </summary>
    public void ResetTurn() {
        CurrentTurn = 0;
    }

    //\\//CHEAT COMMANDS FOR DEVS\\//\\
    //%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%\\

    /// <summary>
    /// Give the Player enough XP to gain a level
    /// </summary>
    void GainLevel() {
        PlayerPrefs.SetInt("CurrentXP", PlayerPrefs.GetInt("CurrentXP") + 123456789);
        DialogueScript.GameInformationText("You cheat and gain way too much XP.");
        GetComponent<LevelUP>().CheckForLevelUp();
    }

    /// <summary>
    /// Give the Player a key
    /// </summary>
    void GetKey() {
        Keys += 1;
        InterfaceScript.InfoKeyText.text = "Your Keys: " + Keys;
    }

    /// <summary>
    /// Restart the current level.
    /// </summary>
    public void Restart() {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    /// <summary>
    /// Change the Level
    /// </summary>
    /// <param name="arg">The argument.</param>
    public void Level(string arg) {
        SceneManager.LoadScene("Level" + arg);
    }
}