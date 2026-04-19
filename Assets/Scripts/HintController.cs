using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;

public class HintController : MonoBehaviour
{
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    static void Bootstrap()
    {
        var go = new GameObject("HintController");
        DontDestroyOnLoad(go);
        go.AddComponent<HintController>();
    }

    private CanvasGroup group;
    private bool nEverPressed = false;

    void Start()
    {
        var canvasGO = new GameObject("HintCanvas");
        canvasGO.transform.SetParent(transform, false);

        Canvas canvas = canvasGO.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = 50;

        CanvasScaler scaler = canvasGO.AddComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1920, 1080);

        canvasGO.AddComponent<GraphicRaycaster>();

        group = canvasGO.AddComponent<CanvasGroup>();
        group.alpha = 0f;

        GameObject labelGO = new GameObject("HintLabel");
        labelGO.transform.SetParent(canvasGO.transform, false);

        Text txt = labelGO.AddComponent<Text>();
        txt.text = "[ hold N ]";
        txt.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        txt.fontSize = 20;
        txt.color = new Color(0.55f, 0.55f, 0.55f, 1f);
        txt.alignment = TextAnchor.LowerRight;

        RectTransform rt = labelGO.GetComponent<RectTransform>();
        rt.anchorMin = new Vector2(0f, 0f);
        rt.anchorMax = new Vector2(1f, 1f);
        rt.offsetMin = Vector2.zero;
        rt.offsetMax = new Vector2(-24f, -20f);

        StartCoroutine(HintLifecycle());
    }

    IEnumerator HintLifecycle()
    {
        // Wait for intro to finish (timeScale goes to 1)
        yield return new WaitUntil(() => Time.timeScale > 0f);

        // Fade in
        yield return Fade(0f, 1f, 1.5f);

        // Wait until user presses N or 45 seconds pass
        float elapsed = 0f;
        while (!nEverPressed && elapsed < 45f)
        {
            if (Keyboard.current != null && Keyboard.current.nKey.isPressed)
                nEverPressed = true;
            elapsed += Time.deltaTime;
            yield return null;
        }

        // Fade out
        yield return Fade(1f, 0f, 2f);
        Destroy(gameObject);
    }

    IEnumerator Fade(float from, float to, float duration)
    {
        float t = 0f;
        while (t < duration)
        {
            group.alpha = Mathf.Lerp(from, to, t / duration);
            t += Time.deltaTime;
            yield return null;
        }
        group.alpha = to;
    }
}
