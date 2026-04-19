using UnityEngine;

public class AudioFilterLeft : MonoBehaviour
{
    public AudioController controller;

    private double phase = 0.0;

    void OnAudioFilterRead(float[] data, int channels)
    {
        if (controller == null) return;

        double increment = controller.GetLeftFreq() / controller.GetSampleRate();

        for (int i = 0; i < data.Length; i += channels)
        {
            float s1 = (float)System.Math.Sin(2.0 * System.Math.PI * phase);
            float s2 = (float)System.Math.Sin(2.0 * System.Math.PI * phase * 0.5);
            float sample = (s1 * 0.5f + s2 * 0.5f) * 0.6f;

            for (int c = 0; c < channels; c++)
                data[i + c] = sample;

            phase = (phase + increment) % 1.0;
        }
    }
}