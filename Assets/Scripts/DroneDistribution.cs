using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class DroneDistribution : MonoBehaviour
{
    [SerializeField] private TMP_InputField  kronusInput;
    [SerializeField] private TMP_InputField lyrionInput;
    [SerializeField] private TMP_InputField mystaraInput;
    [SerializeField] private TMP_InputField eclipsiaInput;
    [SerializeField] private TMP_InputField fioraInput;
    [SerializeField] private TMP_Text errorMessage;
    [SerializeField] private Button submitButton;

    private Dictionary<string, int[]> teams = new()
    {
        { "Команда 2", new[] { 400, 300, 200, 100, 0 } },
        { "Команда 3", new[] { 300, 250, 250, 100, 100 } },
        { "Команда 4", new[] { 320, 210, 210, 130, 130 } },
        { "Команда 5", new[] { 370, 220, 180, 130, 100 } }
    };

    private Dictionary<string, int> scores = new();
    
    void Start()
    {
        if (submitButton != null)
        {
            submitButton.onClick.AddListener(ValidateInput);
        }
        else
        {
            Debug.LogError("Submit button not found!");
        }
    }
    void CalculateScores(int[] player)
    {
        teams.Add("Команда 1", player);
        foreach (var team in teams)
        {
            scores[team.Key] = 0;
        }

        var teamNames = new List<string>(teams.Keys);

        for (int i = 0; i < teamNames.Count; i++)
        {
            for (int j = i + 1; j < teamNames.Count; j++)
            {
                CompareTeams(teamNames[i], teamNames[j]);
            }
        }
    }

    void CompareTeams(string teamA, string teamB)
    {
        int[] dronesA = teams[teamA];
        int[] dronesB = teams[teamB];

        int winsA = 0, winsB = 0;

        for (int i = 0; i < dronesA.Length; i++)
        {
            if (dronesA[i] > dronesB[i]) winsA++;
            else if (dronesA[i] < dronesB[i]) winsB++;
        }

        if (winsA > winsB)
            scores[teamA] += 2;
        else if (winsB > winsA)
            scores[teamB] += 2;
        else
        {
            scores[teamA] += 1;
            scores[teamB] += 1;
        }
    }

    void DisplayResults()
    {
        string result = "Результати раунду:\n";
        string winner = "";
        int maxScore = 0;

        foreach (var score in scores)
        {
            result += $"{score.Key}: {score.Value} балів\n";
            if (score.Value > maxScore)
            {
                maxScore = score.Value;
                winner = score.Key;
            }
        }

        errorMessage.text = result;
        
        if (errorMessage.text.Contains(winner))
        {
            errorMessage.color = Color.green;
        }

        Debug.Log(result);
    }

    void ValidateInput()
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

        int[] result = { kronus, lyrion, mystara, eclipsia, fiora };
        CalculateScores(result);
        DisplayResults();
    }
}
