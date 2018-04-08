using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.SceneManagement;

public class Tutorial : MonoBehaviour {

    private int _tutorial = 1;
    private Dialogue _dialogueScript;
    private readonly List<Vector3> _spawnLocations = new List<Vector3> {
        new Vector3(-5, -1, 6),
        new Vector3(-3, -1, -4),
        new Vector3(-1, -1, -2),
        new Vector3(5, -1, -6),
        new Vector3(5, -1, 0),
        new Vector3(1, -1, 0),
        new Vector3(1, -1, 4)
    };
    private readonly List<GameObject> _itemsToBePlaced = new List<GameObject>();

    public GameObject ExamineInfoPanel;
    public GameObject WallToChange;
    public GameObject PushableWall;

    public GameObject KeyGameObject;
    public GameObject ChestGameObject;
    public GameObject EnemyGameObject;

    public GameObject MovementInfoPanel;
    public GameObject InteractionInfoPanel;
    public GameObject CombatInfoPanel;
    public GameObject FinishedInfoPanel;

    public GameObject EndButton;

    void Awake() {
        //Disable this script if it's not the Tutorial level
        if (SceneManager.GetActiveScene().name != "Tutorial") {
            this.enabled = false;
        }
    }

    void Start() {
        //Get the Dialogue script
        _dialogueScript = GameObject.FindGameObjectWithTag("UI").GetComponent<Dialogue>();
		StartCoroutine(TutorialDialogue());
    }

    /// <summary>
    /// Moves to next tutorial phase.
    /// </summary>
    /// <param name="arg"></param>
    public void Next(string arg) {
        switch (_tutorial) {
            case 1:
                SpawnItems();
                _tutorial++;
                break;

            case 2:
                ClearScene(2);
                SpawnEnemy();
                _tutorial++;
                break;

            case 3:
                _dialogueScript.ErrorText(
                    "You're on the last step.");
                break;
        }
    }

    /// <summary>
    /// Returns to previous tutorial phase
    /// </summary>
    /// <param name="arg"></param>
    public void Previous(string arg) {
        switch (_tutorial) {
            case 1:
                _dialogueScript.ErrorText("You're already on the first part.");
                break;

            case 2:
                ClearScene(2);
                _tutorial--;
                break;

            case 3:
                ClearScene(3);
                _tutorial--;
                SpawnItems();
                break;
        }
    }

    /// <summary>
    /// Respawn items or enemy, depending on phase
    /// </summary>
    public void Respawn() {
        switch (_tutorial) {
            case 1:
                _dialogueScript.ErrorText("There's nothing to respawn.");
                break;

            case 2:
                ClearScene(2);
                SpawnItems();
                break;

            case 3:
                ClearScene(3);
                SpawnEnemy();
                break;
        }
    }

    /// <summary>
    /// Finishes tutorial and loads level 1
    /// </summary>
    public void Finished() {
        //Close any open info panels and clear the scene
        ClearScene(_tutorial);
        //Open the examine function info panel

        //Delete walls over the hatch
        foreach (var o in GameObject.FindGameObjectsWithTag("TutorialWallToDelete")) {
            Destroy(o);
        }
        //Change the front wall to be a pushable one
        Instantiate(PushableWall, WallToChange.transform.position, Quaternion.identity);
        //Destroy the old wall
        Destroy(WallToChange);
        //Set the last panel active.
        FinishedInfoPanel.SetActive(true);
    }

    /// <summary>
    /// Spawn in the items for phase 2. 3 Chests and 3 Keys
    /// </summary>
    private void SpawnItems() {
        List<Vector3> newSpawnLocations = 
            _spawnLocations.Where(spawnLocation => spawnLocation != GameObject.FindGameObjectWithTag("Player").transform.position).ToList();

        _itemsToBePlaced.AddRange(Enumerable.Repeat(KeyGameObject, 3));
        _itemsToBePlaced.AddRange(Enumerable.Repeat(ChestGameObject, 3));

        foreach (GameObject o in _itemsToBePlaced) {
            var location = newSpawnLocations[Random.Range(0, newSpawnLocations.Count)];
            var locX = location.x;
            var locZ = location.z;

            if(o.transform.tag == "Chest") {
                Instantiate(o, new Vector3(locX, -1.1f, locZ),
                    Quaternion.identity);
            } 
            else {
                Instantiate(o, new Vector3(locX, 0, locZ),
                    Quaternion.identity);
            }
            newSpawnLocations.Remove(location);
        }
        _itemsToBePlaced.Clear();
    }

    /// <summary>
    /// Spawn in an Enemy for phase 3
    /// </summary>
    private void SpawnEnemy() {
        List<Vector3> newSpawnLocations =
            _spawnLocations.Where(spawnLocation => spawnLocation != GameObject.FindGameObjectWithTag("Player").transform.position).ToList();

        var location = newSpawnLocations[Random.Range(0, newSpawnLocations.Count)];
        var locX = location.x;
        var locZ = location.z;

        Instantiate(EnemyGameObject, new Vector3(locX, -0.6f, locZ), Quaternion.identity);
    }

    /// <summary>
    /// Remove any phase-spesific object from the scene.
    /// </summary>
    /// <param name="tutorialPart"></param>
    private void ClearScene(int tutorialPart) {
        if (tutorialPart == 2) {
            foreach (GameObject o in GameObject.FindGameObjectsWithTag("Chest")) {
                Destroy(o);
            }
            foreach (GameObject o in GameObject.FindGameObjectsWithTag("Key")) {
                Destroy(o);
            }
        }

        else if (tutorialPart == 3) {
            var enemy = GameObject.FindGameObjectWithTag("Enemy");
            Destroy(enemy);
        }
    }

    
    

    /// <summary>
    /// Open the tutorial panel relevant to the phase
    /// </summary>
    public void Info() {
        switch (_tutorial) {
            case 1:
                if (!MovementInfoPanel.activeSelf) {
                    MovementInfoPanel.SetActive(true);
                }
                break;

            case 2:
                if (!InteractionInfoPanel.activeSelf) {
                    InteractionInfoPanel.SetActive(true);
                }
                break;

            case 3:
                if (!CombatInfoPanel.activeSelf) {
                    CombatInfoPanel.SetActive(true);
                }
                break;
        }
    }

    public void OpenHatch() {
        //Find the hatch and play animation and sound
        GameObject.FindGameObjectWithTag("Hatch").GetComponent<Animator>().SetBool("OpenHatch", true);
        GameObject.FindGameObjectWithTag("Hatch").GetComponent<AudioSource>().PlayDelayed(0.5f);
        //Then inform the Player that something happened.
        _dialogueScript.GameInformationText("Somewhere something opened....");
    }

	IEnumerator TutorialDialogue(){
		_dialogueScript.GameInformationText("Welcome to the Tutorial.");
		yield return new WaitForSeconds(2);
		_dialogueScript.GameInformationText("Play around with the commands until you are comfortable to move on.");
		yield return new WaitForSeconds(2);
		_dialogueScript.GameInformationText("Ask for help with anything, and I will do my best to provide answers.");
		yield return new WaitForSeconds(2);
		_dialogueScript.GameInformationText("Have fun!");
	}
}
