using System;
using System.Collections;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// This script is made to take care of any interaction the Player wants to do with objects in the game.
/// -BV
/// </summary>
public class InteractionCommands : CombatCommands {

    /// <summary>
    /// Get the object.
    /// </summary>
    /// <param name="arg">Object to get</param>
    public void Get(string arg) {
        //TODO: Need to rewrite a bit to allow for the player to get the stone piece.
		GameObject hit = GetRaycastObject();
		if(arg.ToUpper() == "KEY") {          
            if(hit == null) {
                DialogueScript.ErrorText("There's nothing there");
                return;
            }
            if(hit.transform.tag == "Key") {
                hit.transform.gameObject.GetComponent<AudioSource>().Play();
                DialogueScript.GameInformationText("You picked up a shiny key!");
                hit.transform.position = Vector3.one * 99999f;
                Destroy(hit.transform.gameObject, 2);
                MovementCommands.Forward("1");
                Keys += 1;
                InterfaceScript.InfoKeyText.text = "Your Keys: " + Keys;
            } 
			else {
                DialogueScript.ErrorText("You can't get that");
            }
        }
		if (ItemNameListScript.StoneNameList.Any(s => s.Equals(arg, StringComparison.CurrentCultureIgnoreCase))){
			if (hit.tag == "Tablet") {
				MovementCommands.Forward("1");
			}
		}
		else {
            DialogueScript.ErrorText("Get what?");
        }
    }

    /// <summary>
    /// Attempts to open an object in the scene.
    /// </summary>
    /// <param name="arg">Object to be opened.</param>
    public void Open(string arg) {
        CheckString("Open", arg);
        if (NoInput) return;
        GameObject hit = GetRaycastObject();
        if (hit == null) {
            DialogueScript.ErrorText("There's nothing in front of you.");
            return;
        }
        if (InputString == "CHEST") {
            if (hit.GetComponent<Chest>().HasBeenLooted) {
                DialogueScript.ErrorText("It's empty.");
                return;
            }
            if(Keys == 0 && hit.transform.tag == "Chest") {
                DialogueScript.ErrorText("You don't have a key.");
                return;
            }
            //If the Player has keys and is opening a chest, the sound and animation should play
            // to give feedback. ItemGenerator should create an item to be looted.
            //TODO: ItemGenerator doesn't provide proper items outside of some potions.
            if (Keys > 0 && hit.transform.tag == "Chest") {
                //while (hit.transform.forward == transform.forward) {
                //    hit.transform.Rotate(0, 0, 180);
                //}                 
                hit.transform.gameObject.GetComponent<AudioSource>().Play();
                hit.transform.gameObject.GetComponent<Animator>().SetBool("OpenChest", true);
                DialogueScript.GameInformationText("You open the chest.");
                hit.transform.gameObject.AddComponent<ItemGenerator>();
                hit.GetComponent<Chest>().HasBeenLooted = true;
                Keys -= 1;
                InterfaceScript.InfoKeyText.text = "Your Keys: " + Keys;
                return;
            }
            DialogueScript.ErrorText("There's no chest here to open.");
        }

        if ((InputString == "TRAPDOOR" || InputString == "HATCH") && hit.transform.tag == "Hatch") {
            if (LevelScript.ExitOpen) return;
            DialogueScript.GameInformationText("You can't open like this.");
            DialogueScript.GameInformationText("Maybe somewhere else.");
        }

        if (InputString == "DOOR") {
            if (hit.tag != "Door" && hit.tag != "CellDoor") return;
            switch (hit.transform.gameObject.tag) {
                case "Door":
                    //Door shouldn't be opened unless Player has a key - if they do it should play the animation
                    // and disable the collider so the Player can pass.
                    //TODO: Fix the fucking animation.
                    if (hit.transform.gameObject.GetComponent<WoodenDoor>().IsLocked) {
                        if (Keys > 0) {
                            DialogueScript.GameInformationText("You unlock the door with your key.");
                            hit.GetComponentInChildren<Animator>().SetBool("OpenWoodenDoor", true);
                            hit.GetComponent<Collider>().enabled = false;
                        }
                        else {
                            DialogueScript.GameInformationText("You are lacking the key to open this door.");
                        }
                    }
                    else {
                        hit.GetComponentInChildren<Animator>().SetBool("OpenWoodenDoor", true);
                    }
                    break;

                case "CellDoor":
                    if (Keys > 0) {
                        DialogueScript.GameInformationText("You unlock the door with your key.");
                        hit.GetComponentInChildren<Animator>().SetBool("CellDoorOpen", true);
                    }
                    else {
                        DialogueScript.GameInformationText("You are lacking the key to open this door.");
                    }
                    break;
            }
        }
    }

