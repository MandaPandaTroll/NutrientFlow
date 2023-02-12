using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Movement : MonoBehaviour
{
    Rigidbody2D rb;
    public float movementForce;
    public float maxMovementForce;
    public float turnTorque;
    public float maxTorque;
    public float initMaxTorque;
    public int inverseChangeDirectionProbability;
    public float torqueTimer;
    float timePassed;
    int turnDirection = 1;
    int randDir;
    
    // Start is called before the first frame update
    void Start()
    {  
        
        timePassed = Random.Range(0,torqueTimer-1f);
        rb = gameObject.GetComponent<Rigidbody2D>();
        randDir = Random.Range(0,2);
        if(randDir == 1){
            turnDirection = turnDirection*(-1);
        }
         rb.AddTorque(Random.Range(-32f,32f));
        
    }

    // Update is called once per frame
    void Update()
    {   
        timePassed += Time.deltaTime;
        
        if(Random.Range(0,inverseChangeDirectionProbability+1) == 1){
        turnDirection = turnDirection*-1;
    }
    if(timePassed >= torqueTimer){
        timePassed = 0;
    }
    maxTorque = initMaxTorque*(1f - timePassed/torqueTimer);


    movementForce = Random.Range(maxMovementForce/2f,maxMovementForce);
        rb.AddForce(transform.up*movementForce);
        
            turnTorque=Random.Range(maxTorque/2f,maxTorque);
            rb.AddTorque(turnTorque*turnDirection);
        
    }
}
