using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class RestartSceneButton : MonoBehaviour
{
    private Button button;
    private AudioSource audioSource;
    
    void Start()
    {
        // Get the button component
        button = GetComponent<Button>();
        
        if (button != null)
        {
            // Add the restart method to button click event
            button.onClick.AddListener(RestartScene);
        }
        else
        {
            Debug.LogError("RestartSceneButton script must be attached to a GameObject with a Button component!");
        }
        
    }
    
    public void RestartScene()
    {
        DoRestart();
    }
    
    void DoRestart()
    {
        // Get current scene name and reload it
        string currentSceneName = SceneManager.GetActiveScene().name;
        SceneManager.LoadScene(currentSceneName);
    }
    
    // Alternative method to restart by scene index (if you prefer)
    public void RestartSceneByIndex()
    {
        int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
        SceneManager.LoadScene(currentSceneIndex);
    }
    
    // Method to restart a specific scene by name
    public void RestartSpecificScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }
}