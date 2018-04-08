using System;
using UnityEngine;
using System.Collections;
using System.Linq;

public class MovementCommands : InteractionCommands {

	public AudioSource TurnSound;

    /// <summary>
    /// Move the Player forward 'steps' blocks.
    /// </summary>
    /// <param name="steps"></param>
    /// <returns></returns>
    private IEnumerator MoveForward(int steps) {
        for(int i = 0 ; i < (steps * 20) ; i++) {
            if(i % 20 == 0 || i == 0 && steps > 0) {
                CorrectPosition();
                GetComponent<AudioSource>().Play();
                GameObject hit = CommandScript.GetRaycastObject();
                bool canContinue = false;
                if(hit == null) {
                    continue;
                }
                switch (hit.transform.tag) {
                    case "Tablet":
                        DialogueScript.GameInformationText("You swoop up a piece of interesting stone on your way past...");
                        hit.GetComponent<PuzzlePieceScript>().WalkedOverPiece();
                        canContinue = true;
                        break;
                    case "Hatch":
                        if (LevelScript.ExitOpen) {
                            DialogueScript.GameInformationText("You almost trip and fall into the black void....");
                            DialogueScript.GameInformationText("Maybe you should try to enter it instead?");
                            MovementHappening = false;
                        }
                        if (!LevelScript.ExitOpen) {
                            canContinue = true;
                        }
                        break;

                    case "Enemy":
                    case "Door":
                    case "Chest":
                    case "Wall":
                    case "PushableWall":
                    case "Button":
                    case "Torch":
                        DialogueScript.ErrorText("You can't move further that direction");
                        MetObstacle = true;
                        MovementHappening = false;
                        break;
                        
                    case "Stairs":
                        DialogueScript.ErrorText("That'd make you walk into the ladder.");
                        DialogueScript.ErrorText("Maybe try to climb it?");
                        MetObstacle = true;
                        MovementHappening = false;
                        break;
                        
                    case "Key":
                        DialogueScript.GameInformationText("You pick up a key as you walk past it.");
                        hit.transform.gameObject.GetComponent<AudioSource>().Play();
                        hit.transform.position = Vector3.one * 99999f;
                        Destroy(hit.transform.gameObject, 2);
                        Keys += 1;
						InterfaceScript.InfoKeyText.text = "Your Keys: " + Keys;
                        canContinue = true;
                        break;
                }
                if (!canContinue) {
                    yield break;
                }
            }
            transform.Translate(0, 0, 0.1f);
            yield return new WaitForEndOfFrame();
        }
        CorrectRotation();
        MovementHappening = false;
    }

    /// <summary>
    /// Rotate the player. -2 for left and 2 for right direction.
    /// If parameter 'steps' is given, the Player will also move forward. If it is 'turn', the Player will only rotate.
    /// 'length' is 90 for a 90 degree turn and 180 for a 180 degree turn.
    /// </summary>
    /// <param name="steps"></param>
    /// <param name="direction"></param>
    /// <param name="length"></param>
    /// <returns></returns>
    private IEnumerator Rotate(string steps, int direction, int length) {
		TurnSound.Play ();
        for(int i = 0 ; i < length ; i += 2) {
            transform.Rotate(0, direction, 0);
            yield return new WaitForEndOfFrame();
        }
        if(steps != "turn") {
            Forward(steps);
            yield break;
        }
        CorrectRotation();
        MovementHappening = false;
    }

    /// <summary>
    /// Corrects the position.
    /// </summary>
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
    
    /// <summary>
    /// Corrects the rotation.
    /// </summary>
    public void CorrectRotation() {
        float[] full90Ints = { 0f, 90f, 180f, 270f, 360f };
        float y = transform.eulerAngles.y;
        if(!full90Ints.Contains(y)) {
            var nearest = full90Ints.OrderBy(v => Math.Abs((long)v - transform.eulerAngles.y)).First();
            transform.eulerAngles = new Vector3(0, nearest, 0);
        }
    }
    
    
    /// <summary>
    /// Move the Player forward x blocks. Steps given as a string, then gets converted.
    /// </summary>
    /// <param name="steps"></param>
    public void Forward(string steps) {
        MovementHappening = true;
        //Set steps to 1 if the Player doesn't give a number, so it defaults to moving one block
        if(steps == null) {
            steps = "1";
        }
        //Convert the string to int because currently the dissect function isn't smart enough for that
        int move = int.Parse(steps);
        StartCoroutine(MoveForward(move));
    }

    /// <summary>
    /// Make the character walk Left for 'steps' blocks
    /// </summary>
    /// <param name="steps">The steps.</param>
    public void Left(string steps) {
        MovementHappening = true;
        StartCoroutine(Rotate(steps, -2, 90));
    }

    /// <summary>
    /// Make the character walk Right for 'steps' blocks
    /// </summary>
    /// <param name="steps">The steps.</param>
    public void Right(string steps) {
        MovementHappening = true;
        StartCoroutine(Rotate(steps, 2, 90));
    }

    /// <summary>
    /// Make the character walk Backwards for 'steps' blocks
    /// </summary>
    /// <param name="steps">The steps.</param>
    public void Back(string steps) {
        MovementHappening = true;
        StartCoroutine(Rotate(steps, 2, 180));
    }

    /// <summary>
    /// Turns the specified direction.
    /// </summary>
    /// <param name="arg"></param>
    public void Turn(string arg) {
        MovementHappening = true;
        if(arg == "left") {
            StartCoroutine(Rotate("turn", -2, 90));
        }
        if(arg == "right") {
            StartCoroutine(Rotate("turn", 2, 90));
        }
        if(arg == "back") {
            StartCoroutine(Rotate("turn", -2, 180));
        }
    }
}
