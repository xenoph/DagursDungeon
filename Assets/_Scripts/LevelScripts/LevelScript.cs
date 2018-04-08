using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using Random = UnityEngine.Random;

/// <summary>
/// This script was intended to hold everything that is going on inside the levels, i.e. dimming the torches, keeping track of the puzzles,
///  it has become a little bit messy and could probably do with some tidying.
/// -BV
/// </summary>
public class LevelScript : MonoBehaviour {

    private Dialogue _dialogueScript;    
    private GameTurnController _gameTurnController;

    [HideInInspector]
    public bool EnemyAttacking;
    [HideInInspector]
    public bool PuzzleSolved = false;
    [HideInInspector]
    public bool ExitOpen = false;
    [HideInInspector]
    public bool Darkness = false;
    [HideInInspector]
    public int TorchNumber;
    [HideInInspector]
    public GameObject Enemy;
    [HideInInspector]
    public int LevelCounter;

	public int TorchCountdown;
    private List<GameObject> _torchesList;

	private bool _firstLightWarning;
	private bool _secondLightWarning = true;

    void Awake() {
        //Get the name of the level, and if it is tutorial, get the PlayerSpawner and which class to spawn, set the state to PlayerTurn then disable this script
        string levelName = SceneManager.GetActiveScene().name;
        if (levelName == "Tutorial") {
            GameObject.FindGameObjectWithTag("ScriptHolder")
                .GetComponent<PlayerSpawner>()
                .SpawnPlayer(SceneManager.GetActiveScene().name, PlayerPrefs.GetString("Class"));
            _gameTurnController = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameTurnController>();
            _gameTurnController.CurrentState = GameTurnController.PlayerState.PlayerTurn;
            this.enabled = false;
            return;
        }
        //If it is not tutorial, do the same until...
        GameObject.FindGameObjectWithTag("ScriptHolder")
            .GetComponent<PlayerSpawner>()
            .SpawnPlayer(SceneManager.GetActiveScene().name, PlayerPrefs.GetString("Class"));
        _gameTurnController = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameTurnController>();
        _gameTurnController.CurrentState = GameTurnController.PlayerState.PlayerTurn;
        //...this bit, where the script is not disabled, instead the LevelCounter is set to the levelname minus level (i.e. Level1 - Level = 1)
        var remove = levelName.Remove(0, 5);
        LevelCounter = int.Parse(remove);
        PlayerPrefs.SetInt("LevelCounter", LevelCounter);
        
    }

    void Start() {
        //Create a list to hold the torches
        _torchesList = new List<GameObject>();
        //Get the Dialogue script
        _dialogueScript = GameObject.FindGameObjectWithTag("UI").GetComponent<Dialogue>();
        //If it is the first level, show some starting information to the Player
        if (SceneManager.GetActiveScene().name == "Level1") {
            StartCoroutine(FirstLevelInformation());
        }       
        //Give information about what level the player is on. If they came back from previous level, it will say that they went up
        // otherwise it will say that they climbed down.
        NotifyAboutLevel(PlayerPrefs.GetInt("PreviousLevel"));
        //Start the torch countdown
        StartTorchCountdown();
    }

	void Update(){
		if (!_firstLightWarning) {
			CheckLights(1);
		}
		if (!_secondLightWarning) {
			CheckLights(2);
		}
	}

	/// <summary>
	/// Checks the light intensity, and will notify the Player when it has reached 50% and 25%
	/// </summary>
	/// <param name="number">Warning number</param>
	private void CheckLights(int number){
		GameObject[] lights = GameObject.FindGameObjectsWithTag("Torch");
		int intensityLeft = 0;
		int intensityTotal = 0;

		foreach (var light in lights) {
			GameObject torchChildLight = light.transform.FindChild("Point light").gameObject;
			intensityTotal += 4;
			intensityLeft += (int)torchChildLight.GetComponent<Light>().intensity;
		}

		switch (number) {
			case 1:
				if ((intensityTotal / 2) >= intensityLeft) {
					_dialogueScript.GameInformationText("The dungeon darkens as the light intensity has gone down halfway...");
					_firstLightWarning = true;
					_secondLightWarning = false;
				}
				break;

			case 2:
				if ((intensityTotal / 4) >= intensityLeft) {
					_dialogueScript.GameInformationText("Time to relight some torches before darkness comes over you...");
					_secondLightWarning = true;
				}
				break;

			default:
				break;
		}
	}

    /// <summary>
    /// Starts the torch countdown.
    /// </summary>
    void StartTorchCountdown() {
        //Set the torch countdown
        TorchCountdown = 0;
        //Find all the torches, and add them to the list
        GameObject[] lights = GameObject.FindGameObjectsWithTag("Torch");
        foreach(GameObject torch in lights) {
            _torchesList.Add(torch);
        }
        //Set the number of torches
        TorchNumber = _torchesList.Count;
        //Start the torch counter.
        InvokeRepeating("TorchCounter", 30, 1);
    }

