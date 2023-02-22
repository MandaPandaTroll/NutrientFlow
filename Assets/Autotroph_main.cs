using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Autotroph_main : MonoBehaviour
{

    
    public static List<Autotroph_main> individuals = new List<Autotroph_main>();

    public int generation;
   
    //Costs
    public float baseCost_energy, growthCost_energy, gameteCost_energy, energySynthase_UpkeepCost_energy,
     maxMovementCost,movementCost, maxTurningCost, turningCost, perNutrientAbsorptionCost;
    public int baseCost_nutrient,growthCost_nutrient, gameteCost_nutrient, energySynthase_UpkeepCost_nutrient;

   
    //Frequencies
    public int masterFrequency, catabolicFrequency, actionFrequency, anabolicFrequency, photosyntheticFrequency;
    
    //Timers
    int masterTimer, catabolismTimer, actionTimer, anabolismTimer, photosynthesisTimer;

    //Dynamic Somatic variables
    public float energyLevel, energySynthase_Integrity, currentMaturity, movementSpeed;
    public int nutrientLevel, spentNutrients, age;




    //Static Somatic variables
    public float energy_init, growthRate, maxMovementSpeed, energySynthase_DecayAmount, maxTurnAngle, turnProbability, energySynthase_restoreAmount, baseEnergyProduction;
    public int maxNutrients, maximumLifeSpan, absorptionRate, minimumMaturityAge, maxGametes;
    public bool maxGametesEnabled;


    //Other dynamic variables
    public int turnDice;
 
 // Misc variables
    public  IntGrid nutrientGrid;
    

    Rigidbody2D m_Rigidbody2D;

    public GameObject Gamete;

    public Vector2 mapBounds;
    
    // Start is called before the first frame update
    void Awake(){
        age = 0;
        masterTimer = 0;
        catabolismTimer = 0;
        actionTimer = 0;
        anabolismTimer = 0;
        
        currentMaturity = 0;
        energySynthase_Integrity = 1.0f;
        if(actionFrequency <= 0){
            actionFrequency = 1;
        }
        if(catabolicFrequency <= 0){
            catabolicFrequency = 1;
        }
        if(anabolicFrequency <= 0){
            anabolicFrequency = 1;
        }
    }
    void Start()
    {  

        

        mapBounds = GameObject.Find("box").transform.localScale/2f;
        
        turnDice = Mathf.FloorToInt(1f/turnProbability);
         m_Rigidbody2D = GetComponent<Rigidbody2D>();
         
        nutrientGrid = GameObject.Find("NutrientGrid").GetComponent<DiscreteGrid>().nutrientGrid;
        if(individuals.Contains(this) == false){
            individuals.Add(this);
        }
        maximumLifeSpan = Mathf.FloorToInt (ExtraMath.GetNormal(AccessibleGlobalSettings.meanMaximumLifeSpan, AccessibleGlobalSettings.std_lifeSpan));
        movementSpeed = maxMovementSpeed*currentMaturity;
        movementCost = maxMovementCost*currentMaturity;
        turningCost = maxTurningCost*currentMaturity;
    }

   
    Vector2 position;
    public int cellValue;
  
    void FixedUpdate()
    {
        if(transform.position.x > mapBounds.x-8f || transform.position.x < -mapBounds.x+8f || transform.position.y > mapBounds.y-8f || transform.position.y < -mapBounds.y+8f){
            m_Rigidbody2D.MoveRotation(m_Rigidbody2D.rotation+180f);
            Vector2 newPosition = new Vector2((m_Rigidbody2D.position.x*(mapBounds.x-8f)/mapBounds.x),(m_Rigidbody2D.position.y*(mapBounds.y-8f)/mapBounds.y));
            
            m_Rigidbody2D.MovePosition(newPosition);
            
        }
        masterTimer += 1;
        if( masterTimer >= masterFrequency){
            masterTimer = 0;
            age += 1;
        
        
        if(age >= maximumLifeSpan || energyLevel <= 1 ){
            
            Destroy(gameObject,0.5f);
        }
        actionTimer         += 1;
        anabolismTimer      += 1;
        catabolismTimer     += 1;
        photosynthesisTimer += 1;


//Photosynthesis
        if(photosynthesisTimer >= photosyntheticFrequency){
            photosynthesisTimer = 0;
            energyLevel = Photosynthesis(energyLevel);
            
        }

//Anabolism
        if(anabolismTimer >= anabolicFrequency){
            anabolismTimer = 0;
            Anabolism();
            
        }
//Catabolism
        if(catabolismTimer >= catabolicFrequency){
            catabolismTimer = 0;
            Catabolism();
            
        }

        

//Actions are taken each time the actionTimer reaches a pre-defined value
        if(actionTimer >= actionFrequency){
            actionTimer = 0;
            

            //Nutrient absorption 
            if(nutrientLevel < maxNutrients && energyLevel >= perNutrientAbsorptionCost*128f){
                AbsorbNutrients();
            }

            
            if(currentMaturity >= 0.99f && age >= minimumMaturityAge && energyLevel >= gameteCost_energy*16f && nutrientLevel >= gameteCost_nutrient){
                ProduceGamete();
            }
            
            
        
            if(energyLevel >= movementCost*64f && energyLevel >= turningCost*64f){
                Move();
            }
            
        }

    }
        

}
/*
    void OnCollisionEnter2D(Collision2D collision){
        if(collision.gameObject.tag == "Wall"){
            
        }
        else if(collision.gameObject.tag == "Autotroph") {
        ContactPoint2D contact = collision.GetContact(0);
        float angle = Vector2.Angle(transform.up,contact.normal);
        m_Rigidbody2D.MoveRotation(m_Rigidbody2D.rotation + angle);}
        
    }
*/
    void OnDestroy(){
        cellValue = nutrientGrid.GetValue(transform.position);
            nutrientGrid.SetValue(transform.position,cellValue+nutrientLevel + spentNutrients);
            nutrientLevel = 0;
            individuals.Remove(this);
    }

    public float Photosynthesis(float energy){
        float outEnergy = energy + baseEnergyProduction*energySynthase_Integrity*currentMaturity;
        return outEnergy;
    }
    public void Anabolism(){
    if(energySynthase_Integrity < 1f){
                if( energyLevel >= energySynthase_UpkeepCost_energy && nutrientLevel >= energySynthase_UpkeepCost_nutrient){
                
                energySynthase_Integrity += energySynthase_restoreAmount;
                energyLevel -= energySynthase_UpkeepCost_energy;
                nutrientLevel -= energySynthase_UpkeepCost_nutrient;
                spentNutrients += energySynthase_UpkeepCost_nutrient;
                if(energySynthase_Integrity >1f){
                    energySynthase_Integrity = 1f;
                }
            }
            }else if(currentMaturity < 1.000f){
                if(nutrientLevel >= growthCost_nutrient && energyLevel >= growthCost_energy){
                currentMaturity += growthRate;
                nutrientLevel -= growthCost_nutrient;
                energyLevel -= growthCost_energy;
                spentNutrients += growthCost_nutrient;
                Vector2 newSize = new Vector2(0.01f + currentMaturity, 0.01f + currentMaturity);
                transform.localScale = newSize;
                movementSpeed = maxMovementSpeed*currentMaturity;
                movementCost = maxMovementCost*currentMaturity;
                turningCost = maxTurningCost*currentMaturity;
                if(currentMaturity > 1f){
                    currentMaturity = 1f;
                }
            }
            }
            
            return;
    }

    public void AbsorbNutrients(){
        cellValue = nutrientGrid.GetValue(transform.position);
                if(cellValue > 0){
                    if( cellValue < absorptionRate && energyLevel < cellValue*perNutrientAbsorptionCost && energyLevel >= perNutrientAbsorptionCost){

                        nutrientLevel += 1;
                        nutrientGrid.SetValue(transform.position,cellValue-1);
                        energyLevel -= perNutrientAbsorptionCost;
                    }
                    else if( cellValue < absorptionRate && energyLevel >= cellValue*perNutrientAbsorptionCost){

                        nutrientLevel += cellValue;
                        nutrientGrid.SetValue(transform.position,0);
                        energyLevel -= cellValue*perNutrientAbsorptionCost;
                    }else if(cellValue > absorptionRate && energyLevel >= absorptionRate*perNutrientAbsorptionCost){

                        nutrientLevel += absorptionRate;
                        nutrientGrid.SetValue(transform.position,cellValue-absorptionRate);
                        energyLevel -= absorptionRate*perNutrientAbsorptionCost;
                    }    
                }
                //return;
    }

    public void Catabolism(){
        if(energyLevel >= baseCost_energy){
                energyLevel -= baseCost_energy;
            }
            if(nutrientLevel >= baseCost_nutrient){
                nutrientLevel -= baseCost_nutrient;
                spentNutrients += baseCost_nutrient;
            }
            if(energySynthase_Integrity >= energySynthase_DecayAmount){
                energySynthase_Integrity -= energySynthase_DecayAmount;
            }
            return;
    }

    public void ProduceGamete(){
        

        Vector3 gametePosition = new Vector3(transform.position.x+Random.Range(-1f,1f),transform.position.y+Random.Range(-1f,1f),0f);
        GameObject tempGamete = Instantiate(Gamete,gametePosition, transform.rotation);
        tempGamete.GetComponent<GameteMain>().nutrientLevel = gameteCost_nutrient;
        tempGamete.GetComponent<GameteMain>().generation = generation + 1;
        nutrientLevel -= gameteCost_nutrient;
        energyLevel -= gameteCost_energy;
        return;
    }

    public void Move(){

        if(Random.Range(0,turnDice) == 1 && energyLevel >= turningCost){
                m_Rigidbody2D.MoveRotation(m_Rigidbody2D.rotation + Random.Range(-maxTurnAngle,maxTurnAngle));
                energyLevel -= turningCost;
                
        
            }else if(energyLevel >= movementCost){
                m_Rigidbody2D.MovePosition(transform.position + transform.up*movementSpeed);
                energyLevel -= movementCost;
            }
    }

}



public static class IndividualStats{
        
        public static int GetNAutos(){
            int nAuto = Autotroph_main.individuals.Count;
            return nAuto;
        }
        
        public static int GetSumNutrients(){
            int sumNutrients = 0;
            foreach(Autotroph_main individual in Autotroph_main.individuals){
                sumNutrients += individual.nutrientLevel +individual.spentNutrients;
            }
            return sumNutrients;
        }

        public static float GetMeanGeneration(){
            float sumGeneration = 0;
            float divisor = 1f;
            float meanGeneration = 0;
            divisor = Autotroph_main.individuals.Count;
            foreach(Autotroph_main individual in Autotroph_main.individuals){
                sumGeneration += (float)individual.generation;
            }
            meanGeneration = sumGeneration/divisor;
            return meanGeneration;
        }
    }
