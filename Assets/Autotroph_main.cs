using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Autotroph_main : MonoBehaviour
{
public bool IsRepProb;
public float repProb(int age,float k, float a,int maximumLifeSpan){
    float x = ((float)age)/(float)(maximumLifeSpan);

    float output = 1f/(1f+Mathf.Pow(2f,-(k*x-a*k)));
    return output;
}
public float repRoll;
float prep;

private bool isStarving;
public bool isFromSelfing;
int reachedMaturityAge;
public static int maturityNominator{get;set;}
//public static float maturityCoef{get;set;}
public int totalSpentNutrients;
public int totalMigrations_left, totalMigrations_right, totalMigrations_up, totalMigrations_down, totalMigrations_upRight, totalMigrations_downRight, totalMigrations_upLeft,totalMigrations_downLeft;

public int[] coordNute = new int[3]{0,0,0};
public int[] selfCoords = new int[3]{0,0,-1};
//public bool OnlyMiddle;
public static float margin = 2f;

public bool InheritedLifeSpanToggle;
public bool AsexualReproductionEnabled;
public static bool InheritedLifeSpan{get;set;}
    //public static List<int> reproductiveNutes = new List<int>();
    //public static List<float> reproductiveEnergies = new List<float>();
    //public static Vector3 originPosition { get; set; }
    //public static float cellSize { get; set; }
    public int[] parentGametes;
    public int[] parentNumbers;
   


    public static int individualCount;
    public int individualNumber;
    
    public static List<Autotroph_main> individuals = new List<Autotroph_main>();

    public int generation;
   
    //Costs
    public float baseCost_energy, growthCost_energy, gameteCost_energy, energySynthase_UpkeepCost_energy,
     maxMovementCost,movementCost, perNutrientAbsorptionCost;
    public int baseCost_nutrient,growthCost_nutrient, gameteCost_nutrient, energySynthase_UpkeepCost_nutrient, asexualCost_nutrient, asexualCost_energy;

   
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
    //public bool maxGametesEnabled;


    //Other dynamic variables
    public int turnDice, gametesProduced, clonesProduced;
 
 // Misc variables
    public  IntGrid nutrientGrid;
    public bool hardMaturityAgeLimit;

    Rigidbody2D m_Rigidbody2D;

    public GameObject Gamete;
    public GameObject m_Autotroph;

    public Vector2 mapBounds;
    
    
    // Start is called before the first frame update

    void testReceive(){
        Debug.Log("Message Received");
    }
    void Awake(){
        migrationRoll = 1f;
        pMigration = 1;
        isStarving = false;
        canProduceGamete = false;
        canMigrate = false;
        coordNute = new int[3]{-1,-1,0};
        coordNute = IndividualStats.GetCoordNute(this);
        
        selfCoords[0] = coordNute[0];
        selfCoords[1] = coordNute[1];
        reachedMaturityAge = -1;
        if( maturityNominator <= 0){
            maturityNominator = 1;
        }
         m_Rigidbody2D = GetComponent<Rigidbody2D>();
         
        nutrientGrid = GameObject.Find("NutrientGrid").GetComponent<DiscreteGrid>().nutrientGrid;
        totalSpentNutrients = 0;
        totalMigrations_left = 0;
        totalMigrations_right = 0;
        totalMigrations_up = 0;
        totalMigrations_down = 0;
        totalMigrations_upRight = 0;
        totalMigrations_downRight = 0;
        totalMigrations_upLeft = 0;
        totalMigrations_downLeft = 0;
        //safetyVector = new Vector2(0,0);
        gametesProduced = 0;
        clonesProduced = 0;

        if(InheritedLifeSpanToggle == true){
            InheritedLifeSpan = true;
        }else{
            InheritedLifeSpan = false;
        }
        if(individuals.Contains(this) == false){
            individuals.Add(this);
        }
        individualCount += 1;
        individualNumber = individualCount;
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
    bool hasRecordedDeath;
    string causeOfDeath = "";
    
    void Start()
    {
        //migrationRoll = Random.Range(0f,1f);
        mapBounds = boxDims.mapBounds;
        age = 0;
        hasRecordedDeath = false;
        
        if(AsexualReproductionEnabled == false){
            energyLevel = energy_init;
        }else{
            energyLevel = asexualCost_energy;
        }
        
        if(staticGameteCost != gameteCost_nutrient){
            staticGameteCost = gameteCost_nutrient;
        }
        //mapBounds = GameObject.Find("box").transform.localScale/2f;
        //boxDims.mapBounds = mapBounds;
        turnDice = Mathf.FloorToInt(1f/turnProbability);
        
        if(generation == 0 || InheritedLifeSpan == false){
            maximumLifeSpan = Mathf.FloorToInt (ExtraMath.GetNormal(AccessibleGlobalSettings.meanMaximumLifeSpan, AccessibleGlobalSettings.std_lifeSpan));
        }
        if(IsRepProb){
            prep = repProb(age,16f,0.5f,maximumLifeSpan);
        }
        //movementSpeed = maxMovementSpeed*currentMaturity;
        //movementCost = maxMovementCost*currentMaturity;
movementCost = maxMovementCost;
        //turningCost = maxTurningCost*currentMaturity;
        GameteMain.zygoteNutrients = gameteCost_nutrient*2;
        Vector2 newSize = new Vector2(DiscreteGrid.pubCellSize,DiscreteGrid.pubCellSize);//new Vector2(0.01f + currentMaturity, 0.01f + currentMaturity);
                transform.localScale = newSize;
                if(hardMaturityAgeLimit){
                    
                    minimumMaturityAge = Mathf.RoundToInt(maximumLifeSpan*ParamLookup.maturityCoef);
                    
                }else{
                    minimumMaturityAge = 0;
                }
                
                if(ParamLookup.geometricDiagonalCost){
                    diagonalCost = movementCost*1.414214f;
                }else{
                    diagonalCost = movementCost;
                }

                if(generation != 0){
                    isFromSelfing = parentNumbers[0].Equals(parentNumbers[1]);
                    //Debug.Log(isFromSelfing);
                    selfCoords[0] =coordNute[0];
                    selfCoords[1] = coordNute[1];
                    if(isFromSelfing){
                        selfCoords[2] = 1;
                    }else{
                        selfCoords[2] = 0;
                        }
                }
                pMigration = ParamLookup.pMigration;

    }

   
    Vector2 position;
    public int cellValue;
    int asexualCoolDownPeriod = 32;
    int asexualCoolDownTimer = 0;
    Vector2 safetyVector;
    public bool canProduceGamete, canProduceClone, canMigrate;
    int reproduceOrTryMigrate;

    void Update(){
        coordNute = IndividualStats.GetCoordNute(this);
        if(coordNute[0] < 0){
            coordNute[0] = 0;

        }else if(coordNute[0] >= ParamLookup.gridDims[0]){
            coordNute[0] = ParamLookup.gridDims[0]-1;
        }
        if(coordNute[1] < 0){
            coordNute[1] = 0;

        }else if(coordNute[1] >= ParamLookup.gridDims[1]){
            coordNute[1] = ParamLookup.gridDims[1]-1;
        }
        selfCoords[0] = coordNute[0];
        selfCoords[1] = coordNute[1];
     }
    Vector2 pos = new Vector2(0f,0f);
    void FixedUpdate()
    { 
       //canMigrate = false;
       //canProduceClone = false; 
        
          /*
        pos = transform.position;
        if(GlobalTimeControls.globalSteps < 16){
            //Debug.Log("ind: " + individualNumber+ " | nutrientLevel: " + nutrientLevel + " | cell: " + coordNute[0]+","+coordNute[1]);
        }
        if(pos.x > mapBounds.x-(DiscreteGrid.pubCellSize/2f)){
            
            m_Rigidbody2D.MovePosition(new Vector2(mapBounds.x-(DiscreteGrid.pubCellSize/2f),pos.y));
        }else if(pos.x < -mapBounds.x+(DiscreteGrid.pubCellSize/2f)){
            
            m_Rigidbody2D.MovePosition(new Vector2(-mapBounds.x+(DiscreteGrid.pubCellSize/2f),pos.y));
        }
        if(pos.y > mapBounds.y-(DiscreteGrid.pubCellSize/2f)){
            
            m_Rigidbody2D.MovePosition(new Vector2(pos.x,mapBounds.y-(DiscreteGrid.pubCellSize/2f)));
        }else if(pos.y < -mapBounds.y+(DiscreteGrid.pubCellSize/2f)){
           
            m_Rigidbody2D.MovePosition(new Vector2(pos.x,-mapBounds.y+(DiscreteGrid.pubCellSize/2f)));
        }
        */
        coordNute[2] = nutrientLevel;
        /*if (OnlyMiddle)
        {
            Vector2 discretePosition = new Vector2(Mathf.Round(transform.position.x), Mathf.Round(transform.position.y));
            m_Rigidbody2D.MovePosition(discretePosition);
        }*/
        /*
        if(m_Rigidbody2D.position.x <= -mapBounds.x+0.5){
                    Vector2 newPos = m_Rigidbody2D.position;
                    newPos = new Vector2(-mapBounds.x+margin,m_Rigidbody2D.position.y);
                    m_Rigidbody2D.MoveRotation(Random.Range(-180f,180f));
                    m_Rigidbody2D.MovePosition(newPos);
                }
                else if(m_Rigidbody2D.position.x >= mapBounds.x-margin){
                    Vector2 newPos = m_Rigidbody2D.position;
                    newPos = new Vector2(mapBounds.x-margin,m_Rigidbody2D.position.y);
                    m_Rigidbody2D.MoveRotation(Random.Range(-180f,180f));
                    m_Rigidbody2D.MovePosition(newPos);
                }
                if(m_Rigidbody2D.position.y <= -mapBounds.y+margin){
                    Vector2 newPos = m_Rigidbody2D.position;
                    newPos = new Vector2(m_Rigidbody2D.position.x,-mapBounds.y+margin);
                    m_Rigidbody2D.MoveRotation(Random.Range(-180f,180f));
                    m_Rigidbody2D.MovePosition(newPos);
                }
                else if(m_Rigidbody2D.position.y >= mapBounds.y-margin){
                    Vector2 newPos = m_Rigidbody2D.position;
                    newPos = new Vector2(m_Rigidbody2D.position.x,mapBounds.y-margin);
                    m_Rigidbody2D.MoveRotation(Random.Range(-180f,180f));
                    m_Rigidbody2D.MovePosition(newPos);
                }
        */
        masterTimer += 1;
        if( masterTimer >= masterFrequency){
            masterTimer = 0;
            age += 1;
        
        
        if(age >= maximumLifeSpan || energyLevel <= 0f){
            
            causeOfDeath = "";
            string reproductiveEvents = (gametesProduced + clonesProduced).ToString();
            cellValue = nutrientGrid.GetValue(m_Rigidbody2D.position);
            //nutrientGrid.setRequests.Add(new IntGrid.SetRequest(m_Rigidbody2D.position,nutrientLevel + spentNutrients));
            nutrientGrid.SetValue(m_Rigidbody2D.position,cellValue+nutrientLevel + spentNutrients);
            if(age >= maximumLifeSpan){
                causeOfDeath = "age";
                if(energyLevel <= 1){
                    causeOfDeath = "age_starvation";
                }
            } 
            else if(energyLevel <= 1){
                causeOfDeath = "starvation";
            }
            
            if(!hasRecordedDeath){
                IndividualStats.deathEvents.Add(new string[22]{
                GlobalTimeControls.globalSteps.ToString(),
                individualNumber.ToString(),
                causeOfDeath,
                age.ToString(),
                maximumLifeSpan.ToString(),
                generation.ToString(),
                nutrientLevel.ToString(),
                totalSpentNutrients.ToString(),
                energyLevel.ToString(),
                reproductiveEvents,
                totalMigrations_left.ToString(),
                totalMigrations_right.ToString(),
                totalMigrations_up.ToString(),
                totalMigrations_down.ToString(),
                totalMigrations_upLeft.ToString(),
                totalMigrations_upRight.ToString(),
                totalMigrations_downLeft.ToString(),
                totalMigrations_downRight.ToString(),
                coordNute[0].ToString(),
                coordNute[1].ToString(),
                reachedMaturityAge.ToString(),
                currentMaturity.ToString()
                

                });
                //Debug.Log(causeOfDeath);
                hasRecordedDeath = true;
            }
            
            
            nutrientLevel = 0;
            spentNutrients = 0;
            Destroy(gameObject,0.5f);

        }else{
        actionTimer         += 1;
        anabolismTimer      += 1;
        catabolismTimer     += 1;
        photosynthesisTimer += 1;
        reproduceOrTryMigrate = 0;

        if(energyLevel <= 0 ){
            isStarving = true;
        }else{isStarving = false;}

//Photosynthesis
        if(photosynthesisTimer >= photosyntheticFrequency){
            photosynthesisTimer = 0;
            energyLevel = Photosynthesis(energyLevel);
            
        }

        //Nutrient absorption 
            if(nutrientLevel < maxNutrients && energyLevel > 1f + perNutrientAbsorptionCost){
                AbsorbNutrients();
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
        //if(actionTimer >= actionFrequency){
         //   actionTimer = 0;
            
        
            


            if(currentMaturity >= 1f && age >= minimumMaturityAge){
                if(AsexualReproductionEnabled == false){
                    if( energyLevel > 1f +gameteCost_energy && nutrientLevel >= gameteCost_nutrient){
                        canProduceGamete = true;
                    }else{canProduceGamete = false;}
                }else if(AsexualReproductionEnabled == true){
                    if(asexualCoolDownTimer > 0){
                        asexualCoolDownTimer -= 1;
                    }
                    if( energyLevel > 1f + asexualCost_energy && nutrientLevel >= asexualCost_nutrient && asexualCoolDownTimer <= 0){
                        canProduceClone = true;
                        asexualCoolDownTimer = asexualCoolDownPeriod;

                        
                    }else{canProduceClone = false;}
                    
                }
            }
            
        
            if(energyLevel > 1f + diagonalCost){ //&& energyLevel >= turningCost*64f){
                if(ParamLookup.juvenileMigration ){
                    canMigrate = true;
                }else if(currentMaturity >= 1f && age >= minimumMaturityAge){
                    canMigrate = true;
                }
                  
            }else{canMigrate = false;}

            if(ParamLookup.simultRepMig){

                if(canProduceGamete && canMigrate){
                    
                    ProduceGamete();
                    canProduceGamete = false;
                    migrationRoll = Random.Range(0f,1f);
                    if(migrationRoll <= ParamLookup.pMigration){
                        
                        MoveDiscrete();
                        canMigrate = false;
                        
                    }  
                }else if(canProduceGamete){
                    canProduceGamete = false;
                    if(IsRepProb){
                if(Random.Range(0,1f) <= prep){
                    ProduceGamete();
                }
            }else{ProduceGamete();}
                }else if(canMigrate){
                    migrationRoll = Random.Range(0f,1f);
                    if(migrationRoll <= ParamLookup.pMigration){
                        canMigrate = false;
                        MoveDiscrete();
                        
                    }
                }
            }

    if(!ParamLookup.simultRepMig){
        reproduceOrTryMigrate = Random.Range(1,3);
    if(reproduceOrTryMigrate == 1){
        if(canProduceGamete){
            canProduceGamete = false;
            if(IsRepProb){
                if(Random.Range(0,1f) <= prep){
                    ProduceGamete();
                }
            }else{ProduceGamete();}
            
            
        }else if(canProduceClone){
            canProduceClone = false;
            ProduceClone();
        }
    }else if(reproduceOrTryMigrate == 2){
        if(canMigrate){

             migrationRoll = Random.Range(0f,1f);
                    if(migrationRoll <= ParamLookup.pMigration){
                        canMigrate = false;
                        MoveDiscrete();
                        
                    }
                
        }
    }
    }
    
        //}
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
        StatisticsWriter.deaths_individual += 1;
        individuals.Remove(this);
        
            
    }

    public float Photosynthesis(float energy){
        float outEnergy = energy + baseEnergyProduction*energySynthase_Integrity;
        return outEnergy;
    }
    public void Anabolism(){
        if(hardMaturityAgeLimit){
                   // minimumMaturityAge = maximumLifeSpan/maturityNominator;
                   minimumMaturityAge = Mathf.RoundToInt(maximumLifeSpan*ParamLookup.maturityCoef);
                    
                }else{
                    minimumMaturityAge = 0;
                }
        if(IsRepProb){
            prep = repProb(age,16f,0.5f,maximumLifeSpan);
        }
    if(energySynthase_Integrity < 1f){
                if( energyLevel > 1f + energySynthase_UpkeepCost_energy && nutrientLevel >= energySynthase_UpkeepCost_nutrient){
                
                energySynthase_Integrity += energySynthase_restoreAmount;
                energyLevel -= energySynthase_UpkeepCost_energy;
                nutrientLevel -= energySynthase_UpkeepCost_nutrient;
                spentNutrients += energySynthase_UpkeepCost_nutrient;
                totalSpentNutrients += energySynthase_UpkeepCost_nutrient;
                //nutrientGrid.setRequests.Add(new IntGrid.SetRequest(m_Rigidbody2D.position, spentNutrients));
                nutrientGrid.SetValue(m_Rigidbody2D.position,nutrientGrid.GetValue(m_Rigidbody2D.position)+spentNutrients);
                
                spentNutrients = 0;
                if(energySynthase_Integrity >1f){
                    energySynthase_Integrity = 1f;
                }
            }
            }else if(currentMaturity < 1.000f && energySynthase_Integrity >= 1f){
                if(nutrientLevel >= growthCost_nutrient && energyLevel > 1f+ growthCost_energy){
                currentMaturity += growthRate;
                nutrientLevel -= growthCost_nutrient;
                energyLevel -= growthCost_energy;
                spentNutrients += growthCost_nutrient;
                totalSpentNutrients += growthCost_nutrient;
                //nutrientGrid.setRequests.Add(new IntGrid.SetRequest(m_Rigidbody2D.position, spentNutrients));
                nutrientGrid.SetValue(m_Rigidbody2D.position,nutrientGrid.GetValue(m_Rigidbody2D.position)+spentNutrients);
                spentNutrients = 0;
                Vector2 newSize = new Vector2(DiscreteGrid.pubCellSize,DiscreteGrid.pubCellSize);//new Vector2(0.01f + currentMaturity, 0.01f + currentMaturity);
                transform.localScale = newSize;
                //movementSpeed = Mathf.Clamp(maxMovementSpeed/2f+ (maxMovementSpeed/2f)*currentMaturity,0f,maxMovementSpeed);
                //movementCost = maxMovementCost*currentMaturity;
movementCost = maxMovementCost;
                //turningCost = maxTurningCost*currentMaturity;
                if(currentMaturity >= 1f){
                    currentMaturity = 1f;
                    reachedMaturityAge = age;
                }
            }
            }
            
            return;
    }
    
    public void AbsorbNutrients(){
        
        cellValue = nutrientGrid.GetValue(transform.position);
                if(cellValue > 0 ){
                    if( cellValue >= 1){

                        nutrientLevel += absorptionRate;
                        //nutrientGrid.setRequests.Add(new IntGrid.SetRequest(m_Rigidbody2D.position, -1));
                        nutrientGrid.SetValue(m_Rigidbody2D.position,cellValue-1);
                        energyLevel -= perNutrientAbsorptionCost;
                    }
                    /*else if( cellValue < absorptionRate && energyLevel >= cellValue*perNutrientAbsorptionCost){

                        nutrientLevel += cellValue;
                        nutrientGrid.SetValue(transform.position,0);
                        energyLevel -= cellValue*perNutrientAbsorptionCost;
                    }else if(cellValue > absorptionRate && energyLevel >= absorptionRate*perNutrientAbsorptionCost){

                        nutrientLevel += absorptionRate;
                        nutrientGrid.SetValue(transform.position,cellValue-absorptionRate);
                        energyLevel -= absorptionRate*perNutrientAbsorptionCost;
                    }*/    
                }
                return;
    }

    public void Catabolism(){
        if(energyLevel > 0 && baseCost_energy > 0){
            if(energyLevel >= baseCost_energy){
                energyLevel -= baseCost_energy;
                }else{
                energyLevel = 0;
                }
        }
        

            if(nutrientLevel >= baseCost_nutrient && baseCost_nutrient > 0){
                nutrientLevel -= baseCost_nutrient;
                spentNutrients += baseCost_nutrient;
                totalSpentNutrients += baseCost_nutrient;
                //nutrientGrid.setRequests.Add(new IntGrid.SetRequest(m_Rigidbody2D.position, spentNutrients));
                nutrientGrid.SetValue(m_Rigidbody2D.position,nutrientGrid.GetValue(m_Rigidbody2D.position)+spentNutrients);
                spentNutrients = 0;
            }
            if(energySynthase_Integrity > 0){
                if(energySynthase_Integrity >= energySynthase_DecayAmount){
                energySynthase_Integrity -= energySynthase_DecayAmount;
                }else{energySynthase_Integrity = 0;}
            }
            
            return;
    }

    int preNute, postNute;
    float preEnergy, postEnergy;
    string repType;
    public void ProduceGamete(){
        canProduceGamete = false;
        repType = "gametic";
        Vector3 gametePosition = new Vector3(transform.position.x,transform.position.y,transform.position.z);
        preNute = nutrientLevel;
        nutrientLevel -= gameteCost_nutrient;
        postNute = nutrientLevel;
        preEnergy = energyLevel;
        energyLevel -= gameteCost_energy;
        postEnergy = energyLevel;
        GameObject tempGamete = Instantiate(Gamete,new Vector3(transform.position.x,transform.position.y,0f), Quaternion.identity);
        gametesProduced += 1;
        GameteMain tempGameteScript = tempGamete.GetComponent<GameteMain>();
        tempGameteScript.generation = generation + 1;
        tempGameteScript.parentNumber = individualNumber;
        if(InheritedLifeSpan == true){
            tempGameteScript.parentLifeSpan = Mathf.FloorToInt(ExtraMath.GetNormal((double)maximumLifeSpan,1.0));
        }
        if(ParamLookup.doSampleRepEvents){
            IndividualStats.repEvents.Add(new string[20]{
            GlobalTimeControls.globalSteps.ToString(),
            individualNumber.ToString(),
            gametesProduced.ToString(),
            preNute.ToString(),
            postNute.ToString(),
            preEnergy.ToString(),
            postEnergy.ToString(),
            repType,
            age.ToString(),
            totalMigrations_left.ToString(),
            totalMigrations_right.ToString(),
            totalMigrations_up.ToString(),
            totalMigrations_down.ToString(),
            totalMigrations_upLeft.ToString(),
                totalMigrations_upRight.ToString(),
                totalMigrations_downLeft.ToString(),
                totalMigrations_downRight.ToString(),
            coordNute[0].ToString(),
            coordNute[1].ToString(),
            maximumLifeSpan.ToString()
            });
        //time_steps, indnum, preNute, postNute, preEnergy, postEnergy, repType
        }
        
        return;
    }

    public void ProduceClone(){
        repType = "asexual";
        asexualCoolDownTimer = asexualCoolDownPeriod;
        preNute = nutrientLevel;
        nutrientLevel -= asexualCost_nutrient;
        postNute = nutrientLevel;
        preEnergy = energyLevel;
        energyLevel -= asexualCost_energy;
        postEnergy = energyLevel;
        Vector3 thisPos = this.gameObject.transform.position;
        Vector3 clonePosition = new Vector3(thisPos.x+Random.Range(-0.5f,0.5f),thisPos.y+Random.Range(-0.5f,0.5f),0f);
        
        GameObject tempAutotroph = Instantiate(m_Autotroph,clonePosition, transform.rotation);
        Autotroph_main tempAutotrophScript = tempAutotroph.GetComponent<Autotroph_main>();
        tempAutotrophScript.nutrientLevel = 0;
        tempAutotrophScript.spentNutrients = 0;
        tempAutotrophScript.generation = generation + 1;
        tempAutotrophScript.parentGametes = new int[2]{individualNumber, individualNumber};
        tempAutotrophScript.nutrientLevel = asexualCost_nutrient;
        tempAutotrophScript.energyLevel = asexualCost_energy;
        clonesProduced += 1;
        StatisticsWriter.zygotesFormed += 1;
        
        

        if(InheritedLifeSpan == true){
            tempAutotrophScript.maximumLifeSpan = Mathf.FloorToInt(ExtraMath.GetNormal((double)maximumLifeSpan,1.0));
        }


        if(ParamLookup.doSampleRepEvents){
            IndividualStats.repEvents.Add(new string[9]{
            GlobalTimeControls.globalSteps.ToString(),
            individualNumber.ToString(),
            clonesProduced.ToString(),
            preNute.ToString(),
            postNute.ToString(),
            preEnergy.ToString(),
            postEnergy.ToString(),
            repType,
            age.ToString()});
        //time_steps, indnum, preNute, postNute, preEnergy, postEnergy, repType
        }

    }
    Vector2 moveVector = new Vector2(0,0);
    Vector2 newVector = new Vector2(0,0);
    float moveMag;
    //public float pMigration;
    float migrationRoll;
    static int[,] destinationCellMatrix = new int[8,2]{
        {-1,1}, {0,1}, {1,1},
        {-1,0},        {1,0},
        {-1,-1},{0,-1},{1,-1}
    };
    Vector2 destinationVector = new Vector2(0f,0f);
    Vector2 sourceVector = new Vector2(0f,0f);
    public static float diagonalCost{get;set;}
    public static float pMigration{get;set;}
    public void MoveDiscrete(){
        migrationRoll = 1f;
        canMigrate = false;
        if(energyLevel > 1f+ diagonalCost){
        
            sourceVector = transform.position;
            int destinationIndex = Random.Range(0,8);
            float newX = destinationCellMatrix[destinationIndex,0]*DiscreteGrid.pubCellSize + sourceVector.x;
            float newY = destinationCellMatrix[destinationIndex,1]*DiscreteGrid.pubCellSize + sourceVector.y;
        destinationVector.x = Mathf.Min(mapBounds.x,Mathf.Max(-mapBounds.x,newX));
        destinationVector.y = Mathf.Min(mapBounds.y,Mathf.Max(-mapBounds.y,newY));
        //var dxy = (destinationVector.x - sourceVector.x) +(destinationVector.y - sourceVector.y);
        if(destinationVector.x != sourceVector.x || destinationVector.y != sourceVector.y){
            
            m_Rigidbody2D.MovePosition(destinationVector);
            switch (destinationIndex){
                case 0:
                totalMigrations_upLeft += 1;
                energyLevel -= diagonalCost;
                break;
                case 1:
                totalMigrations_up += 1;
                energyLevel -= movementCost;
                break;
                case 2:
                totalMigrations_upRight += 1;
                energyLevel -= diagonalCost;
                break;
                case 3:
                totalMigrations_left += 1;
                energyLevel -= movementCost;
                break;
                case 4:
                totalMigrations_right += 1;
                energyLevel -= movementCost;
                break;
                case 5:
                totalMigrations_downLeft += 1;
                energyLevel -= diagonalCost;
                break;
                case 6:
                totalMigrations_down += 1;
                energyLevel -= movementCost;
                break;
                case 7:
                totalMigrations_downRight += 1;
                energyLevel -= diagonalCost;
                break;

            }
        }
        
    }
        /*
        moveMag = DiscreteGrid.pubCellSize;
        if(energyLevel > movementCost*2f){
            float randAngle = Random.Range(0,Mathf.PI);
            float randMag =(float) ExtraMath.GetNormal(0,ParamLookup.migrationSD);
            var absMag = Mathf.Abs(randMag);
            float xRoll = (float)(Mathf.Cos(randAngle));
            float yRoll = (float)( Mathf.Sin(randAngle));
            Vector2 testVector = new Vector2(xRoll,yRoll);
            testVector = testVector*randMag;
           
        //double xRoll = ExtraMath.GetNormal(0,ParamLookup.migrationSD);
        //double yRoll = ExtraMath.GetNormal(0,ParamLookup.migrationSD);
        //Debug.Log(xRoll+","+yRoll);
        moveVector.x = 0;
        moveVector.y = 0;
        newVector.x = 0;
        newVector.y = 0;
        float thisMag = testVector.sqrMagnitude;
        if(  absMag > 0 && absMag < 1.414214f){
            if(testVector.x <= -1f){
                moveVector.x = -DiscreteGrid.pubCellSize;
                energyLevel -= movementCost;
                totalMigrations_left += 1;
            }else if(testVector.x >= 1f){
                moveVector.x = DiscreteGrid.pubCellSize;
                energyLevel -= movementCost;
                totalMigrations_right += 1;
            }else if(testVector.y >= 1f){
                moveVector.y = DiscreteGrid.pubCellSize;
                energyLevel -= movementCost;
                totalMigrations_up += 1;
            }else if(testVector.y <= -1f){
                moveVector.y = -DiscreteGrid.pubCellSize;
                energyLevel -= movementCost;
                totalMigrations_down += 1;
            }
        }else if(absMag >= 1.41f ){
            if(testVector.x <= -1f && testVector.y <= -1f){
            moveVector.x = -DiscreteGrid.pubCellSize; moveVector.y = -DiscreteGrid.pubCellSize;
                energyLevel -= movementCost*1.4f;
                totalMigrations_downLeft += 1;
        }
        else if(testVector.x <= -1f && testVector.y >= 1f){
            moveVector.x = -DiscreteGrid.pubCellSize; moveVector.y = DiscreteGrid.pubCellSize;
                energyLevel -= movementCost*1.41f;
                totalMigrations_upLeft += 1;
        }
        else if(testVector.y <= -1f && testVector.x >= 1f){
            moveVector.y = -DiscreteGrid.pubCellSize; moveVector.x = DiscreteGrid.pubCellSize;
                energyLevel -= movementCost*1.41f;
                totalMigrations_downRight += 1;
        }
        else if(testVector.x >= 1f && testVector.y >= 1f){
           moveVector.x = DiscreteGrid.pubCellSize; moveVector.y = DiscreteGrid.pubCellSize;
                energyLevel -= movementCost*1.414214f;
                totalMigrations_upRight += 1;
        }
        }
        

        
        

        if(testVector.x < -1f ){
            moveVector.x = -DiscreteGrid.pubCellSize;
            energyLevel-=movementCost;
            totalMigrations_left +=1;
        }else if(testVector.x > 1f){
            moveVector.x = DiscreteGrid.pubCellSize;
            energyLevel-=movementCost;
            totalMigrations_right +=1;
        }

        if(testVector.y < -1f ){
            moveVector.y = -DiscreteGrid.pubCellSize;
            energyLevel-=movementCost;
            totalMigrations_down +=1;
        }else if(testVector.y > 1f){
            moveVector.y = DiscreteGrid.pubCellSize;
            energyLevel-=movementCost;
            totalMigrations_up +=1;
        }
        if(moveVector.sqrMagnitude != 0){
            newVector.x = Mathf.Floor(transform.position.x + moveVector.x);
            if(newVector.x >= mapBounds.x-(DiscreteGrid.pubCellSize/2f)){
                newVector.x = mapBounds.x-(DiscreteGrid.pubCellSize/2f);
                }else if(newVector.x <= -mapBounds.x+(DiscreteGrid.pubCellSize/2f)){
                    newVector.x = -mapBounds.x+(DiscreteGrid.pubCellSize/2f);
                }
            newVector.y = Mathf.Floor(transform.position.y + moveVector.y);
            if(newVector.y >= mapBounds.y-(DiscreteGrid.pubCellSize/2f)){
                newVector.y = mapBounds.y-(DiscreteGrid.pubCellSize/2f);
                }else if(newVector.y <= -mapBounds.y+(DiscreteGrid.pubCellSize/2f)){
                    newVector.y = -mapBounds.y+(DiscreteGrid.pubCellSize/2f);
                }
            

             m_Rigidbody2D.MovePosition(newVector);
        }
            
        
       
        
        }
        */

        return;
    }


    /*public void Move(){
        
        if(Random.Range(0,turnDice) == 1 && energyLevel >= turningCost){
                m_Rigidbody2D.MoveRotation(m_Rigidbody2D.rotation + Random.Range(-maxTurnAngle,maxTurnAngle));
                energyLevel -= turningCost;
                
        
            }else if(energyLevel >= movementCost){
                if(m_Rigidbody2D.position.x <= -mapBounds.x+0.5){
                    Vector2 newPos = m_Rigidbody2D.position;
                    newPos = new Vector2(-mapBounds.x+1.5f,m_Rigidbody2D.position.y);
                    m_Rigidbody2D.MoveRotation(Random.Range(-180f,180f));
                    m_Rigidbody2D.MovePosition(newPos);
                }
                if(m_Rigidbody2D.position.x >= mapBounds.x-margin){
                    Vector2 newPos = m_Rigidbody2D.position;
                    newPos = new Vector2(mapBounds.x-1.5f,m_Rigidbody2D.position.y);
                    m_Rigidbody2D.MoveRotation(Random.Range(-180f,180f));
                    m_Rigidbody2D.MovePosition(newPos);
                }
                if(m_Rigidbody2D.position.y <= -mapBounds.y+margin){
                    Vector2 newPos = m_Rigidbody2D.position;
                    newPos = new Vector2(m_Rigidbody2D.position.x,-mapBounds.y+1.5f);
                    m_Rigidbody2D.MoveRotation(Random.Range(-180f,180f));
                    m_Rigidbody2D.MovePosition(newPos);
                }
                if(m_Rigidbody2D.position.y >= mapBounds.y-margin){
                    Vector2 newPos = m_Rigidbody2D.position;
                    newPos = new Vector2(m_Rigidbody2D.position.x,mapBounds.y-1.5f);
                    m_Rigidbody2D.MoveRotation(Random.Range(-180f,180f));
                    m_Rigidbody2D.MovePosition(newPos);
                }
                m_Rigidbody2D.MovePosition(transform.position + transform.up*movementSpeed);
                energyLevel -= movementCost;
            }
    }
*/
    public static int staticGameteCost{get;set;}

    void GetGridPosition(Vector3 worldPosition, out int x, out int y)
    {
        x = Mathf.FloorToInt((worldPosition - DiscreteGrid.pubOriginPosition).x / DiscreteGrid.pubCellSize);
        y = Mathf.FloorToInt((worldPosition - DiscreteGrid.pubOriginPosition).y / DiscreteGrid.pubCellSize);
    }

}



public static class IndividualStats{
        
        public static List<string[]> repEvents = new List<string[]>(); //time_steps, indnum, preNute, postNute, preEnergy, postEnergy, repType
        public static List<string[]> deathEvents = new List<string[]>();
        

        public static int[,] GetGridPop(List<Autotroph_main> inds, int[] gridDims){
            int[,] outGrid = new int[gridDims[0],gridDims[1]];
            foreach(Autotroph_main ind in inds){
                outGrid[ind.coordNute[0],ind.coordNute[1]] +=1;
            }
            return outGrid;
        }

            public static int[,] GetSelfingGrid(List<Autotroph_main> inds, int[] gridDims){
            int[,] outGrid = new int[gridDims[0],gridDims[1]];
            foreach(Autotroph_main ind in inds){
                if(ind.selfCoords[2] > 0){
                    outGrid[ind.coordNute[0],ind.coordNute[1]] += ind.selfCoords[2];
                }
                
            }
            return outGrid;
        }

         
        public static int GetNAutos(){
            int nAuto = Autotroph_main.individuals.Count;
            
            return nAuto;
        }
        
        public static int GetSumNutrients(){
            int sumNutrients = 0;
            GameObject[] inds = GameObject.FindGameObjectsWithTag("Autotroph");
        foreach (GameObject ind in inds){
            Autotroph_main tempScript = ind.GetComponent<Autotroph_main>();
            sumNutrients+= tempScript.nutrientLevel + tempScript.spentNutrients;
        }
            /*foreach(Autotroph_main individual in Autotroph_main.individuals){
                sumNutrients += individual.nutrientLevel +individual.spentNutrients;
            }*/
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

        public static string[] GetIndividualNumbersAsString(){
            string [] individualNumbers = new string[Autotroph_main.individuals.Count];
            for(int i = 0; i < individualNumbers.Length;i++){
                individualNumbers[i] = Autotroph_main.individuals[i].individualNumber.ToString();
            }
            return individualNumbers;

        }
        public static string[] GetParentGametes0(){
            string [] parGams = new string[Autotroph_main.individuals.Count];
            for(int i = 0; i < parGams.Length;i++){
                parGams[i] = Autotroph_main.individuals[i].parentGametes[0].ToString();
            }
            return parGams;
        }
        public static string[] GetParentGametes1(){
            string [] parGams = new string[Autotroph_main.individuals.Count];
            for(int i = 0; i < parGams.Length;i++){
                parGams[i] = Autotroph_main.individuals[i].parentGametes[1].ToString();
            }
            return parGams;
        }

        public static string[] GetIDAsString(){
            string [] IDs = new string[Autotroph_main.individuals.Count];
            for(int i = 0; i < IDs.Length;i++){
                IDs[i] = Autotroph_main.individuals[i].GetInstanceID().ToString();
            }
            return IDs;
        }

        public static int CheckPopulationSize(){
            int foundIndividuals = GameObject.FindGameObjectsWithTag("Autotroph").Length;
            int foundGametes = GameObject.FindGameObjectsWithTag("smallGamete").Length;
            int diffInd = foundIndividuals - Autotroph_main.individuals.Count;
             return diffInd;
            
        }

        public static int CheckGameteSize(){
            int foundIndividuals = GameObject.FindGameObjectsWithTag("Autotroph").Length;
            int foundGametes = GameObject.FindGameObjectsWithTag("smallGamete").Length;
            int diffGam = foundGametes - GameteMain.gameteScripts.Count;
            return diffGam;
        }

        public static int[] GetGenerationVals(){
            int[] generationVals = new int[Autotroph_main.individuals.Count];
            for(int i = 0; i < generationVals.Length; i++){
                generationVals[i] = Autotroph_main.individuals[i].generation;
            }
            return generationVals;
        }


        public static int[] GetCoordNute( Autotroph_main ind){
            
            int[] output = new int[3]{0,0,0};
            
            output[0] = Mathf.FloorToInt((ind.transform.position - DiscreteGrid.pubOriginPosition).x / DiscreteGrid.pubCellSize);
            output[1] = Mathf.FloorToInt((ind.transform.position - DiscreteGrid.pubOriginPosition).y / DiscreteGrid.pubCellSize);
            output[2] = ind.nutrientLevel + ind.spentNutrients;
                if(output[0] < 0  || output[1] < 0 || output[0] > ParamLookup.gridDims[0]-1 || output[1] > ParamLookup.gridDims[1]-1){
                output[0] = -1;
                output[1] = -1;
                output[2] = 0;
            }
            return output;
        }

        public static float globalSelfingRatio{get;set;}
        public static float totalNotSelf{get;set;}
        public static float totalSelf{get;set;}
        public static float GetGlobalSelfingRatio(List<Autotroph_main> inds){
            int totalSelf = 0;
            int totalNotSelf = 0;
            foreach(Autotroph_main ind in inds){
                if(ind.selfCoords[2] == 0){
                    totalNotSelf +=1;
                }else if(ind.selfCoords[2] == 1){
                    totalSelf += 1;
                }
                IndividualStats.totalNotSelf = totalNotSelf;
                IndividualStats.totalSelf = totalSelf;
            }

            return( ((float)totalSelf/((float)inds.Count) ));
        }
    }
