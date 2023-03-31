using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AutotrophSpawner : MonoBehaviour
{
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
        int I0, I1;
        spawnAutos = false;
        realisedSpawnRange = new Vector2(boxWidth*initialSpawnRange, boxHeight*initialSpawnRange);
        ParamLookup.initPop = initialPopulationSize;
        for(int i = 0; i < initialPopulationSize;i++){
            I0 = Random.Range(1,L0-1);//(L0/2)-1;//
            I1 = Random.Range(1,L1-1);//(L1/2)-1;//
            Quaternion tempRotation = Quaternion.identity;//Quaternion.Euler(0,0,Random.Range(-180f,180f));

            Vector3 tempSpawnPosition = DiscreteGrid.cellCenters[I0,I1];//new Vector3(Random.Range(-realisedSpawnRange.x,realisedSpawnRange.x),Random.Range(-realisedSpawnRange.y,realisedSpawnRange.y), 0);
            thisAutotroph = Instantiate(autotroph_prefab,tempSpawnPosition, tempRotation);
            thisAutotroph_script = thisAutotroph.GetComponent<Autotroph_main>();
            thisAutotroph_script.nutrientLevel = initialNutrientLevel;
            thisAutotroph_script.currentMaturity = 1f;//Random.Range(0,1f);
            thisAutotroph_script.age = 0;//thisAutotroph_script.maximumLifeSpan/2;//Random.Range(0,thisAutotroph_script.maximumLifeSpan/4);
            thisAutotroph_script.parentGametes = new int[2]{-1,-1};
        }
        spawnAutos = false;
    }
}