    /// <summary>
    /// Enables the Player to light up torches (or possibly other objects).
    /// </summary>
    /// <param name="arg">Object to light up.</param>
    public void Light(string arg) {
        //Set MovementHappening to true to make sure that the Player disabled darkness before losing health
        MovementHappening = true;
        GameObject hit = GetRaycastObject("Torch");
        if(hit == null) {
            DialogueScript.ErrorText("There's nothing here to light");
            MovementHappening = false;
            return;
        }
		if(arg.ToUpper() == "TORCH") {
            if(hit.transform.tag == "Torch") {
				StartCoroutine (RotateAndInteract (hit));
				return;
            } else {
                DialogueScript.ErrorText("That's not a torch.");
            }
        } else {
            DialogueScript.ErrorText("You can't light that.");
        }
        MovementHappening = false;
    }

    /// <summary>
    /// Pushes the wall
    /// </summary>
    /// <param name="arg">The argument.</param>
    /// <param name="length">Length (for pushing wall)</param>
    public void Push(string arg, string length) {
        //If the player wants to push a button
        if (string.Equals(arg, "button", StringComparison.CurrentCultureIgnoreCase)) {
            
            GameObject hit = CommandScript.GetRaycastObject("Button");
            if (hit == null) {
                DialogueScript.ErrorText("There's nothing here to push.");
                return;
            }
            if (hit.transform.GetComponent<Button>().IsPressed == false) {
                MovementHappening = true;
                StartCoroutine(RotateAndInteract(hit));

            }
            else {
                DialogueScript.ErrorText("It seems like you've already pressed the button.");
            }
        }
        int pushDistance = 0;
        if (length != null && length.All(char.IsDigit)) {
            pushDistance = int.Parse(length);
        }
        if (string.Equals(arg, "wall", StringComparison.CurrentCultureIgnoreCase)) {
            GameObject hit = GetRaycastObject();
            if (hit.tag == "PushableWall") {
                if (pushDistance == 0) {
                    pushDistance = 100;
                }
                StartCoroutine(PushTheWall(hit, pushDistance));
            }
            else {
                DialogueScript.ErrorText("There's no wall there to push.");
            }
        }
    }

    /// <summary>
    /// Pulls the wall.
    /// </summary>
    /// <param name="arg">The argument.</param>
    /// <param name="length">How far to pull</param>
    public void Pull(string arg, string length) {
        if (arg.All(char.IsDigit)) {
            DialogueScript.ErrorText("It'd be nice if you specified what you want to pull.");
            return;
        }
        GameObject hit = GetRaycastObject();
        if (hit == null) {
            DialogueScript.ErrorText("There's nothing in front of you.");
            return;
        }
        if (arg.ToUpper() == "WALL" && hit.tag == "PushableWall") {
			int pushDistance;
			if (length == null) {
				pushDistance = 100;
			}
			else {
            	pushDistance = int.Parse(length);
			}
            
            StartCoroutine(PullTheWall(hit, pushDistance));
        }
    }

