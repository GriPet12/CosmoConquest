using UnityEngine;

public class VisibilityTimerController : MonoBehaviour
{
    void Update()
    {
        gameObject.SetActive(TimerState.IsTimerVisible);
    }
    public static class TimerState
    {
        public static bool IsTimerVisible = true;
    }

}
