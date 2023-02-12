using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KinematicMovement : MonoBehaviour
{
    // Start is called before the first frame update
    Rigidbody2D m_Rigidbody2D;
    
    void Start()
    {
        //Fetch the Rigidbody from the GameObject with this script attached
        m_Rigidbody2D = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if(Random.Range(0,32) == 1){
            //Store user input as a movement vector
        
        m_Rigidbody2D.MoveRotation(m_Rigidbody2D.rotation + Random.Range(-16f,16));
        
        
        }else{
            m_Rigidbody2D.MovePosition(transform.position + transform.up*0.01f);

        }


    }

    
}
