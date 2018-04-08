using UnityEngine;
using System.Collections.Generic;
using System.Text;
using UnityEngine.UI;

public class LevelUP : MonoBehaviour {

    private readonly List<int> _newLevels = new List<int> {300, 500, 800, 1200, 1600, 2000, 2500, 3000, 3600, 4000};

    private Dialogue _dScript;
    private string _newStats;

    void Start() {
        //Get the Dialogue script, then set the amount needed to level up
        _dScript = GameObject.FindGameObjectWithTag("UI").GetComponent<Dialogue>();
        PlayerPrefs.SetInt("ToLevelUp", _newLevels[PlayerPrefs.GetInt("Level") - 1]);
    }

    /// <summary>
    /// Checks if the Player has reached the needed XP to level up
    /// </summary>
    public void CheckForLevelUp() {
        //Called everytime an enemy dies - get the current xp and check if it is more than what is needed to level up
        if(PlayerPrefs.GetInt("CurrentXP") >= _newLevels[PlayerPrefs.GetInt("Level") - 1]) {
            //Set the current XP back to 0, and set the level up +1
            PlayerPrefs.SetInt("CurrentXP", 0);
            PlayerPrefs.SetInt("Level", PlayerPrefs.GetInt("Level") + 1);
            GameObject.FindGameObjectWithTag("UI").GetComponent<InterfaceScript>().InfoLevelText.text = "Your Level: " +
                                                                                                        PlayerPrefs
                                                                                                            .GetInt(
                                                                                                                "Level");
            //Play the level up sound and the animation to go with it
            GameObject.Find("LevelUp").GetComponent<AudioSource>().Play();
            GameObject.FindGameObjectWithTag("UI").GetComponent<InterfaceScript>().ShowLevelUp();
            //Inform the player through text how much they need to level up more, and set the ToLevelUp PlayerPrefs
            _dScript.GameInformationText("Level up! To get to the next level you now need " + _newLevels[PlayerPrefs.GetInt("Level") - 1] + " experience points");
            PlayerPrefs.SetInt("ToLevelUp", _newLevels[PlayerPrefs.GetInt("Level") - 1]);
            //Call function to give the Player some stats
            RandomizeStats();
        }
    }

    /// <summary>
    /// Gives the Player random stat points (1-3) in random stats (1-3 stats)
    /// </summary>
    private void RandomizeStats() {
        //Create a list of the Player stats, then a Dict to help show feedback afterwards
        List<string> stats = new List<string> {"Dexterity", "Strength", "Constitution"};
        Dictionary<string, int> upgradedStats = new Dictionary<string, int>();
        //Pick a random number of stats
        int numberOfStats = Random.Range(1, 4);
        //Do a For loop with the random number
        for (int i = 0; i < numberOfStats; i++) {
            //Create a copy of the stat list, pick a random one
            string[] copiedStats = stats.ToArray(); 
            string randomStatChosen = copiedStats[Random.Range(0, copiedStats.Length)];
            //Buff the stat with a random number, then change the PlayerPrefs for the buffed stat
            int bonusStatNumber = Random.Range(1, 4);
            PlayerPrefs.SetInt(randomStatChosen, PlayerPrefs.GetInt(randomStatChosen) + bonusStatNumber);
            //Remove the chosen stat from the stat list (if the random stat number was more than one)
            stats.Remove(randomStatChosen);
            //Add the stat and buff number to the Dict
            upgradedStats.Add(randomStatChosen, bonusStatNumber);
        }
        //Inform the player by iterating through the Dict
        StringBuilder newStats = new StringBuilder();
        foreach (KeyValuePair<string, int> upgradedStat in upgradedStats) {
            newStats.Append(upgradedStat.Key + " : " + upgradedStat.Value + "\n");
            //_dScript.GameInformationText(upgradedStat.Key + " : " + upgradedStat.Value);
        }
        _newStats = newStats.ToString();
        _dScript.GameInformationText("Your stats have your increased! You gained: " );
        _dScript.GameInformationText(_newStats);
        //Show the level up panel after 3 seconds
        Invoke("ShowLevelUpPanel", 3);
    }

    /// <summary>
    /// Shows the level up panel.
    /// </summary>
    private void ShowLevelUpPanel() {
        //Get the level up panel gameobject that is stored in the InterfaceScript and set it to active
        GameObject levelUpPanel = GameObject.FindGameObjectWithTag("UI").GetComponent<InterfaceScript>().LevelUpPanel;
        levelUpPanel.SetActive(true);
        //Find the various texts on the panel
        Text congratulationText = GameObject.Find("CongratulationText").GetComponent<Text>();
        Text reachedLevelText = GameObject.Find("ReachedLevelText").GetComponent<Text>();
        Text gainedStatsText = GameObject.Find("GainedStatsText").GetComponent<Text>();
        congratulationText.text = "Congratulations!";
        reachedLevelText.text = "You've reached Level " + PlayerPrefs.GetInt("Level") + "!";
        gainedStatsText.text = "You have raised your stats!\n" + _newStats;
        Invoke("HideLevelUpPanel", 4);
    }

    /// <summary>
    /// Hides the level up panel.
    /// </summary>
    private void HideLevelUpPanel() {
        GameObject levelUpPanel = GameObject.FindGameObjectWithTag("UI").GetComponent<InterfaceScript>().LevelUpPanel;
        levelUpPanel.SetActive(false);
    }

    /// <summary>
    /// Returns experience needed to reach next level
    /// </summary>
    /// <returns></returns>
    public int NextLevel() {
        return _newLevels[PlayerPrefs.GetInt("Level") - 1];
    }
}
