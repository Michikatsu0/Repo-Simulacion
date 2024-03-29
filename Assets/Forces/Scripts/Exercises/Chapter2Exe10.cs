using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chapter2Exe10 : MonoBehaviour
{
    // Geometry defined in the inspector.
    [SerializeField] float ceilingY;
    [SerializeField] float floorY;
    [SerializeField] float leftWallX;
    [SerializeField] float rightWallX;
    [SerializeField] Transform moverSpawnTransform;
    // Create a list of Movers 
    List<Mover2_E10> movers = new List<Mover2_E10>();
    Attractor2_E10 attractor;
    // Start is called before the first frame update
    void Start()
    {
        // Create copys of our mover and add them to our list
        while (movers.Count < 30)
        {
            // Instantiate the movers at random vectors from the left to the right wall and from our floor to ceiling.
            moverSpawnTransform.position = new Vector2(UnityEngine.Random.Range(leftWallX, rightWallX), UnityEngine.Random.Range(floorY, ceilingY));
            movers.Add(new Mover2_E10(moverSpawnTransform.position, leftWallX, rightWallX, ceilingY, floorY));
        }
        attractor = new Attractor2_E10();
    }

    void Update()
    {
        Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        attractor.UpdatePosition(mousePos);
        foreach(Mover2_E10 mover in movers)
        {
            Vector2 attractionForce = attractor.Attract(mover.body);
            mover.ApplyForce(attractionForce);
            mover.CheckEdges();
        }
    }

    void FixedUpdate()
    {
        // We want to create two groups of movers to iterate through.
        // We do this so that a mover never tries to attract itself
        for (int i = 0; i < movers.Count; i++)
        {
            for (int j = 0; j < movers.Count; j++)
            {
                if (i != j)
                {
                    // Now that we are sure that our Mover will not attract itself, we need it to attract a different Mover
                    // We do that by directing a mover to use their Attract() method on another mover Rigidbodys
                    Vector2 attractedMover = movers[j].Repel(movers[i].body);

                    // We then apply that force the mover with the Rigidbody's Addforce() method
                    movers[i].body.AddForce(attractedMover, ForceMode.Impulse);
                }
            }
            // Now we check the boundaries of our scene to make sure the movers don't fly off
            // When we use gravity, the Movers will naturally fall out of the camera's view
            // This stops that.
            movers[i].CheckEdges();
        }
    }
}

public class Attractor2_E10
{
    private float radius;
    private float mass;
    private float G;

    private Vector2 location;

    private Rigidbody body;
    public GameObject attractor;

    public Attractor2_E10()
    {
        attractor = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        Object.Destroy(attractor.GetComponent<SphereCollider>());
        Renderer renderer = attractor.GetComponent<Renderer>();

        body = attractor.AddComponent<Rigidbody>();

        body.position = Vector3.zero;

        // Generate a radius
        radius = 4;

        // Place our mover at the specified spawn position relative
        // to the bottom of the sphere
        attractor.transform.position = body.position;

        // The default diameter of the sphere is one unit
        // This means we have to multiple the radius by two when scaling it up
        attractor.transform.localScale = radius * Vector3.one;

        // We need to calculate the mass of the sphere.
        // Assuming the sphere is of even density throughout,
        // the mass will be proportional to the volume.
        body.mass = (4f / 3f) * Mathf.PI * radius * radius * radius;
        body.useGravity = false;
        body.isKinematic = true;

        renderer.material = new Material(Shader.Find("Diffuse"));
        renderer.material.color = Color.red;

        G = 9.8f;
    }

    public void UpdatePosition(Vector2 mousePos)
    {
        attractor.transform.position = mousePos;
    }

    public Vector2 Attract(Rigidbody m)
    {
        Vector2 force = body.position - m.position;
        float distance = force.magnitude;

        // Remember we need to constrain the distance so that our circle doesn't spin out of control
        distance = Mathf.Clamp(distance, 5f, 25f);

        force.Normalize();
        float strength = (distance * distance) / (G * body.mass * m.mass);
        force *= strength;
        return force;
    }

}


public class Mover2_E10
{
    public Rigidbody body;
    private GameObject gameObject;
    private float radius;

    private float xMin;
    private float xMax;
    private float yMin;
    private float yMax;

    public Mover2_E10(Vector3 position, float xMin, float xMax, float yMin, float yMax)
    {
        this.xMin = xMin;
        this.xMax = xMax;
        this.yMin = yMin;
        this.yMax = yMax;

        // Create the components required for the mover
        gameObject = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        body = gameObject.AddComponent<Rigidbody>();

        //We need to create a new material for WebGL
        Renderer r = gameObject.GetComponent<Renderer>();
        r.material = new Material(Shader.Find("Diffuse"));

        // Remove functionality that come with the primitive that we don't want
        gameObject.GetComponent<SphereCollider>().enabled = false;
        UnityEngine.Object.Destroy(gameObject.GetComponent<SphereCollider>());

        // Generate random properties for this mover
        radius = UnityEngine.Random.Range(0.1f, .4f);

        // Place our mover at the specified spawn position relative
        // to the bottom of the sphere
        gameObject.transform.position = position + Vector3.up * radius;

        // The default diameter of the sphere is one unit
        // This means we have to multiple the radius by two when scaling it up
        gameObject.transform.localScale = 2 * radius * Vector3.one;

        // We need to calculate the mass of the sphere.
        // Assuming the sphere is of even density throughout,
        // the mass will be proportional to the volume.
        body.mass = (4f / 3f) * Mathf.PI * radius * radius * radius;

        //Make sure the regular gravity is off
        body.useGravity = false;

        //Turn off the angular drag as well
        body.angularDrag = 0f;
    }

    public void ApplyForce(Vector2 force)
    {
        body.AddForce(force, ForceMode.Force);
    }

    public Vector2 Repel(Rigidbody m)
    {
        Vector2 force = body.position - m.position;
        float distance = force.magnitude;

        // Remember we need to constrain the distance so that our circle doesn't spin out of control
        distance = Mathf.Clamp(distance, 5f, 25f);

        force.Normalize();
        float strength = (9.81f * body.mass * m.mass) / (distance * distance);
        force *= -strength;
        return force;
    }

    //Checks to ensure the body stays within the boundaries
    public void CheckEdges()
    {
        Vector2 velocity = body.velocity;
        if (body.position.x > xMax || body.position.x < xMin)
        {
            velocity.x *= -1 * Time.deltaTime;
        }
        if (body.position.y > yMax || body.position.y < yMin)
        {
            velocity.y *= -1 * Time.deltaTime;
        }
        body.velocity = velocity;
    }
}