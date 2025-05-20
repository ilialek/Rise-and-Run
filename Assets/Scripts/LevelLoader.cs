using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

// The LevelLoader class handles transitioning between scenes in the game.
// It includes functionality for playing transition animations and managing audio during transitions.
public class LevelLoader : MonoBehaviour
{
    // Serialized fields to set in the Unity Editor
    [SerializeField] private Animator transition; // Reference to the Animator component for scene transition
    [SerializeField] private float transitionTime = 1f; // Duration of the transition animation

    // Private variable to hold reference to the AudioManager
    private AudioManager audioManager;

    // Start is called before the first frame update
    void Start()
    {
        // Find the AudioManager object in the scene and get its AudioManager component
        if (GameObject.Find("Audio_manager") != null)
        {
            audioManager = GameObject.Find("Audio_manager").GetComponent<AudioManager>();
        }
    }

    // Method to load the next level
    public void LoadNextLevel()
    {
        // Start the coroutine to load the next level
        StartCoroutine(LoadLevel(SceneManager.GetActiveScene().buildIndex + 1));

        // Call the OnSceneEnding method on the AudioManager to handle volume decrease
        if (audioManager != null)
        {
            audioManager.OnSceneEnding();
        }
    }

    // Coroutine to handle the level loading process with transition
    IEnumerator LoadLevel(int levelIndex)
    {
        // Trigger the start transition animation
        transition.SetTrigger("Start");

        // Wait for the duration of the transition animation
        yield return new WaitForSeconds(transitionTime);

        // Load the specified scene
        SceneManager.LoadScene(levelIndex);
    }
}
