using UnityEngine;

public class VisualController : MonoBehaviour
{
    [Header("Dependencies")]
    public StateController state;
    public PhaseController phase;

    [Header("Sphere Transforms")]
    public Transform sphereLeft;
    public Transform sphereRight;
    public Transform ghostLeft;
    public Transform ghostRight;

    [Header("Ghost Renderers")]
    public Renderer ghostLeftRenderer;
    public Renderer ghostRightRenderer;

    [Header("Motion")]
    public float amplitude = 3f;
    public float speed     = 2f;
    public float eyeSep    = 0.05f;

    [Header("Breathing")]
    public float baseScale  = 0.6f;
    public float breatheAmt = 0.08f;

    [Header("Ghost")]
    public float ghostLag     = 0.35f;
    public float ghostAmpMult = 0.9f;

    [Header("Background")]
    public Renderer bgRenderer;

    private float phaseValue = 0f;
    private static readonly int ColorProp  = Shader.PropertyToID("_Color");
    private static readonly int EnergyProp = Shader.PropertyToID("_Energy");

    void Update()
    {
        float dt = Time.deltaTime;

        phaseValue += speed * state.E * dt;

        float x = Mathf.Sin(phaseValue) * amplitude;

        float breath = baseScale + Mathf.Sin(phaseValue * 0.5f) * breatheAmt * state.E;
        Vector3 scaleVec = Vector3.one * breath;

        sphereLeft.localPosition  = new Vector3(x - eyeSep, 0f, 0f);
        sphereRight.localPosition = new Vector3(x + eyeSep, 0f, 0f);
        sphereLeft.localScale     = scaleVec;
        sphereRight.localScale    = scaleVec;

        HandleGhost();

        if (bgRenderer != null)
            bgRenderer.material.SetFloat(EnergyProp, state.E);
    }

    void HandleGhost()
    {
        bool ghostActive = (phase.PhaseLevel >= 3);

        ghostLeft.gameObject.SetActive(ghostActive);
        ghostRight.gameObject.SetActive(ghostActive);

        if (!ghostActive) return;

        float ghostPhase  = phaseValue - ghostLag;
        float ghostX      = Mathf.Sin(ghostPhase) * amplitude * ghostAmpMult;
        float ghostBreath = baseScale + Mathf.Sin(ghostPhase * 0.5f) * breatheAmt * state.E * 0.6f;

        ghostLeft.localPosition  = new Vector3(ghostX - eyeSep * 0.8f, 0f, 0.1f);
        ghostRight.localPosition = new Vector3(ghostX + eyeSep * 0.8f, 0f, 0.1f);
        ghostLeft.localScale     = Vector3.one * ghostBreath;
        ghostRight.localScale    = Vector3.one * ghostBreath;

        float alpha = Mathf.Lerp(0f, 0.45f, phase.PhaseProgress);

        if (ghostLeftRenderer != null)
        {
            Color c = new Color(1f, 0.13f, 0f, alpha);
            ghostLeftRenderer.material.SetColor(ColorProp, c);
        }
        if (ghostRightRenderer != null)
        {
            Color c = new Color(0f, 0.2f, 0.2f, alpha);
            ghostRightRenderer.material.SetColor(ColorProp, c);
        }
    }
}