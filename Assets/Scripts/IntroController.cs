using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.UI;
using System.Collections;

public class IntroController : MonoBehaviour
{
    private GameObject introCanvas;
    private Coroutine vanishRoutine;
    private bool introComplete = false;

    private readonly Color c64White = new Color(1f, 1f, 1f, 1f);
    private readonly Color c64Dim   = new Color(0.6f, 0.6f, 0.6f, 1f);
    private readonly Color c64Black = new Color(0f, 0f, 0f, 1f);
    private readonly Color stickRed = new Color(0.102f, 1f, 0.102f, 1f);

    void Start()
    {
        // Enforce windowed mode at launch
        Screen.fullScreen = false;
        
        Time.timeScale = 0f;
        BuildIntro();
    }

    void BuildIntro()
    {
        introCanvas = new GameObject("IntroCanvas");
        Canvas canvas = introCanvas.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = 100;
        
        // Ensure UI scales properly with window resizing
        CanvasScaler scaler = introCanvas.AddComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1920, 1080);
        
        introCanvas.AddComponent<GraphicRaycaster>();

        if (Object.FindAnyObjectByType<EventSystem>() == null)
        {
            GameObject es = new GameObject("EventSystem");
            es.AddComponent<EventSystem>();
            es.AddComponent<InputSystemUIInputModule>();
        }

        // Background
        CreateElement("IntroBG", (rt) => {
            RawImage bg = rt.gameObject.AddComponent<RawImage>();
            bg.color = c64Black;
            rt.anchorMin = Vector2.zero;
            rt.anchorMax = Vector2.one;
            rt.offsetMin = rt.offsetMax = Vector2.zero;
        });

        // Title
        CreateElement("Title", (rt) => {
            Text txt = rt.gameObject.AddComponent<Text>();
            txt.text = "COMMUNION";
            txt.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            txt.fontSize = 72;
            txt.color = c64White;
            txt.alignment = TextAnchor.MiddleCenter;
            rt.anchorMin = new Vector2(0f, 0.62f);
            rt.anchorMax = new Vector2(1f, 0.82f);
            rt.offsetMin = rt.offsetMax = Vector2.zero;
        });

        // Tagline
        CreateElement("Tagline", (rt) => {
            Text txt = rt.gameObject.AddComponent<Text>();
            txt.text = "Binaural Desynchronization for Nonhuman Contact.";
            txt.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            txt.fontSize = 22;
            txt.color = c64Dim;
            txt.alignment = TextAnchor.MiddleCenter;
            rt.anchorMin = new Vector2(0f, 0.52f);
            rt.anchorMax = new Vector2(1f, 0.62f);
            rt.offsetMin = rt.offsetMax = Vector2.zero;
        });

        // Sticks
        for (int i = 0; i < 5; i++)
        {
            int index = i;
            CreateElement("Stick_" + i, (rt) => {
                rt.gameObject.AddComponent<RawImage>().color = stickRed;
                rt.anchorMin = rt.anchorMax = new Vector2(0.5f, 0.5f);
                rt.sizeDelta = new Vector2(10f, 100f);
                rt.anchoredPosition = new Vector2(-72f + (index * 36f), -60f);
                rt.localRotation = Quaternion.Euler(0, 0, -45);
            });
        }
        
        vanishRoutine = StartCoroutine(VanishSticks());

        // Button
        CreateElement("EnterBtn", (rt) => {
            rt.gameObject.AddComponent<RawImage>().color = new Color(0, 0, 0, 0.01f);
            rt.anchorMin = new Vector2(0.25f, 0.2f);
            rt.anchorMax = new Vector2(0.75f, 0.3f);
            rt.offsetMin = rt.offsetMax = Vector2.zero;
            rt.gameObject.AddComponent<Button>().onClick.AddListener(OnEnter);

            GameObject label = new GameObject("Label");
            label.transform.SetParent(rt, false);
            Text t = label.AddComponent<Text>();
            t.text = "[ ENTER COMMUNION ]";
            t.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            t.fontSize = 28;
            t.alignment = TextAnchor.MiddleCenter;
            RectTransform lRT = label.GetComponent<RectTransform>();
            lRT.anchorMin = Vector2.zero;
            lRT.anchorMax = Vector2.one;
            lRT.offsetMin = lRT.offsetMax = Vector2.zero;
        });
    }

    void CreateElement(string name, System.Action<RectTransform> action)
    {
        GameObject go = new GameObject(name);
        go.transform.SetParent(introCanvas.transform, false);
        action(go.AddComponent<RectTransform>());
    }

    IEnumerator VanishSticks()
    {
        while (!introComplete)
        {
            for (int i = 0; i < 5; i++)
            {
                Transform s = introCanvas.transform.Find("Stick_" + i);
                if (s) s.gameObject.SetActive(true);
            }

            for (int i = 4; i >= 0; i--)
            {
                yield return new WaitForSecondsRealtime(0.94f);
                Transform s = introCanvas.transform.Find("Stick_" + i);
                if (s) s.gameObject.SetActive(false);
            }
            yield return new WaitForSecondsRealtime(0.85f);
        }
    }

    void Update()
    {
        if (!introComplete && Keyboard.current != null &&
            (Keyboard.current.enterKey.wasPressedThisFrame || Keyboard.current.numpadEnterKey.wasPressedThisFrame))
            OnEnter();
    }

    void OnEnter()
    {
        if (introComplete) return;
        introComplete = true;
        
        if (vanishRoutine != null) StopCoroutine(vanishRoutine);
        
        Time.timeScale = 1f;
        Destroy(introCanvas);
    }
}