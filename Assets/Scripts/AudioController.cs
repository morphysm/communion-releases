using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class AudioController : MonoBehaviour
{
    [Header("Dependencies")]
    public StateController state;
    public PhaseController phase;

    [Header("Audio Sources")]
    public AudioSource leftSource;
    public AudioSource rightSource;

    [Header("Frequency - Deep Mode (Phase 1-2)")]
    public float deepBase = 40f;
    public float deepBeat = 2f;

    [Header("Frequency - Theta Mode (Phase 4-5)")]
    public float thetaBase = 55f;
    public float thetaBeat = 4f;

    [Range(0f, 1f)] public float masterVolume = 0.4f;

    private float sampleRate;
    private float currentBaseFreq;
    private float currentBeatFreq;

    public float GetLeftFreq()   => currentBaseFreq;
    public float GetRightFreq()  => currentBaseFreq + currentBeatFreq;
    public float GetSampleRate() => sampleRate;

    void Start()
    {
        sampleRate      = AudioSettings.outputSampleRate;
        currentBaseFreq = deepBase;
        currentBeatFreq = deepBeat;

        leftSource.clip  = MakeSilentClip();
        rightSource.clip = MakeSilentClip();
        leftSource.loop  = true;
        rightSource.loop = true;
        leftSource.Play();
        rightSource.Play();
    }

    void Update()
    {
        float g     = phase.GlobalProgress();
        float blend = Mathf.InverseLerp(0.08f, 0.5f, g);

        currentBaseFreq = Mathf.Lerp(deepBase,  thetaBase, blend);
        currentBeatFreq = Mathf.Lerp(deepBeat,  thetaBeat, blend);

        float vol = Mathf.Pow(state.E, 2f) * masterVolume;
        leftSource.volume  = vol;
        rightSource.volume = vol;
    }

    AudioClip MakeSilentClip()
    {
        int len        = (int)sampleRate;
        AudioClip clip = AudioClip.Create("silence", len, 1, (int)sampleRate, false);
        clip.SetData(new float[len], 0);
        return clip;
    }
}