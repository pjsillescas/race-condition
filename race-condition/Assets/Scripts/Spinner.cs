using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spinner : MonoBehaviour
{
    public float AngularSpeed = 0.0002f;

    // Start is called before the first frame update
    void Start()
    {
        Debug.Log("speed: " + AngularSpeed);
    }

    // Update is called once per frame
    void Update()
    {
        transform.Rotate(new Vector3(0,1,0), AngularSpeed * Time.deltaTime);
    }
}
