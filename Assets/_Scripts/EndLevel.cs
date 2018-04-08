using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

/// <summary>
/// Could this have been done better? You'd think so, wouldn't you?
/// </summary>
public class EndLevel : MonoBehaviour {

	public GameObject CellDoor;
	public GameObject BallOfLight;
	public Image FadeToWhite;

	private Dialogue _dialogueScript;

	void Start(){
		_dialogueScript = GameObject.Find("Canvas").GetComponent<Dialogue>();
	}

	public void BossDied(){
		StartCoroutine(DramaticPauses());	
	}

	private IEnumerator DramaticPauses(){
		GameObject player = GameObject.FindGameObjectWithTag("Player");

		_dialogueScript.GameInformationText("The evil darkness has perished!");

		player.GetComponent<MovementCommands>().Right("0");
		yield return new WaitForSeconds(2);

		CellDoor.GetComponentInChildren<Animator>().SetBool("CellDoorOpen", true);
		yield return new WaitForSeconds(2);

		for (int i = 0; i < (4 * 20); i++) {
			BallOfLight.transform.Translate(0, 0, -0.1f);
			yield return new WaitForEndOfFrame();
		}
		yield return new WaitForSeconds(2);

		_dialogueScript.GameInformationText("As the brightness stands before you a booming voice fills your head.");
		yield return new WaitForSeconds(5);
		_dialogueScript.GameInformationText("YOU HAVE RELEASED ME FROM MY PRISON BRAVE MORTAL!");
		yield return new WaitForSeconds(3);
		_dialogueScript.GameInformationText("THE WORLD WILL NOW KNOW LIGHT AGAIN THANKS TO YOU!");
		yield return new WaitForSeconds(3);
		_dialogueScript.GameInformationText("REST ASSURED THAT YOU WILL NEVER AGAIN KNOW DARKNESS!");
		yield return new WaitForSeconds(5);

		//This doesn't work. Fuck if I know why. I don't like UI stuff.
		int alpha = 255;
		Color c = FadeToWhite.color;
		c.a = 0;
		for (int i = 0; i < alpha; i++) {
			c.a = i;
			FadeToWhite.color = c;
			yield return new WaitForEndOfFrame();
		}

		yield return new WaitForSeconds(5);
		SceneManager.LoadScene("MainMenu");
	}
}