using System;
using System.Collections;
using UnityEngine;

using Random = UnityEngine.Random;

public static class ShakeExtensions
{
    public enum Type
    {
        Perlin,
    }

    private struct ShakeData
    {
        public Transform Transform;
        public Vector3 RestingLocalPosition;
        public float Magnitude;
        public float Duration;
        public float Frequency;
    }

    public static void Shake(this MonoBehaviour mb, Vector3 restingLocalPosition, float magnitude, float frequency, float duration)
    {
        var data = new ShakeData
        {
            Transform = mb.transform,
            RestingLocalPosition = restingLocalPosition,
            Magnitude = magnitude,
            Duration = duration,
            Frequency = frequency,
        };

        mb.StartCoroutine(PerlinShake(data));
    }

    private static IEnumerator PerlinShake(ShakeData data)
    {
        var endTime = Time.time + data.Duration;

        while (Time.time < endTime)
        {
            var seed = Time.time * data.Frequency;
            var result = new Vector2
            {
                x = Mathf.Clamp01(Mathf.PerlinNoise(seed, 0f)) - 0.5f,
                y = Mathf.Clamp01(Mathf.PerlinNoise(0f, seed)) - 0.5f,
            };
            result *= data.Magnitude;

            data.Transform.localPosition = new Vector3(result.x, result.y, data.RestingLocalPosition.z);
            yield return null;
        }

        data.Transform.localPosition = data.RestingLocalPosition;
    }
}
