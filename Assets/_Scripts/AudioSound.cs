using UnityEngine;
using UnityEngine.SceneManagement;

public class AudioSound : MonoBehaviour {

    public static AudioSound Instance;

    void Awake() {
        if(Instance == null) {
            DontDestroyOnLoad(gameObject);
            Instance = this;
        } else if(Instance != this) {
            Destroy(gameObject);
        }
    }

    void Update() {
        if (SceneManager.GetActiveScene().name != "MainMenu" &&
            SceneManager.GetActiveScene().name != "Options" &&
            SceneManager.GetActiveScene().name != "Credits") {
            GetComponent<AudioSource>().Stop();
        }
    }

    public void MuteSound() {
        AudioListener.pause = true;
    }

    public void EnableSound() {
        AudioListener.pause = false;
    }
}
