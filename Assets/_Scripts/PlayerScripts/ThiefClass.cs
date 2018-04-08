using UnityEngine;
using System.Collections;

public class ThiefClass : CombatCommands {

    public string ClassName = "ThiefClass";
    public int Strength = 10;
    public int Constitution = 12;
    public int Intellect = 8;
    public int Dexterity = 15;
    public int Wisdom = 7;
    public int Charisma = 8;


    void Start() {
        //_pScript.SetPlayerStats(Strength, Constitution, Intellect, Dexterity, Wisdom, Charisma, "ThiefClass");
        PlayerScript.AbilitiesDictionary.Add("KnifeDance", "Quickly slash your enemy for a small amount of damage.");
    }

    //Special Abilities
    public void KnifeDance() {
        //Quickly slash your enemy for x damage
    }

    public void UncannyDodge() {
        //Use your dodge ability for free for x seconds
    }

    public void CripplingStrike() {
        //Cripple your enemy, decrease his attack rate
    }

    public void Opportunist() {
        //Follow up an attack with another quick attack for x damage
    }

    public void SneakAttack() {
        //Vanish behind a cloud to appear again behind the enemy and hit him hard for x damage
    }
}
