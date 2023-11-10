using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParallaxEffect : MonoBehaviour
{
    //Object References
    public Camera cam;
    public Transform followTarget;

    //Variables
    Vector2 startPos; //starting position of game object
    Vector2 camMovement; //camera movement since start
    float zPos; //z depth of game object
    float distanceFromTarget; //distance from target to object
    float parallaxFactor; //strength of parallax effect
    float clippingPlane;

    // Start is called before the first frame update
    void Start()
    {

        startPos = transform.position;
        zPos = transform.position.z;

    }

    // Update is called once per frame
    void Update()
    {
        //get distance from target
        distanceFromTarget = transform.position.z - followTarget.position.z;

        //determine what clip plane to use by using a ternary
        clippingPlane = (cam.transform.position.z + (distanceFromTarget > 0 ? cam.farClipPlane : cam.nearClipPlane));

        //calc camera movement 
        camMovement = (Vector2)cam.transform.position - startPos;

        //calc parallax
        parallaxFactor = Mathf.Abs(distanceFromTarget) / clippingPlane;

        Vector2 newPos = startPos + camMovement * parallaxFactor;
        transform.position = new Vector3(newPos.x, newPos.y, zPos);
    }
}
