using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// The AudioManager class handles audio playback and volume control for a scene.
// It includes functionality to gradually increase and decrease the volume at scene start and end.
public class AudioManager : MonoBehaviour
{
    // Serialized field to set in the Unity Editor
    [SerializeField] private AudioSource source; // Reference to the AudioSource component

    // Private variable to track if the volume is currently decreasing
    private bool onVolumeDecreasing = false;

    // Start is called before the first frame update
    void Start()
    {
        // Check if the AudioSource reference is set
        if (source == null)
        {
            Debug.LogError("No Audio source reference was found");
        }

        // Check if the PlayerPrefs contains a volume key
        if (!PlayerPrefs.HasKey("Volume"))
        {
            Debug.LogError("No PlayerPrefs' volume key was found");
        }
        else
        {
            // Play the audio and start increasing the volume
            source.Play();
            OnSceneStarting();
        }
    }

    // Update is called once per frame
    void Update()
    {
        // If volume is not decreasing, update the AudioSource volume from PlayerPrefs
        if (!onVolumeDecreasing)
        {
            source.volume = PlayerPrefs.GetFloat("Volume");
        }
    }

    // Method to start the coroutine for increasing the volume at scene start
    void OnSceneStarting()
    {
        StartCoroutine(IncreaseVolume());
    }

    // Method to start the coroutine for decreasing the volume at scene end
    public void OnSceneEnding()
    {
        StartCoroutine(DecreaseVolume());
    }

    // Coroutine to gradually increase the volume over 4 seconds
    private IEnumerator IncreaseVolume()
    {
        float elapsedTime = 0f; // Track the elapsed time
        float startVolume = 0f; // Starting volume is 0
        float targetVolume = PlayerPrefs.GetFloat("Volume"); // Target volume from PlayerPrefs

        // Gradually increase the volume
        while (elapsedTime < 4f)
        {
            float newVolume = Mathf.Lerp(startVolume, targetVolume, elapsedTime / 4f);
            source.volume = newVolume;

            elapsedTime += Time.deltaTime;
            yield return null; // Wait for the next frame
        }

        source.volume = targetVolume; // Ensure the final volume is set to target
    }

    // Coroutine to gradually decrease the volume over 1 second
    private IEnumerator DecreaseVolume()
    {
        onVolumeDecreasing = true; // Indicate that the volume is decreasing

        float elapsedTime = 0f; // Track the elapsed time
        float startVolume = source.volume; // Starting volume is the current volume
        float targetVolume = 0f; // Target volume is 0

        // Gradually decrease the volume
        while (elapsedTime < 1f)
        {
            float newVolume = Mathf.Lerp(startVolume, targetVolume, elapsedTime / 1f);
            source.volume = newVolume;

            elapsedTime += Time.deltaTime;
            yield return null; // Wait for the next frame
        }

        source.volume = targetVolume; // Ensure the final volume is set to 0
    }
}