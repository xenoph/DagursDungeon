using UnityEngine;
using System.Collections;

public class MageClass : CombatCommands {

    public string ClassName = "MageClass";
    public int Strength = 8;
    public int Constitution = 11;
    public int Intellect = 16;
    public int Dexterity = 9;
    public int Wisdom = 7;
    public int Charisma = 9;


    void Start() {
        //_pScript.SetPlayerStats(Strength, Constitution, Intellect, Dexterity, Wisdom, Charisma, "MageClass");
        PlayerScript.AbilitiesDictionary.Add("ArcaneShield", "Reflect a small amount of damage back at your attacker for 10 seconds.");
    }

    //Special Abilities
    public void ArcaneShield() {
        //Reflect x damage
    }

    public void Fireball() {
        //Send off a ball of fire for x damage
    }

    public void Daze() {
        //Stun your enemy for x seconds
    }

    public void MagicMissile() {
        //Conjure a magic missile to do x damage
    }

    public void WallOfForce() {
        //Send out a forcewall to hit the enemy hard for x damage
    }
}
