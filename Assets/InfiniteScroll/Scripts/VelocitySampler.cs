using System;
using UnityEngine;

namespace Rabbit.UI.Rabbit.UI
{
    [Serializable]
    public sealed class VelocitySampler
    {
        [SerializeField] private int count;
        [SerializeField] private AnimationCurve weight;

        private Sample[] samples;
        private int index;
        private int validSampleCount;

        private struct Sample
        {
            public float time;
            public float value;
        }

        public void Init()
        {
            samples = new Sample[count];
            index = 0;
            validSampleCount = 0;
        }

        public void PushSample(float time, float value)
        {
            if(samples == null) Init();
            
            index = TakeMod(index + 1, count);
            samples[index] = new Sample { time = time, value = value };

            if (validSampleCount < count)
                validSampleCount++;
        }

        public float GetSample()
        {
            if(samples == null) Init();
            
            var mapped = index;
            var result = 0f;
            for (var i = 0; i < validSampleCount; i++)
            {
                mapped = TakeMod(mapped - 1, count);
                result += weight.Evaluate(Time.timeSinceLevelLoad - samples[mapped].time) * samples[mapped].value;
            }

            return result / validSampleCount;
        }

        private static int TakeMod(int index, int cnt)
        {
            if (cnt == 0)
                return index;
            if (index >= cnt)
            {
                index -= (index / cnt) * cnt;
            }
            else if (index < 0)
            {
                var extra = ((-index - 1) / cnt + 1) * cnt;
                index += extra;
            }

            return index;
        }
    }
}