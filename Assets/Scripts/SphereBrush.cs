using UnityEngine;
using UnityEngine.EventSystems;

public class SphereBrush: Brush {
  private bool isDrawing = false;
  private Vector3 mouseDownPoint = Vector3.zero;
  private float radius = 1.0f;
  private Palette palette;
  private BoidManager boidManager;
  private float lastBoidSpawnTime;
  private float boidPerSecond = 20.0f;

  public SphereBrush(BoidManager boidManager, Palette palette) {
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

    if (!isDrawing) {
      return;
    }

    float currentTime = Time.time;
    if (currentTime - lastBoidSpawnTime > (1.0f / this.boidPerSecond)) {
      lastBoidSpawnTime = currentTime;
      if (Input.touchCount > 0) {
        mousePos = Input.GetTouch(0).position;
      }

      Vector3 initialVelocity = Vector3.zero; //mouseDelta;
                                              //initialVelocity.Normalize();

      // TODO: What should this value be set to?
      //initialVelocity *= 5.0f;

      Vector3 insertPos = this.boidManager.ScreenToWorld(mousePos);
      Vector3 worldDownPos = this.boidManager.ScreenToWorld(this.mouseDownPoint);
      if ((insertPos - worldDownPos).magnitude < 0.5f) {
        return;
      }

      //Color color = this.palette.Item(1 + Mathf.FloorToInt(Random.value * 4));
      Color color = Colors.Next1(this.palette.Item(1));

      Boid boid = this.boidManager.AddBoid(insertPos, initialVelocity, color);

     
      boid.SphereConstraint = true;
      boid.SBCenter = worldDownPos;
      boid.Scale = 0.5f;
      boid.Fixed = true;
      //Debug.Log(worldDownPos);
    }
  }

  private void StartStroke(Vector3 mousePos) {
    this.isDrawing = true;
    this.mouseDownPoint = mousePos;
    Debug.Log("Start drawing");
    Debug.Log(mousePos);
  }

  private void EndStroke() {
    this.isDrawing = false;

    Debug.Log("Ending stroke");
  }

  public override void SetPalette(Palette palette) {
    this.palette = palette;
  }

  Vector3 CircleConstraint(Vector3 center, Boid boid) {
    //Vector3 center = new Vector3(0,0, this.distanceFromCamera);

    // Need to keep a constant distance from the center each iteration
    // What about velocity magnitude, should be a function of ditance from the center
    // Is there randomness added each frame
    // How to pick colors

    Vector3 delta = boid.Position - center;
    delta.Normalize();
    Vector3 v = new Vector3(delta.y, -delta.x, 0);
    v.Normalize();

    // TODO:
    return v *= 5;
  }
}