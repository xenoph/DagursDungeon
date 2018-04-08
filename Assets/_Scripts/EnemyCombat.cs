using UnityEngine;
using System.Collections;

public class EnemyCombat : CombatCommands {

	//stats; minimum requirement for enemies
	public int maxHp;
	public int strenght; //damage
	private string attackPattern; //a present tense word to describe their attack
	public int cowardness; //higher number means more chance to use block
	public string enemyName;

	//special
	public bool hasRegen;
	public int regenSpeed;
	public bool hasRage;
	public bool canDarken;

	//states and storage
	public int currentHp; //current hp
	private int lastTurnHp; //how much hp the monster had laast turn
	private int charge; //not in use, might be used for attacks that take time/do more damage if charged
	public bool isBlocking; //if the monster used block in its turn

	private string lastPlayerCommand;

	// Use this for initialization
	void Start () {

		enemyName = this.name.Remove(1, name.Length-1); //removes all but the last letter of the monsters name
		if (enemyName == "S") {enemyName = "Skeleton";} //Skeleton_Enemy
		if (enemyName == "D") {enemyName = "Troll";} //DungeonTrollPrefab
		if (enemyName == "B") {enemyName = "Darkness";} //BallOfDarkness

		if (enemyName == "Skeleton") {maxHp = 70; strenght = 7; attackPattern = "stabs"; cowardness = 25; hasRage = true;}
		else if (enemyName == "Troll") {maxHp = 100; strenght = 10; attackPattern = "clubs"; cowardness = 20; regenSpeed = 5; hasRegen = true; }
		else if (enemyName == "Darkness") {maxHp = 200; strenght = 5; attackPattern = "blinds"; cowardness = 0; canDarken = true;}
		else {enemyName = "???"; maxHp = 1; strenght = 0; attackPattern = "nulls"; cowardness = 5;} //can't find Monster, give it a new name
		currentHp = maxHp; //start off life with max hp
	}

	public void StartTurn()
	{
		lastPlayerCommand = GameObject.FindWithTag("Player").GetComponent<WarriorClass>().lastMove;
		//GetComponent<EnemyScript>().IsAttacking = true;
		//GameObject.Find("ScriptHolder").GetComponent<LevelScript>().EnemyAttacking = true;

		GameObject.Find("Canvas").GetComponent<InterfaceScript>().ChangeCommandPool("off");


		if (isBlocking && currentHp < lastTurnHp) //if the enemy was blocking
		{
			currentHp += (lastTurnHp - currentHp); //no damage was done to it
		}
		isBlocking = false; //no more blocking

		if (canDarken) {StartCoroutine("Darken"); return;} //will dim one torch
		if (hasRage && currentHp < maxHp/3) {StartCoroutine("WildAttack"); return;} //if the monster has rage and is under 1/3 hp do a rage attack
		if (Random.Range(0,cowardness) >= 15 && currentHp != maxHp) {Block(); return;} //else do a swing or a block
		Swing(); //default attack
	}

	void Swing()
	{
		int damage = Random.Range(Mathf.CeilToInt(strenght * 0.75f),Mathf.RoundToInt(strenght * 1.25f)); //some damage
		GameObject.FindGameObjectWithTag("Player").GetComponent<WarriorClass>().storedDamage = damage; //used for counterattack
		if (lastPlayerCommand == "ShieldBlock")
		{
			DialogueScript.CombatTextEnemy("The " + enemyName.ToLower() + " " + attackPattern + " you, but you block the damage.");
		}
		else
		{
		PlayerPrefs.SetInt("Health", PlayerPrefs.GetInt("Health") - damage); //damage the player
		DialogueScript.CombatTextEnemy("The " + enemyName.ToLower() + " " + attackPattern + " you for " + damage + " damage!");//damage done
		}
		Animate();
		FinishTurn();
	}

	IEnumerator Darken()
	{
		if (GameObject.Find("ScriptHolder").GetComponent<LevelScript>().TorchNumber == 0) //if all torches are dead, don't do anything but speak
		{
			DialogueScript.CombatTextEnemy("The darkness is the best isn't it?");
		}
		else
		{
			int randomText = Random.Range(0,3); //for picking a text

			if (randomText == 1) {DialogueScript.CombatTextEnemy("Dagur shall stay here forever!");}
			else if (randomText == 2){DialogueScript.CombatTextEnemy("The sun will never rise!");} 
			else {DialogueScript.CombatTextEnemy("Let there be darkness!");} 

			yield return new WaitForSeconds(3);
			DialogueScript.GameInformationText("One of the torches flickers");
			GameObject.Find("ScriptHolder").GetComponent<LevelScript>().TorchCountdown += 20; //add to the tourchcounter
			GameObject.Find("ScriptHolder").GetComponent<LevelScript>().TorchCounter(); //dim one light
		}

		FinishTurn();
	}

