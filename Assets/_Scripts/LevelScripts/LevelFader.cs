using UnityEngine;
using System.Collections;
using UnityEngine.UI;

[RequireComponent(typeof(GUITexture))]
public class LevelFader : MonoBehaviour {

    /*----------------------------------------------------------------------------*/
    /* THIS SCRIPT IS TAKEN FROM AN UNITY TUTORIAL AND ONLY SLIGHTLY MODIFIED /BV */
    /*----------------------------------------------------------------------------*/

    private float _fadeSpeed = 0.5f;          // Speed that the screen fades to and from black.


    private bool _sceneStarting = true;      // Whether or not the scene is still fading in.
    public bool SceneFinished;

    void Awake() {
        // Set the texture so that it is the the size of the screen and covers it.
        GetComponent<GUITexture>().pixelInset = new Rect(0f, 0f, Screen.width, Screen.height);
    }


    void Update() {
        // If the scene is starting...
        if(_sceneStarting)
            // ... call the StartScene function.
            StartScene();

        if (SceneFinished) {
            EndScene();
        }
    }


    void FadeToClear() {
        // Lerp the colour of the texture between itself and transparent.
        GetComponent<GUITexture>().color = Color.Lerp(GetComponent<GUITexture>().color, Color.clear, _fadeSpeed * Time.deltaTime);
    }


    void FadeToBlack() {
        // Lerp the colour of the texture between itself and black.
        GetComponent<GUITexture>().color = Color.Lerp(GetComponent<GUITexture>().color, Color.black, _fadeSpeed * Time.deltaTime);
    }

    void StartScene() {
        // Fade the texture to clear.
        FadeToClear();

        // If the texture is almost clear...
        if(GetComponent<GUITexture>().color.a <= 0.05f) {
            // ... set the colour to clear and disable the GUITexture.
            GetComponent<GUITexture>().color = Color.clear;
            GetComponent<GUITexture>().enabled = false;

            // The scene is no longer starting.
            _sceneStarting = false;
        }
    }


    public void EndScene() {
        // Make sure the texture is enabled.
        GetComponent<GUITexture>().enabled = true;

        // Start fading towards black.
        FadeToBlack();

        // If the screen is almost black...
        if (GetComponent<GUITexture>().color.a >= 0.95f) {
            SceneFinished = false;
        }
    }
}
