using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class CommandList : MonoBehaviour {
    //Create a List of lists that will contain all the commands that can be aliased
    public List<List<string>> ValidCommandsList = new List<List<string>>();
    //Also create a list that contains all commands regardless
    public List<string> AllCommandsList = new List<string>();

    //Create the lists that will contain the valid commands for each interaction
    //The Player can use the Alias command to add additional aliases for a command.
    //Interaction commands
    public List<string> PushList = new List<string>() {"PUSH", "PRESS", "SHOVE", "NUDGE"};
    public List<string> OpenList = new List<string>() {"OPEN", "UNLOCK", "UNSEAL", "UNFASTEN"};
    public List<string> GetList = new List<string>() {"GET", "TAKE", "GRAB", "FETCH"};
    public List<string> LightList = new List<string>() {"LIGHT", "IGNITE", "BRIGHTEN", "ILLUMINATE"};
    //Movement commands
    public List<string> ForwardList = new List<string>() {"FORWARD", "ADVANCE", "AHEAD", "ONWARD"};
    public List<string> BackList = new List<string>() {"BACK", "TURNAROUND", "OPPOSITE", "BACKWARD", "REVERSE"};
    public List<string> LeftList = new List<string>() {"LEFT", "LEFTFIELD", "LEFTOVER"};
    public List<string> RightList = new List<string>() {"RIGHT", "RIGHTFIELD", "RIGHTOVER"};
    public List<string> TurnList = new List<string>() {"TURN", "SPIN", "WHIRL", "TWIST"};
    //Attack commands
    public List<string> AttackList = new List<string>() {"ATTACK"};
    //Start commands
    public List<string> StartList = new List<string>() {"START", "RUN"};
    //Tutorial commands
    public List<string> TutorialCommands = new List<string>() {"NEXT", "PREVIOUS", "FINISHED", "CLOSE", "INFO", "RESPAWN"};
    //Aliases
    public Dictionary<string, string> AliasDictionary = new Dictionary<string, string>();
    //Combat commands
    public List<string> CombatCommands = new List<string>() { "SLASH", "THRUST", "SHIELDBLOCK", "SLAM" };

    //Other commands
    public List<string> ExamineList = new List<string>() {"EXAMINE"};
    public List<string> LookList = new List<string>() {"LOOK"};
    public List<string> RestList = new List<string>() {"REST"};
    public List<string> EnterList = new List<string>() {"ENTER"};
    public List<string> CancelList = new List<string>() {"CANCEL"};
    public List<string> HelpList = new List<string>() {"HELP"};

    //Other commands that can't be aliased
    public List<string> CommandsList = new List<string> {
        "ALIAS",
        "UNALIAS",
        "LEVEL",
        "RESTART",
        "LIST",
        "EXPERIENCE",
        "STATS"
    };

    //Emote commands
    public List<string> EmoteList = new List<string>() {
        "KICK",
        "WEAR",
        "LICK",
        "JUMP",
        "FLEX",
        "HIDE",
        "WHINE",
        "DANCE",
        "CLAP",
        "CHEER",
        "STRUT"
    };

    void Awake() {
        DontDestroyOnLoad(this);

        //Add all of the different command lists to the main alias list
        ValidCommandsList.Add(PushList);
        ValidCommandsList.Add(OpenList);
        ValidCommandsList.Add(GetList);
        ValidCommandsList.Add(LightList);
        ValidCommandsList.Add(ForwardList);
        ValidCommandsList.Add(BackList);
        ValidCommandsList.Add(LeftList);
        ValidCommandsList.Add(RightList);
        ValidCommandsList.Add(TurnList);
        ValidCommandsList.Add(AttackList);
        ValidCommandsList.Add(StartList);
        ValidCommandsList.Add(ExamineList);
        ValidCommandsList.Add(LookList);
        ValidCommandsList.Add(RestList);
        ValidCommandsList.Add(EnterList);
        ValidCommandsList.Add(CancelList);
        ValidCommandsList.Add(HelpList);

        //Also update the list with all the commands
        UpdateAllCommandsList();
    }

    public void UpdateAllCommandsList() {
        foreach (string s in ValidCommandsList.SelectMany(list => list)) {
            AllCommandsList.Add(s);
        }
        foreach (string s in CommandsList) {
            AllCommandsList.Add(s);
        }
        foreach (string s in CombatCommands) {
            AllCommandsList.Add(s);
        }
        foreach (string s in StartList) {
            AllCommandsList.Add(s);
        }
    }
}
