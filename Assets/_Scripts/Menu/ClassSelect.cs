using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ClassSelect : MonoBehaviour {

	public Text[] selectable = new Text[11];
	public int[] statStorage = new int[6];
	public string[] statName = new string[6];
	private string[] infoText = new string[6];

	public int selectionIndex = 0;


	public Text warriorSelection, thiefSelection, mageSelection ,druidSelection; //classes

	public Text strengthStat, constitutionStat, dexterityStat, intellectStat, wisdomStat, charismaStat; //stats text

	public Text healthStat, staminaStat, pointsLeftStat; //stat text that cant be changed

	public Text statInfo, nextScene;


	public GameObject classStorage; //stores info about classes

	private WarriorClass warriorStatStorage;
	private DruidClass druidStatStorage;
	private ThiefClass thiefStatStorage;
	private MageClass mageStatStorage;

	public string chosenClass; //for storing what class you have picked dunno what type it should be

	private int strength, constitution, dexterity, intellect, wisdom, charisma; //stores stats

	private int pointsLeft = 0;

	// Use this for initialization
	void Start () {
		selectable[0] = warriorSelection;
		selectable[1] = thiefSelection;
		selectable[2] = mageSelection;
		selectable[3] = druidSelection;

		selectable[4] = strengthStat;
		selectable[5] = constitutionStat;
		selectable[6] = dexterityStat;
		selectable[7] = intellectStat;
		selectable[8] = wisdomStat;
		selectable[9] = charismaStat;

		selectable[10] = nextScene;

		selectable[0].text = "Warrior";
		selectable[1].text = "Thief";
		selectable[2].text = "Mage";
		selectable[3].text = "Druid";

		selectable[4].text = "-";
		selectable[5].text = "-";
		selectable[6].text = "-";
		selectable[7].text = "-";
		selectable[8].text = "-";
		selectable[9].text = "-";

		selectable[10].text = "Start your adventure";

		infoText[0] = "Strength - Determines physical damage and carrying weight"; //"ME SMASH With me Big HEAVY BoUlder!!11"; 
		infoText[1] = "Constitution - Determines your health"; //"Life is short, make it worthwhile."; 
		infoText[2] = "Dexterity - Determines your crit chance, evade chance"; //"Slime yourself up!"; 
		infoText[3] = "Intellect - Determines your magic damage, ability to save up action points"; // "Knowledge is power, and I know some things."; 
		infoText[4] = "Wisdom - Determines the amount of actions point you get"; // "Experinced with experiences."; 
		infoText[5] = "Charisma - Gives you better prices at shops, better items"; //"One mans trash is another ones treasure."; 

		warriorStatStorage = classStorage.GetComponent<WarriorClass>();
		thiefStatStorage = classStorage.GetComponent<ThiefClass>();
		druidStatStorage = classStorage.GetComponent<DruidClass>();
		mageStatStorage = classStorage.GetComponent<MageClass>();

		statName[0] = "Strength";
		statName[1] = "Constitution";
		statName[2] = "Dexterity";
		statName[3] = "Intellect";
		statName[4] = "Wisdom";
		statName[5] = "Charisma";
	}

	void Update()
	{
		MoveSelector();

		if (Input.GetKeyDown(KeyCode.Escape))
		{
			SceneManager.LoadScene("MainMenu");
		}

		if (Input.GetKeyDown(KeyCode.RightArrow) || Input.GetKeyDown(KeyCode.Return)) // if you press right or enter
		{
			if (selectable[selectionIndex].text == "Warrior") //and you are on warrior
			{
				strength = warriorStatStorage.Strength;
				constitution = warriorStatStorage.Constitution;
				dexterity = warriorStatStorage.Dexterity;
				intellect = warriorStatStorage.Intellect;
				wisdom = warriorStatStorage.Wisdom;
				charisma = warriorStatStorage.Charisma;

				chosenClass = "WarriorClass";
				SaveStat();
				NoBold();
				warriorSelection.fontStyle = FontStyle.Bold;
				pointsLeftStat.text = "10";
				pointsLeft = 10;
			}
			else if (selectable[selectionIndex].text == "Thief") //and you are on thief
			{
				strength = thiefStatStorage.Strength;
				constitution = thiefStatStorage.Constitution;
				dexterity = thiefStatStorage.Dexterity;
				intellect = thiefStatStorage.Intellect;
				wisdom = thiefStatStorage.Wisdom;
				charisma = thiefStatStorage.Charisma;

				chosenClass = "ThiefClass";
				SaveStat();
				NoBold();
				thiefSelection.fontStyle = FontStyle.Bold;
				pointsLeftStat.text = "10";
				pointsLeft = 10;
			}
			else if (selectable[selectionIndex].text == "Mage") //and you are on mage
			{
				strength = mageStatStorage.Strength;
				constitution = mageStatStorage.Constitution;
				dexterity = mageStatStorage.Dexterity;
				intellect = mageStatStorage.Intellect;
				wisdom = mageStatStorage.Wisdom;
				charisma = mageStatStorage.Charisma;

				chosenClass = "MageClass";
				SaveStat();
				NoBold();
				mageSelection.fontStyle = FontStyle.Bold;
				pointsLeftStat.text = "10";
				pointsLeft = 10;
			}
			else if (selectable[selectionIndex].text == "Druid") //and you are on druid
			{
				strength = druidStatStorage.Strength;
				constitution = druidStatStorage.Constitution;
				dexterity = druidStatStorage.Dexterity;
				intellect = druidStatStorage.Intellect;
				wisdom = druidStatStorage.Wisdom;
				charisma = druidStatStorage.Charisma;

				chosenClass = "DruidClass";
				SaveStat();
				NoBold();
				druidSelection.fontStyle = FontStyle.Bold;
				pointsLeftStat.text = "10";
				pointsLeft = 10;
			}
			else if (pointsLeft > 0 && selectable[selectionIndex].text != "Start your adventure")
			{
				pointsLeft --;
				pointsLeftStat.text = pointsLeft.ToString();

				statStorage[(selectionIndex - 4)]++;
			}

			if (pointsLeftStat.text == "0" && selectable[selectionIndex].text == "Start your adventure")
			{
				SaveStat();
				SceneManager.LoadScene("Tutorial");
			}
		}
			
		if (Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetKeyDown(KeyCode.Backspace))
		{
			if (selectionIndex >= 4 && selectionIndex <= 9 && PlayerPrefs.GetInt(statName[(selectionIndex - 4)]) < (System.Int32.Parse(selectable[selectionIndex].text)))
			{
				pointsLeft ++;
				pointsLeftStat.text = pointsLeft.ToString();

				statStorage[(selectionIndex - 4)]--;
			}
		}

		UpdateStats();
	}

	void UpdateStats()
	{
		//gives you the new stat scores
		strength += statStorage[0];
		constitution += statStorage[1];
		dexterity += statStorage[2];
		intellect += statStorage[3];
		wisdom += statStorage[4];
		charisma += statStorage[5];

		//used for storing when you put a stat into something
		statStorage[0] = 0;
		statStorage[1] = 0;
		statStorage[2] = 0;
		statStorage[3] = 0;
		statStorage[4] = 0;
		statStorage[5] = 0;

		//just shows the player what their non editable stats will look like
		healthStat.text = "Health: " + (constitution * 10);
		staminaStat.text = "Action Points: " + (dexterity * 10);

		//shows what your stats are
		strengthStat.text = strength.ToString();
		constitutionStat.text = constitution.ToString();
		dexterityStat.text = dexterity.ToString();
		intellectStat.text = intellect.ToString();
		wisdomStat.text = wisdom.ToString();
		charismaStat.text = charisma.ToString();
	}

	void NoBold()
	{
		for (int i = 0; i < 4; i++)
		{
			selectable[i].fontStyle = FontStyle.Normal; //makes all classes non-bold
		}
	}

	void TextToBlack()
	{
		for (int i = 0; i < selectable.GetLength(0); i++) //makes all black (so only the selected one will be green
			{
			selectable[i].color = Color.black;
			}
	}

	void MoveSelector()
	{
		if (Input.GetKeyDown(KeyCode.DownArrow)) //pressing the downArrow
		{
			TextToBlack();	//makes everything black

			if (selectionIndex != selectable.GetLength(0) - 1)	//if you are not at the end of the array
			{
				selectionIndex ++;	//add one to the selector
			}
			else
			{
				selectionIndex = 0; //if you are at the end go back to start
			}

			selectable[selectionIndex].color = Color.green; //make the currently selected item green
		}

		if (Input.GetKeyDown(KeyCode.UpArrow)) //if you press the upArrow
		{
			TextToBlack(); //make all text black

			if (selectionIndex != 0) //if you are not at the start of the array
			{
				selectionIndex --; // subtract one from the selector
			}
			else
			{
				selectionIndex = selectable.GetLength(0) - 1; //if you are at the start go to the end
			}

			selectable[selectionIndex].color = Color.green; //make the currently selected item green
		}

		if (selectionIndex >= 4 && selectionIndex <= 9) //if you have selected a stat
		{
			statInfo.text = infoText[selectionIndex - 4]; //show what the stat do
		}
		else
		{
			statInfo.text = ""; //else blank out the text
		}
	}

	void SaveStat()
	{
        PlayerPrefs.SetString("Class", chosenClass);
		PlayerPrefs.SetInt("Strength", strength);
		PlayerPrefs.SetInt("Constitution", constitution);
		PlayerPrefs.SetInt("Dexterity", dexterity);
		PlayerPrefs.SetInt("Intellect", intellect);
		PlayerPrefs.SetInt("Wisdom", wisdom);
		PlayerPrefs.SetInt("Charisma", charisma);
	    PlayerPrefs.SetInt("CurrentXP", 0);
        PlayerPrefs.SetInt("Level", 1);
	}
}
