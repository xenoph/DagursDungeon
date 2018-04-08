using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class EnemyMovement : CombatCommands {

    private int _turns;

	private bool canMoveForward;
	private bool canMoveLeft;
	private bool canMoveRight;

	private bool justTurned;

	private List<string> haveTurned = new List<string>(); //stores the already chosen paths

	public GameObject player; //stores the player

	private int visionLenght = 6; //this is a multiple of 2(since each square is 2x2m)
	private bool nearPlayer; //if it can "hear" the player
	private bool canSeePlayer; //if the player is in front
	private int lastSeen; //saves how many SQUARES away the player was seen

	public float startPosX; //for correction of positon
	public float startPosZ;
	private float lastGoodX; //saves the last known position
	private float lastGoodZ;


	void Start()
	{
		haveTurned.Add("Right");
		haveTurned.Add("Left");
		haveTurned.Add("Forward");

		startPosX = transform.position.x;
		startPosZ = transform.position.z;
		lastGoodX = startPosX;
		lastGoodZ = startPosZ;

		player = GameObject.Find("Player"); //stores the player
	}


	public void MoveEnemy() 
	{
		RaycastHit hit;
		FindPlayer(); //can this see or hear the player
		FindPath(); //what directions are allowed


		//if (player.transform.position == transform.position + (transform.forward * 2)) //if the players position on the block in front of this
		//=========CAN ATTACK==============
		if (Physics.Raycast(transform.position, transform.forward, out hit, 2)) //if facing the player
		{
			if (hit.transform.tag == "Player")
			{
				StartCoroutine(GetComponent<EnemyScript>().RotatePlayer()); //rotate the player into combat view
				GameTurnController.CurrentState = GameTurnController.PlayerState.EnemyCombatTurn; //set the turn to enemy combat
				GetComponent<EnemyCombat>().StartTurn(); //then the enemy does its thing
				GameObject.Find("ScriptHolder").GetComponent<LevelScript>().EnemyAttacking = true; //the enemy is attacking
				return; //don't go back to this script
			}
		}

		//============USE SENSES TO LOACTE THE PLAYER====================
		//if (player.transform.position == transform.position + (transform.right * 4)) //if the players position is two blocks to the right of this
		if (Physics.Raycast(transform.position, transform.right, out hit, 4)) //if player is up to 2 SQUARES to the right
		{
			if (hit.transform.tag == "Player")
			{
				TurnRight();
				FindPlayer(); //then look at the player and remember the position
				goto EndOfMove;
			}
		}

		//if (player.transform.position == transform.position + (transform.right * -4)) //if the players position is two blocks to the left of this
		if (Physics.Raycast(transform.position, transform.right * -1, out hit, 4)) //if player is up to 2 SQUARES to the left
		{
			if (hit.transform.tag == "Player")
			{
				TurnLeft();
				FindPlayer(); //then look at it and remember the position
				goto EndOfMove;
			}
		}

		//if (player.transform.position == transform.position + (transform.forward * -4)) //if the players position is two blocks behind this
		if (Physics.Raycast(transform.position, transform.forward * -1, out hit, 4)) //if player is up to 2 SQUARES behind
		{
			if (hit.transform.tag == "Player")
			{
				TurnLeft();
				TurnLeft();
				FindPlayer(); //then look at it and remember the position
				goto EndOfMove;
			}
		}

		//------MEMORY------
		if ((lastSeen > 0 || canSeePlayer) && canMoveForward)//if player has been seen or is seen and the enemy can move forward
		{
			StartCoroutine("AnimateMoveEnemy");
			lastSeen --; //removes one square from the memory of the enemy
			goto EndOfMove;
		}

		//=============CANT FIND PLAYER, WHERE CAN IT GO=====================

		//only one way to go
		if (!canMoveLeft && !canMoveForward && !canMoveRight) //if blocked all sides but back
		{
			TurnRight();
			TurnRight();
			justTurned = false; //in case the enemy has nowhere to move, allow it to finish its turn by turning around

			goto EndOfMove;
		}

		if (!canMoveLeft && canMoveForward && !canMoveRight) //if can only move Forward
		{
			StartCoroutine("AnimateMoveEnemy");
			//MoveEnemy(); //if forward is the only way you can go do everything again
			goto EndOfMove;
		}

		if (canMoveLeft && !canMoveForward && !canMoveRight) //if can only move Left
		{
			TurnLeft();
			goto EndOfMove;
		}

		if (!canMoveLeft && !canMoveForward && canMoveRight) //if can only move Right
		{
			TurnRight();
			goto EndOfMove;
		}

		//=============CAN MOVE MORE THAN ONE WAY===================
		StartOfMove:;

		if (justTurned) {StartCoroutine("AnimateMoveEnemy"); goto EndOfMove;} //if enemy just turned make it always go forward 

		string randomMovement = haveTurned[Random.Range(0,haveTurned.Count - 1)]; //weighted random number for movement

		if (randomMovement == "Forward") //if the random number falls in the range of forward
		{
			if (!canMoveForward) {goto StartOfMove;} // if cant move Forward, go back and find a new random number

			haveTurned.Add("Left");
			haveTurned.Add("Right"); //more chance for another path to be chosen

			StartCoroutine("AnimateMoveEnemy");
		}
		else if (randomMovement == "Left") // if the random number falls in the range of left
		{
			if (!canMoveLeft) {goto StartOfMove;} // if cant move Left, go back and find a new random number

			haveTurned.Add("Forward");
			haveTurned.Add("Forward");
			haveTurned.Add("Right"); //more chance for another path to be chosen

			TurnLeft();
		}
		else if (randomMovement == "Right") //if the random number falls in the range of Right
		{
			if (!canMoveRight) {goto StartOfMove;} // if cant move Right, go back and find a new random number

			haveTurned.Add("Left");
			haveTurned.Add("Forward"); //more chance for another path to be chosen
			haveTurned.Add("Forward");

			TurnRight();
		}
		else
		{
			goto StartOfMove; //finds a new random number
		}

		EndOfMove:;

		//==========RESET SENSES==============
		canSeePlayer = false;
		canMoveLeft = false;
		canMoveForward = false;
		canMoveRight = false;
		Invoke("CheckForMoreMovement", 0.1f);
	}

	//What actually moves the enemy
	IEnumerator AnimateMoveEnemy()
	{
		for(int i = 0 ; i < 20 ; i++) {
			//transform.Translate(0, 0, 0.05f);
			transform.position += transform.forward * 0.1f; //move one tenth of a block for 20 times to move 2 squares
			yield return new WaitForEndOfFrame();
		}
		yield return new WaitForEndOfFrame();
		yield return new WaitForEndOfFrame();

		GetComponent<EnemyScript>().CorrectPosition(); //rounds the position
		justTurned = false; //since this is the move forward command, the enemy did not turn

		if (startPosX %2 == transform.position.x %2) {lastGoodX = transform.position.x;} //if the start position and the current position is both odd or both even
		if (startPosZ %2 == transform.position.z %2) {lastGoodZ = transform.position.z;} //save the position as the last known position that is ok
	}

	public void RepairPosition()
	{
		transform.position = new Vector3(lastGoodX,transform.position.y,lastGoodZ); //set the position to the last good xyz coordinates
	}

	void CheckForMoreMovement()
	{
		GetComponent<EnemyScript>().StartedMoving = false; //very last thing to do
	}


	//Find player
	void FindPlayer()
	{
		RaycastHit hit; //stores hit information

		//if this has one wall between itself and the player
//		if ((transform.position - player.transform.position).magnitude < 6 && (transform.position - player.transform.position).magnitude > -6)
//		{
//			canHear = true;
//		}

		//=============VISION================
		if (Physics.Raycast(transform.position, transform.forward, out hit, visionLenght)) //if player is in a line in front of this
		{
			if (hit.transform.tag == "player") //check if the raycast only hits the player
			{
				canSeePlayer = true;
				lastSeen = Mathf.FloorToInt(hit.distance)/2; //saves how many SQUARES away the player is
			}
			else
			{
				canSeePlayer = false; //if anything but the player is in front, it cant see the player
			}
		}
	}

	void TurnLeft()
	{
		transform.Rotate(new Vector3(0,-90,0), Space.Self); //Need a move Right command
		justTurned = true;
		GetComponent<EnemyScript>().MoveCounter ++; //when the enemy turns, add one to the movecounter so it can end its turn

	}

	void TurnRight()
	{
		transform.Rotate(new Vector3(0,90,0), Space.Self); //Need a move Right command
		justTurned = true;
		GetComponent<EnemyScript>().MoveCounter ++; //if the enemy turns add one to the move counter so it can stop moving


	}


	public void FindPath() 
	{
		RaycastHit hit;
		if (!Physics.Raycast(transform.position, transform.forward, 2)) //if nothing is in FRONT
		{
			canMoveForward = true;
		}
		if (Physics.Raycast(transform.position, transform.forward, out hit, 4))
		{
			if (hit.transform.tag == "Enemy" && hit.transform.forward == transform.forward * -1 && hit.transform.GetComponent<EnemyScript>().MoveCounter < 2) //if an enemy is looking at this enemy two blocks ahead and has moved less than 2 times
			{
				canMoveForward = false; //avoid going into the same space
			}
		}

		if (!Physics.Raycast(transform.position, transform.right * -1, 2)) //if nothing is to the LEFT
		{
			canMoveLeft = true;
		}

		if (!Physics.Raycast(transform.position, transform.right, 2)) //if nothing is to the RIGHT
		{
			canMoveRight = true;
		}
	}
}
