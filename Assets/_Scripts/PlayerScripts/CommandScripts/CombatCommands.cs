using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

public class CombatCommands : CommandBaseClass {

    public GameObject Target;

    private int _chanceToHit;
    private int _chanceToCrit;

    private int _actualDamage;

    public List<string> CooldownList;

    void Start() {
        CooldownList = new List<string>();
    }

    public void Slash(string arg) {
        if (OnCooldown("slash")) return;
        DialogueScript.CombatTextPlayer("You slash wildly at the enemy");
        EndOfTurn("Slash", 1, 7);
    }

    public void Thrust(string arg) {
        if (OnCooldown("thrust")) return;
            DialogueScript.CombatTextPlayer("You thrust your weapon towards the enemy.");
            EndOfTurn("Thrust", 4, 15);
    }

    public void Rekt(string arg) {
        DialogueScript.CombatTextPlayer("You cheat and teabag the enemy.");
        EndOfTurn("Rekt", 1, 10000);
    }

    internal bool OnCooldown(string function) {
        if(CooldownList.Any(s => s.Equals(function, StringComparison.CurrentCultureIgnoreCase))) {
            DialogueScript.ErrorText("You're currently cooling down from using that ability.");
            InterfaceScript.ActivateInput();
            return true;
        }
        return false;
    }

    /// <summary>
    /// Called after an ability is used, places abilites on cooldown and checks if the player hits.
    /// </summary>
    /// <param name="ability">The ability.</param>
    /// <param name="cd">The cd.</param>
    /// <param name="damage">The damage.</param>
    public void EndOfTurn(string ability, int cd, int damage) {
        //Make sure the script has a reference to the GameTurnController
        if (!GameTurnController) {
            GameTurnController = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameTurnController>();
        }
        switch(ability) {
            case "Thrust":
                GetComponent<Animator>().SetTrigger("PlayerThrust");
                break;

            case "Slash":
            case "StrainSlash":
                GetComponent<Animator>().SetTrigger("PlayerSlash");
                break;

            case "Block":
                GetComponent<Animator>().SetTrigger("PlayerBlock");
                break;

            case "Slam":
                GetComponent<Animator>().SetTrigger("PlayerSDash");
                break;
        }
        //Check if the Player hits
        string hit = CalculateHit();
        //Get a reference to the enemy
        GameObject enemyGameObject = CommandScript.GetRaycastObject();
        switch (hit) {
            //If missing
            case "Miss":
                //Inform the Player and let the enemy know
			if (ability != "Block" && ability != "ShieldBlock"){
                    DialogueScript.CombatTextPlayer("You missed the enemy!");               
				    enemyGameObject.GetComponent<EnemyScript>().PlayerMissed();
                }
                break;
            //If hit
            case "Hit":
                //Call the function to do damage and place the ability on cooldown
                StartCoroutine(DoDamage(damage, ability));
                AddToCooldownList(ability, cd);
                break;
            //Or if it's a crit
            case "Crit":
                //Call the damage function and give it twice the amount of damage, then set ability cooldown
                StartCoroutine(DoDamage(damage*2, ability));
                AddToCooldownList(ability, cd);
                break;
        }
        //Change the turn
        ChangeTurn(ability, hit);
    }

    /// <summary>
    /// Changes the turn after a combat move has finished checking for hit or miss
    /// </summary>
    /// <param name="ability">The ability.</param>
    /// <param name="hit">The hit.</param>
    public void ChangeTurn(string ability, string hit) {
		GameObject.FindGameObjectWithTag("Player").GetComponent<WarriorClass>().chargeAmount --;
        //If the ability was Slam and it hit (or crit)
        if(ability == "Slam" && (hit == "Hit" || hit == "Crit")) {
            //Inform the Player that the enemy is stunned
            DialogueScript.CombatTextPlayer("Your foe seems stunned by your shield!");
            DialogueScript.CombatTextPlayer("Quickly strike again before he regains his whereabouts!");
            //Return the state to Combat, add one to Turn counter and activate input
            GameTurnController.CurrentState = GameTurnController.PlayerState.Combat;
            CurrentTurn++;
            InterfaceScript.ActivateInput();
            return;
        }
        //Else, if the ability was null (i.e. missed)
        if (ability == null) {
            //Change state to Enemy turn, do 0 damage and add one to turn counter
            GameTurnController.CurrentState = GameTurnController.PlayerState.EnemyCombatTurn;
            StartCoroutine(DoDamage(0, "null"));
            CurrentTurn++;
            return;
        }
        //Otherwise, just change to enemy turn and add one to turn counter
        GameTurnController.CurrentState = GameTurnController.PlayerState.EnemyCombatTurn;
        CurrentTurn++;
    }

    //Is this needed now? Christer needs to implement his scripts.
    private int CalculateDamage(int basedamage) {
        _actualDamage = basedamage + (PlayerScript.DamageStat/2);
        return _actualDamage;
    }

    IEnumerator DoDamage(int damage, string ability) {
        if (!PlayerScript) {
           PlayerScript = GetComponent<Player>();
        }
        
        GameObject enemyGameObject = CommandScript.GetRaycastObject();
        var enemyScript = enemyGameObject.GetComponent<EnemyScript>();        
        yield return new WaitForSeconds(1);
        if (damage == 0) {
            enemyScript.PlayerMissed();
            yield break;
        }
		if (enemyGameObject.GetComponent<EnemyCombat>().isBlocking == false)
		{
       	 int damageDone = CalculateDamage(damage);
        	GameObject.FindGameObjectWithTag("Weapon").GetComponent<AudioSource>().Play();
            if (enemyGameObject.name != "BallOfDarkness"){
    	        enemyGameObject.GetComponent<AudioSource>().Play();
            }
  	      if (ability == "Slam") {
        	    enemyScript.CheckHealth(damageDone, ability);
    	    }
	        else {
            	enemyScript.CheckHealth(damageDone, ability);
        	}
        	GameObject.FindGameObjectWithTag("UI").GetComponent<InterfaceScript>().ShowDamage(damageDone);
		}
		else
		{
			DialogueScript.CombatTextEnemy("The enemy blocks your attack");
			GameTurnController.CurrentState = GameTurnController.PlayerState.EnemyCombatTurn;
			enemyGameObject.GetComponent<EnemyCombat>().StartTurn();
		}
    }

    IEnumerator RemoveFromCooldownList(string function, int cd, int turn) {
        int turnAtEndOfAbility = turn;
        while (turnAtEndOfAbility + cd != CurrentTurn) {
            yield return new WaitForFixedUpdate();
        }
        CooldownList.Remove(function);
    }

    private void AddToCooldownList(string function, int cd) {
        CooldownList.Add(function);
        StartCoroutine(RemoveFromCooldownList(function, cd, CurrentTurn));
    }

    private string CalculateHit() {
        RaycastHit target;
        Physics.Raycast(transform.position, transform.forward, out target, 2);

        _chanceToHit = 100 - 10;
        _chanceToCrit = PlayerPrefs.GetInt("Dexterity")/2;

        int actualHit = Random.Range(0, 100);
        if (actualHit < 100 - _chanceToHit) {
            return "Miss";
        }
        if (actualHit >= 10 && actualHit <= 100 - _chanceToCrit) {
            return "Hit";
        }
        if (actualHit > 100 - _chanceToCrit) {
            return "Crit";
        }

        return null;
    }

    public void Help(string arg, string arg2) {
        CommandScript.ShowHelp(arg, arg2);
    }

    public void List(string arg) {
        CommandScript.List(arg);
    }
}
