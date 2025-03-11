using UnityEngine;
using UnityEngine.SceneManagement;

public class buttons : MonoBehaviour
{
    public void load_scene(string scene_name)
    {
        SceneManager.LoadScene(scene_name);
    }

    public void quit_game()
    {
        Application.Quit();
        UnityEditor.EditorApplication.isPlaying = false;
    }
}
