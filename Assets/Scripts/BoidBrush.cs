using UnityEngine;
using UnityEngine.EventSystems;

public class BoidBrush: Brush {
  private float lastBoidSpawnTime;
  private float boidPerSecond = 40.0f;
  private BoidManager boidManager;
  private Vector3 lastMousePos;
  private bool isDrawing;
  private Palette palette;

  public BoidBrush(BoidManager boidManager, Palette palette) {
    this.boidManager = boidManager;
    this.palette = palette;
  }

  public override void Update() {
    bool clickingResetButton = EventSystem.current.IsPointerOverGameObject();
    if (clickingResetButton) {
      return;
    }

    Vector3 mousePos = Input.mousePosition;
    if (!this.isDrawing) {
      if (Input.GetMouseButtonDown(0)) {
        this.StartStroke(mousePos);
      }
    } else {
      if (Input.GetMouseButtonUp(0)) {
        this.EndStroke();
      }
    }
    
    if (!(Input.GetMouseButton(0) || Input.touchCount > 0)) {
      return;
    }

    if (this.isDrawing) {
      Vector3 mouseDelta = mousePos - this.lastMousePos;

      float currentTime = Time.time;
      if (currentTime - lastBoidSpawnTime > (1.0f / this.boidPerSecond)) {
        lastBoidSpawnTime = currentTime;
        if (Input.touchCount > 0) {
          mousePos = Input.GetTouch(0).position;
        }

        Vector3 initialVelocity = mouseDelta;
        initialVelocity.Normalize();

        // TODO: What should this value be set to?
        initialVelocity *= 5.0f;
        //Color color = this.palette.Item(1 + Mathf.FloorToInt(Random.value * 4));
        Color color = Colors.Next1(this.palette.Item(1));
        this.boidManager.AddBoidAtScreenPos(mousePos, initialVelocity, color);
      }
    }

    this.lastMousePos = mousePos;
  }

  private void StartStroke(Vector3 mousePos) {
    this.isDrawing = true;
    this.lastMousePos = mousePos;
  }

  private void EndStroke() {
    this.isDrawing = false;
  }

  public override void SetPalette(Palette palette) {
    this.palette = palette;
  }

}
