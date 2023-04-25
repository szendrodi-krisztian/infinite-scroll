using System;
using UnityEngine;

namespace Rabbit.Utils

{
    [Serializable]
    public sealed class VelocitySampler
    {
        [SerializeField] private int count;
        [SerializeField] private AnimationCurve weight;

        private int index;

        private Sample[] samples;
        private int validSampleCount;

        private static Sample[] Init(int count)
        {
            var samples = new Sample[count];
            return samples;
        }

        public void PushSample(float time, float value)
        {
            samples ??= Init(count);

            index = TakeMod(index + 1, count);

            samples[index] = new Sample(time, value);

            if (validSampleCount < count)
                validSampleCount++;
        }

        public float GetSample()
        {
            samples ??= Init(count);

            var mapped = index;
            var result = 0f;

            for (var i = 0; i < validSampleCount; i++)
            {
                mapped = TakeMod(mapped - 1, count);
                result += weight.Evaluate(Time.timeSinceLevelLoad - samples[mapped].Time) * samples[mapped].Value;
            }

            return result / validSampleCount;
        }

        private static int TakeMod(int index, int cnt)
        {
            if (cnt == 0)
                return index;

            if (index >= cnt)
            {
                index -= index / cnt * cnt;
            }
            else if (index < 0)
            {
                var extra = ((-index - 1) / cnt + 1) * cnt;
                index += extra;
            }

            return index;
        }

        private struct Sample
        {
            public Sample(float time, float value)
            {
                Time = time;
                Value = value;
            }

            public float Time { get; }
            public float Value { get; }
        }
    }
}