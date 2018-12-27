using System.Collections.Generic;
using UnityEngine;
using Unity;
using UnityEngine.EventSystems;

public class BoidManager {

  public GameObject BoidPrefab;
  public GameObject RepellorPrefab;
  public GameObject AttractorPrefab;

  private List<Boid> boids = new List<Boid>();
  private List<Boid> attractors = new List<Boid>();
  private List<Boid> repellors = new List<Boid>();

  private int initialBoidCount = 0;

  private float distanceFromCamera = 30.0f;

  private float timeToCrossScreen = 4.0f;

  // Calculated dynamically
  private float maxBoidSpeed;
  private float minBoidSpeed;

  // The width of the plane that the boid are rendered on
  private float planeWidth;
  private float xMin, xMax, yMin, yMax;

  private float cohensionRadiusPercentage = 0.25f;
  private float cohensionMultiplier = 0.08f;

  private float alignmentRadiusPercentage = 0.25f;
  private float alignmentMultiplier = 0.02f;

  private float separartionRadiusPercentage = 0.05f;
  private float separationMultiplier = 1.4f;

  private float colorRadiusPercentage = 0.1f;

  private float avoidRadiusPercentage = 0.2f;
  private float avoidMultiplier = 5.0f;

  private float boundsMultiplier = 1.5f;//0.8f;

  private float randomnessMultiplier = 0.4f;

  public void Reset() {

    for (int i = 0; i < this.boids.Count; ++i) {
      this.boids[i].Destroy();
    }
    this.boids.Clear();
    for (int i = 0; i < this.repellors.Count; ++i) {
      this.repellors[i].Destroy();
    }
    this.repellors.Clear();

    this.GenerateBoids();
    //this.GenerateAttractors();
    this.GenerateRepellors(); 
  }

  public void AddRepellor() {
    Vector3 position = new Vector3(
      Random.value,
      Random.value,
      this.distanceFromCamera
    );
    Vector3 velocity = new Vector3(
      (-1.0f + Random.value * 2.0f),
      (-1.0f + Random.value * 2.0f),
      0.0f
    );
    velocity.Normalize();
    velocity *= this.maxBoidSpeed;

    Color color = Color.red;//this.palette.Item(1 + Mathf.FloorToInt(Random.value * 4));
    repellors.Add(new Boid(this.BoidPrefab, position, velocity, color));
  }

  private void GenerateBoids() {
    Camera mainCamera = Camera.main;

    // Calculate the distance from the left to the right side of the plane
    // where the boid are rendered
    Vector3 left = mainCamera.ViewportToWorldPoint(new Vector3(0, 0.5f, this.distanceFromCamera));
    Vector3 right = mainCamera.ViewportToWorldPoint(new Vector3(1, 0.5f, this.distanceFromCamera));
    Vector3 top = mainCamera.ViewportToWorldPoint(new Vector3(0, 1.0f, this.distanceFromCamera));
    Vector3 bottom = mainCamera.ViewportToWorldPoint(new Vector3(0, 0.0f, this.distanceFromCamera));
    xMin = left.x;
    xMax = right.x;
    yMin = bottom.y;
    yMax = top.y;
    this.planeWidth = (right - left).magnitude;

    // Now we know the distance we can specify the maximum time it takes for
    // a boid to travel across the surface, to set an initial velocity
    this.maxBoidSpeed = this.planeWidth / this.timeToCrossScreen;
    this.minBoidSpeed = this.maxBoidSpeed * 0.5f;

    for (int i = 0; i < this.initialBoidCount; ++i) {
      //this.AddBoid(
      //  new Vector3(Randomness.Value(), Randomness.Value(), this.distanceFromCamera), 
      //  Randomness.TwoDWithMagnitude(this.maxBoidSpeed));
    }

    //this.repellors.Add(new Boid(this.RepellorPrefab, new Vector3(0, 0, this.distanceFromCamera), Vector3.zero));
  }

  public Boid AddBoidAtScreenPos(
    Vector3 screenPos,
    Vector3 velocity,
    Color color
    ) {

    Vector3 worldPos = this.ScreenToWorld(screenPos);
    return this.AddBoid(worldPos, velocity, color);
  }

  public Vector3 ScreenToWorld(Vector3 screenPos) {
    Vector3 worldPos = screenPos;
    worldPos.z = this.distanceFromCamera;
    worldPos = Camera.main.ScreenToWorldPoint(worldPos);
    return worldPos;
  }

  public void AddBoid(Boid boid) {
    this.boids.Add(boid);
  }

  public Boid AddBoid(Vector3 position, Vector3 velocity, Color color) {
    Boid boid = new Boid(this.BoidPrefab, position, velocity, color);
    boids.Add(boid);
    return boid;
  }

