using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//in order not to allow multiple scripts on same game object
[DisallowMultipleComponent]
public class Oscillator : MonoBehaviour
{
    [SerializeField] Vector3 movementVector = new Vector3(15f, 0f, 0f);
    [SerializeField] float period = 2f;

    // ToDo remove from inspector later
    //[Range(0, 1)]
    //[SerializeField]
    //float movementFactor;   // 0 for not move, 1 for moved. 

    float movementFactor; // 0 for not moved, 1 for fully moved.

    Vector3 startingPos;    //must be stored for absolute movement

    // Start is called before the first frame update
    void Start()
    {
        startingPos = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        //set movement factor automatically 
        
        if (period <= Mathf.Epsilon) { return; } // protect against period is zero
        // todo protect against period is zero
        float cycles = Time.time / period; // grows continually from 0

        const float tau = Mathf.PI * 2f; // about 6.28
        float rawSinWave = Mathf.Sin(cycles * tau); // goes from -1 to +1

        movementFactor = rawSinWave / 2f + 0.5f;


        Vector3 offset = movementVector * movementFactor;
        transform.position = startingPos + offset;
    }
}
