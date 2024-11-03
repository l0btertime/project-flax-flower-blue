using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    // this script uses a singleton pattern
    // a static object is instantiated so that there will only ever be one sceneloader
    public static SceneLoader loader;

    private void Awake()
    {
        // sets up the singleton object
        if(loader == null)
        {
            loader = this;
        }
        else
        {
            Destroy(this.gameObject);
        }
        // ensures sceneloader exists outside of other scenes, so it is not unloaded ever
        DontDestroyOnLoad(this);
    }

    public static void LoadScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }

    public static void LoadScene(int sceneIndex)
    {
        SceneManager.LoadScene(sceneIndex);
    }
}
