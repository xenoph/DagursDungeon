using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class Bars : MonoBehaviour {

	private Slider barBase; //stores this

	public bool isPlayerHealth; //if this is the players healthbar
	public bool isEnemyHealth; //if this is the enemys healthbar
	public bool isXp; //if this is the xpBar

	// Use this for initialization
	void Start () {
		barBase = GetComponent<Slider>(); //makes is so one can edit this

		if (isPlayerHealth)
		{
			barBase.maxValue = PlayerPrefs.GetInt("Constitution") * 10; //sets max value to max health of player (con*10)
			//PlayerPrefs.SetInt("CurrentHealth", 50);
		}
		else if (isXp)
		{
			//PlayerPrefs.SetInt("ToLevelUp", 1000);
			barBase.maxValue = PlayerPrefs.GetInt("ToLevelUp"); //Gets how far this has to go to get a new level
		}
		else if (isEnemyHealth)
		{
			barBase.maxValue = this.GetComponent<EnemyScript>().BaseHealth; //gets the max health an enemy can have
		}

	}

	void Update()
	{
		if (isPlayerHealth)
		{
			barBase.value = PlayerPrefs.GetInt("Health"); //updates to show the current health
            barBase.maxValue = PlayerPrefs.GetInt("Constitution") * 10;
        }
		else if (isXp)
		{
			barBase.value = PlayerPrefs.GetInt("CurrentXP"); //updates to show the current xp the player has
			barBase.maxValue = PlayerPrefs.GetInt("ToLevelUp"); //updates to show if the player has leveled up
		}
		else if (isEnemyHealth)
		{
			barBase.value = this.GetComponent<EnemyScript>().BaseHealth; //updates to show the current health of the enemy
		}
	}
}
