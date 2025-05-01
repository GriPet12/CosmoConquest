using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Networking;
using System.Collections;
using UnityEngine.SceneManagement;

public class GameRegisterManager : MonoBehaviour
{
    [SerializeField] private TMP_InputField usernameField;
    [SerializeField] private Button registerButton;
    [SerializeField] private TMP_Text statusText;
    
    void Start()
    {
        registerButton.onClick.AddListener(RegisterPlayer);
    }

    void RegisterPlayer()
    {
        StartCoroutine(SendRegisterRequest());
    }

    IEnumerator SendRegisterRequest()
    {
        string jsonData = JsonUtility.ToJson(new UsernameWrapper { username = usernameField.text });

        UnityWebRequest www = new UnityWebRequest("http://localhost/cosmo_conquest/register.php", "POST");
        byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(jsonData);
        www.uploadHandler = new UploadHandlerRaw(bodyRaw);
        www.downloadHandler = new DownloadHandlerBuffer();
        www.SetRequestHeader("Content-Type", "application/json");

        yield return www.SendWebRequest();

        if (www.result == UnityWebRequest.Result.Success)
        {
            string jsonResponse = www.downloadHandler.text;
            PlayerData responseData = JsonUtility.FromJson<PlayerData>(jsonResponse);
            if (responseData.player_id > 0)
            {
                PlayerPrefs.SetInt("player_id", responseData.player_id);
                SceneManager.LoadScene("StartScene");
            }
            else
            {
                statusText.text = "Registration failed: Invalid player ID.";
            }
        }
        else
        {
            statusText.text = "Error: " + www.error + "\n" + www.downloadHandler.text;
        }
    }

    [System.Serializable]
    public class UsernameWrapper
    {
        public string username;
    }

}

[System.Serializable]
public class PlayerData
{
    public int player_id;
}
