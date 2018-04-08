using UnityEngine;
using System.Collections;

public class ItemGenerator : MonoBehaviour {

	private string playerClass; //stores the player class
	private int playerLevel; //stores the player level

	public string itemName; //the name of the generated item

	private string weapon; //the name of the item for each class
	private string armor; //the armor picked
	private string mainBuff; //the main buff of each class

	private string[] stats = new string[6]; //stores the stats
	private string[] rarity = new string[3]; //stores the rarities
	private string[] itemTypes = new string[4]; //the different kinds of items

	private int itemIndex = 0;

	//Giving stats to items
	public string mainBuffName; //name of main buff
	public string rareBuffName; //name of rare buff
	public string legendaryBuffName; //name of legendary bugg
	public int mainBuffPoints; //points from main buff
	public int rareBuffPoints; //points from rare buff
	public int legendaryBuffPoints; //points from legendary buff

    private Dialogue _dialogueScript;
    private Commands _commandScript;

	public void Start () {
	    _dialogueScript = GameObject.FindGameObjectWithTag("UI").GetComponent<Dialogue>();
	    _commandScript = GameObject.FindGameObjectWithTag("Player").GetComponent<Commands>();
		//levelCounter = PlayerPrefs.GetInt ("LevelCounter");

		playerClass = PlayerPrefs.GetString("Class"); //gets the class of the player
		playerLevel = 1; //player level dummy

		//stores the stats
		stats[0] = "Constitution";
		stats[1] = "Dexterity";
		stats[2] = "Strength";
		stats[3] = "Intellect";
	    //stats[4] = "Wisdom";
	    //stats[5] = "Charisma";
	    // stores the rarities
		rarity[0] = "";
		rarity[1] = "Rare ";
		rarity[2] = "Legendary ";
		//stores the item types
		itemTypes[0] = "Weapon";
		itemTypes[1] = "Armor";
		itemTypes[2] = "TempPotion";
		itemTypes[3] = "PermPotion";

	    int itemIndex = Random.Range(2,4);
		if (itemIndex == 4) //if item index is 4
		{
			itemIndex --; //sets the item index to 3 which dobbles the chance for 3
		}

		int rarityScore = Random.Range(playerLevel, playerLevel * 2);//culculates a score for rarity
		int raritySelected = 0; //rarity starts off at common
		int multiplier = 1; //stat multiplier starts off at normal (*2 if legendary)

		if (rarityScore > 10) //if the rarityscore is over this
		{
			raritySelected = 2; //it is legendary
			multiplier = 2; //it gets a bonus to stats
		}
		else if (rarityScore > 5) //if it is over this
		{
			raritySelected = 1; //it is rare
		}


		//=======Weapon and Stat from class
		if (playerClass == "WarriorClass") //if player is warrior
		{
			weapon = "Sword"; //they use a sword
			mainBuff = "Strength"; //and the stat they wants buffed is strength
		}
		if (playerClass == "ThiefClass") //if the player is thief
		{
			weapon = "Daggers"; //they use daggers
			mainBuff = "Dexterity"; //and want a dexterity buff
		}
		if (playerClass == "DruidClass") //if the player is druid
		{
			if (PlayerPrefs.GetInt("Strength") >= PlayerPrefs.GetInt("Intellect")) //and their strength is higher than their intellect
			{
				mainBuff = "Strength"; //they want to get more strength
			}
			else
			{
				mainBuff = "Intellect"; //if their intellect is bigger they want an intellect buff
			}
			weapon = "Staff"; //and they use a staff
		}
		if (playerClass == "MageClass") //if the player is a mage
		{
			weapon = "Wand"; //they use a wand
			mainBuff = "Intellect"; //and want an intellect buff
		}


		if (itemIndex == 0) //is a weapon
		{
			GenerateWeapon(raritySelected, multiplier, rarityScore);
		}
		else if (itemIndex == 1) //is an armor
		{
			GenerateArmor();
		}
		else if (itemIndex == 2) //is temp potion
		{
			GenerateTempPotion();
		}
		else if (itemIndex == 3) //is perm potion
		{
			GeneratePermPotion();	
		}

		//if any stat is under 0 set them to 0
		if (mainBuffPoints < 0)
		{
			mainBuffPoints = 0;
		}
		if (rareBuffPoints < 0)
		{
			rareBuffPoints = 0;
		}
		if (legendaryBuffPoints < 0)
		{
			legendaryBuffPoints = 0;
		}

		if (Random.Range(0,2) == 1)
		{
			GenerateArmor();
		}
		else
		{
			GenerateWeapon(raritySelected, multiplier, rarityScore);
		}
	}