    /// <summary>
    /// Checks the puzzle on a level. Function is called whenever a button is pressed
    /// Can be expanded upon to include other puzzles as well.
    /// </summary>
    public void CheckPuzzle() {
        //Make sure the puzzle isn't already solved.
        if (!PuzzleSolved) {
            //Set a bool to false
            bool stillUnsolved = false;
            //Find all the buttons
            GameObject[] buttons = GameObject.FindGameObjectsWithTag("Button");
            //Iterate through them to check if they are pressed, if one is, set the bool to true for not being solved
            foreach (var button in buttons) {
                if (button.GetComponent<Button>().IsPressed == false) {
                    stillUnsolved = true;
                }
            }
            //If it is still unsolved, return from the function
            if (stillUnsolved) {
                _dialogueScript.GameInformationText("The exit is not open yet, you should try to find another button.");
                return;
            }
            //Otherwise, set the puzzle to solved
            PuzzleSolved = true;
            //Then set the exit to open, find the hatch and play animation and sound
            ExitOpen = true;
            if (GameObject.Find("Clue").GetComponent<Clue>().FoundPuzzlePiece || SceneManager.GetActiveScene().name == "Tutorial") {
                GameObject.FindGameObjectWithTag("Hatch").GetComponent<Animator>().SetBool("OpenHatch", true);
                GameObject.FindGameObjectWithTag("Hatch").GetComponent<AudioSource>().PlayDelayed(0.5f);
                //Then inform the Player that something happened.
                _dialogueScript.GameInformationText("Somewhere something opened....");
            }
            else {
                _dialogueScript.GameInformationText("You press the button, but nothing happens.");
                _dialogueScript.GameInformationText("Perhaps you are missing something else from this dungeon...?");
            }
        }
    }

    /// <summary>
    /// Opens the hatch if the Exit is already open. Called when the puzzle piece is picked up.
    /// </summary>
    public void OpenHatch() {
        if (ExitOpen) {
            GameObject.FindGameObjectWithTag("Hatch").GetComponent<Animator>().SetBool("OpenHatch", true);
            GameObject.FindGameObjectWithTag("Hatch").GetComponent<AudioSource>().PlayDelayed(0.5f);
            //Then inform the Player that something happened.
            _dialogueScript.GameInformationText("As you pick up the piece, something unlocked somewhere.");
        }
    }

    /// <summary>
    /// Checks for Enemy movement. Currently called from the EnemyScript whenever an Enemy is finished moving.
    /// </summary>
    public void CheckForMovement() {
        //Set a bool to false
        bool stillMoving = false;
        //Get all enemies and iterate through them
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
        foreach (var enemy in enemies) {
            //If one is still moving, set bool to true
            if (enemy.GetComponent<EnemyScript>().StartedMoving) {
                stillMoving = true;
            }
        }
        //If someone is still moving, return from function
        if (stillMoving) return;
        //Or if someone is attacking, return from function
        if (EnemyAttacking) return;
        //Otherwise, inform the Player that it is their turn, change the state and activate the input.
		if (_gameTurnController.CurrentState != GameTurnController.PlayerState.PlayerTurn){_dialogueScript.GameInformationText("It is your turn.");}
        _gameTurnController.CurrentState = GameTurnController.PlayerState.PlayerTurn;
        GameObject.FindGameObjectWithTag("UI").GetComponent<InterfaceScript>().ActivateInput();
    }

    /// <summary>
    /// Pauses the torchcountdown. Called when Player gets into combat. True to set pause, false to restart it.
    /// </summary>
    /// <param name="pause">if set to <c>true</c> [pause].</param>
    public void PauseTorches(bool pause) {
        if (pause) {
            //Cancel the countdown if true
            CancelInvoke("TorchCounter");
            return;
        }
        //Else, restart the countdown, but give the Player 15 seconds before ticking down
        InvokeRepeating("TorchCounter", 15, 1);
    }

    /// <summary>
    /// Relights the torch.
    /// </summary>
    /// <param name="torch">The torch.</param>
    public void RestartTorch(GameObject torch) {
        //Find the particles and light from the GameObject passed to the function.
        GameObject torchChildParticles = torch.transform.FindChild("Particle System").gameObject;
        GameObject torchChildLight = torch.transform.FindChild("Point light").gameObject;
        //The EmissionModule is needed to restart the particles
        ParticleSystem.EmissionModule em = torchChildParticles.GetComponent<ParticleSystem>().emission;
        //Enable emission, light and sound.
        em.enabled = true;
        torchChildLight.GetComponent<Light>().enabled = true;
        torch.GetComponent<AudioSource>().Play();
        //If the number of torches had reached 0, inform the Player that they will no longer lose health
        if (TorchNumber == 0) {
            _dialogueScript.GameInformationText(
                "Health has stopped decaying.");
            _torchesList.Add(torch);
            TorchNumber++;
            //Restart the torch countdown, give the Player 20 seconds before ticking, and set Darkness to false.
            InvokeRepeating("TorchCounter", 20, 1);
            Darkness = false;
            return;
        }
        //Otherwise, add the torch to the torch list and add to the number of torches
        _torchesList.Add(torch);
        TorchNumber++;
    }

