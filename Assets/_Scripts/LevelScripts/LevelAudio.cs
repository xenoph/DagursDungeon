using UnityEngine;

/// <summary>
/// Plays a certain background sound based on the level that loads.
/// -BV
/// </summary>
public class LevelAudio : MonoBehaviour {

    public AudioSource[] BackgroundAudioSources;

    void Start() {

        int level = PlayerPrefs.GetInt("LevelCounter");

        switch (level) {
            case 1:
            case 2:
            case 3:
            case 4:
            case 5:
                BackgroundAudioSources[0].Play();
                break;

            case 6:
            case 7:
            case 8:
            case 9:
            case 10:
            case 11:
                BackgroundAudioSources[1].Play();
                break;

            case 12:
            case 13:
            case 14:
            case 15:
            case 16:
            case 17:
            case 18:
            case 19:
            case 20:
                BackgroundAudioSources[2].Play();
                break;
        }
    }
}
