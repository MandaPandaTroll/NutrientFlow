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
        initialPopulationSize = initPopInputField.value;
        if (initialSpawnRange > 1.0f){
            initialSpawnRange = 1.0f;
        }
       boxWidth = box.transform.localScale.x/2f;
       boxHeight = box.transform.localScale.y/2f;
       realisedSpawnRange = new Vector2(boxWidth*initialSpawnRange, boxHeight*initialSpawnRange);
        
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
    public void SpawnAutotrophs(){
        
        int L0 = DiscreteGrid.cellCenters.GetLength(0);
        int L1 = DiscreteGrid.cellCenters.GetLength(1);
        int[] middle0 = new int[2];
        int[] middle1  = new int[2];
        
        if(L0 % 2 == 0){
            middle0[0] = (L0/2) - 1;
            middle0[1] = (L0/2);
        }else if(L0 %2 != 0){
            middle0[0] = (L0/2);
            middle0[1] = (L0/2);
        }
        if(L1 % 2 == 0){
            middle1[0] = (L1/2) - 1;
            middle1[1] = (L1/2);
        }else if(L1 %2 != 0){
            middle1[0] = (L1/2);
            middle1[1] = (L1/2);
        }

        
        int[] spawnRange2 = new int[2]{Mathf.RoundToInt((float)L0*initialSpawnRange/2f),Mathf.RoundToInt((float)L1*initialSpawnRange/2f)};
        int I0, I1;
        spawnAutos = false;
        realisedSpawnRange = new Vector2(boxWidth*initialSpawnRange, boxHeight*initialSpawnRange);
        ParamLookup.initPop = initialPopulationSize;
        
        I0 = middle0[0];
        I1 = middle1[0];

        for(int i = 0; i < initialPopulationSize;i++){
            //I0 = Random.Range(1,L0-1);
            if(!singleHabitat){
                I0 = Random.Range(middle0[0]-spawnRange2[0]+1,middle0[1]+spawnRange2[0]);
            //I1 = Random.Range(1,L1-1);
            I1 = Random.Range(middle1[0]-spawnRange2[1]+1,middle1[1]+spawnRange2[1]);
            }
            
            Quaternion tempRotation = Quaternion.identity;//Quaternion.Euler(0,0,Random.Range(-180f,180f));

            Vector3 tempSpawnPosition = DiscreteGrid.cellCenters[I0,I1];//new Vector3(Random.Range(-realisedSpawnRange.x,realisedSpawnRange.x),Random.Range(-realisedSpawnRange.y,realisedSpawnRange.y), 0);
            thisAutotroph = Instantiate(autotroph_prefab,tempSpawnPosition, tempRotation);
            thisAutotroph_script = thisAutotroph.GetComponent<Autotroph_main>();
            thisAutotroph_script.nutrientLevel = initialNutrientLevel;
            thisAutotroph_script.currentMaturity = Random.Range(0,0.25f);//0;
            thisAutotroph_script.age = Random.Range(0,thisAutotroph_script.maximumLifeSpan/4);//0;//thisAutotroph_script.maximumLifeSpan/2;;
            thisAutotroph_script.parentGametes = new int[2]{-1,-1};
        }
        spawnAutos = false;
    }
}
