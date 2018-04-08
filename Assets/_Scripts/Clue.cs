using UnityEngine;

public class Clue : MonoBehaviour {

    private Dialogue _dialogueScript;
    //private InterfaceScript _interfaceScript;
    private LevelScript _levelScript;

    public bool FoundPuzzlePiece;

    public GameObject TabletPiece;

    void Awake() {
        FoundPuzzlePiece = false;
        _dialogueScript = GameObject.FindGameObjectWithTag("UI").GetComponent<Dialogue>();
        //_interfaceScript = GameObject.FindGameObjectWithTag("UI").GetComponent<InterfaceScript>();
        _levelScript = GameObject.FindGameObjectWithTag("ScriptHolder").GetComponent<LevelScript>();
    }

    void Start() {
        SpawnTheClue();
    }

    private void SpawnTheClue() {
        if (_levelScript.LevelCounter < 3) return;
        GameObject[] allEnemies = GameObject.FindGameObjectsWithTag("Enemy");
        allEnemies[Random.Range(0, allEnemies.Length)].GetComponent<EnemyScript>().HasPuzzlePiece = true;
    }

	//It was meant to give various feedback based on level. Was scrapped for the time being
	// and instead feedback is given through the enemy that carried the clue.
    public void CallDialogue() {
        switch (_levelScript.LevelCounter) {
            case 3:
                ThirdLevelClue();
                break;

            case 4:
            case 5:
            case 6:
            case 7:
            case 8:
            case 9:
            case 10:
            case 11:
            case 12:
            case 13:
            case 14:
            case 15:
            case 16:
            case 17:
            case 18:
            case 19:
            case 20:
                FoundPiece();
                break;
        }

		GetComponent<AudioSource>().Play();
    }

    public void FirstLevelClue() {
        _dialogueScript.GameInformationText("You've picked up a piece of a stone tablet!");
        _dialogueScript.GameInformationText("It seems to contain magical letters. Perhaps there are more of these further in?");
        FoundPiece();
    }

    public void SecondLevelClue() {
        _dialogueScript.GameInformationText("Another piece of the tablet!");
        _dialogueScript.GameInformationText("Looks to be more pieces missing though...");
		_dialogueScript.GameInformationText("It's possible that you might have to find every piece before you can open the exit.");
        FoundPiece();
    }

    public void ThirdLevelClue() {
        _dialogueScript.GameInformationText("As your foe falls, he drops a third piece of the tablet which you quickly grab.");
        _dialogueScript.GameInformationText("Perhaps they are meant to guard them... They must be important!");
        FoundPiece();
    }

    private void FoundPiece() {
        FoundPuzzlePiece = true;
    }

}
