
using UnityEngine;

public static class Randomness { 

  public static float Value() {
    // TODO: Use a seed for reproducibility
    return Random.value;
  }

  public static Vector3 TwoDWithMagnitude(float magnitude) {
    Vector3 v = new Vector3(
      (-1.0f + Randomness.Value() * 2.0f),
      (-1.0f + Randomness.Value() * 2.0f),
      0.0f
    );
    v.Normalize();
    v *= magnitude;
    return v;
  }
}