	void GenerateArmor()
	{
		int pickStat = Random.Range(1,6);
		while (stats[pickStat] == mainBuff)
		{
			pickStat = Random.Range(1,6);
		}

		mainBuffName = "Constitution";
		mainBuffPoints = Random.Range(playerLevel, Mathf.FloorToInt(playerLevel + PlayerPrefs.GetInt("Charisma"))/2);

		rareBuffName = stats[pickStat];
		rareBuffPoints = Random.Range(Mathf.FloorToInt(playerLevel), Mathf.FloorToInt(playerLevel + PlayerPrefs.GetInt("Charisma"))/4);
		itemName = "Armor" + " +" + mainBuffPoints + " - " + rareBuffName + " +" + rareBuffPoints;
	}

	void GenerateWeapon(int raritySelected, int multiplier, int rarityScore)
	{

		itemName = rarity[raritySelected] + weapon; //Starts nameing the weapon after the class weapon

		if (Random.Range(0,5) != 1) // 16% chance of not getting
		{
			mainBuffName = mainBuff; //the mainbuff
		}
		else
		{
			mainBuffName = stats[Random.Range(1,6)]; 
		}

		mainBuffPoints = Random.Range(Mathf.FloorToInt(rarityScore/4) * multiplier, Mathf.FloorToInt(rarityScore/3 * 1.5f * multiplier)); //sets the mainstat of the weapon


		if (mainBuffPoints > 0) //if the common stat is over 0
		{
			itemName += " of " + mainBuffName + " +" + mainBuffPoints; //give it its common name
		}

		if (raritySelected >= 1) //if it is rare or better
		{
			int selectedBuff = Random.Range(1,6); //selects a stat
			while (stats[selectedBuff] == mainBuffName)
			{
				selectedBuff = Random.Range(1,6); //selects a different stat if this was the same as another stat
			}

			rareBuffName = stats[selectedBuff];
			rareBuffPoints = Random.Range(Mathf.FloorToInt(rarityScore/4) * multiplier, Mathf.FloorToInt(rarityScore/3 * multiplier)); //sets the mainstat of the weapon

			if (rareBuffPoints > 0)
			{
				if (mainBuffPoints == 0)
				{
					itemName += " of";
				}
				else
				{
					itemName += " -";
				}
				itemName += " " + stats[selectedBuff] + " +" + rareBuffPoints;
			}
		}

		if (raritySelected >= 2)
		{
			int selectedBuff = Random.Range(1,6); //selects a stat
			while (stats[selectedBuff] == mainBuffName || stats[selectedBuff] == rareBuffName)
			{
				selectedBuff = Random.Range(1,6); //selects a different stat if this was the same as another stat
			}

			legendaryBuffName = stats[selectedBuff];
			legendaryBuffPoints = Random.Range(Mathf.FloorToInt(rarityScore/4) * multiplier, Mathf.FloorToInt(rarityScore/4 * multiplier)); //sets the mainstat of the weapon

			//			abilityBuff3 = Random.Range(Mathf.FloorToInt(playerLevel/4) * multiplier, playerLevel * multiplier); //sets the legendary stat of the weapon
			//			PlayerPrefs.SetInt(stats[selectedBuff], PlayerPrefs.GetInt(stats[selectedBuff]) + abilityBuff2); //gives the player the legendary buff
			//			PlayerPrefs.SetInt(stats[selectedBuff] + "Buff", PlayerPrefs.GetInt(stats[selectedBuff] + "Buff") + abilityBuff3); //saves the legendary buff so it can be subtracted on new item

			if (legendaryBuffPoints > 0)
			{
				if (mainBuffPoints == 0 && rareBuffPoints == 0)
				{
					itemName += " of";
				}
				else
				{
					itemName += " -";
				}
				itemName += " " + stats[selectedBuff] + " +" + legendaryBuffPoints;
			}
		}
	}

	void GenerateTempPotion()
	{
		int pickedStat = Random.Range(0,3); //pick a stat
		int randomBonus = Random.Range(1,PlayerPrefs.GetInt("LevelCounter")); //random bonus added to a stat
		PlayerPrefs.SetInt(stats[pickedStat], PlayerPrefs.GetInt(stats[pickedStat]) - PlayerPrefs.GetInt(stats[pickedStat] + "TempBuff")); //remove your previous buff from your stat (so it is back to normal)
		PlayerPrefs.SetInt(stats[pickedStat] + "TempBuff", PlayerPrefs.GetInt(stats[pickedStat] + "TempBuff") + randomBonus); //add the random bonus to the temp bonus
	    PlayerPrefs.SetInt(stats[pickedStat],
	        PlayerPrefs.GetInt(stats[pickedStat]) + PlayerPrefs.GetInt(stats[pickedStat] + "TempBuff"));
	        //add the temp bonus to your stat
	    _dialogueScript.GameInformationText("A potion! It gives you an additional " + randomBonus + " points in " +
	                             GetStatName(pickedStat) + " for the remainder of this level!");
        _commandScript.GettingLoot("temporary");
    }