  private void GenerateAttractors() {
    //attractors.Add(new Boid(new Vector3(0, 0, 20), Vector3.zero));
    //attractors.Add(new Boid(new Vector3(10, 10, 20), Vector3.zero));
    //attractors.Add(new Boid(new Vector3(-5, -5, 20), Vector3.zero));
  }

  private void GenerateRepellors() {
  }

  private Vector3 mouseDownPoint = Vector3.zero;


  public void Update () {

    float deltaTime = Time.deltaTime;

    if (Input.GetMouseButtonDown(0)) {
      mouseDownPoint = Input.mousePosition;
      mouseDownPoint.z = this.distanceFromCamera;
      mouseDownPoint = Camera.main.ScreenToWorldPoint(mouseDownPoint);
    }

    bool invertCohesion = Input.GetMouseButton(0);
    bool invertAlignment = Input.GetMouseButton(1);

    for (int i = 0; i < this.boids.Count; ++i) {
      
      // Rules:
      // 1. Cohesion, birds try to fly to center of mass of neighbouring boids
      // 2. Separation, birds try to keep a small distance away
      // 3. Alignment, birds try to fly in the same direction

      bool hasCohension = true;
      bool hasSeparation = true;
      bool hasAlignment = true;
      bool hasAttraction = false;
      bool hasRepulsion = true;
      bool hasRandomness = true;

      Boid boid = this.boids[i];

      if (boid.Fixed) {
        continue;
      }
      Vector3 cohesion = hasCohension ? this.Cohesion(boid) : Vector3.zero;
      Vector3 separation = hasSeparation ? this.Separation(boid) : Vector3.zero;
      Vector3 alignment = hasAlignment ? this.Alignment(boid) : Vector3.zero;
      Vector3 attraction = hasAttraction ? this.Attraction(boid) : Vector3.zero;
      Vector3 repulsion = hasRepulsion ? this.Repulsion(boid) : Vector3.zero;
      Vector3 randomness = hasRandomness ? this.RandomnessX(boid) : Vector3.zero;
      Vector3 boundPosition = this.BoundPosition(boid);
      Vector3 avoid = this.Avoid(boid);

      if (invertCohesion) {
        cohesion *= -1.0f;
      }
      if (invertAlignment) {
        alignment *= -1.0f;
      }
      Vector3 newVelocity = boid.Velocity +
                                cohesion +
                                separation +
                                alignment +
                                attraction +
                                repulsion +
                                randomness +
                                avoid +
                                boundPosition;

      // TODO: Should this be here?
      if (boid.SphereConstraint) {
        newVelocity = this.CircleConstraint(boid);// + randomness;
      }
      Vector3 newPos = boid.Position + newVelocity * deltaTime;
      boid.Velocity = this.BoundVelocity(newVelocity);
      boid.Position = newPos;

      // TODO:
      //  boid.SetRenderColor(this.MaxColor(boid));

      //  new Boid(this.BoidPrefab, boid.Position, boid.Velocity, boid.Color);
    }

    for (int i = 0; i < this.repellors.Count; ++i) {
      Boid repellor = this.repellors[i];
      Vector3 boundPosition = this.BoundPosition(repellor);
      Vector3 randomness = this.RandomnessX(repellor);
      Vector3 newVelocity = repellor.Velocity + randomness + boundPosition;

      Vector3 newPos = repellor.Position + newVelocity * deltaTime;
      repellor.Velocity = this.BoundVelocity(newVelocity);
      repellor.Position = newPos;
    }
	}

  // TODO: Remove
  Vector3 CircleConstraint(Boid boid) {
    //Vector3 center = new Vector3(0,0, this.distanceFromCamera);

    Vector3 delta = boid.Position - boid.SBCenter;
    delta.Normalize();
    Vector3 v = new Vector3(delta.y, -delta.x, 0);
    v.Normalize();
    return v *= 5;
  }

  Color AverageColor(Boid boid) {

    Vector3 color = new Vector3(boid.Color.r, boid.Color.g, boid.Color.b);
    int numFound = 0;
    float colorRadius = this.planeWidth * this.colorRadiusPercentage;
    for (int i = 0; i < this.boids.Count; ++i) {
      Boid currentBoid = this.boids[i];
      if (boid == currentBoid) {
        continue;
      }
      Vector3 delta = currentBoid.Position - boid.Position;
      if (delta.magnitude < colorRadius) {
        color += new Vector3(currentBoid.Color.r, currentBoid.Color.g, currentBoid.Color.b);
        numFound++;
      }
    }

    if (numFound > 0) {
      color /= numFound;
    }

    return new Color(color.x, color.y, color.z);
  }

