using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.Networking;
using System.Collections;

public class DroneDistribution : MonoBehaviour
{
    [SerializeField] private DroneManager droneManager;
    [SerializeField] private Main mainScript; 
    
    [SerializeField] private TMP_InputField kronusInput;
    [SerializeField] private TMP_InputField lyrionInput;
    [SerializeField] private TMP_InputField mystaraInput;
    [SerializeField] private TMP_InputField eclipsiaInput;
    [SerializeField] private TMP_InputField fioraInput;
    [SerializeField] private TMP_Text errorMessage;
    [SerializeField] private Button submitButton;
    
    private const int TotalDrones = 1000;
    private int remainingDrones = TotalDrones;
    
    void Start()
    {
        if (submitButton != null)
        {
            submitButton.onClick.AddListener(ValidateAndSubmit);
        }
        else
        {
            Debug.LogError("Submit button not found!");
        }
        
        if (droneManager == null)
        {
            Debug.LogError("DroneManager not assigned! Please assign it in the Inspector.");
        }

        if (mainScript == null)
        {
            Debug.LogError("Main script not assigned! Please assign it in the Inspector.");
        }

        kronusInput.onValueChanged.AddListener(delegate { CalculateRemainingDrones(); });
        lyrionInput.onValueChanged.AddListener(delegate { CalculateRemainingDrones(); });
        mystaraInput.onValueChanged.AddListener(delegate { CalculateRemainingDrones(); });
        eclipsiaInput.onValueChanged.AddListener(delegate { CalculateRemainingDrones(); });
        fioraInput.onValueChanged.AddListener(delegate { CalculateRemainingDrones(); });

        UpdateRemainingDronesText();
    }

    void CalculateRemainingDrones()
    {
        int kronus = string.IsNullOrEmpty(kronusInput.text) ? 0 : int.Parse(kronusInput.text);
        int lyrion = string.IsNullOrEmpty(lyrionInput.text) ? 0 : int.Parse(lyrionInput.text);
        int mystara = string.IsNullOrEmpty(mystaraInput.text) ? 0 : int.Parse(mystaraInput.text);
        int eclipsia = string.IsNullOrEmpty(eclipsiaInput.text) ? 0 : int.Parse(eclipsiaInput.text);
        int fiora = string.IsNullOrEmpty(fioraInput.text) ? 0 : int.Parse(fioraInput.text);

        int totalUsed = kronus + lyrion + mystara + eclipsia + fiora;
        remainingDrones = TotalDrones - totalUsed;
        UpdateRemainingDronesText();
    }

    void UpdateRemainingDronesText()
    {
        if (mainScript != null && mainScript.textMessage != null)
        {
            string message = $"Enter the number of drones (remaining: {remainingDrones} / {TotalDrones})";
            mainScript.textMessage.text = message;
        }
    }

    void ValidateAndSubmit()
    {
        if (!int.TryParse(kronusInput.text, out var kronus) ||
            !int.TryParse(lyrionInput.text, out var lyrion) ||
            !int.TryParse(mystaraInput.text, out var mystara) ||
            !int.TryParse(eclipsiaInput.text, out var eclipsia) ||
            !int.TryParse(fioraInput.text, out var fiora))
        {
            errorMessage.text = "Enter numeric values.";
            return;
        }

        if (kronus < 0 || kronus > 1000 || lyrion < 0 || lyrion > 1000 ||
            mystara < 0 || mystara > 1000 || eclipsia < 0 || eclipsia > 1000 ||
            fiora < 0 || fiora > 1000)
        {
            errorMessage.text = "Values \u200b\u200bmust be between 0 and 1000.";
            return;
        }

        if (!(kronus >= lyrion && lyrion >= mystara && mystara >= eclipsia && eclipsia >= fiora))
        {
            errorMessage.text = "The order is broken: Kronus ≥ Lyrion ≥ Mystara ≥ Eclipsia ≥ Fiora.";
            return;
        }

        if (kronus + lyrion + mystara + eclipsia + fiora != 1000)
        {
            errorMessage.text = "The amount should equal 1000.";
            return;
        }
        
        submitButton.interactable = false;

        StartCoroutine(SubmitDistribution(kronus, lyrion, mystara, eclipsia, fiora));
    }

    IEnumerator SubmitDistribution(int kronus, int lyrion, int mystara, int eclipsia, int fiora)
    {
        int player_id = PlayerPrefs.GetInt("player_id", -1);
        if (player_id < 0)
        {
            errorMessage.text = "Player ID not found. Please register first.";
            yield break;
        }
        
        string jsonData = JsonUtility.ToJson(new DistributionData
        {
            player_id = player_id,
            kronus = kronus,
            lyrion = lyrion,
            mystara = mystara,
            eclipsia = eclipsia,
            fiora = fiora
        });
        
        UnityWebRequest www = new UnityWebRequest("https://cosmo-conquest-main-d99f72b06883.herokuapp.com/submit_move.php", "POST");
        byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(jsonData);
        www.uploadHandler = new UploadHandlerRaw(bodyRaw);
        www.downloadHandler = new DownloadHandlerBuffer();
        www.SetRequestHeader("Content-Type", "application/json");

        errorMessage.text = "Submitting distribution...";

        yield return www.SendWebRequest();
        
        if (www.result == UnityWebRequest.Result.Success)
        {
            string jsonResponse = www.downloadHandler.text;
            DistributionResponse response = JsonUtility.FromJson<DistributionResponse>(jsonResponse);

            if (response.status == "success")
            {
                droneManager.DeployDrones(kronus, lyrion, mystara, eclipsia, fiora);
                
                if (mainScript != null)
                {
                    mainScript.StartCheckingResults();
                    errorMessage.text = "Move submitted. Waiting for results...";
                }
            }
            else if (response.error != null)
            {
                errorMessage.text = "Error: " + response.error;
            }
        }
        else
        {
            errorMessage.text = "Error submitting: " + www.error;
            Debug.LogError("Network error: " + www.error);
        }
    }

    [System.Serializable]
    private class DistributionData
    {
        public int player_id;
        public int kronus;
        public int lyrion;
        public int mystara;
        public int eclipsia;
        public int fiora;
    }

    [System.Serializable]
    private class DistributionResponse
    {
        public string status;
        public int move_id;
        public string error;
    }
}
