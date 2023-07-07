using UnityEngine.SceneManagement;
using UnityEngine;

public class SceneController : MonoBehaviour
{
    public static void LoadScene(string name)
    {
        SceneManager.LoadScene(name);
    }
}
