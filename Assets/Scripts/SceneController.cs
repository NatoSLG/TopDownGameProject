using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

//Handles all scene transitions in game
public class SceneController : MonoBehaviour
{
   public void SceneChange(string name)
    {
        SceneManager.LoadScene(name);
        Time.timeScale = 1f; //resets time to 1 when changing screens, prevents the game to be frozen if players gets gameover and plays again
    }
}
