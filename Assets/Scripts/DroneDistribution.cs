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

        errorMessage.text = "Distribution successful!";
    }
}