    /// <summary>
    /// Push the wall forward
    /// </summary>
    /// <param name="wall"></param>
    /// <param name="length">How far to pull</param>
    /// <returns></returns>
    IEnumerator PullTheWall(GameObject wall, int length) {
        //Set moveThePlayer to false in the beginning as the Player should move a little bit first
        bool moveWall = false;
        //If the rotation of the wall is not the same as the Player, rotate it.
        while(wall.transform.forward != transform.forward) {
            wall.transform.Rotate(0, 90, 0);
        }
        //So far, the Player has not crashed into anything
        bool crashed = false;
        //Blocks is the amount of blocks to move. This is set in the inspector, under the variable HowFarToMove
        //If it's not set, the Player should be able to move it as far as possible.
        int blocks = wall.GetComponent<Wall>().HowFarToMove;
        if (blocks == 0) {
            blocks = length;
        }
        //Stop ActivatingCommands from finishing before the wall is moved all the way
        MovementHappening = true;
        //Iterate to move over time.
        for(int i = 0 ; i < (blocks * 80) ; i++) {
            if(i == 15) {
                moveWall = true;
            }
            //Check every block if there's something in front that they can crash into
            if(i % 80 == 0) {
                RaycastHit hit;
                if(Physics.Raycast(transform.position, -transform.forward, out hit, 2)) {
                    switch(hit.transform.gameObject.tag) {
                        //Currently it shouldn't crash into another wall, as they are static, but it can 
                        // hit other pushable walls.
                        case "PushableWall":
                        case "Wall":
                        case "Stairs":
                        case "Torch":
                            DialogueScript.GameInformationText("You can't move farther backwards.");
                            crashed = true;
                            break;
                        //Squish the enemy if he's in the wrong place. Squiiiiiiish!
                        case "Enemy":
                            DialogueScript.GameInformationText(
                                "There's an enemy behind you!");
                            crashed = true;
                            break;
                        //Should not be able to move over a chest.
                        case "Chest":
                            DialogueScript.GameInformationText(
                                "Walls are not made to go over chests...");
                            crashed = true;
                            break;

                        default:
                            crashed = false;
                            break;
                    }
                }
				if (length > 1 && crashed == false) {
					wall.GetComponent<AudioSource>().Play();
				}
            }
            //Break out if crashed
            if(crashed) {
                if (moveWall) {
                    for(int x = 0 ; x < 15 ; x++) {
                        wall.transform.Translate(0, 0, -0.025f);
                        yield return new WaitForEndOfFrame();
                    }
                }
                MovementHappening = false;
                yield break;
            }
            //Move it 0.025f per iteration.
            transform.Translate(0, 0, -0.025f);
            //Wait with moving the Player until the wall has moved 15 iterations to be more visible
            if(moveWall) {
                wall.transform.Translate(0, 0, -0.025f);
            }
            yield return new WaitForEndOfFrame();
        }
        //Move the Wall the remaining 15 iterations to become center if the Player is finished moving
        if(moveWall) {
            for(int i = 0 ; i < 15 ; i++) {
                wall.transform.Translate(0, 0, -0.025f);
                yield return new WaitForEndOfFrame();
            }
        }
        MovementHappening = false;
    }

