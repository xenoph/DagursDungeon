using System;
using UnityEngine;
using System.Collections;
using System.Linq;
using UnityEngine.SceneManagement;
using Random = UnityEngine.Random;

public class EnemyScript : MonoBehaviour {

    private Player _playerScript;
    private Dialogue _dialogueScript;
    private Commands _commandScript;
    private GameTurnController _gameTurnController;
    private InterfaceScript _uiScript;
    private LevelScript _levelScript;
	public GameObject DialogueStorage; //aka the player
	public EnemyMovement enemyMovementStorage;

    //Damage should differ based on level - tuning will have to be done once it works
    public int BaseDamage = 4;
    [HideInInspector]public int Level;
    public int BaseHealth = 40;
    public int StartingHealth;

    public bool IsMakingMoves;
    public int MoveCounter;
    private int _chanceToHit = 80;
    private int _xpWorth;
    //bool must be public so that the player can activate that if they come from behind
    public bool IsAttacking;

	public bool HasPuzzlePiece;

    private bool _playerCloseBy;

	private bool shouldMove = true;
    public bool StartedMoving;
    private string _enemyName;


    void Awake() {
        _gameTurnController = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameTurnController>();              
        _uiScript = GameObject.FindGameObjectWithTag("UI").GetComponent<InterfaceScript>();
        _levelScript = GameObject.FindGameObjectWithTag("ScriptHolder").GetComponent<LevelScript>();
        _enemyName = this.name.Remove(1, name.Length-1);
        if (SceneManager.GetActiveScene().name == "Tutorial") {
            GetScripts();
        }
    }

	// Use this for initialization
	void Start () {
        //Get the level. If it is a tutorial level, set the level to 1
	    string levelNameStart = SceneManager.GetActiveScene().name.Remove(1, SceneManager.GetActiveScene().name.Length - 1);
	    Level = levelNameStart != "T" ? int.Parse(SceneManager.GetActiveScene().name.Remove(0, 5)) : 1;
	    //Change the BaseDamage and BaseHealth to use the current level.
        BaseDamage += (Level);
	    BaseHealth += (Level*2);
	    StartingHealth = BaseHealth;
	    _xpWorth = 100+(Level*10);
		enemyMovementStorage = GetComponent<EnemyMovement>();
	}

    void Update() {

        if (_gameTurnController.CurrentState == GameTurnController.PlayerState.EnemyTurn && !StartedMoving) {
            GetScripts();
			if (shouldMove == true)
				{
				StartedMoving = true;
				}
            
			float timeMod = 1;
			GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
			for (int i = 0; i < enemies.Length - 1; i++)
				{
				if (enemies[i] == gameObject)
				{
					timeMod = (i*0.7f)+1;
				}
			}
			Invoke("Walk", timeMod);
			
        }

		if (_gameTurnController.CurrentState == GameTurnController.PlayerState.PlayerTurn && MoveCounter != 0)
		{
			MoveCounter = 0; //reset the move counter
			shouldMove = true;
		}
    }

	void Walk()
	{
		if (enemyMovementStorage.startPosX %2 != transform.position.x %2 || enemyMovementStorage.startPosZ %2 != transform.position.z %2) //if its not the same
		{
			//enemyMovementStorage.RepairPosition(); //set the position to the last know good position
		}
			
		if (MoveCounter > 2) //if taken more than 4 moves
		{
			shouldMove = false;
			if (_gameTurnController.CurrentState != GameTurnController.PlayerState.EnemyCombatTurn) //if enemy did not attack in its turn
			{
				StartedMoving = false;
				_levelScript.CheckForMovement(); //if the enemy stops, looks if other enemies are still moving
			}
		}
		else if (StartedMoving)
		{
			enemyMovementStorage.MoveEnemy();
		}
	}

    /// <summary>
    /// Gets the scripts from the Player.
    /// </summary>
    public void GetScripts() {
        DialogueStorage = GameObject.FindGameObjectWithTag("Player");
        _dialogueScript = GameObject.FindGameObjectWithTag("UI").GetComponent<Dialogue>();
        _playerScript = DialogueStorage.GetComponent<Player>();
        _commandScript = DialogueStorage.GetComponent<Commands>();
    }
		

