using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class StrobeController : MonoBehaviour
{
    [Header("Dependencies")]
    public PhaseController phase;
    public StateController state;

    [Header("Textures - One Per Round")]
    public Texture2D round1Texture;   // Lucifuge Rofocale
    public Texture2D round2Texture;   // Lilith
    public Texture2D round3Texture;   // Prometheus

    [Range(0f, 1f)] public float strobeAlpha   = 0.85f;
    [Range(0f, 1f)] public float baphometAlpha = 0.5f;

    [Header("Round (set by ShutdownController)")]
    public int currentRound = 1;

    private RawImage baphometImg;
    private RawImage strobeImg;

    private bool burst20Done   = false;
    private bool burst45Done   = false;
    private bool phase3Started = false;

    void Start()
    {
        GameObject canvasGO     = new GameObject("StrobeCanvas");
        Canvas canvas           = canvasGO.AddComponent<Canvas>();
        canvas.renderMode       = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder     = 10;
        canvasGO.AddComponent<CanvasScaler>();
        canvasGO.AddComponent<GraphicRaycaster>();

        GameObject bGO      = new GameObject("RoundImage");
        bGO.transform.SetParent(canvasGO.transform, false);
        baphometImg         = bGO.AddComponent<RawImage>();
        baphometImg.texture = GetRoundTexture();
        baphometImg.color   = new Color(1f, 1f, 1f, 0f);
        RectTransform bRT   = baphometImg.GetComponent<RectTransform>();
        bRT.anchorMin       = new Vector2(0.3f, 0.1f);
        bRT.anchorMax       = new Vector2(0.7f, 0.9f);
        bRT.offsetMin       = Vector2.zero;
        bRT.offsetMax       = Vector2.zero;

        GameObject sGO      = new GameObject("Strobe");
        sGO.transform.SetParent(canvasGO.transform, false);
        strobeImg           = sGO.AddComponent<RawImage>();
        strobeImg.texture   = null;
        strobeImg.color     = new Color(0f, 0f, 0f, 0f);
        RectTransform sRT   = strobeImg.GetComponent<RectTransform>();
        sRT.anchorMin       = Vector2.zero;
        sRT.anchorMax       = Vector2.one;
        sRT.offsetMin       = Vector2.zero;
        sRT.offsetMax       = Vector2.zero;
    }

    Texture2D GetRoundTexture()
    {
        switch (currentRound)
        {
            case 1:  return round1Texture;
            case 2:  return round2Texture;
            case 3:  return round3Texture;
            default: return round1Texture;
        }
    }

    public void SetRound(int round)
    {
        currentRound = round;
        if (baphometImg != null)
            baphometImg.texture = GetRoundTexture();
    }

    void Update()
    {
        float t = state.T_hold;

        if (t < 0.5f && burst20Done)
        {
            burst20Done   = false;
            burst45Done   = false;
            phase3Started = false;
            CancelInvoke();
            HideImages();
            // Refresh texture for new round
            if (baphometImg != null)
                baphometImg.texture = GetRoundTexture();
        }

        if (t >= 20f && !burst20Done)
        {
            burst20Done = true;
            StartCoroutine(SingleWhiteBurst());
        }

        if (t >= 45f && !burst45Done)
        {
            burst45Done = true;
            StartCoroutine(DoubleBurst());
        }

        if (t >= 60f && !phase3Started)
        {
            phase3Started = true;
            InvokeRepeating("Phase3Cycle", 0f, 1.2f);
        }
    }

    IEnumerator SingleWhiteBurst()
    {
        for (int i = 0; i < 3; i++)
        {
            strobeImg.color = new Color(1f, 1f, 1f, strobeAlpha);            // #ffffff WHITE
            yield return new WaitForSeconds(0.04f);
            strobeImg.color = new Color(0f, 0f, 0f, 0f);
            yield return new WaitForSeconds(0.06f);
        }
    }

    IEnumerator DoubleBurst()
    {
        // First burst — lime green
        for (int i = 0; i < 3; i++)
        {
            strobeImg.color = new Color(0.380f, 0.902f, 0.949f, strobeAlpha);    // #61e6f2 CYAN
            yield return new WaitForSeconds(0.04f);
            strobeImg.color = new Color(0f, 0f, 0f, 0f);
            yield return new WaitForSeconds(0.06f);
        }

        yield return new WaitForSeconds(1.5f);

        // Second burst — dark red
        for (int i = 0; i < 3; i++)
        {
            strobeImg.color = new Color(0.702f, 0f, 0f, strobeAlpha);        // #b30000 DARK RED
            yield return new WaitForSeconds(0.04f);
            strobeImg.color = new Color(0f, 0f, 0f, 0f);
            yield return new WaitForSeconds(0.06f);
        }
    }

    void Phase3Cycle()
    {
        StartCoroutine(Phase3Burst());
    }

    IEnumerator Phase3Burst()
    {
        baphometImg.color = new Color(1f, 1f, 1f, baphometAlpha);

        bool red = false;
        for (int g = 0; g < 3; g++)
        {
            for (int i = 0; i < 3; i++)
            {
                red = !red;
                strobeImg.color = red
                    ? new Color(1f, 0.05f, 0.05f, strobeAlpha)               // #ff0d0d RED
                    : new Color(0.85f, 0.88f, 0.92f, strobeAlpha);           // #d9e0eb SILVER
                yield return new WaitForSeconds(0.04f);
                strobeImg.color = new Color(0f, 0f, 0f, 0f);
                yield return new WaitForSeconds(0.06f);
            }
            yield return new WaitForSeconds(0.15f);
        }

        baphometImg.color = new Color(0f, 0f, 0f, 0f);
    }

    void HideImages()
    {
        if (baphometImg != null) baphometImg.color = new Color(0f, 0f, 0f, 0f);
        if (strobeImg   != null) strobeImg.color   = new Color(0f, 0f, 0f, 0f);
    }

    public void HideAll()
    {
        CancelInvoke();
        StopAllCoroutines();
        HideImages();
    }
}