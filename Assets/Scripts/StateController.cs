using UnityEngine;

public class StateController : MonoBehaviour
{
    [Header("Dependencies")]
    public InputController input;

    [Header("Energy")]
    [Range(0f, 1f)] public float E = 0f;
    public float riseRate  = 1.2f;
    public float decayRate = 0.5f;

    [Header("Time Hold")]
    public float T_hold  = 0f;
    public float T_decay = 0.2f;
    public float maxTime = 300f;

    void Update()
    {
        if (UnityEngine.InputSystem.Keyboard.current.f11Key.wasPressedThisFrame)
            Screen.fullScreen = !Screen.fullScreen;

        float dt = Time.deltaTime;

        if (input.IsHolding)
        {
            E       += riseRate  * dt;
            T_hold  += dt;
        }
        else
        {
            E       -= decayRate * dt;
            T_hold  -= T_decay   * dt;
        }

        E      = Mathf.Clamp01(E);
        T_hold = Mathf.Clamp(T_hold, 0f, maxTime);
    }
}