  Color MaxColor(Boid boid) {

    Vector3 color = new Vector3(boid.Color.r, boid.Color.g, boid.Color.b);
    float colorRadius = this.planeWidth * this.colorRadiusPercentage;
    for (int i = 0; i < this.boids.Count; ++i) {
      Boid currentBoid = this.boids[i];
      if (boid == currentBoid) {
        continue;
      }
      Vector3 delta = currentBoid.Position - boid.Position;
      if (delta.magnitude < colorRadius) {
        if (currentBoid.Color.r > color.x) {
          color = new Vector3(currentBoid.Color.r, currentBoid.Color.g, currentBoid.Color.b);
        }
      }
    }

    return new Color(color.x, color.y, color.z);
  }

  Vector3 Avoid(Boid boid) {
    Vector3 avoid = Vector3.zero;

    float avoidRadius = this.planeWidth * this.avoidRadiusPercentage;
    for (int i = 0; i < this.repellors.Count; ++i) {
      Boid repellor = this.repellors[i];
      Vector3 delta = boid.Position - repellor.Position;
      if (delta.magnitude < avoidRadius) {
        avoid += (delta.normalized / delta.magnitude) * this.avoidMultiplier;
        //centroid += currentBoid.Position;
        //numFound++;
      }
    }

    return avoid;
  }

  Vector3 Cohesion(Boid boid) {
    Vector3 centroid = Vector3.zero;
    int numFound = 0;

    // TODO: Can do in init
    float cohesionRadius = this.planeWidth * this.cohensionRadiusPercentage;
    for (int i = 0; i < this.boids.Count; ++i) {
      Boid currentBoid = this.boids[i];
      if (boid == currentBoid) {
        continue;
      }
      Vector3 delta = currentBoid.Position - boid.Position;
      if (delta.magnitude < cohesionRadius) {
        centroid += currentBoid.Position;
        numFound++;
      }
    }

    if (numFound > 0) {
      centroid /= numFound;
      return (centroid - boid.Position) * this.cohensionMultiplier;
    } else {
      return Vector3.zero;
    }
  }

  Vector3 Alignment(Boid boid) {
    Vector3 align = Vector3.zero;
    int numFound = 0;

    float alignmentRadius = this.planeWidth * this.alignmentRadiusPercentage;
    for (int i = 0; i < this.boids.Count; ++i) {
      Boid currentBoid = this.boids[i];
      if (currentBoid == boid) {
        continue;
      }
      Vector3 delta = currentBoid.Position - boid.Position;
      if (delta.magnitude < alignmentRadius) {
        align += currentBoid.Velocity;
        numFound++;
      }
    }

    if (numFound > 0) {
      align /= numFound;
      return align * this.alignmentMultiplier;
    } else {
      return Vector3.zero;
    }
  }

  Vector3 Separation(Boid boid) {
    Vector3 vector = Vector3.zero;

    float separationRadius = this.planeWidth * this.separartionRadiusPercentage;
    for (int i = 0; i < this.boids.Count; ++i) {
      Boid currentBoid = this.boids[i];
      if (currentBoid == boid) {
        continue;
      }
      Vector3 delta = boid.Position - currentBoid.Position;
      if (delta.magnitude < separationRadius) {
        vector += (delta.normalized / delta.magnitude);
      }
    }
    return vector * this.separationMultiplier;
  }

  Vector3 BoundPosition(Boid boid) {
    Vector3 pos = boid.Position;
    Vector3 vector = Vector3.zero;

    if (pos.x < this.xMin) {
      vector.x = this.boundsMultiplier;
    } else if (pos.x > this.xMax) {
      vector.x = -this.boundsMultiplier;
    }
    if (pos.y < this.yMin) {
      vector.y = this.boundsMultiplier;
    } else if (pos.y > this.yMax) {
      vector.y = -this.boundsMultiplier;
    }
    return vector;
  }

  Vector3 BoundVelocity(Vector3 velocity) {
    float magnitude = velocity.magnitude;
    if (magnitude > this.maxBoidSpeed) {
      return velocity.normalized * this.maxBoidSpeed;
    } else if (magnitude < this.minBoidSpeed) {
      return velocity.normalized * this.minBoidSpeed;
    }
    return velocity;
  }

  Vector3 RandomnessX(Boid boid) {
    Vector3 random = new Vector3(
      (-1.0f + Random.value * 2.0f),
      (-1.0f + Random.value * 2.0f),
      0.0f
    );
    random.Normalize();
    random *= this.randomnessMultiplier;
    return random;
  }

  Vector3 Attraction(Boid boid) {
    Vector3 vector = Vector3.zero;
    for (int i = 0; i < this.attractors.Count; ++i) {
      vector += (this.attractors[i].Position - boid.Position) / 1000.0f;
    }
    return vector;
  }

  Vector3 Repulsion(Boid boid) {
    Vector3 vector = Vector3.zero;
    for (int i = 0; i < this.repellors.Count; ++i) {
      Boid repellor = this.repellors[i];
      Vector3 delta = repellor.Position - boid.Position;
      if (delta.magnitude < 5.0f) {
        vector -= (repellor.Position - boid.Position) * 0.1f;
      }
    }
    return vector;
  }

}