using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

public class Chapter4Fig1 : MonoBehaviour
{
    particleSystemFigure1 psf1;
    Vector3 particleSystemLocation;
    public Material mat;
    public Vector3 velocity;
    public float lifeTime;
    public float startSpeed;

    int maxParticles;

    // Start is called before the first frame update
    void Start()
    {
        //Let's just have one particle
        maxParticles = 1;
        psf1 = new particleSystemFigure1(particleSystemLocation, startSpeed, velocity, lifeTime, maxParticles, mat);
    }
}

public class particleSystemFigure1
{
    //We need to create a GameObject to hold the ParticleSystem component
    GameObject particleSystemGameObject;

    //This is the ParticleSystem component but we'll need to access everything through the .main property
    //This is because ParticleSystems in Unity are interfaces and not independent objects
    ParticleSystem particleSystemComponent;

    public particleSystemFigure1(Vector3 particleSystemLocation, float startSpeed, Vector3 velocity, float lifeTime, int maxParticles, Material mat)
    {
        //Create the GameObject in the constructor
        particleSystemGameObject = new GameObject();
        //Move the GameObject to the right position
        particleSystemGameObject.transform.position = particleSystemLocation;
        //Add the particle system
        particleSystemComponent = particleSystemGameObject.AddComponent<ParticleSystem>();

        //Now we need to gather the interfaces of our ParticleSystem
        //The main interface covers general properties
        var main = particleSystemComponent.main;

        //In the Main Interface we'll sat the initial start LifeTime (how long a single particle will live)
        //And, of course, we'll set our Max Particles
        main.startLifetime = lifeTime;
        main.startSpeed = startSpeed;
        main.maxParticles = maxParticles;

        //Now we'll call methods to control the velocity of individual particles and their colors
        velocityModule(velocity);
        colorModule(mat);
    }

    public void velocityModule(Vector3 velocity)
    {
        //The velocityOverLifetime inferface controls the velocity of individual particles
        var velocityOverLifetime = particleSystemComponent.velocityOverLifetime;

        //First we need to enable the Velocity Over Lifetime Interface;
        velocityOverLifetime.enabled = true;
        velocityOverLifetime.space = ParticleSystemSimulationSpace.Local;

        //We then to create a MinMaxCurves which will manage the change in velocity a
        ParticleSystem.MinMaxCurve minMaxCurveX = new ParticleSystem.MinMaxCurve(-velocity.x * velocity.x, velocity.x);
        ParticleSystem.MinMaxCurve minMaxCurveY = new ParticleSystem.MinMaxCurve(-velocity.y * velocity.y, -velocity.y);

        velocityOverLifetime.x = minMaxCurveX;
        velocityOverLifetime.y = minMaxCurveY;
        //Even though we are not using Z, Unity needs us to otherwise it will throw an error. 
        //This is a bug in 2019.
        velocityOverLifetime.z = minMaxCurveY;
    }

    public void colorModule(Material mat)
    {
        //The colorOverLifetime interfaces manages the color of the objects over their lifetime.
        var colorOverLifetime = particleSystemComponent.colorOverLifetime;

        //While we are here, let's add a material to our particles
        ParticleSystemRenderer r = particleSystemGameObject.GetComponent<ParticleSystemRenderer>();
        //There a few different ways to do this, but we've created a material that is based on the default particle shader
        r.material = mat;

        //To have the particle become transparent we need to access the colorOverLifetime Interface
        colorOverLifetime.enabled = true;
        Gradient grad = new Gradient();
        //This gradient key lets us choose points on a gradient that represent different RGBA or Unity.Color values.
        //These gradient values exist in an array
        grad.SetKeys(new GradientColorKey[] { new GradientColorKey(Color.white, 0.0f) }, new GradientAlphaKey[] { new GradientAlphaKey(1.0f, 0.0f), new GradientAlphaKey(0f, 2.0f) });
        //Set the color to the gradient we created above
        colorOverLifetime.color = grad;
    }

}