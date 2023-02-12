using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Autotroph_main : MonoBehaviour
{
    public static List<Autotroph_main> individuals = new List<Autotroph_main>();
    
    public float nutrientLevel;
    public float maxNutrients;
    public float growthCost;
   public float growthRate;
    public float currentMaturity;
    public float gameteCost_small;
    public float gameteCost_large;
    public int minimumMaturityAge;
    public int maximumLifeSpan;
    public float movementCost;
    public float turningCost;
    public float absorptionRate;
    public float movementSpeed;
    public float maxTurnAngle;
    
    public float actionFrequency;
   public float actionTimer;
    public float turnProbability;
    public int turnDice;
    public int numSteps;
    FloatGrid nutrientGrid;
    NutrientGridHandler nuteHandler;

    Rigidbody2D m_Rigidbody2D;
    // Start is called before the first frame update
    void Start()
    {  numSteps = 0;
        turnDice = Mathf.FloorToInt(1f/turnProbability);
         m_Rigidbody2D = GetComponent<Rigidbody2D>();
        nuteHandler = GameObject.Find("NutrientGrid").GetComponent<NutrientGridHandler>();
        nutrientGrid = nuteHandler.nutrientGrid;
        if(individuals.Contains(this) == false){
            individuals.Add(this);
        }
        maximumLifeSpan = Mathf.FloorToInt (ExtraMath.GetNormal(AccessibleGlobalSettings.meanMaximumLifeSpan, AccessibleGlobalSettings.std_lifeSpan));
        currentMaturity = 0;
    }

   

    public float cellValue;
    void FixedUpdate()
    {
        
        numSteps += 1;
        cellValue = nutrientGrid.GetValue(transform.position);
        actionTimer += Time.fixedDeltaTime;

        //Actions are taken each time the actionTimer reaches a pre-defined value
        if(actionTimer >= actionFrequency){
            //Nutrient absorption 
            if(nutrientLevel < maxNutrients){
                if(cellValue > 0){

                    if( cellValue < absorptionRate){

                        nutrientLevel += cellValue;
                        nutrientGrid.SetValue(transform.position,0);

                    }else if(cellValue > absorptionRate){

                        nutrientLevel += absorptionRate;
                        nutrientGrid.SetValue(transform.position,cellValue-absorptionRate);

                    }    
                }
            }
        if(nutrientLevel > 0){
            if(currentMaturity < 1.000f && nutrientLevel >= growthCost){
                currentMaturity += growthRate;
                nutrientLevel -= growthCost;

                if(currentMaturity > 1f){
                    currentMaturity = 1f;
                }
                }


        
            
            if(Random.Range(0,turnDice) == 1 && nutrientLevel >= turningCost){
                m_Rigidbody2D.MoveRotation(m_Rigidbody2D.rotation + Random.Range(-maxTurnAngle,maxTurnAngle));
                nutrientLevel -= turningCost;
        
            }else if(nutrientLevel > movementCost){
                m_Rigidbody2D.MovePosition(transform.position + transform.up*movementSpeed);
                nutrientLevel -= movementCost;
            }
        }

            
            
            actionTimer = 0;

        }



    }

    void OnCollisionEnter2D(Collision2D collision){
        ContactPoint2D contact = collision.GetContact(0);
        float angle = Vector2.Angle(transform.up,contact.normal);
        m_Rigidbody2D.MoveRotation(m_Rigidbody2D.rotation + angle);
    }

    
}

public static class IndividualStats{
        
        
        
        public static float GetSumNutrients(){
            float sumNutrients = 0;
            foreach(Autotroph_main individual in Autotroph_main.individuals){
                sumNutrients += individual.nutrientLevel;
            }
            return sumNutrients;
        }
    }
public static class normalDistributions{
    static float[] baseNormalLifeLengths = new float[64];
    
}