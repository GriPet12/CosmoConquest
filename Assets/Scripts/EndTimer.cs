using UnityEngine;
using UnityEngine.SceneManagement;

public class EndTimer : MonoBehaviour
{
    public void EndTime()
    {
        SceneManager.LoadScene("StartScene");
    }
}