    /// <summary>
    /// Push the wall forward
    /// </summary>
    /// <param name="wall"></param>
    /// <param name="length">How far to push</param>
    /// <returns></returns>
    IEnumerator PushTheWall(GameObject wall, int length) {
        //Set moveThePlayer to false in the beginning as the wall should move a little bit first
        bool moveThePlayer = false;
        //If the rotation of the wall is not the same as the Player, rotate it.
        while (wall.transform.forward != transform.forward) {
            wall.transform.Rotate(0, 90, 0);
        }
        //So far, the wall has not crashed into anything
        bool crashed = false;
        //Blocks is the amount of blocks to move. This is set in the inspector, under the variable HowFarToMove,
        // except in the tutorial where the block is spawned in and should move 2 blocks.
		//If the variable has not been set in the Inspector, the block can be moved until crash
        int blocks = 0;
        if (SceneManager.GetActiveScene().name == "Tutorial") {
            blocks = 2;
        }
        else if (SceneManager.GetActiveScene().name != "Tutorial") {
            blocks = wall.GetComponent<Wall>().HowFarToMove;
            if (blocks == 0) {
                blocks = length;
            }
        }
        //Stop ActivatingCommands from finishing before the wall is moved all the way
        MovementHappening = true;
		wall.GetComponent<AudioSource>().Play();
        //Iterate to move over time.
        for (int i = 0; i < (blocks*80); i++) {
            if (i == 15 && SceneManager.GetActiveScene().name != "Tutorial") {
                moveThePlayer = true;
            }
            //Check every block if there's something in front that they can crash into
            if (i%80 == 0) {
                RaycastHit hit;
                if (Physics.Raycast(wall.transform.position, transform.forward, out hit, 2)) {
                    switch (hit.transform.gameObject.tag) {
                        //Currently it shouldn't crash into another wall, as they are static, but it can 
                        // hit other pushable walls.
                        case "PushableWall":
                        case "Wall":
                            DialogueScript.GameInformationText("You push the block up against another wall.");
                            crashed = true;
                            break;
                            //Squish the enemy if he's in the wrong place. Squiiiiiiish!
                        case "Enemy":
                            DialogueScript.GameInformationText(
                                "As you push the wall you can hear the sound of an enemy being squished...");
                            Destroy(hit.transform.gameObject);
                            break;
                            //Should not be able to move over a chest.
                        case "Chest":
                        case "Stairs":
                        case "Light":
                            DialogueScript.GameInformationText(
                                "There's something blocking the wall on the other side.");
                            crashed = true;
                            break;

						default:							
                            crashed = false;
                            break;
                    }
                }

				if (length > 1 && crashed == false) {
					wall.GetComponent<AudioSource>().Play();
				}
            }
            //Break out if crashed
            if (crashed) {
                if(moveThePlayer) {
                    for(int x = 0 ; x < 15 ; x++) {
                        transform.Translate(0, 0, 0.025f);
                        yield return new WaitForEndOfFrame();
                    }
                }
                MovementHappening = false;
                yield break;
            }
            //Move it 0.025f per iteration.
            wall.transform.Translate(0, 0, 0.025f);
            //Wait with moving the Player until the wall has moved 15 iterations to be more visible
            if (moveThePlayer) {
                this.transform.Translate(0, 0, 0.025f);
            }
            yield return new WaitForEndOfFrame();
        }
        //If it's the tutorial, notify the Player about 
        if (SceneManager.GetActiveScene().name == "Tutorial") {
            DialogueScript.GameInformationText("Something appeared to your left...");
            GameObject.FindGameObjectWithTag("ScriptHolder").GetComponent<Tutorial>().EndButton.SetActive(true);
        }
		//Move the Player the remaining 15 iterations to become center if the Player is set to move
        if (moveThePlayer) {
            for (int i = 0; i < 15; i++) {
                transform.Translate(0, 0, 0.025f);
                yield return new WaitForEndOfFrame();
            }
        }
        MovementHappening = false;
    }

