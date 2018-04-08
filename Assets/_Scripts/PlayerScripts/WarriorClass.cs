using UnityEngine;
using System.Collections;

public class WarriorClass : CombatCommands {

    public string ClassName = "Warrior";       
    public int Strength = 14;
    public int Constitution = 14;
    public int Intellect = 7;
    public int Dexterity = 10;
    public int Wisdom = 7;
    public int Charisma = 8;

	public int chargeAmount;
	public int storedDamage;
	public string lastMove;


    //Special Abilities
    public void Shield(string arg) {
        if (OnCooldown("shield")) return;
		lastMove = "ShieldBlock";
        DialogueScript.GameInformationText("You raise your shield.");
        EndOfTurn("ShieldBlock", 1, 0);
        //You block all attacks for 5 seoconds
    }

    public void Slam() {
        if (OnCooldown("slam")) return;
		lastMove = "Slam";
		int damageToWeapon = Random.Range(0,2);

        if (PlayerPrefs.GetInt("Level") < 2) {
            DialogueScript.ErrorText("I think you're trying to do something you've not learned yet.");
            DialogueScript.CombatTextEnemy("Your confusing face makes the foe ready to strike.");
            ChangeTurn(null, null);
            return;
        }
        DialogueScript.GameInformationText("You slam your weapon hard into the enemy, stunning him!");
		if (PlayerPrefs.GetInt("StrenghtWeaponBuff") > 0 && damageToWeapon > 2)
		{
			PlayerPrefs.SetInt("StrengthWeaponBuff", PlayerPrefs.GetInt("StrenghtWeaponBuff") - damageToWeapon);
			DialogueScript.GameInformationText("Your weapon dulls in with your attack.");
		}
		EndOfTurn("Slam", 3, Random.Range(PlayerPrefs.GetInt("Strength")/2, PlayerPrefs.GetInt("Strenght")));
    }

    public void Cleave() {
        if (OnCooldown("cleave")) return;
		lastMove = "Cleave";
        //Do a vertical cleave for x damage
		if (PlayerPrefs.GetInt("Level") < 4)
		{
			DialogueScript.ErrorText("You're not quite sure how to do that yet.");
			DialogueScript.ErrorText("Maybe after a couple more battles");
			DialogueScript.CombatTextEnemy("You space out a bit, and the enemy strikes!");
			ChangeTurn(null, null);
			return;
		}
		if (chargeAmount <= 0)
		{
			DialogueScript.GameInformationText("You fill up with rage.");
			DialogueScript.GameInformationText("Next turn you can do the attack!");
			EndOfTurn("Cleave", 1, 0);
			chargeAmount = 3;
		}
		else
		{
			chargeAmount = 0;
			DialogueScript.GameInformationText("You strike the enemy with a heavy hit!");
			EndOfTurn("Cleave", 3, Random.Range(PlayerPrefs.GetInt("Strength") + chargeAmount, PlayerPrefs.GetInt("Strenght") + PlayerPrefs.GetInt("Level") + chargeAmount));
		}

    }

    public void CounterAttack() {
        if (OnCooldown("counterattack")) return;
		if (PlayerPrefs.GetInt("Level") < 6)
		{
			DialogueScript.ErrorText("Oh, that is something you havn't learnt to do yet.");
			DialogueScript.CombatTextEnemy("The enemy sees an opening, and stikes!");
			ChangeTurn(null, null);
			return;
		}
		if (lastMove != "ShieldBlock")
		{
			DialogueScript.ErrorText("You need to block first!");
		}
		else
		{
			int cm = PlayerPrefs.GetInt("Level");
			if (cm > 3) {cm = 3;} //cant do more than three times stored damage
			DialogueScript.GameInformationText("You see an opening, and strike!");
			EndOfTurn("CounterAttack", 1, Random.Range(storedDamage, storedDamage * cm));
		}
        //Do a quick attack for x damage after blocking an attack
		lastMove = "CounterAttack";
		storedDamage = 0;

    }

	public void StrainSlash() {
	    if (OnCooldown("strainslash")) return;
		lastMove = "StrainSlash";
		if (PlayerPrefs.GetInt("Level") < 8)
		{
			DialogueScript.ErrorText("While you grasp the consept, you still don't know how to do it");
			DialogueScript.CombatTextEnemy("The enemy sees your uncertainty, and strikes!");
			ChangeTurn(null,null);
			return;
		}

		if (PlayerPrefs.GetInt("Strength") > 5)
		{
			DialogueScript.GameInformationText("You strain yourself in the attack!");
			DialogueScript.GameInformationText("You feel weaker.");
			PlayerPrefs.SetInt("Strenght", PlayerPrefs.GetInt("Strenght") - 1); //-1 permanent strenght
			EndOfTurn("StrainSlash", 3, Random.Range((PlayerPrefs.GetInt("Strength") + 1) * 2, 
				(PlayerPrefs.GetInt("Strenght") + 1 + PlayerPrefs.GetInt("Level"))*2)); 
		}
		else
		{
				DialogueScript.GameInformationText("You are not strong enough for this anymore.");
				DialogueScript.GameInformationText("Try a different attack");
		}
			
	}

    public void Execute() {
        if (OnCooldown("execute")) return;
		lastMove = "Execute";
        if (Target != null) {
            if (Target.GetComponent<EnemyScript>().BaseHealth < (Target.GetComponent<EnemyScript>().StartingHealth/2)) {
                DialogueScript.CombatTextPlayer("You perform a hard-hitting Execute on your weakened target!");
                //EndOfTurn("Execute", 3, (Target.GetComponent<EnemyScript>().BaseHealth/2));
				EndOfTurn("Execute", 3, (PlayerPrefs.GetInt("Strength") * 5));
            }
        }
        else {
            throw new UnityException("There's no target.");
        }
    }
}
