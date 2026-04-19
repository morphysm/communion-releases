using UnityEngine;

public class PhaseController : MonoBehaviour
{
    [Header("Dependencies")]
    public StateController state;

    [Header("Read-Only")]
    public int PhaseLevel = 1; 
    public float PhaseProgress = 0f;

    void Update()
    {
        float t = state.T_hold;

        if      (t < 20f)  { PhaseLevel = 1; PhaseProgress = t / 20f; }
        else if (t < 60f)  { PhaseLevel = 2; PhaseProgress = (t - 20f)  / 40f; }
        else if (t < 120f) { PhaseLevel = 3; PhaseProgress = (t - 60f)  / 60f; }
        else if (t < 240f) { PhaseLevel = 4; PhaseProgress = (t - 120f) / 120f; }
        else               { PhaseLevel = 5; PhaseProgress = 1f; }
    }

    public float GlobalProgress()
    {
        return Mathf.Clamp01(state.T_hold / 240f);
    }
}