    public void FindValidDirections() {
		print ("FindValidDirections was called");
        Vector3[] randomRotations = {new Vector3(0, 90, 0), new Vector3(0, -90, 0), new Vector3(0, 180, 0)};

        RaycastHit hit;
        bool hasFound = false;

        if (_playerCloseBy) {
            int[] full90Ints = {0, 90, 180, 270};
            float newRotation = 0;

            GameObject playerTarget = GameObject.FindGameObjectWithTag("Player");
            transform.LookAt(playerTarget.transform.position);

            var nearest = full90Ints.OrderBy(v => Math.Abs((long) v - transform.eulerAngles.y)).First();
            if (nearest > transform.eulerAngles.y) {
                newRotation = nearest - transform.eulerAngles.y;
            }
            else if (nearest < transform.eulerAngles.y) {
                newRotation = transform.eulerAngles.y - nearest;
            }
            transform.Rotate(0, newRotation, 0);

            while (!hasFound) {
                if (Physics.Raycast(transform.position, transform.forward, out hit, 2)) {
                    if (hit.transform.tag == "Enemy") {
                        hasFound = true;
                        StartedMoving = false;
                        _levelScript.CheckForMovement();
                    }
                    else if (hit.transform.tag != "Player") {
                        var relativePoint = transform.InverseTransformPoint(playerTarget.transform.position);
                        if (relativePoint.x < 0.0) {
                            transform.Rotate(new Vector3(0, -90, 0));
                        }
                        else if (relativePoint.x > 0.0) {
                            transform.Rotate(new Vector3(0, 90, 0));
                        }
                        else {
                            if (hit.transform.tag == "Wall") {
                                Physics.Raycast(hit.transform.position, transform.forward, out hit, 2);
                                if (hit.transform.tag == "Player") {
                                    transform.Rotate(new Vector3(0, -90, 0));
                                    hasFound = true;
                                    _levelScript.CheckForMovement();
                                    _gameTurnController.CurrentState = GameTurnController.PlayerState.PlayerTurn;
                                    StartedMoving = false;
                                }
                                else {
                                    hasFound = true;
                                    _levelScript.CheckForMovement();
                                    _gameTurnController.CurrentState = GameTurnController.PlayerState.PlayerTurn;
                                    StartedMoving = false;
                                }
                            }
                        }
                    }

                    else if (hit.transform.tag == "Player") {
                        if (_levelScript.EnemyAttacking) {
                            hasFound = true;
                            StartedMoving = false;
                            _levelScript.CheckForMovement();
                        }
                        else {
                            _dialogueScript.GameInformationText("You've been caught by the Enemy! Combat starts!");
                            _levelScript.PauseTorches(true);
                            hasFound = true;
                            StartedMoving = false;
                            _levelScript.EnemyAttacking = true;
                            _levelScript.CheckForMovement();
                            StartCoroutine(RotatePlayer());
                            StartCoroutine(DoDamage());
                        }
                    }
                }
                else {
                    hasFound = true;
                    //StartCoroutine(Move(1));

					//
                }
            }
        }

        if (!_playerCloseBy) {
            while (!hasFound) {
                if (Physics.Raycast(transform.position, transform.forward, out hit, 2)) {
                    transform.Rotate(randomRotations[Random.Range(0, randomRotations.Length - 2)]);
                }
                else {
                    hasFound = true;
                    StartCoroutine(Move(1));
                }
            }
        }
    }

    IEnumerator Move(int steps) {
        for(int i = 0 ; i < (steps * 40) ; i++) {
            transform.Translate(0, 0, 0.05f);
            yield return new WaitForEndOfFrame();
        }
        CorrectPosition();
        MoveCounter++;
        if (MoveCounter < 5) {
            FindValidDirections();
            yield break;
        }
        StartedMoving = false;
        _levelScript.CheckForMovement();
        if (!_levelScript.EnemyAttacking) {
            _gameTurnController.CurrentState = GameTurnController.PlayerState.PlayerTurn;
        }
    }

