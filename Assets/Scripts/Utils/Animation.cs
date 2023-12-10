using System.Collections;
using UnityEngine;

namespace Animation {
    public enum Ease {
        Linear,

        IN_CUBIC,
        OUT_CUBIC,
        IN_OUT_CUBIC,
    }

    public static class EaseUtil {
        public static float Ease(float Current, float Duration, Ease ease) {
            float t = Current / Duration;

            switch(ease) {
                case Animation.Ease.IN_CUBIC: t = t*t*t; break;
                case Animation.Ease.OUT_CUBIC: t = 1 - Mathf.Pow(1 - t, 3); break;
                case Animation.Ease.IN_OUT_CUBIC: if (t < 0.5f) t = 4*t*t*t; else t = 1-Mathf.Pow(-2*t+2,3)/2; break;
            }

            return t;
        }
    }
}


