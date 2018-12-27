using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boid {

  private Vector3 velocity;
  private GameObject gameObject;
  private Material material;

  public Boid(GameObject prefab, Vector3 position, Vector3 velocity, Color color) {
   // this.gameObject = GameObject.CreatePrimitive(PrimitiveType.Sphere);
    this.gameObject = GameObject.Instantiate(prefab);
    this.material = this.gameObject.GetComponent<Renderer>().material;
    this.SetRenderColor(color);
    this.Color = color;
    this.Velocity = velocity;
    this.Position = position;
  }

  public void Destroy() {
    GameObject.Destroy(this.gameObject);
  }

  public Color Color { get; private set; }

  public void SetRenderColor(Color color) {
    this.gameObject.transform.GetChild(0).GetComponent<MeshRenderer>().material.color = color;
    this.gameObject.transform.GetChild(1).GetComponent<MeshRenderer>().material.color = color;
    this.material.color = color;
  }

  public float Scale {
    set {
      this.gameObject.transform.localScale = new Vector3(value, value, value);
    }
  }

  public Vector3 Position { 
    get {
      return this.gameObject.transform.position;
    }
    set {
      this.gameObject.transform.position = value;
    }
  }

  public Vector3 Velocity { 
    get {
      return this.velocity;
    }
    set {
      this.velocity = value;
      this.velocity.z = 0.0f;
      Vector3 direction = this.velocity.normalized;
      this.gameObject.transform.rotation = Quaternion.FromToRotation(Vector3.forward, direction);
    }
  }

  // Returns true if the boid is in a sphere constraint
  public bool SphereConstraint { get; set; }

  public bool Fixed { get; set; }
  // SphereBrush values
  public Vector3 SBCenter { get; set; }

  // ttl?
}