    public void CorrectPosition() {
        //Get the current x and z positions
        float x = transform.position.x;
        float z = transform.position.z;
        //Round them up or down to the nearest int
        int newX = (int)Math.Round(x, 0);
        int newZ = (int)Math.Round(z, 0);
        //move the player slightly with the new numbers
        transform.position = new Vector3(newX, transform.position.y, newZ);
    }

	public IEnumerator DoDamage()
	{
		GameObject.Find("Canvas").GetComponent<InterfaceScript>().ChangeCommandPool("off");

		yield return new WaitForSeconds(2);
		_dialogueScript.CombatTextEnemy("The " + GetComponent<EnemyCombat>().enemyName + " seems to be considering its move");

		GetComponent<EnemyCombat>().StartTurn();
	}

    public IEnumerator XDoDamage() {

        yield return new WaitForSeconds(3);
        _dialogueScript.CombatTextEnemy("He seems to be considering his next strike...");
        yield return new WaitForSeconds(1);
        switch(_enemyName) {
            case "D":
                GetComponent<Animator>().SetTrigger("Punch");
                break;

            case "S":
                GetComponent<Animator>().SetTrigger("StabAnimationTrigger");
                break;
        }
        yield return new WaitForSeconds(1);
        int hit = Random.Range(0, 100);
        if (hit <= _chanceToHit) {
            //Remove the damage the enemy do from the Player
            PlayerPrefs.SetInt("Health", PlayerPrefs.GetInt("Health") - BaseDamage);
            //Inform the Player that they have taken x damage
            
            if (_playerScript.Health <= 0) {
                _dialogueScript.CombatTextEnemy("The Enemy strikes a mortal blow and the world is darkening....");
                GameObject.FindGameObjectWithTag("ScriptHolder").GetComponent<LevelFader>().SceneFinished = true;
                _uiScript.DeActivateInput();
                Invoke("PlayerDead", 3);
                yield break;
            }
            
            _dialogueScript.CombatTextEnemy("You have been hit for " + BaseDamage + "!");            
        }
        else {
            _dialogueScript.CombatTextEnemy("He misses you!");
        }
        _gameTurnController.CurrentState = GameTurnController.PlayerState.Combat;
        _dialogueScript.GameInformationText("The battle presents an opening, strike with an ability now!");
        _uiScript.ActivateInput();
    }

    void PlayerDead() {
		GameObject.Find("Canvas").GetComponent<InterfaceScript>().ChangeCommandPool("on");
        _levelScript.StartOver();
    }

    public void GotAttacked(GameObject player) {
        GetScripts();
        IsAttacking = true;
        //_runningCoroutine = false;
        transform.LookAt(player.transform.position);
    }

    public IEnumerator RotatePlayer() {
        //Wait until the Player has stopped moving
        yield return new WaitForSeconds(2);
        int[] full90Ints = { 0, 90, 180, 270, 360 };

        GameObject playerTarget = GameObject.FindGameObjectWithTag("Player");
        playerTarget.transform.LookAt(transform.position);
        var nearest = full90Ints.OrderBy(v => Math.Abs((long)v - playerTarget.transform.eulerAngles.y)).First();
        playerTarget.transform.eulerAngles = new Vector3(0, nearest, 0);
    }

    public void PlayerMissed() {
        StartCoroutine(DoDamage());
    }

