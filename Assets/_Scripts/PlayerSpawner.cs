using System;
using UnityEngine;
using System.Collections;

public class PlayerSpawner : MonoBehaviour {
    public GameObject PlayerMage;
    public GameObject PlayerWarrior;
    public GameObject PlayerDruid;
    public GameObject PlayerThief;

    /// <summary>
    /// Spawn in the Player to the level.
    /// </summary>
    /// <param name="level"></param>
    /// <param name="playerClass"></param>
    public void SpawnPlayer(string level, string playerClass) {
        //Set variables for what, where and rotation.
        GameObject objectToInstantiate = PlayerWarrior;
        Vector3 locationOfInstantiate = new Vector3();
        Quaternion rotationDirection = Quaternion.Euler(0, 0, 0);


        //Check which level to spawn in on.
        //Each level have their own location where the Player should be instantiated,
        // and subsequently a rotation.
        switch (level) {
            case "Tutorial":
                locationOfInstantiate = new Vector3(-7, -1, 0);
                rotationDirection = Quaternion.Euler(0, 90, 0);
                break;

            case "Level3":
                locationOfInstantiate = new Vector3(5, -1, -8);
                rotationDirection = Quaternion.Euler(0, 0, 0);
                break;

            case "Level4":
                locationOfInstantiate = new Vector3(9, -1, 0);
                rotationDirection = Quaternion.Euler(0, -180, 0);
                break;

            case "Level1":
            case "Level2":            
            case "Level5":
            case "Level6":
            case "Level7":
                locationOfInstantiate = new Vector3(9, -1, -2);
                rotationDirection = Quaternion.Euler(0, -90, 0);
                break;

            case "Level8":
                locationOfInstantiate = new Vector3(5, -1, -6);
                rotationDirection = Quaternion.Euler(0, 90, 0);
                break;

            case "Level9":
                locationOfInstantiate = new Vector3(7, -1, -6);
                rotationDirection = Quaternion.Euler(0, 90, 0);
                break;

            case "Level10":
                locationOfInstantiate = new Vector3(7, -1, -13);
                rotationDirection = Quaternion.Euler(0, 270, 0);
                break;

            case "Level11":
                locationOfInstantiate = new Vector3(5, -1, -13);
                rotationDirection = Quaternion.Euler(0, -90, 0);
                break;

            case "Level12":
                locationOfInstantiate = new Vector3(7, -1, -11);
                rotationDirection = Quaternion.Euler(0, 180, 0);
                break;

            case "Level13":
                locationOfInstantiate = new Vector3(3, -1, -13);
                rotationDirection = Quaternion.Euler(0, 0, 0);
                break;

            case "Level14":
            case "Level15":
                locationOfInstantiate = new Vector3(13, -1, -2);
                rotationDirection = Quaternion.Euler(0, -90, 0);
                break;

            case "Level16":
                locationOfInstantiate = new Vector3(9, -1, -6);
                rotationDirection = Quaternion.Euler(0, -90, 0);
                break;

            case "Level17":
                locationOfInstantiate = new Vector3(9, -1, -6);
                rotationDirection = Quaternion.Euler(0, 0, 0);
                break;

            case "Level18":
                locationOfInstantiate = new Vector3(21, -1, -18);
                rotationDirection = Quaternion.Euler(0, 0, 0);
                break;

            case "Level19":
                locationOfInstantiate = new Vector3(-9, -1, -14);
                rotationDirection = Quaternion.Euler(0, 0, 0);
                break;

			case "Level20":
				locationOfInstantiate = new Vector3(9, -1, -2);
				rotationDirection = Quaternion.Euler(0, 270, 0);
				break;

			default:
				throw new ArgumentException(level + " doesn't exist.");
        }
        //Then instantiate the Player to the relevant location and rotation
        //Also set the number of keys to 0.
        Instantiate(objectToInstantiate, locationOfInstantiate, rotationDirection);
        if (objectToInstantiate != null) objectToInstantiate.GetComponent<Commands>().Keys = 0;
    }
}
