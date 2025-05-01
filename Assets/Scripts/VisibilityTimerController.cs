using UnityEngine;

public class VisibilityTimerController : MonoBehaviour
{
    void Update()
    {
        gameObject.SetActive(timerState.isTimerVisible);
    }
    public static class timerState
    {
        public static bool isTimerVisible = true;
    }

}
