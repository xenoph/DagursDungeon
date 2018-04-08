using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Options : MonoBehaviour {

	private GameObject[] selectable = new GameObject[3];
	private int selectIndex = 0;

	public GameObject back, mute, colorblind;

    private AudioSound _audio;

	void Start () {
        _audio = GameObject.FindGameObjectWithTag("AudioListener").GetComponent<AudioSound>();
		selectable[0] = colorblind;
		selectable[1] = mute;
		selectable[2] = back;

		selectable[selectIndex].GetComponent<Image>().color = Color.green;

		//selectable[selectIndex].GetComponentInChildren<Text>().color = Color.green;

		if (PlayerPrefs.GetInt("mute") == 1)
		{
			mute.GetComponent<Toggle>().isOn = true;
            _audio.MuteSound();
		}
        
		if (PlayerPrefs.GetInt("colorblind") == 1)
		{
			colorblind.GetComponent<Toggle>().isOn = true;
		}

	    
	}

	void Update()
	{
		if (Input.GetKeyDown(KeyCode.DownArrow))
		{
			if (selectIndex < selectable.Length - 1)
			{
				ButtonsToBlack();
				selectIndex ++;
				selectable[selectIndex].GetComponent<Image>().color = Color.green;
			}
			else
			{
				ButtonsToBlack();
				selectIndex = 0;
				selectable[selectIndex].GetComponent<Image>().color = Color.green;
			}
		}

		if (Input.GetKeyDown(KeyCode.UpArrow))
		{
			if (selectIndex > 0)
			{
				ButtonsToBlack();
				selectIndex --;
				selectable[selectIndex].GetComponent<Image>().color = Color.green;
			}
			else
			{
				ButtonsToBlack();
				selectIndex = selectable.Length - 1;
				selectable[selectIndex].GetComponent<Image>().color = Color.green;
			}
		}

		if(Input.GetKeyDown(KeyCode.RightArrow) || Input.GetKeyDown(KeyCode.Return))
		{
			if (selectable[selectIndex] == back)
			{
				SceneManager.LoadScene("MainMenu");
			}
			if (selectable[selectIndex] == mute)
			{
				if (PlayerPrefs.GetInt("mute") == 0)
				{
						PlayerPrefs.SetInt("mute" , 1);
						mute.GetComponent<Toggle>().isOn = true;
                        _audio.MuteSound();
				}
				else
				{
						PlayerPrefs.SetInt("mute" , 0);
						mute.GetComponent<Toggle>().isOn = false;
                        _audio.EnableSound();
				}
			}

			if (selectable[selectIndex] == colorblind)
			{
				if (PlayerPrefs.GetInt("colorblind") == 0)
				{
					PlayerPrefs.SetInt("colorblind" , 1);
					colorblind.GetComponent<Toggle>().isOn = true;
				}
				else
				{
					PlayerPrefs.SetInt("colorblind" , 0);
					colorblind.GetComponent<Toggle>().isOn = false;
				}
			}
			//else if (PlayerPrefs.GetInt(selectable[selectIndex].ToString()) == 0)
			//	{
			//		PlayerPrefs.SetInt(selectable[selectIndex].ToString(), 1);
			//		selectable[selectIndex].GetComponent<Toggle>().isOn = true;
			//	}
			//	else
			//	{
			//	PlayerPrefs.SetInt(selectable[selectIndex].ToString(), 1);
			//		selectable[selectIndex].GetComponent<Toggle>().isOn = false;
			//	}
		}
		//if Esc is pressed
		if (Input.GetKeyDown(KeyCode.Escape))
		{
			SceneManager.LoadScene("MainMenu");
		}
	}

	void ButtonsToBlack()
	{
		for (int i = 0; i < selectable.Length; i++)
		{
			selectable[selectIndex].GetComponent<Image>().color = new Color(0.140625f,0.140625f,0.140625f,1);
		}
	}
}
