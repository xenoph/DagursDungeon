using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Credits : MonoBehaviour {

	public GameObject back;

	void Start()
	{
		back.GetComponent<Image>().color = Color.green;
	}

	void Update () {
		
		if (Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(KeyCode.Return))
		{
			SceneManager.LoadScene("MainMenu");
		}
	}
}
