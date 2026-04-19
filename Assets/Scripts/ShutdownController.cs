using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ShutdownController : MonoBehaviour
{
    [Header("Dependencies")]
    public StateController state;

    [Header("Settings")]
    public float shutdownTime = 77f;

    [Header("Finale Texture")]
    public Texture2D finaleTexture;   // Lucifuge Rofocale

    private bool shutdownStarted = false;
    private AudioSource pitchSource;
    private GameObject countdownCanvas;
    private GameObject finaleCanvas;
    private int currentRound = 1;

    private readonly Color limeGreen  = new Color(0.2f, 1f, 0.1f, 1f);
    private readonly Color bloodRed   = new Color(0.6f, 0f, 0f, 1f);

    void Start()
    {
        pitchSource             = gameObject.AddComponent<AudioSource>();
        pitchSource.playOnAwake = false;
        pitchSource.loop        = false;
        pitchSource.volume      = 0f;
    }

    void Update()
    {
        // Esc quits at any time
        if (UnityEngine.InputSystem.Keyboard.current != null &&
            UnityEngine.InputSystem.Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            Application.Quit();
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#endif
        }

        if (!shutdownStarted && state.T_hold >= shutdownTime)
        {
            shutdownStarted = true;
            StartCoroutine(ShutdownSequence());
        }
    }

    IEnumerator ShutdownSequence()
    {
        StartCoroutine(PlayScreech());

        // Build countdown canvas
        countdownCanvas         = new GameObject("CountdownCanvas");
        Canvas canvas           = countdownCanvas.AddComponent<Canvas>();
        canvas.renderMode       = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder     = 20;
        countdownCanvas.AddComponent<CanvasScaler>();
        countdownCanvas.AddComponent<GraphicRaycaster>();

        RawImage[] sticks = new RawImage[5];
        for (int i = 0; i < 5; i++)
            sticks[i] = CreateStick(countdownCanvas, i);

        for (int i = 4; i >= 0; i--)
        {
            yield return new WaitForSeconds(1f);
            if (sticks[i] != null)
                sticks[i].color = new Color(0f, 0f, 0f, 0f);
        }

        yield return new WaitForSeconds(0.5f);
        pitchSource.Stop();
        Destroy(countdownCanvas);

        currentRound++;

        if (currentRound > 3)
            StartCoroutine(FinaleSequence());
        else
            ResetAll();
    }

    IEnumerator FinaleSequence()
    {
        // Stop all audio
        AudioController audio = FindObjectOfType<AudioController>();
        if (audio != null)
        {
            audio.leftSource.Stop();
            audio.rightSource.Stop();
        }

        // Build finale canvas
        finaleCanvas            = new GameObject("FinaleCanvas");
        Canvas fc               = finaleCanvas.AddComponent<Canvas>();
        fc.renderMode           = RenderMode.ScreenSpaceOverlay;
        fc.sortingOrder         = 30;
        finaleCanvas.AddComponent<CanvasScaler>();
        finaleCanvas.AddComponent<GraphicRaycaster>();

        // Lucifuge fullscreen
        GameObject imgGO        = new GameObject("FinaleImage");
        imgGO.transform.SetParent(finaleCanvas.transform, false);
        RawImage finaleImg      = imgGO.AddComponent<RawImage>();
        finaleImg.texture       = finaleTexture;
        finaleImg.color         = new Color(1f, 1f, 1f, 0.7f);
        RectTransform fRT       = finaleImg.GetComponent<RectTransform>();
        fRT.anchorMin           = new Vector2(0.2f, 0.05f);
        fRT.anchorMax           = new Vector2(0.8f, 0.95f);
        fRT.offsetMin           = Vector2.zero;
        fRT.offsetMax           = Vector2.zero;

        // Strobe layer
        GameObject strobeGO     = new GameObject("FinaleStrobe");
        strobeGO.transform.SetParent(finaleCanvas.transform, false);
        RawImage strobeImg      = strobeGO.AddComponent<RawImage>();
        strobeImg.texture       = null;
        strobeImg.color         = new Color(0f, 0f, 0f, 0f);
        RectTransform sRT       = strobeImg.GetComponent<RectTransform>();
        sRT.anchorMin           = Vector2.zero;
        sRT.anchorMax           = Vector2.one;
        sRT.offsetMin           = Vector2.zero;
        sRT.offsetMax           = Vector2.zero;

        // Frenzy silver strobe for 4 seconds
        float endTime = Time.time + 4f;
        while (Time.time < endTime)
        {
            strobeImg.color = new Color(0.85f, 0.88f, 0.92f, 0.9f);  // silver
            yield return new WaitForSeconds(0.025f);
            strobeImg.color = new Color(0f, 0f, 0f, 0f);
            yield return new WaitForSeconds(0.025f);
        }

        // Sudden silence — hide strobe
        strobeImg.color = new Color(0f, 0f, 0f, 0f);

        yield return new WaitForSeconds(0.8f);

        // Show STAY / LEAVE buttons
        ShowFinalButtons(finaleCanvas);
    }

    void ShowFinalButtons(GameObject parent)
    {
        // EventSystem needed for button clicks
       if (FindObjectOfType<UnityEngine.EventSystems.EventSystem>() == null)
        {
            GameObject es = new GameObject("EventSystem");
            es.AddComponent<UnityEngine.EventSystems.EventSystem>();
            es.AddComponent<UnityEngine.InputSystem.UI.InputSystemUIInputModule>();
        }
        // STAY — black background, white text
        GameObject stayGO    = new GameObject("BtnStay");
        stayGO.transform.SetParent(parent.transform, false);
        RawImage stayImg     = stayGO.AddComponent<RawImage>();
        stayImg.color        = new Color(0f, 0f, 0f, 1f);
        RectTransform stayRT = stayImg.GetComponent<RectTransform>();
        stayRT.anchorMin     = new Vector2(0.1f, 0.04f);
        stayRT.anchorMax     = new Vector2(0.42f, 0.14f);
        stayRT.offsetMin     = Vector2.zero;
        stayRT.offsetMax     = Vector2.zero;

        GameObject stayTxtGO  = new GameObject("StayText");
        stayTxtGO.transform.SetParent(stayGO.transform, false);
        Text stayTxt          = stayTxtGO.AddComponent<Text>();
        stayTxt.text          = "— STAY —";
        stayTxt.font          = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        stayTxt.fontSize      = 28;
        stayTxt.fontStyle     = FontStyle.Bold;
        stayTxt.color         = Color.white;
        stayTxt.alignment     = TextAnchor.MiddleCenter;
        RectTransform stayTRT = stayTxtGO.GetComponent<RectTransform>();
        stayTRT.anchorMin     = Vector2.zero;
        stayTRT.anchorMax     = Vector2.one;
        stayTRT.offsetMin     = Vector2.zero;
        stayTRT.offsetMax     = Vector2.zero;

        Button stayBtn        = stayGO.AddComponent<Button>();
        stayBtn.targetGraphic = stayImg;
        stayBtn.onClick.AddListener(OnStay);

        // LEAVE — white background, black text
        GameObject leaveGO    = new GameObject("BtnLeave");
        leaveGO.transform.SetParent(parent.transform, false);
        RawImage leaveImg     = leaveGO.AddComponent<RawImage>();
        leaveImg.color        = new Color(1f, 1f, 1f, 1f);
        RectTransform leaveRT = leaveImg.GetComponent<RectTransform>();
        leaveRT.anchorMin     = new Vector2(0.58f, 0.04f);
        leaveRT.anchorMax     = new Vector2(0.9f, 0.14f);
        leaveRT.offsetMin     = Vector2.zero;
        leaveRT.offsetMax     = Vector2.zero;

        GameObject leaveTxtGO  = new GameObject("LeaveText");
        leaveTxtGO.transform.SetParent(leaveGO.transform, false);
        Text leaveTxt          = leaveTxtGO.AddComponent<Text>();
        leaveTxt.text          = "— LEAVE —";
        leaveTxt.font          = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        leaveTxt.fontSize      = 28;
        leaveTxt.fontStyle     = FontStyle.Bold;
        leaveTxt.color         = Color.black;
        leaveTxt.alignment     = TextAnchor.MiddleCenter;
        RectTransform leaveTRT = leaveTxtGO.GetComponent<RectTransform>();
        leaveTRT.anchorMin     = Vector2.zero;
        leaveTRT.anchorMax     = Vector2.one;
        leaveTRT.offsetMin     = Vector2.zero;
        leaveTRT.offsetMax     = Vector2.zero;

        Button leaveBtn        = leaveGO.AddComponent<Button>();
        leaveBtn.targetGraphic = leaveImg;
        leaveBtn.onClick.AddListener(OnLeave);
    }

    void OnStay()
    {
        // Reset everything back to round 1
        if (finaleCanvas != null) Destroy(finaleCanvas);
        currentRound = 1;
        ResetAll();
    }

    void OnLeave()
    {
        Application.Quit();
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }

    IEnumerator PlayScreech()
    {
        int sampleRate  = AudioSettings.outputSampleRate;
        int samples     = sampleRate * 6;
        float[] data    = new float[samples];
        float freq      = 4000f;

        for (int i = 0; i < samples; i++)
        {
            float amp = Mathf.Clamp01((float)i / (sampleRate * 1f));
            data[i]   = amp * Mathf.Sin(2f * Mathf.PI * freq * i / sampleRate);
        }

        AudioClip clip     = AudioClip.Create("screech", samples, 1, sampleRate, false);
        clip.SetData(data, 0);
        pitchSource.clip   = clip;
        pitchSource.volume = 0.7f;
        pitchSource.Play();

        yield return null;
    }

    RawImage CreateStick(GameObject parent, int index)
    {
        GameObject go           = new GameObject("Stick_" + index);
        go.transform.SetParent(parent.transform, false);
        RawImage img            = go.AddComponent<RawImage>();
        img.color               = limeGreen;

        RectTransform rt        = img.GetComponent<RectTransform>();
        rt.anchorMin            = new Vector2(0.5f, 0.5f);
        rt.anchorMax            = new Vector2(0.5f, 0.5f);
        rt.pivot                = new Vector2(0.5f, 0.5f);
        rt.sizeDelta            = new Vector2(12f, 120f);

        float spacing           = 40f;
        float startX            = -((5 - 1) * spacing) / 2f;
        rt.anchoredPosition     = new Vector2(startX + index * spacing, 0f);
        go.transform.rotation   = Quaternion.Euler(0f, 0f, -45f);

        return img;
    }

    void ResetAll()
    {
        shutdownStarted = false;

        StateController sc = FindObjectOfType<StateController>();
        if (sc != null) { sc.T_hold = 0f; sc.E = 0f; }

        StrobeController strobe = FindObjectOfType<StrobeController>();
        if (strobe != null)
        {
            strobe.SetRound(currentRound);
            strobe.HideAll();
        }

        AudioController audio = FindObjectOfType<AudioController>();
        if (audio != null)
        {
            audio.leftSource.Stop();
            audio.rightSource.Stop();
        }
    }
}