using UnityEngine;
using System.Collections;

public class LevelWallChanger : MonoBehaviour {

    public GameObject NewWall;

    void Start() {
        GameObject[] allWalls = GameObject.FindGameObjectsWithTag("Wall");
        foreach (GameObject wall in allWalls) {
            Instantiate(NewWall, wall.transform.position, Quaternion.identity);
            Destroy(wall);
        }
    }
}
