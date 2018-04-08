using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class AbilityPanel : MonoBehaviour {

	private GameObject[] abilities; //stores all pictures
	public int[] levelsNeeded = new int[8]; //stores the level requirement


	// Use this for initialization
	void Start () {
		abilities = new GameObject[transform.childCount]; //set the size of the array to fit all images

		for (int i = 0; i < abilities.Length; i++)
		{
			abilities[i] = transform.GetChild(i).gameObject; //store all images in the array
		}

		gameObject.GetComponent<CanvasRenderer>().SetAlpha(0); //start off with the background hidden
		UpdateVisibility(); //and hide all
	}
	
	public void UpdateVisibility () {

		if (gameObject.GetComponent<CanvasRenderer>().GetAlpha() == 0) //if the panel has 0 alpha
		{
			for (int i = 0; i < abilities.Length; i++)
			{
				abilities[i].SetActive(false); //set all pictures to invisible
			}
		}
		else //if the panel has alpha
		{
			for (int i = 0; i < abilities.Length; i++)
			{
				if (PlayerPrefs.GetInt("Level") < levelsNeeded[i]) //if you do not meet the level requirement
				{
					abilities[i].SetActive(false); //hide the ability
				}
				else
				{
					abilities[i].SetActive(true); //show it

				}
			}
		}
	}
}
