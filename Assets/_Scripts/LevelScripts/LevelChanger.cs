using System;
using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

/// <summary>
/// Called when the game needs to load a new scene. 
/// -BV
/// </summary>
public class LevelChanger : MonoBehaviour {

    //Create a list of all the levels currently in the game
    private readonly string[] _levels = {"Tutorial", "Level1", "Level2", "Level3", "Level4", "Level5", "Level6", "Level7",
        "Level8", "Level9", "Level10", "Level11", "Level12", "Level13", "Level14", "Level15", "Level16", "Level17", "Level18",
        "Level19"};

    /// <summary>
    /// Changes the level to next level (Player goes down)
    /// </summary>
    public void ChangeLevel() {
        StartCoroutine(FadeAndChangeLevel(1));
    }

    /// <summary>
    /// Changes the level to previous level (Player goes up)
    /// </summary>
    public void GoUp() {
        StartCoroutine(FadeAndChangeLevel(-1));
    }

    /// <summary>
    /// Fades out and changes level
    /// </summary>
    /// <param name="direction">The direction.</param>
    /// <returns></returns>
    private IEnumerator FadeAndChangeLevel(int direction) {
        //If it's any other level than the tutorial and player is going down
        if (direction == 1) {
            //Set the previous level number
            if (SceneManager.GetActiveScene().name != "Tutorial") {
                PlayerPrefs.SetInt("PreviousLevel", int.Parse(SceneManager.GetActiveScene().name.Remove(0, 5)));
            }
            //Play the animation that moves the camera towards the hatch
            GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Animator>().SetBool("GoDown", true);
            yield return new WaitForSeconds(0.5f);
            //Set the scene to finished to make the fader start
            GetComponent<LevelFader>().SceneFinished = true;
            //Then play the sound for walking down stairs
            GameObject.Find("LadderAudio").GetComponent<AudioSource>().Play();
            yield return new WaitForSeconds(3.5f);
        }
        else if (SceneManager.GetActiveScene().name != "Tutorial" && direction == -1) {
            //Wait a second so the Player can see the stairs
            yield return new WaitForSeconds(0.5f);
            //Set the previous level number
            PlayerPrefs.SetInt("PreviousLevel", int.Parse(SceneManager.GetActiveScene().name.Remove(0, 5)));
            //Play the animation that moves the camera upwards towards the roof
            GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Animator>().SetBool("GoUp", true);
            yield return new WaitForSeconds(0.5f);
            //Set the scene to finished to make the fader start
            GetComponent<LevelFader>().SceneFinished = true;
            //Then play the sound for walking down stairs
            GameObject.Find("LadderAudio").GetComponent<AudioSource>().Play();
            yield return new WaitForSeconds(3.5f);
        }
        else {
            //If it's the tutorial level, just end scene with fading
            GetComponent<LevelFader>().SceneFinished = true;
            yield return new WaitForSeconds(3);
        }        
        //Then get the current level from the level list by finding the index
        int currentLevelIndex = Array.IndexOf(_levels, SceneManager.GetActiveScene().name);
        //And load the new level which would be index + 1 for next and index - 1 for previous.
        SceneManager.LoadScene(_levels[currentLevelIndex+direction]);
    }
}
