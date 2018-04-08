using UnityEngine;
using System.Collections.Generic;

public class Player : MonoBehaviour {

    //Variables for the Players stats
    //NOTE TO SELF: Must get these saved into PlayerPrefs and update that as the Player progresses.
    public int Health;
    public int Strength;
    public int Constitution;
    public int Intellect;
    public int Dexterity;
    public int Wisdom;
    public int Charisma;
    public int Stamina;
    public string ClassName;
    public int Level;
    public int DamageStat;

    private InterfaceScript _uScript;
    public Camera PlayerCamera;

    public Dictionary<string, string> AbilitiesDictionary = new Dictionary<string, string>(5);
    public List<string> AbilitiesList = new List<string>(5);

    private void Awake() {
        
        GameObject userInterface = GameObject.FindGameObjectWithTag("UI");
        _uScript = userInterface.GetComponent<InterfaceScript>();
        _uScript.FindPlayerScript();
        Level = 1;
        SetPlayerStats();
        _uScript.InfoKeyText.text = "Your Keys: " + GetComponent<Commands>().Keys;
    }

    private void SetPlayerStats() {
        Strength = PlayerPrefs.GetInt("Strength");
        Constitution = PlayerPrefs.GetInt("Constitution");
        Dexterity = PlayerPrefs.GetInt("Dexterity");
        Charisma = PlayerPrefs.GetInt("Charisma");
        ClassName = PlayerPrefs.GetString("Class");
        Health = PlayerPrefs.GetInt("Health");
        SetDamageStat();
    }

    private void UpdateStats(int strength, int intellect, int dexterity, int constitution, int wisdom, int charisma) {
        Strength += strength;
        Intellect += intellect;
        Dexterity += dexterity;
        Constitution += constitution;
        Wisdom += wisdom;
        Charisma += charisma;
        SetDamageStat();
    }

    private void SetDamageStat() {
        if (ClassName == "WarriorClass" || ClassName == "ThiefClass") {
            DamageStat = Strength;
        }
        if (ClassName == "MageClass") {
            DamageStat = Intellect;
        }
        if (ClassName == "DruidClass") {
            DamageStat = (Strength + Intellect)/2;
        }
    }

    public void AddDictKeysToList() {
        foreach (KeyValuePair<string, string> keyValuePair in AbilitiesDictionary) {
            AbilitiesList.Add(keyValuePair.Key);
        }
    }

    public void FindAbilityDescription() {
        foreach (string ability in AbilitiesList) {
            if (AbilitiesDictionary.ContainsKey(ability)) {
                print(AbilitiesDictionary[ability]);
            }
        }
    }
}
