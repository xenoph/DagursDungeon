using UnityEngine;
using System.Collections;

public class DruidClass : CombatCommands {

    public string ClassName = "DruidClass";
    public int Strength = 11;
    public int Constitution = 12;
    public int Intellect = 11;
    public int Dexterity = 11;
    public int Wisdom = 7;
    public int Charisma = 8;


    void Start() {
        //_pScript.SetPlayerStats(Strength, Constitution, Intellect, Dexterity, Wisdom, Charisma, "DruidClass");
        PlayerScript.AbilitiesDictionary.Add("Charge", "Charge into your enemy to stun him for 3 seconds");
        PlayerScript.AbilitiesDictionary.Add("CureWounds",
            "Heal yourself for an amount based on your intelligence and level");
    }

    //Special Abilities
    public void Charge() {
        DialogueScript.GameInformationText("You charge at the enemy and stun him for 3 seconds!");
        //Charge into your enemy to stun him for x seconds
    }

    public void CureWounds() {
        DialogueScript.GameInformationText("You heal yourself for " + (PlayerScript.Intellect*PlayerScript.Level) + " hit points");
        //Heal yourself for x
    }

    public void Trip() {
        //Trip your enemy to stun him for x seconds
    }

    public void CallNature() {
        //Call upon nature to do x damage
    }

    public void BearsBite() {
        //Shapeshift into a bear and bite your enemy hard for x damage
    }
}
