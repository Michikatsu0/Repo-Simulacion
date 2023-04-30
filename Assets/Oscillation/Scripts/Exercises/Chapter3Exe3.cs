using System.Collections;
using System.Collections.Generic;
using System.Threading;
using Unity.VisualScripting;
using UnityEngine;

public class Chapter3Exe3 : MonoBehaviour
{
    Mover3_E3 mover;

    float turnSpeed = 30f;
    float moveSpeed = 10f;

    float x, y = 1;

    


    // Start is called before the first frame update
    void Start()
    {
        mover = new Mover3_E3();
    }

    // Update is called once per frame
    void Update()
    {
       

        if (Input.GetKey(KeyCode.LeftArrow))
            x = -1f;

        if (Input.GetKey(KeyCode.RightArrow))
            x = 1f;

        float angle = Mathf.Atan2(Mathf.Clamp(x,-1,1), y);

        mover.Translate(moveSpeed);
        mover.Rotate(angle, turnSpeed);
        mover.Update();
    }
}

public class Mover3_E3
{
    // The basic properties of a mover class
    public Vector2 location, velocity, acceleration;
    public float mass;

    private Vector2 maximumPos;

    public GameObject mover;

    public Mover3_E3()
    {
        mover = GameObject.CreatePrimitive(PrimitiveType.Cube);

        Renderer renderer = mover.GetComponent<Renderer>();
        renderer.material = new Material(Shader.Find("Diffuse"));
        renderer.material.color = Color.black;

        mover.transform.localScale = new Vector3(0.5f, 1, 0.5f);

        mass = 1;

        location = Vector2.zero;
        velocity = Vector2.zero;
        acceleration = Vector2.zero;

        FindWindowLimits();
    }

    public void Translate(float speed)
    {
        velocity = (Vector2)mover.transform.up * speed;
    }

    public void Rotate(float radiansAngle, float turnSpeed)
    {
        float eulerAngle = (-radiansAngle * Mathf.Rad2Deg) + 180;
        float toSpin = eulerAngle - ((mover.transform.eulerAngles.z + 180) % 360);
        if (toSpin > 180 || toSpin < -180)
        {
            toSpin %= 180;
            toSpin *= -1;
        }
        toSpin = Mathf.Clamp(toSpin, -turnSpeed, turnSpeed);
        mover.transform.Rotate(new Vector3(0, 0, toSpin));
    }

    public void Update()
    {
        velocity += acceleration * Time.deltaTime;
        location += velocity * Time.deltaTime;

        acceleration = Vector2.zero;

        mover.transform.position = location;

        CheckEdges();
    }

    public void CheckEdges()
    {
        if (location.x > maximumPos.x)
        {
            location.x = -maximumPos.x;
        }
        else if (location.x < -maximumPos.x)
        {
            location.x = maximumPos.x;
        }
        if (location.y > maximumPos.y)
        {
            location.y = -maximumPos.y;
        }
        else if (location.y < -maximumPos.y)
        {
            location.y = maximumPos.y;
        }
    }

    private void FindWindowLimits()
    {
        Camera.main.orthographic = true;
        Camera.main.orthographicSize = 8;
        Camera.main.transform.position = new Vector3(0, 0, -10);
        maximumPos = Camera.main.ScreenToWorldPoint(new Vector2(Screen.width, Screen.height));
    }
}