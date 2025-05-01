using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;

public class ExitGame : MonoBehaviour
{
    public void QuitGame()
    {
        int player_id = PlayerPrefs.GetInt("player_id", -1);
        StartCoroutine(SendEndRequest(player_id));
        Debug.Log("Quitting game...");
        Application.Quit();
        
    #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
    #endif
    }
    
    IEnumerator SendEndRequest(int playerId)
    {
        string jsonData = JsonUtility.ToJson(new PlayerIdWrapper { player_id = playerId });
    
        UnityWebRequest www = new UnityWebRequest("http://localhost/cosmo_conquest/exit.php", "POST");
        byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(jsonData);
        www.uploadHandler = new UploadHandlerRaw(bodyRaw);
        www.downloadHandler = new DownloadHandlerBuffer();
        www.SetRequestHeader("Content-Type", "application/json");
    
        yield return www.SendWebRequest();
    
        if (www.result == UnityWebRequest.Result.Success)
        {
            PlayerPrefs.DeleteKey("player_id");
            SceneManager.LoadScene("RegisterScene");
        }
        else
        {
            Debug.LogError("Error: " + www.error + "\n" + www.downloadHandler.text);
        }
    }
    
    [System.Serializable]
    private class PlayerIdWrapper
    {
        public int player_id;
    }
}