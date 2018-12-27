using UnityEngine;

public class App: MonoBehaviour {
  private BoidManager boidManager;
  private Brush brush;
  private Palette palette;

  public GameObject BoidPrefab;
  public GameObject RepellorPrefab;
  public GameObject AttractorPrefab;

  void Start() {
    this.boidManager = new BoidManager {
      BoidPrefab = this.BoidPrefab
    };
    this.Reset();

    // TODO: Remove
    this.SetBoidBrush();
    this.SetSphereBrush();
  }

  void Update() {
    if (this.brush != null) {
      this.brush.Update();
    }
    this.boidManager.Update();
  }

  public void SetSphereBrush() {
    this.SetBrush(new SphereBrush(this.boidManager, this.palette));
    this.palette.Randomize();
  }

  public void SetBoidBrush() {
    this.SetBrush(new BoidBrush(this.boidManager, this.palette));
    this.palette.Randomize();
  }

  public void Reset() {
    this.palette = Palettes.RandomPalette();
    this.palette = Palettes.StaryNight;
    this.palette.Randomize();
    Camera.main.backgroundColor = this.palette.Item(0);

    if (this.brush != null) {
      this.brush.SetPalette(this.palette);
    }
    this.boidManager.Reset();
  }

  private void SetBrush(Brush _brush) {
    if (this.brush != null) {
      this.brush.Deactivate();
    }
    this.brush = _brush;
    this.brush.Activate();
  }

}
