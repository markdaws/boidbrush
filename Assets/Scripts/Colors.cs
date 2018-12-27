using UnityEngine;
using System.Collections;

public class Colors {

  // Vary the base color by some random amount
  public static Color Next1(Color color) {
    float h, s, v;
    Color.RGBToHSV(color, out h, out s, out v);

    float hueRange = 0.1f;
    float hueRandom = -0.5f * hueRange + Randomness.Value() * hueRange;

    float saturationRange = 0.5f;
    float saturationRandom = -0.5f * hueRange + Randomness.Value() * saturationRange;

    float newH = Mathf.Min(1.0f, Mathf.Max(0.0f, h + hueRandom));
    float newS = Mathf.Min(1.0f, Mathf.Max(0.0f, s + saturationRandom));

    return Color.HSVToRGB(newH, newS, v);
  }
}
