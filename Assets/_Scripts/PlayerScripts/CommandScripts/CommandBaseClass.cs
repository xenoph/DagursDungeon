using System.Collections.Generic;
using UnityEngine;

public class CommandBaseClass : MonoBehaviour {

    //References to all the different scripts
    internal Dialogue DialogueScript;
    internal Player PlayerScript;
    internal InterfaceScript InterfaceScript;
    internal GameTurnController GameTurnController;
    internal LevelScript LevelScript;
    internal CombatCommands CombatCommands;
    internal MovementCommands MovementCommands;
    internal DissectInputScript DissectingScript;
    internal Commands CommandScript;
    internal InteractionCommands InteractionScript;
    internal CommandList CommandListScript;
    internal ItemNameList ItemNameListScript;

    //Variables only used by all the scripts inheriting from CommandBaseClass
    internal bool NoInput = false;
    internal string InputString = null;
    internal bool MovementHappening;
    internal bool MetObstacle = false;
    internal int Keys;
    internal bool LevelFinished = false;
    internal int CurrentTurn;
    internal bool IsInstantCommand = false;

    //Public variable used by the InterfaceScript
    [HideInInspector]
    public bool IsInputActive;

    [HideInInspector] public bool LootedTempPotion = false;
    [HideInInspector] public bool LootedPermPotion = false;

    public void Awake() {       
        //Get all of the scripts
        InteractionScript = GetComponent<InteractionCommands>();
        CommandScript = GetComponent<Commands>();
        CombatCommands = GetComponent<CombatCommands>();
        MovementCommands = GetComponent<MovementCommands>();
        DissectingScript = GetComponent<DissectInputScript>();
        DialogueScript = GameObject.FindGameObjectWithTag("UI").GetComponent<Dialogue>();
        PlayerScript = GetComponent<Player>();
        InterfaceScript = GameObject.FindGameObjectWithTag("UI").GetComponent<InterfaceScript>();
        ItemNameListScript = GameObject.FindGameObjectWithTag("ScriptHolder").GetComponent<ItemNameList>();
        GameTurnController = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameTurnController>();
        if (!GameObject.FindGameObjectWithTag("CommandListHolder")) {
            Instantiate(gameObject, Vector3.zero, Quaternion.identity);
            gameObject.tag = "CommandListHolder";
            gameObject.name = "CommandListHolder";
            gameObject.AddComponent<CommandList>();
            CommandListScript = gameObject.GetComponent<CommandList>();
        }
        else {
            CommandListScript = GameObject.FindGameObjectWithTag("CommandListHolder").GetComponent<CommandList>();
        }     
        LevelScript = GameObject.FindGameObjectWithTag("ScriptHolder").GetComponent<LevelScript>();
    }

    /// <summary>
    /// Casts a Ray 2 meters forward and returns the GameObject it hits.
    /// </summary>
    /// <returns></returns>
    public GameObject GetRaycastObject() {
        RaycastHit hit;
        if(Physics.Raycast(PlayerScript.PlayerCamera.transform.position, transform.forward, out hit, 2)) {
            return hit.transform.gameObject;
        }
        return null;
    }

    /// <summary>
    /// Cast a ray in four directions and see if it hits the target.
    /// </summary>
    /// <param name="target">The target.</param>
    /// <returns></returns>
    public GameObject GetRaycastObject(string target) {
        //Create an array of the different directions to look at
        Vector3[] directions = new[] { transform.right, -transform.right, transform.forward, -transform.forward };
        //Iterate through the directions
        foreach (var direction in directions) {
            //If the raycast hits something in a direction
            RaycastHit hit;
            if (Physics.Raycast(PlayerScript.PlayerCamera.transform.position, direction, out hit, 2)) {
                //Check if it is what the Player was trying to use
                if (hit.transform.tag == target) {
                    //If it was, return the gameobject found.
                    return hit.transform.gameObject;
                }
            }
        }
        return null;
    }

    /// <summary>
    /// Function that will check the parameter given by the Player when they enter a command.
    /// If it's empty, it will return that to the function, which will not run.
    /// It will also ask the Player 'where' or 'what' depending on the command to give feedback that they were missing something.
    /// </summary>
    /// <param name="command"></param>
    /// <param name="arg"></param>
    public void CheckString(string command, string arg) {
        NoInput = false;
        InputString = null;
        if(arg == null) {
            NoInput = true;
            switch (command) {
                case "Turn":
                case "Look":
                    DialogueScript.ErrorText(command + " where?");
                    return;
                case "Help":
                case "Enter":
                    return;
            }

            DialogueScript.ErrorText(command + " what?");
        } else {
            InputString = arg.ToUpper();
        }
    }
}
