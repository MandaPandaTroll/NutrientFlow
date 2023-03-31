using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Autotroph_main : MonoBehaviour
{



public int totalMigrations_left, totalMigrations_right, totalMigrations_up, totalMigrations_down;

public int[] coordNute = new int[3]{0,0,0};

    public bool OnlyMiddle;
public static float margin = 2f;

public bool InheritedLifeSpanToggle;
public bool AsexualReproductionEnabled;
public static bool InheritedLifeSpan{get;set;}
public static List<int> reproductiveNutes = new List<int>();
public static List<float> reproductiveEnergies = new List<float>();
public static Vector3 originPosition { get; set; }
    public static float cellSize { get; set; }
    public int[] parentGametes;
   


    public static int individualCount;
    public int individualNumber;
    
    public static List<Autotroph_main> individuals = new List<Autotroph_main>();

    public int generation;
   
    //Costs
    public float baseCost_energy, growthCost_energy, gameteCost_energy, energySynthase_UpkeepCost_energy,
     maxMovementCost,movementCost, maxTurningCost, turningCost, perNutrientAbsorptionCost;
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
    public bool maxGametesEnabled;


    //Other dynamic variables
    public int turnDice, gametesProduced, clonesProduced;
 
 // Misc variables
    public  IntGrid nutrientGrid;
    

    Rigidbody2D m_Rigidbody2D;

    public GameObject Gamete;
    public GameObject m_Autotroph;

    public Vector2 mapBounds;
    
    // Start is called before the first frame update
    void Awake(){
        totalMigrations_left = 0;
        totalMigrations_right = 0;
        totalMigrations_up = 0;
        totalMigrations_down = 0;
        safetyVector = new Vector2(0,0);
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
    void Start()
    {
        hasRecordedDeath = false;
        coordNute = new int[3]{0,0,0};
        if(AsexualReproductionEnabled == false){
            energyLevel = energy_init;
        }else{
            energyLevel = asexualCost_energy;
        }
        
        if(staticGameteCost != gameteCost_nutrient){
            staticGameteCost = gameteCost_nutrient;
        }
        mapBounds = GameObject.Find("box").transform.localScale/2f;
        boxDims.mapBounds = mapBounds;
        turnDice = Mathf.FloorToInt(1f/turnProbability);
         m_Rigidbody2D = GetComponent<Rigidbody2D>();
         
        nutrientGrid = GameObject.Find("NutrientGrid").GetComponent<DiscreteGrid>().nutrientGrid;
        if(generation == 0 || InheritedLifeSpan == false){
            maximumLifeSpan = Mathf.FloorToInt (ExtraMath.GetNormal(AccessibleGlobalSettings.meanMaximumLifeSpan, AccessibleGlobalSettings.std_lifeSpan));
        }
        
        movementSpeed = maxMovementSpeed*currentMaturity;
        movementCost = maxMovementCost*currentMaturity;
        turningCost = maxTurningCost*currentMaturity;
        GameteMain.zygoteNutrients = gameteCost_nutrient*2;
        Vector2 newSize = new Vector2(cellSize,cellSize);//new Vector2(0.01f + currentMaturity, 0.01f + currentMaturity);
                transform.localScale = newSize;
                minimumMaturityAge = maximumLifeSpan/8;
                
    }

   
    Vector2 position;
    public int cellValue;
    int asexualCoolDownPeriod = 32;
    int asexualCoolDownTimer = 0;
    Vector2 safetyVector;
    void FixedUpdate()
    {
        if(transform.position.x >= mapBounds.x){
            
            m_Rigidbody2D.MovePosition(new Vector2(mapBounds.x-1f,transform.position.y));
        }else if(transform.position.x <= -mapBounds.x){
            
            m_Rigidbody2D.MovePosition(new Vector2(-mapBounds.x+1f,transform.position.y));
        }
        if(transform.position.y >= mapBounds.y){
            
            m_Rigidbody2D.MovePosition(new Vector2(transform.position.x,mapBounds.y-1f));
        }else if(transform.position.y <= -mapBounds.y){
           
            m_Rigidbody2D.MovePosition(new Vector2(transform.position.x,-mapBounds.y+1f));
        }
        coordNute = IndividualStats.GetCoordNute(this);

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
        
        
        if(age >= maximumLifeSpan || energyLevel <= 1 ){
            
            string causeOfDeath = "";
            string reproductiveEvents = (gametesProduced + clonesProduced).ToString();
            cellValue = nutrientGrid.GetValue(m_Rigidbody2D.position);
            nutrientGrid.SetValue(m_Rigidbody2D.position,cellValue+nutrientLevel + spentNutrients);
            if(age >= maximumLifeSpan){
                causeOfDeath = "age";
                if(energyLevel <= 1){
                    causeOfDeath = "age_starvation";
                }
            }else if(energyLevel <= 1){
                causeOfDeath = "starvation";
            }

            if(!hasRecordedDeath){
                IndividualStats.deathEvents.Add(new string[15]{
                GlobalTimeControls.globalSteps.ToString(),
                individualNumber.ToString(),
                causeOfDeath,
                age.ToString(),
                generation.ToString(),
                nutrientLevel.ToString(),
                spentNutrients.ToString(),
                energyLevel.ToString(),
                reproductiveEvents,
                totalMigrations_left.ToString(),
                totalMigrations_right.ToString(),
                totalMigrations_up.ToString(),
                totalMigrations_down.ToString(),
                coordNute[0].ToString(),
                coordNute[1].ToString()

                });
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
            if(nutrientLevel < maxNutrients && energyLevel >= perNutrientAbsorptionCost*2){
                AbsorbNutrients();
            }


            if(currentMaturity >= 0.99f && age >= minimumMaturityAge){
                if(AsexualReproductionEnabled == false){
                    if( energyLevel >= gameteCost_energy*16f && nutrientLevel >= gameteCost_nutrient*8){
                    ProduceGamete();
                    }
                }else if(AsexualReproductionEnabled == true){
                    if(asexualCoolDownTimer > 0){
                        asexualCoolDownTimer -= 1;
                    }
                    if( energyLevel >= asexualCost_energy*2f && nutrientLevel >= asexualCost_nutrient*2 && asexualCoolDownTimer <= 0){
                        asexualCoolDownTimer = asexualCoolDownPeriod;
                        ProduceClone();
                    }
                    
                }
            }
            
            
            
            
        
            if(energyLevel >= movementCost*64f && energyLevel >= turningCost*64f){
                MoveDiscrete();
            }
            
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
        StatisticsWriter.deaths_individual += 1;
        individuals.Remove(this);
        
            
    }

    public float Photosynthesis(float energy){
        float outEnergy = energy + baseEnergyProduction*energySynthase_Integrity*currentMaturity;
        return outEnergy;
    }
    public void Anabolism(){
        minimumMaturityAge = maximumLifeSpan/8;
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
            }else if(currentMaturity < 1.000f && energySynthase_Integrity >= 1f){
                if(nutrientLevel >= growthCost_nutrient && energyLevel >= growthCost_energy*8f){
                currentMaturity += growthRate;
                nutrientLevel -= growthCost_nutrient;
                energyLevel -= growthCost_energy;
                spentNutrients += growthCost_nutrient;
                Vector2 newSize = new Vector2(cellSize,cellSize);//new Vector2(0.01f + currentMaturity, 0.01f + currentMaturity);
                transform.localScale = newSize;
                movementSpeed = Mathf.Clamp(maxMovementSpeed/2f+ (maxMovementSpeed/2f)*currentMaturity,0f,maxMovementSpeed);
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
            //return;
    }

    int preNute, postNute;
    float preEnergy, postEnergy;
    string repType;
    public void ProduceGamete(){
        
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
            IndividualStats.repEvents.Add(new string[7]{StatDisplay.tSteps.ToString(),
            individualNumber.ToString(),
            preNute.ToString(),
            postNute.ToString(),
            preEnergy.ToString(),
            postEnergy.ToString(),
            repType});
        //time_steps, indnum, preNute, postNute, preEnergy, postEnergy, repType
        }
        
        //return;
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
            IndividualStats.repEvents.Add(new string[7]{StatDisplay.tSteps.ToString(),
            individualNumber.ToString(),
            preNute.ToString(),
            postNute.ToString(),
            preEnergy.ToString(),
            postEnergy.ToString(),
            repType});
        //time_steps, indnum, preNute, postNute, preEnergy, postEnergy, repType
        }

    }
    Vector2 moveVector = new Vector2(0,0);
    Vector2 newVector = new Vector2(0,0);
    float moveMag;
    public void MoveDiscrete(){
        
        moveMag = cellSize;
        if(energyLevel > movementCost){
        double xRoll = ExtraMath.GetNormal(0,0.25);
        double yRoll = ExtraMath.GetNormal(0,0.25);
        //Debug.Log(xRoll+","+yRoll);
        moveVector.x = 0;
        moveVector.y = 0;
        newVector.x = 0;
        newVector.y = 0;
        if(xRoll < -1.0 ){
            moveVector.x = -cellSize;
            energyLevel-=movementCost;
            totalMigrations_left +=1;
        }else if(xRoll > 1.0){
            moveVector.x = cellSize;
            energyLevel-=movementCost;
            totalMigrations_right +=1;
        }

        if(yRoll < -1.0 ){
            moveVector.y = -cellSize;
            energyLevel-=movementCost;
            totalMigrations_down +=1;
        }else if(yRoll > 1.0){
            moveVector.y = cellSize;
            energyLevel-=movementCost;
            totalMigrations_up +=1;
        }
        
            newVector.x = Mathf.Round(transform.position.x + moveVector.x);
            if(newVector.x >= mapBounds.x-(cellSize/2f)){
                newVector.x = mapBounds.x-(cellSize/2f);
                }else if(newVector.x <= -mapBounds.x+(cellSize/2f)){
                    newVector.x = -mapBounds.x+(cellSize/2f);
                }
            newVector.y = Mathf.Round(transform.position.y + moveVector.y);
            if(newVector.y >= mapBounds.y-(cellSize/2f)){
                newVector.y = mapBounds.y-(cellSize/2f);
                }else if(newVector.y <= -mapBounds.y+(cellSize/2f)){
                    newVector.y = -mapBounds.y+(cellSize/2f);
                }
            

             m_Rigidbody2D.MovePosition(newVector);
        
       
        
        }
        


    }


    public void Move(){
        
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

    public static int staticGameteCost{get;set;}

    void GetGridPosition(Vector3 worldPosition, out int x, out int y)
    {
        x = Mathf.FloorToInt((worldPosition - originPosition).x / cellSize);
        y = Mathf.FloorToInt((worldPosition - originPosition).y / cellSize);
    }

}



public static class IndividualStats{
        
        public static List<string[]> repEvents = new List<string[]>(); //time_steps, indnum, preNute, postNute, preEnergy, postEnergy, repType
        public static List<string[]> deathEvents = new List<string[]>();
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
            
            output[0] = Mathf.FloorToInt((ind.transform.position - Autotroph_main.originPosition).x / Autotroph_main.cellSize);
            output[1] = Mathf.FloorToInt((ind.transform.position - Autotroph_main.originPosition).y / Autotroph_main.cellSize);
            output[2] = ind.nutrientLevel + ind.spentNutrients;
            if(!(output[0] >= 0 && output[0] < ParamLookup.gridDims[0] && output[1] >= 0 && output[1] < ParamLookup.gridDims[1])){
                output[0] = 0;
                output[1] = 0;
                output[2] = 0;
            }
            return output;
        }
    }