    //Function that is called from the Player when they hit the Enemy
    public void CheckHealth(int hit, string ability) {
        GetScripts();
        //First remove the health
        //BaseHealth -= hit;
		GetComponent<EnemyCombat>().currentHp -= hit;
        //Inform the Player
        _dialogueScript.CombatTextPlayer("You strike him for " + hit);
        //Check if that hit took the health below 0
		if (GetComponent<EnemyCombat>().currentHp <= 0) {
			GetComponent<EnemyCombat>().currentHp = 0;
            //If it did, let the player know they killed the Enemy and destroy this object
			if (GetComponent<EnemyCombat>().currentHp <= 0) {
                _dialogueScript.CombatTextPlayer("Victorious! Your foe crumbles before your eyes!");
                switch (_enemyName) {
                    case "D":
                        GetComponent<Animator>().SetBool("TrollDeath", true);
                        break;

                    case "S":
                        GetComponent<Animator>().SetTrigger("DeathAnimationTrigger");
                        break;
                }
                Invoke("EndOfMe", 2);
            }
        }
        //Otherwise inform the player about how much health the Enemy has left
        else {
            //THIS SEEMS LIKE A HARD WAY TO DO SOMETHING EASY!
			float startHealth = GetComponent<EnemyCombat>().maxHp;
			float currentHealth = GetComponent<EnemyCombat>().currentHp;
            float healthLeft = startHealth - currentHealth;
            float healthMissingPercentage = (healthLeft/startHealth)*100;
            float healthPercentage = 100 - healthMissingPercentage;
            if (healthPercentage > 75) {
                _dialogueScript.CombatTextPlayer("Standing strong, your foe seems at good spirits still");
            }
            else if (healthPercentage > 50) {
                _dialogueScript.CombatTextPlayer("It seems like he is faltering somewhat now.");
            }
            else if (healthPercentage > 25) {
                _dialogueScript.CombatTextPlayer("Your foe appears to be struggling.");
            }
            else if (healthPercentage > 10) {
                _dialogueScript.CombatTextPlayer("He is almost falling over. Now is the time to finish him!");
            }
            if (ability == "Slam") {
                return;
            }
            StartCoroutine(DoDamage());
        }
    }

    public void EndOfMe() {
		GameObject.Find("Canvas").GetComponent<InterfaceScript>().ChangeCommandPool("on");
        _levelScript.EnemyAttacking = false;
        _gameTurnController.CurrentState = GameTurnController.PlayerState.PlayerTurn; //Also take the Player out of AttackStance so he can move
        if (SceneManager.GetActiveScene().name != "Tutorial") {
            CalculateXpGiven();
        }
        Destroy(gameObject);
		if (gameObject.name == "BallOfDarkness") {
			GameObject.Find("EndLevel").GetComponent<EndLevel>().BossDied();
			return;
		}
		if (HasPuzzlePiece)
		{
			GameObject.Find("Clue").GetComponent<Clue>().CallDialogue();
		}
        _dialogueScript.GameInformationText("With the foe vanquished, you feel free to move again.");
        _commandScript.ResetTurn();
		if (SceneManager.GetActiveScene().name != "Tutorial") {
			_levelScript.PauseTorches(false);
		}
        _uiScript.ActivateInput();
        GameObject.FindGameObjectWithTag("Player").GetComponent<CombatCommands>().CooldownList.Clear();
		if (HasPuzzlePiece && SceneManager.GetActiveScene().name != ("Level3")){
			_dialogueScript.GameInformationText("You can hear something falling to the floor...");
			_dialogueScript.GameInformationText("Bending down to pick it up, you see it's another piece of the tablet.");
		}
    }

    void CalculateXpGiven() {
        int playerLevel = PlayerPrefs.GetInt("Level");
        int currentXp = PlayerPrefs.GetInt("CurrentXP");
        int xpToGain;
        if (playerLevel <= Level) {
            xpToGain = (_xpWorth + (Level * 10));
            currentXp += xpToGain;
        }
        else if (playerLevel > Level && playerLevel - Level == 1) {
            xpToGain = (_xpWorth/2);
            currentXp += xpToGain;
        } 
        else if (playerLevel > Level && playerLevel - Level == 2) {
            int modulatedNumber = _xpWorth%3;
            int roundedXp = _xpWorth - modulatedNumber;
            xpToGain = (roundedXp/3);
            currentXp += xpToGain;
        }
        else {
            return;
        }
        _uiScript.ShowXpGain(xpToGain);
        PlayerPrefs.SetInt("CurrentXP", currentXp);
        GameObject.FindGameObjectWithTag("Player").GetComponent<LevelUP>().CheckForLevelUp();
    }

    void OnTriggerEnter(Collider other) {
        if (other.tag == "Player") {
            _playerCloseBy = true;
        }
    }

    void OnTriggerExit(Collider other) {
        if (other.tag == "Player") {
            _playerCloseBy = false;
        }
    }
}
