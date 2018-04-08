using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class InGameMenu : MonoBehaviour {

	public GameObject mainMenu, close, muteBox;

	private bool shown = false; //stores if the menu is shown or not

	public GameObject[] selectable = new GameObject[3]; //stores the menu items
	private int selectionIndex = 2; //stores where in the menu is selected

	// Update is called once per frame
	void Update () {
		
		if (Input.GetKeyDown(KeyCode.DownArrow) && shown == true) //if down is pressed an the menu is shown
			{
			selectable[selectionIndex].GetComponent<Image>().color = Color.black; //makes the previous selected black
				if (selectionIndex < selectable.Length - 1) //then if this isnt the last
				{
					selectionIndex ++; //select one "down"
				}
				else //if it is the last
				{
					selectionIndex = 0; // go to the start
				}
			selectable[selectionIndex].GetComponent<Image>().color = Color.green; //then the newly selected becomes green
			}

		if (Input.GetKeyDown(KeyCode.UpArrow) && shown == true) //if up is pressed and the menu is shown
		{
			selectable[selectionIndex].GetComponent<Image>().color = Color.black; //the previously selected becomes black
			if (selectionIndex > 0) //if this isnt the first
			{
				selectionIndex --; //go one "up"
			}
			else
			{
				selectionIndex = selectable.Length - 1; //go to the last
			}
			selectable[selectionIndex].GetComponent<Image>().color = Color.green; //then the newly selected becomes green
		}

		if (Input.GetKeyDown(KeyCode.Return)) //if enter is pressed
		{
			if (shown == false) //and the menu is not shown
			{
				Show();
			}
			else if (shown == true) //if it is shown
			{
				if (selectable[selectionIndex].name == "mainMenuText") //if the selected is main menu
				{
                    UnityEngine.SceneManagement.SceneManager.LoadScene("MainMenu"); //goes to the main menu
					//needs to clear all buffs and items
				}

				if (selectable[selectionIndex].name == "closeText") //if close is selected
				{
					Close();
				}
			}
		}
	}

	public void Show () {
		shown = true; //the menu is shown

		selectionIndex = 2; //go to close

		GameObject mainMenuText = Instantiate(mainMenu, new Vector2 (0,30), Quaternion.identity) as GameObject; //make a new text
		GameObject closeText = Instantiate(close, new Vector2 (0,-60), Quaternion.identity) as GameObject; //-||-
		GameObject muteGame = Instantiate(muteBox, new Vector2(0, -30), Quaternion.identity) as GameObject;

		mainMenuText.name = "mainMenuText"; //name the text
		closeText.name = "closeText"; //-||-
		muteGame.name = "muteGame";

		mainMenuText.transform.SetParent(GameObject.Find("Canvas").transform, false); //put it on the canvas
		closeText.transform.SetParent(GameObject.Find("Canvas").transform, false); //-||-
		muteGame.transform.SetParent(GameObject.Find("Canvas").transform,false);

		selectable[0] = GameObject.Find("mainMenuText");//mainMenuText; //store it in the selectable
		selectable[2] = GameObject.Find("closeText");//closeText; //-||-
		selectable[1] = GameObject.Find("muteGame"); //muteGame;

		selectable[selectionIndex].GetComponent<Image>().color = Color.green; //make the one selected(always close) green

		Time.timeScale = 0; //stop time while menu is shown
	}

	public void Close()
	{
		Destroy(GameObject.Find("mainMenuText")); //destroy the texts
		Destroy(GameObject.Find("closeText")); //-||-
		Destroy(GameObject.Find("muteGame"));

		shown = false; //close the menu

		Time.timeScale = 1; //and start time again
	}
}
