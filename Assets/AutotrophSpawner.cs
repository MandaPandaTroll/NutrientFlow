using System.Collections;
using System.Collections.Generic;
using UnityEngine;





public class AutotrophSpawner : MonoBehaviour
{

    
    public bool singleHabitat;
    public GameObject autotroph_prefab;
    public InputFieldToInt initPopInputField;
    GameObject thisAutotroph;
    public int initialNutrientLevel;
    public int initialPopulationSize;
    public float initialSpawnRange;

    float boxWidth, boxHeight;
    Vector2 realisedSpawnRange;
    public GameObject box;
    public bool spawnAutos {get;set;}


    // Start is called before the first frame update
    void Start()
    {
        Autotroph_main startPrefabScript = autotroph_prefab.GetComponent<Autotroph_main>();
        initialPopulationSize = initPopInputField.value;
        if (initialSpawnRange > 1.0f){
            initialSpawnRange = 1.0f;
        }
       boxWidth = box.transform.localScale.x/2f;
       boxHeight = box.transform.localScale.y/2f;
       realisedSpawnRange = new Vector2(boxWidth*initialSpawnRange, boxHeight*initialSpawnRange);
        ParamLookup.gameteCost_nutrient = startPrefabScript.gameteCost_nutrient;
        ParamLookup.gameteCost_energy   = startPrefabScript.gameteCost_energy;
        ParamLookup.growthCost_nutrient = startPrefabScript.growthCost_nutrient;
        ParamLookup.growthCost_energy   = startPrefabScript.growthCost_energy;
        ParamLookup.growthRate          = startPrefabScript.growthRate;
    }

    // Update is called once per frame
    void Update()
    {   
        if(spawnAutos == true){
            
            initialPopulationSize = initPopInputField.value;
            SpawnAutotrophs();
        }
        
    }

    Autotroph_main thisAutotroph_script;
    int numMiddleCells_X;
    int numMiddleCells_Y;
    int summiddleCells;
    int indsPerMiddleCell;
    Quaternion noRot = Quaternion.identity;
    
    public void SpawnAutotrophs(){
        if(ParamLookup.randomStartAge){
            randomStartAge = true;
        }else{randomStartAge = false;}
        if(ParamLookup.randomStartMaturity){
            randomStartMaturity = true;
        }else{randomStartMaturity = false;}

        int L0 = DiscreteGrid.cellCenters.GetLength(0);
        int L1 = DiscreteGrid.cellCenters.GetLength(1);
        int[] middle0 = new int[2];
        int[] middle1  = new int[2];
        
        if(L0 % 2 == 0){
            middle0[0] = (L0/2) - 1;
            middle0[1] = (L0/2);
            numMiddleCells_X = 2;
        }else if(L0 %2 != 0){
            middle0[0] = (L0/2);
            middle0[1] = (L0/2);
            numMiddleCells_X = 1;
        }
        if(L1 % 2 == 0){
            middle1[0] = (L1/2) - 1;
            middle1[1] = (L1/2);
            numMiddleCells_Y = 2;
        }else if(L1 %2 != 0){
            middle1[0] = (L1/2);
            middle1[1] = (L1/2);
            numMiddleCells_Y = 1;
        }
        summiddleCells = numMiddleCells_X+numMiddleCells_Y;
        
        int[] spawnRange2 = new int[2]{Mathf.RoundToInt((float)L0*initialSpawnRange/2f),Mathf.RoundToInt((float)L1*initialSpawnRange/2f)};
        int I0, I1;
        spawnAutos = false;
        realisedSpawnRange = new Vector2(boxWidth*initialSpawnRange, boxHeight*initialSpawnRange);
        ParamLookup.initPop = initialPopulationSize;
        
        I0 = middle0[0];
        I1 = middle1[0];
        if(ParamLookup.spawnCenterEvenInd){
            indsPerMiddleCell = initialPopulationSize/(summiddleCells);
            for(int x = 0; x < 2; x++){
                for(int y = 0; y < 2; y++){
                    I0 = middle0[x];
                    I1 = middle1[y];
                    for(int i = 0; i< indsPerMiddleCell; i++){
                        

                       CreateIndividual(I0,I1); 
                    }
                }
            }
            
            
            
            
        }else{
            for(int i = 0; i < initialPopulationSize;i++){
            //I0 = Random.Range(1,L0-1);
            if(!singleHabitat){
                I0 = Random.Range(middle0[0]-spawnRange2[0]+1,middle0[1]+spawnRange2[0]);
            //I1 = Random.Range(1,L1-1);
                I1 = Random.Range(middle1[0]-spawnRange2[1]+1,middle1[1]+spawnRange2[1]);
            }
            
            

            CreateIndividual(I0,I1);
        }
        }
        
        spawnAutos = false;
    }
    bool randomStartAge;
    bool randomStartMaturity;
    public void CreateIndividual(int I0, int I1){
        Vector3 tempSpawnPosition = DiscreteGrid.cellCenters[I0,I1];//new Vector3(Random.Range(-realisedSpawnRange.x,realisedSpawnRange.x),Random.Range(-realisedSpawnRange.y,realisedSpawnRange.y), 0);
                        thisAutotroph = Instantiate(autotroph_prefab,tempSpawnPosition, noRot);
                        thisAutotroph_script = thisAutotroph.GetComponent<Autotroph_main>();
                        thisAutotroph_script.nutrientLevel = initialNutrientLevel;
                        if(randomStartAge){
                        thisAutotroph_script.age = GetRandAge(thisAutotroph_script.maximumLifeSpan/4);
                        }else{
                        thisAutotroph_script.age = 0;
                        }
                        if(randomStartMaturity){
                            thisAutotroph_script.currentMaturity = GetRandMaturity(0,0.25f);
                        }else{
                            thisAutotroph_script.currentMaturity = 0;
                        }
                        
                       
                        thisAutotroph_script.parentGametes = new int[2]{-1,-1};
                        thisAutotroph_script.parentNumbers = new int[2]{-1,-1};
                return;
    }

    public int GetRandAge(int lspan){
        return(Random.Range(0,lspan));
    }
    public float GetRandMaturity(float min, float max){
        return(Random.Range(min,max));
    }
}