	void GeneratePermPotion() {
		int pickedStatToBuff = Random.Range(0,3);
		int pickedStatToNerf = Random.Range(0,3);

		while (pickedStatToBuff == pickedStatToNerf)
		{
			pickedStatToNerf = Random.Range(0,3);
		}

		mainBuffName = stats[pickedStatToBuff];
		rareBuffName = stats[pickedStatToNerf];

		mainBuffPoints = Random.Range(1,PlayerPrefs.GetInt("LevelCounter"));
		rareBuffPoints = Random.Range(PlayerPrefs.GetInt("LevelCounter") * -1, -1);

		itemName ="Potion of " + mainBuffName + " +" + mainBuffPoints + " And " + rareBuffName + " " + rareBuffPoints;
        _dialogueScript.GameInformationText("You picked up " + itemName + "!");
        PlayerPrefs.SetString("Buff", mainBuffName);
        PlayerPrefs.SetString("Debuff", rareBuffName);
        PlayerPrefs.SetInt("Buff", mainBuffPoints);
        PlayerPrefs.SetInt("Debuff", rareBuffPoints);
	    _commandScript.LootedPermPotion = true;
        _commandScript.GettingLoot("permanent");
	}

    string GetStatName(int statPlacement) {
        switch (statPlacement) {
            case 0:
                return "Constitution";

            case 1:
                return "Dexterity";

            case 2:
                return "Strength";

            case 3:
                return "Intellect";

            case 4:
                return "Wisdom";

            case 5:
                return "Charisma";
        }
        return null;
    }

	public void StoreItemToPlayerPrefs()
	{
		//removes the old item
		if (itemTypes[itemIndex] != "TempPotion" && itemTypes[itemIndex] != "PermPotion")
		{
			for (int i = 0; i < stats.Length; i++)
			{
				PlayerPrefs.SetInt(stats[i], PlayerPrefs.GetInt(stats[i]) - PlayerPrefs.GetInt(stats[i] + itemTypes[itemIndex] + "Buff")); //subtract the buff from the stat
				PlayerPrefs.SetInt(stats[i] + itemTypes[itemIndex] + "Buff", 0); //set the buff to 0
			}
		}

		//Adds the new item
		PlayerPrefs.SetString(itemTypes[itemIndex], itemName); //give the new name to the weapon
		PlayerPrefs.SetInt(mainBuffName + itemTypes[itemIndex] + "Buff", mainBuffPoints); //store the points of the mainbuff in ((MainStat)Buff)
		PlayerPrefs.SetInt(rareBuffName + itemTypes[itemIndex] + "Buff",rareBuffPoints); //store the points of the rarebuff in ((picked rare stat)Buff)
		PlayerPrefs.SetInt(legendaryBuffName + itemTypes[itemIndex] + "Buff",rareBuffPoints); //store the points of the legendarybuff in ((picked legendary stat)Buff)

		PlayerPrefs.SetInt(mainBuffName, PlayerPrefs.GetInt(mainBuffName) + mainBuffPoints); //Set the mainstat of the player to the mainbuffed amount
		PlayerPrefs.SetInt(rareBuffName, PlayerPrefs.GetInt(rareBuffName) + rareBuffPoints); //Set the rare stat of the player to the rare buffed amount
		PlayerPrefs.SetInt(legendaryBuffName, PlayerPrefs.GetInt(legendaryBuffName) + rareBuffPoints); //Set the legendary buff of the player to the legendary amount

	}

	public void ClearTempBonuses()
	{
		for (int i = 0; i < stats.Length; i++)
		{
			PlayerPrefs.SetInt(stats[i], PlayerPrefs.GetInt(stats[i]) - PlayerPrefs.GetInt(stats[i] + "TempBuff"));
		}
	}

	public void ShowItem()
	{
		for (int i = 0; i <stats.Length; i++)
		{
			if (PlayerPrefs.GetInt(stats[i] + itemTypes[itemIndex] + "Buff") > 0)
			{
				print(stats[i] + " +" + PlayerPrefs.GetInt(stats[i] + itemTypes[itemIndex] + "Buff"));
			}
		}
		print(PlayerPrefs.GetString(itemTypes[itemIndex]));
	}
}
