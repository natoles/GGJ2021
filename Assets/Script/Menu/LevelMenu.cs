using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelMenu : MonoBehaviour
{
    public string LevelToLoad; 
    public void LoadLevel()
    {
        SceneManager.LoadScene(LevelToLoad);
    }
}

