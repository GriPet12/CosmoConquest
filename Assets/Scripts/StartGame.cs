using UnityEngine;
using UnityEngine.SceneManagement;

public class StartGame : MonoBehaviour
{
    public void initGame()
    {
        var playerId = PlayerPrefs.GetInt("player_id", -1);
        SceneManager.LoadScene(playerId == -1 ? "RegisterScene" : "MainScene");
    }
}