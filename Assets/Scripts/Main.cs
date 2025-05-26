using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using TMPro;
using UnityEngine.UI;

public class Main : MonoBehaviour
{
    public Timer timerHUD;
    public bool isStart;
    public bool isNotEnd;
    private float timer = 0f;
    private float checkInterval = 5f;
    [SerializeField] public TMP_Text textMessage;
    [SerializeField] private TMP_InputField[] inputs;
    [SerializeField] private Button[] buttons;

    private bool isCheckingResults = false;
    private float resultCheckInterval = 2f;
    private float resultCheckTimer = 0f;

    void Start()
    {
        isStart = false; // Set to true immediately
        isNotEnd = true;
        ToggleUI(false); // Make UI elements visible right away

        if (timerHUD == null)
        {
            Debug.LogError("Timer HUD is not assigned in the inspector!");
        }

        int player_id = PlayerPrefs.GetInt("player_id", -1);
        if (player_id > 0)
        {
            Debug.Log(player_id);
        }
    }

    void Update()
    {
        if (isNotEnd)
        {
            if (isStart)
            {
                if (isCheckingResults)
                {
                    resultCheckTimer += Time.deltaTime;
                    if (resultCheckTimer >= resultCheckInterval)
                    {
                        resultCheckTimer = 0f;
                        StartCoroutine(CheckScoreStatus());
                    }
                }
                else
                {
                    timer += Time.deltaTime;
                    if (timer >= checkInterval)
                    {
                        timer = 0f;
                        StartCoroutine(CheckScoreStatus());
                    }
                }
            }
            else
            {
                timer += Time.deltaTime;
                if (timer >= checkInterval)
                {
                    timer = 0f;
                    StartCoroutine(CheckGameStatus());
                }
            }
        }
    }

    public void ToggleUI(bool show)
    {
        foreach (var input in inputs)
        {
            input.gameObject.SetActive(show);
            input.interactable = show;
        }

        foreach (var button in buttons)
        {
            button.gameObject.SetActive(show);
            button.interactable = show;
        }
    }

    public void StartCheckingResults()
    {
        isCheckingResults = true;
        resultCheckTimer = resultCheckInterval; // Force an immediate check
    }

    IEnumerator CheckScoreStatus()
    {
        int player_id = PlayerPrefs.GetInt("player_id", -1);
        string jsonData = JsonUtility.ToJson(new PlayerIdWrapper { player_id = player_id });

        UnityWebRequest www = new UnityWebRequest("https://cosmo-conquest-main-d99f72b06883.herokuapp.com/get_results.php", "POST");
        byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(jsonData);
        www.uploadHandler = new UploadHandlerRaw(bodyRaw);
        www.downloadHandler = new DownloadHandlerBuffer();
        www.SetRequestHeader("Content-Type", "application/json");

        yield return www.SendWebRequest();

        if (www.result == UnityWebRequest.Result.Success)
        {
            string jsonResponse = www.downloadHandler.text;
            ScoreStatusResponse response = JsonUtility.FromJson<ScoreStatusResponse>(jsonResponse);

            if (response.status == "ok")
            {
                isStart = false;
                isCheckingResults = false;
                Debug.Log("Game completed! Scores calculated.");

                if (response.results != null && response.results.Length > 0)
                {
                    string scoreText = "Game Results:\n";

                    for (int i = 0; i < response.results.Length; i++)
                    {
                        var playerScore = response.results[i];

                        if (i == 0)
                        {
                            scoreText += $"<color=yellow>Player {playerScore.username}: {playerScore.score} points</color>\n";
                        }
                        else
                        {
                            scoreText += $"Player {playerScore.username}: {playerScore.score} points\n";
                        }
                    }

                    textMessage.text = scoreText;
                }

                ToggleUI(false);
                isNotEnd = false;
            }
            else if (isCheckingResults && response.status == "waiting")
            {
                textMessage.text = "Waiting for other players...";
            }
        }
        else
        {
            Debug.LogError("Error checking score status: " + www.error);
        }
    }

    [System.Serializable]
    private class ScoreStatusResponse
    {
        public string status;
        public string message;
        public PlayerScore[] results;
    }

    [System.Serializable]
    private class PlayerScore
    {
        public string username;
        public int score;
    }

    IEnumerator CheckGameStatus()
    {
        int player_id = PlayerPrefs.GetInt("player_id", -1);
        string jsonData = JsonUtility.ToJson(new PlayerIdWrapper { player_id = player_id });

        UnityWebRequest www = new UnityWebRequest("https://cosmo-conquest-main-d99f72b06883.herokuapp.com/start_game.php", "POST");
        byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(jsonData);
        www.uploadHandler = new UploadHandlerRaw(bodyRaw);
        www.downloadHandler = new DownloadHandlerBuffer();
        www.SetRequestHeader("Content-Type", "application/json");

        yield return www.SendWebRequest();

        if (www.result == UnityWebRequest.Result.Success)
        {
            string jsonResponse = www.downloadHandler.text;
            GameStatusResponse response = JsonUtility.FromJson<GameStatusResponse>(jsonResponse);

            if (response.status == "active")
            {
                isStart = true;
                timerHUD.StopTimer();
                VisibilityTimerController.TimerState.IsTimerVisible = false;
                textMessage.text = "Enter the number of drones, amount = 1000";
                ToggleUI(true);
            }
        }
        else
        {
            string errorDetails = string.IsNullOrEmpty(www.downloadHandler.text)
                ? www.error
                : $"{www.error}: {www.downloadHandler.text}";
            Debug.LogError($"Error checking game status: {errorDetails}");

            if (textMessage != null)
            {
                textMessage.text = "Server connection error. Please try again later.";
            }
        }
    }

    [System.Serializable]
    private class PlayerIdWrapper
    {
        public int player_id;
    }

    [System.Serializable]
    private class GameStatusResponse
    {
        public string status;
        public int player_count;
    }
}
