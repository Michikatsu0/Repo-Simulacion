using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chapter3Exe4 : MonoBehaviour
{
    [SerializeField] Material tracerMaterial;
    float r = 0;
    float theta = 45;

    private GameObject sphere;
    private MeshRenderer sphereRenderer;

    //Create variables for rendering the line between two vectors
    private GameObject lineDrawing;
    private LineRenderer lineRender;

    // Start is called before the first frame update
    void Start()
    {
        //Set the camera to orthographic and make its size 8
        Camera.main.orthographic = true;
        Camera.main.orthographicSize = 20;

        // Create a GameObject that will be the line
        lineDrawing = new GameObject();

        // Make the sphere as a primitive sphere type.
        sphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        var tracer = sphere.AddComponent<TrailRenderer>();

        tracer.startWidth = 1f;
        tracer.endWidth = 0;
        tracer.material=tracerMaterial;



        tracer.startColor = Color.red;
        tracer.endColor = Color.yellow;
        tracer.time = 0.3f;
        tracer.emitting = true;

        //We need to create a new material for WebGL
        sphereRenderer = sphere.GetComponent<MeshRenderer>();
        sphereRenderer.material = new Material(Shader.Find("Diffuse"));

        //Add the Unity Component "LineRenderer" to the GameObject lineDrawing. We will see a black line.
        lineRender = lineDrawing.AddComponent<LineRenderer>();

        //Make the line smaller for aesthetics
        lineRender.startWidth = 0.1f;
        lineRender.endWidth = 0.1f;

        //We need to create a new material for WebGL
        lineRender.material = new Material(Shader.Find("Diffuse"));
    }

    // Update is called once per frame
    void Update()
    {
        r += Time.deltaTime;
        float x = r * Mathf.Cos(theta);
        float y = r * Mathf.Sin(theta);

        sphere.transform.position = new Vector2(x, y);

        theta += 1f * Time.deltaTime; //Time.deltaTime to keep the speed consistant

        //Begin rendering the line between the two objects. Set the first point (0) at the centerSphere Position
        //Make sure the end of the line (1) appears at the new Vector3
        Vector2 center = new Vector2(0f, 0f);
        lineRender.SetPosition(0, center);
        lineRender.SetPosition(1, sphere.transform.position);
    }

}