	void Block()
	{
		if (cowardness > 17) {cowardness -= 3;} //make it less likley to block
		GameObject.FindGameObjectWithTag("Player").GetComponent<WarriorClass>().storedDamage = 0; //so counterattack does 0 damage
		DialogueScript.CombatTextEnemy("The " + enemyName.ToLower() + " prepares for your attack."); //what did the enemy do
		isBlocking = true; //saves last action
		FinishTurn();
	}

	IEnumerator WildAttack()
	{
		int damage = Random.Range(Mathf.CeilToInt(strenght * 1.5f),Mathf.RoundToInt(strenght * 2.5f)); //some damage
		GameObject.FindGameObjectWithTag("Player").GetComponent<WarriorClass>().storedDamage = damage; //used for counterattack

		DialogueScript.CombatTextEnemy("The enraged " + enemyName.ToLower() + attackPattern + " at you wildly."); //what did the enemy do
		if (lastPlayerCommand == "ShieldBlock") 
		{
			yield return new WaitForSeconds(2f);
			DialogueScript.CombatTextEnemy("You try to block the enraged " + enemyName.ToLower() + " but the attacks are too random.");
		}
		yield return new WaitForSeconds(3f);
		Animate();
		if (Random.Range(0,2) == 0)
		{
			DialogueScript.CombatTextEnemy("The enraged " + enemyName.ToLower() + " hits! And " + attackPattern + " you for " + damage + " damage!"); //what did the enemy do
			PlayerPrefs.SetInt("Health", PlayerPrefs.GetInt("Health") - damage); //damage the player
			GameObject.FindGameObjectWithTag("Player").GetComponent<WarriorClass>().storedDamage = damage;
		}
		else
		{
			DialogueScript.CombatTextEnemy("The enraged " + enemyName.ToLower() + " misses you!");//writes
			yield return new WaitForSeconds(3f);
			DialogueScript.CombatTextEnemy("In the missed attack, the enraged " + enemyName.ToLower() + " damages itself.");//writes
			currentHp -= 10; //self damage
			GameObject.FindGameObjectWithTag("Player").GetComponent<WarriorClass>().storedDamage = 0;
		}
		FinishTurn();
	}


	void Animate()
	{
		if (currentHp > 0) //if it has ove 0 hp, attack else die
		{
			if (enemyName == "Troll"){GetComponent<Animator>().SetTrigger("Punch");}
			if (enemyName == "Skeleton"){GetComponent<Animator>().SetTrigger("StabAnimationTrigger");}
		}
		else
		{
			if (enemyName == "Troll"){GetComponent<Animator>().SetBool("TrollDeath", true);}
			if (enemyName == "Skeleton"){GetComponent<Animator>().SetTrigger("DeathAnimationTrigger");}
		}
	}


	void FinishTurn()
	{
		if (currentHp < lastTurnHp) {cowardness ++;} //if it lost hp this turn, make it more likley to block
		if (currentHp <= 0) //if it has no hp
		{
			if (enemyName != "Boss") {Animate();}//die
			else 
			{
				DialogueScript.CombatTextEnemy("NO YOU FOILED MY PLANS! BUT I WILL BE BACK!");
			}
			switch (enemyName) {
                    case "D":
                        GetComponent<Animator>().SetBool("TrollDeath", true);
                        break;

                    case "S":
                        GetComponent<Animator>().SetTrigger("DeathAnimationTrigger");
                        break;
                }
			Invoke("EndOfMe", 2f); //get deadededed
		}

		if (hasRegen == true && currentHp < maxHp && currentHp > 0) //if it has regen, and it hp is less than max, and more than 0
		{
			currentHp += regenSpeed; //if the enemy has regen, add regen to health
			DialogueScript.CombatTextEnemy("Some of the " + enemyName + "s wounds close up!");
		} 
		if (currentHp > maxHp) {currentHp = maxHp;} //if health exeeds max health set it to max health
		lastTurnHp = currentHp; //ends the turn, and stores how much hp this used to have

		if (PlayerPrefs.GetInt("Health") <= 0) {KillPlayer();} //if the player has 0 hp kill it

		CurrentTurn ++; //add to current turn
		GameTurnController.CurrentState = GameTurnController.PlayerState.Combat; //change turn
		InterfaceScript.ActivateInput();
	}

	void EndOfMe()
	{		
		GetComponent<EnemyScript>().EndOfMe();
	}

	void KillPlayer()
	{
		DialogueScript.CombatTextEnemy("The Enemy strikes a mortal blow and the world is darkening...."); 
		GameObject.FindGameObjectWithTag("ScriptHolder").GetComponent<LevelFader>().SceneFinished = true; //fade to darkness
		GameObject.FindGameObjectWithTag("UI").GetComponent<InterfaceScript>().DeActivateInput();
		GetComponent<EnemyScript>().Invoke("PlayerDead", 3);
	}
}
