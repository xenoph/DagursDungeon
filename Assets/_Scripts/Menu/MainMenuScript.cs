using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenuScript : MonoBehaviour {

    private GameObject[] selectable;
	private int selectIndex = 0;

    public GameObject AudioSoundObject;
	public GameObject newGameButton, TutorialButton, optionsButton, creditsButton, quitGameButton;

    void Awake() {
        if (!PlayerPrefs.HasKey("PlayedTutorial")) {
            selectable = new GameObject[4];
            GameObject.Find("PlayTutorialButton").SetActive(false);
            PlayerPrefs.SetInt("PlayedTutorial", 0);
        }
        else {
            if (PlayerPrefs.GetInt("PlayedTutorial") == 0) {
                GameObject.Find("PlayTutorialButton").SetActive(false);
                selectable = new GameObject[4];
            }
            else {
                selectable = new GameObject[5];
            }
        }
        if (!GameObject.FindGameObjectWithTag("AudioListener")) {
            AudioSoundObject.SetActive(true);
        }
    }

	void Start()
	{
	    if (PlayerPrefs.GetInt("PlayedTutorial") == 0) {
	        selectable[0] = newGameButton;
	        selectable[1] = optionsButton;
	        selectable[2] = creditsButton;
	        selectable[3] = quitGameButton;
	    }
        else if (PlayerPrefs.GetInt("PlayedTutorial") == 1) {
            selectable[0] = TutorialButton;
            selectable[1] = newGameButton;       
            selectable[2] = optionsButton;
            selectable[3] = creditsButton;
            selectable[4] = quitGameButton;
        }

        selectable[selectIndex].GetComponent<Image>().color = Color.white;
        selectable[selectIndex].GetComponentInChildren<Text>().color = Color.black;
	}

	void Update()
	{
		if (Input.GetKeyDown(KeyCode.DownArrow))
		{
			ButtonsToBlack();
			if (selectIndex < selectable.Length - 1)
			{
				selectIndex ++;
			}
			else
			{
				selectIndex = 0;
			}
			selectable[selectIndex].GetComponent<Image>().color = Color.white;
            selectable[selectIndex].GetComponentInChildren<Text>().color = Color.black;
        }

		if (Input.GetKeyDown(KeyCode.UpArrow))
		{
			ButtonsToBlack();
			if (selectIndex > 0)
			{
				selectIndex --;
			}
			else
			{
				selectIndex = selectable.Length - 1;
			}
			selectable[selectIndex].GetComponent<Image>().color = Color.white;
            selectable[selectIndex].GetComponentInChildren<Text>().color = Color.black;
        }

		if (Input.GetKeyDown(KeyCode.Return))
			{
			
			if (selectable[selectIndex] == newGameButton)
			{
				NewGame();
			}
		    if (selectable[selectIndex] == TutorialButton) {
                PlayerPrefs.SetInt("PlayedTutorial", 0);
		        NewGame();
		    }
			if (selectable[selectIndex] == optionsButton)
			{
				Options();
			}
			if (selectable[selectIndex] == creditsButton)
			{
				Credits();
			}
			if (selectable[selectIndex] == quitGameButton) //needs a proper function
			{
				selectable[selectIndex].SetActive(false);
				selectIndex --;
				selectable[selectIndex].GetComponent<Image>().color = Color.white;
                selectable[selectIndex].GetComponentInChildren<Text>().color = Color.black;
				Application.Quit();
            }
		}
	}

	void ButtonsToBlack()
	{
		for (int i = 0; i < selectable.Length; i++)
		{
            selectable[i].GetComponentInChildren<Text>().color = Color.white;
            selectable[i].GetComponent<Image>().color = new Color(0.140625f,0.140625f,0.140625f,1);
		}
	}

	public bool MuteSound = false;

	public void NewGame()
	{
        PlayerPrefs.SetString("Class", "WarriorClass");
        PlayerPrefs.SetInt("Strength", 18);
        PlayerPrefs.SetInt("Constitution", 16);
        PlayerPrefs.SetInt("Dexterity", 12);
        PlayerPrefs.SetInt("Intellect", 7);
        PlayerPrefs.SetInt("Wisdom", 7);
        PlayerPrefs.SetInt("Charisma", 10);
        PlayerPrefs.SetInt("CurrentXP", 0);
        PlayerPrefs.SetInt("Level", 1);
        PlayerPrefs.SetInt("Health", PlayerPrefs.GetInt("Constitution")*10);
	    if (PlayerPrefs.GetInt("PlayedTutorial") == 0) {
	        SceneManager.LoadScene("Tutorial");
	    }
        else if (PlayerPrefs.GetInt("PlayedTutorial") == 1) {
            SceneManager.LoadScene("Level1");
        }
	}
	public void Options()
	{
		SceneManager.LoadScene("Options");
	}
	public void QuitGame()
	{
		
	}
	public void ReturnToMainMenu()
	{
		SceneManager.LoadScene("MainMenu");
	}
	public void Credits()
	{
		SceneManager.LoadScene("Credits");
	}

	/*public void MuteSoundBiatch()
	{
		if(MuteSound = true)
		{
			AudioListener = false;
		}

	}
	*/
}