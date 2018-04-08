using UnityEngine;
using System.Collections;

public class PuzzlePieceScript : MonoBehaviour {

    public void WalkedOverPiece() {
        switch (GameObject.Find("ScriptHolder").GetComponent<LevelScript>().LevelCounter) {
            case 1:
                GameObject.Find("Clue").GetComponent<Clue>().FirstLevelClue();
                break;

            case 2:
                GameObject.Find("Clue").GetComponent<Clue>().SecondLevelClue();
                break;
        }
        Destroy(gameObject);
    }
}