    /// <summary>
    /// If the GameObject is to the right or left, rotates camera that way, then back once done.
    /// </summary>
    /// <param name="hit"></param>
    /// <returns></returns>
    IEnumerator RotateAndInteract(GameObject hit) {
        //Set the initial direction to 0. -2 is left and 2 is right
        int direction = 0;
        //Get the relative point of the GameObject
        var relativePoint = transform.InverseTransformPoint(hit.transform.position);
        print(relativePoint);
        //If it is less than -0.2 (set to -0.2 to compensate for any slight errors)
        if(relativePoint.x < -0.1) {
            direction = -2;
            GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Animator>().SetBool("LookLeft", true);
            yield return new WaitForSeconds(2);
        }
        //Else if it's more than 0.2 rotate the other way
        else if(relativePoint.x > 0.1) {
            direction = 2;
            GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Animator>().SetBool("LookRight", true);
            yield return new WaitForSeconds(2);
        }
		//Do a switch to check what the Player is trying to rotate towards
		switch (hit.tag){
			case "Torch":
				GameObject lightGameObject = hit.transform.FindChild ("Point light").gameObject;
				if (lightGameObject.GetComponent<Light> ().intensity == 4) {
					DialogueScript.ErrorText ("This light is already fully lit");
				}
				else if (lightGameObject.GetComponent<Light> ().intensity < 4) {
					lightGameObject.GetComponent<Light> ().intensity = 4;
					DialogueScript.GameInformationText ("You've relit the torch.");
					LevelScript.RestartTorch (hit.transform.gameObject);
					lightGameObject.GetComponent<Light> ().intensity = 4;
					DialogueScript.GameInformationText ("The flame seems to burn brighter again.");
				}
				yield return new WaitForSeconds (0.5f);
				break;

			case "Button":
				//Mark the button as pressed and play the sound and animation for it
				if (GameObject.Find("Clue").GetComponent<Clue>().FoundPuzzlePiece || SceneManager.GetActiveScene().name == "Tutorial") {
					hit.transform.GetComponent<Button>().IsPressed = true;
					hit.transform.GetComponent<AudioSource>().Play();
					hit.transform.gameObject.GetComponentInChildren<Animation>().Play();
					//Text feedback for the Player
					DialogueScript.GameInformationText("The button creaks into the wall...");
					//Make the LevelScript check if the puzzle is done (i.e. the buttons are pressed). Only if it's not the tutorial.
					if (SceneManager.GetActiveScene().name != "Tutorial") {
						LevelScript.CheckPuzzle();
					} else if (SceneManager.GetActiveScene().name == "Tutorial") {
						GameObject.FindGameObjectWithTag("ScriptHolder").GetComponent<Tutorial>().OpenHatch();
					}
					yield return new WaitForSeconds(0.5f);
					break;
				}
				DialogueScript.ErrorText("Trying to push the button and failing, you think you are missing something from this dungeon.");
				yield return new WaitForSeconds(0.5f);
				break;
		}       
        //Then rotate the camera back the opposite direction of previously
        if (direction == -2) {
            GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Animator>().SetBool("LookLeft", false);
            GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Animator>().SetBool("IdleFromLeft", true);
            yield return new WaitForSeconds(2);
            GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Animator>().SetBool("IdleFromLeft", false);
        }
        else if (direction == 2) {
            GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Animator>().SetBool("LookRight", false);
            GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Animator>().SetBool("IdleFromRight", true);
            yield return new WaitForSeconds(2);
            GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Animator>().SetBool("IdleFromRight", false);
        }
        //End movement so the Activation function can continue.
        MovementHappening = false;
    }

    /// <summary>
    /// Enters the specified argument.
    /// </summary>
    /// <param name="arg">The argument.</param>
    public void Enter() {
        //CheckString("Enter", arg);
        GameObject hit = CommandScript.GetRaycastObject();
        if (hit == null) {
            DialogueScript.ErrorText("There's nothing in front of you to enter");
            return;
        }
        if (hit.transform.tag == "Hatch") {
            if (LevelScript.ExitOpen || SceneManager.GetActiveScene().name == "Tutorial") {
                PlayerPrefs.SetInt("PlayedTutorial", 1);
                DialogueScript.GameInformationText("You slowly descend into the darkness.");
                GameObject.FindGameObjectWithTag("ScriptHolder").GetComponent<LevelChanger>().ChangeLevel();
                LevelFinished = true;
                return;
            }

            DialogueScript.ErrorText("You can't enter something that isn't open.");
        }
    }
}
