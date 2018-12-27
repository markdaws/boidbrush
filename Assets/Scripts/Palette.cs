using UnityEngine;
using System.Collections;

public class Palette {
  private Color[] colors;

  public Palette(string[] colors) {
    this.colors = new Color[colors.Length];
    for (int i = 0; i < colors.Length; ++i) {
      Color color;
      ColorUtility.TryParseHtmlString(colors[i], out color);
      this.colors[i] = color;
    } 
  }

  public void Randomize() {
    int currentIndex = this.colors.Length;
    Color temporaryValue;
    int randomIndex;

    while (currentIndex != 0) {
      randomIndex = Mathf.FloorToInt(Random.value * currentIndex);
      currentIndex -= 1;

      // And swap it with the current element.
      temporaryValue = this.colors[currentIndex];
      this.colors[currentIndex] = this.colors[randomIndex];
      this.colors[randomIndex] = temporaryValue;
    }  
  }

  public Color Item(int index) {
    return this.colors[index];
  }
}
