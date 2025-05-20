using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// The Menu class handles the functionality for a settings menu in a game.
// It allows toggling between main buttons and settings menu, and updates volume settings.
public class Menu : MonoBehaviour
{
    // Variables accessible in the Unity Editor
    [SerializeField] private Slider slider; // Reference to the UI Slider component to control volume
    [SerializeField] private GameObject settings; // Reference to the settings menu GameObject
    [SerializeField] private GameObject mainButtons; // Reference to the main buttons GameObject

    // Awake is called when the script instance is being loaded
    void Awake()
    {
        // Set the default volume to 0.35 when the game starts
        PlayerPrefs.SetFloat("Volume", 0.35f);
    }

    // Update is called once per frame
    void Update()
    {
        // Check if the slider reference is set
        if (slider != null)
        {
            // Update the volume value in PlayerPrefs to match the slider's value
            PlayerPrefs.SetFloat("Volume", slider.value);
        }
        else
        {
            // Log an error if the slider reference is missing
            Debug.LogError("No slider reference was found");
        }

        // Log the current value of the slider for debugging purposes
        Debug.Log(slider.value);
    }

    // Method to open the settings menu and hide the main buttons
    public void OpenSettings()
    {
        // Check if the main buttons are active and the settings menu is not active
        if (mainButtons.activeSelf && !settings.activeSelf)
        {
            // Disable the main buttons and enable the settings menu
            mainButtons.SetActive(false);
            settings.SetActive(true);
        }
        else
        {
            // Log errors if the conditions are not met
            if (!mainButtons.activeSelf)
            {
                Debug.LogError("Main buttons are disabled");
            }
            else if (settings.activeSelf)
            {
                Debug.LogError("Settings menu is active");
            }
        }
    }

    // Method to open the main buttons and hide the settings menu
    public void OpenMainButtons()
    {
        // Check if the main buttons are not active and the settings menu is active
        if (!mainButtons.activeSelf && settings.activeSelf)
        {
            // Disable the settings menu and enable the main buttons
            settings.SetActive(false);
            mainButtons.SetActive(true);
        }
        else
        {
            // Log errors if the conditions are not met
            if (mainButtons.activeSelf)
            {
                Debug.LogError("Main buttons are active");
            }
            else if (!settings.activeSelf)
            {
                Debug.LogError("Settings menu is disabled");
            }
        }
    }
}