    /// <summary>
    /// The actual (badly named) torch countdown. Called with Invoke to be repeatable
    /// </summary>
    public void TorchCounter() {
        //Add one to the torch countdown. This function is called every second, so it'll gain one per second.
        TorchCountdown++;
        //If all the torches have gone out
        if (_torchesList.Count == 0) {
            //Inform the player, cancel this function and set Darkness to true
            _dialogueScript.GameInformationText("All lights are out. Your health will decay after used turn.");
            CancelInvoke("TorchCounter");
            Darkness = true;
            //Also reset the torchcountdown
            TorchCountdown = 0;
            return;
        }
        //Else, wait until the countdown reaches 20
        if (TorchCountdown <= 20) return;
        //Get a random torch from the list
        int randomTorch = Random.Range(0, _torchesList.Count);
        //Find the particles and light in that torch.
        GameObject torchChildParticles = _torchesList[randomTorch].transform.FindChild("Particle System").gameObject;
        GameObject torchChildLight = _torchesList[randomTorch].transform.FindChild("Point light").gameObject;
        //Particle emission is needed to start and stop particle effects
        ParticleSystem.EmissionModule em = torchChildParticles.GetComponent<ParticleSystem>().emission;
        //Turn down the intensity on the light. It starts at 4
		if (SceneManager.GetActiveScene().name == "Level 20") {
			torchChildLight.GetComponent<Light>().intensity -= 2;
		} else {
			torchChildLight.GetComponent<Light>().intensity--;
		}
        //Else if it has reached 0
        if (torchChildLight.GetComponent<Light>().intensity == 0) {
            //Disable light and particle emission
            torchChildLight.GetComponent<Light>().enabled = false;
            em.enabled = false;
            //Then find the sound and disable that too
            _torchesList[randomTorch].GetComponent<AudioSource>().Stop();
            //Remove the torch from the list
            _torchesList.RemoveAt(randomTorch);
            //Remove one from the list of torches
            TorchNumber--;
        }
        //Then set the torch countdown to 0
        TorchCountdown = 0;
    }

    /// <summary>
    /// Restarts the level
    /// </summary>
    public void StartOver() {
        //Reload the currently active scene
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        //Set the Players health to full
        PlayerPrefs.SetInt("Health", PlayerPrefs.GetInt("Constitution")*10);
    }

    /// <summary>
    /// Removes 20 health from the Player
    /// </summary>
    public void DarknessHealthLoss() {
        //Remove 20 health
        PlayerPrefs.SetInt("Health", PlayerPrefs.GetInt("Health") - 20);
		_dialogueScript.GameInformationText("You lose some health as the darkness embraces you.");
        //If the health is still above 0, return.
        if (PlayerPrefs.GetInt("Health") > 0) return;
        //Else, inform the Player that they are dying
        _dialogueScript.GameInformationText("You succumb to the darkness....");
        //Call the fader to finish the level and deactivate the input
        GetComponent<LevelFader>().SceneFinished = true;
        GameObject.FindGameObjectWithTag("UI").GetComponent<InterfaceScript>().DeActivateInput();
        //After 3 seconds, to let the fader finish, restart the level.
        //TODO: Game over screen where player can choose between restarting level or main menu.
        GameObject.Find("HealthBar").GetComponent<AudioSource>().Play();
        Invoke("StartOver", 5);
    }

    /// <summary>
    /// Information about the game provided to the Player on the first level
    /// </summary>
    /// <returns></returns>
    IEnumerator FirstLevelInformation() {
        //Print out a line of text every 2 seconds.
        _dialogueScript.GameInformationText("Welcome to Dagur's Dungeon, " + PlayerPrefs.GetString("Class").Remove(PlayerPrefs.GetString("Class").Length - 5, 5) + "!");
        yield return new WaitForSeconds(2);
        _dialogueScript.GameInformationText("Dagur is kidnapped, and it is up to you to save him!");
        yield return new WaitForSeconds(2);
		_dialogueScript.GameInformationText("Write \"Help\" to open up a help panel.");
        yield return new WaitForSeconds(2);
        _dialogueScript.GameInformationText("It would also be wise to know that torches will burn out unless they are re-lit...");
        yield return new WaitForSeconds(2);
    }

    /// <summary>
    /// Notifies the Player about which level they are on and where they came from
    /// </summary>
    /// <param name="previousLevel">The previous level.</param>
    public void NotifyAboutLevel(int previousLevel) {
        //Check if the previous level was higher than the current (Player used Go Up), or if it
        // is lower, then inform the Player about which 'floor' they're on.
        int currentLevel = int.Parse(SceneManager.GetActiveScene().name.Remove(0, 5));
        if (previousLevel > currentLevel) {
            _dialogueScript.GameInformationText("You climb back up to floor " + currentLevel + "!");
        }
        else {
            _dialogueScript.GameInformationText("After climbing down you find yourself on floor " + currentLevel + "!");
        }
    }
}
