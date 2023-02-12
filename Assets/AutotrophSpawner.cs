using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AutotrophSpawner : MonoBehaviour
{
    public GameObject autotroph_prefab;
    GameObject thisAutotroph;
    public float initialNutrientLevel;
    public int initialPopulationSize;
    public float initialSpawnRange;

    float boxWidth, boxHeight;
    Vector2 realisedSpawnRange;
    public GameObject box;

    // Start is called before the first frame update
    void Start()
    {
        if (initialSpawnRange > 0.9f){
            initialSpawnRange = 0.9f;
        }
       boxWidth = box.transform.localScale.x;
       boxHeight = box.transform.localScale.y;
       realisedSpawnRange = new Vector2(boxWidth*initialSpawnRange, boxHeight*initialSpawnRange);
        SpawnAutotrophs();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void SpawnAutotrophs(){
        for(int i = 0; i < initialPopulationSize;i++){
            Quaternion tempRotation = Quaternion.Euler(0,0,Random.Range(-180f,180f));

            Vector3 tempSpawnPosition = new Vector3(Random.Range(-realisedSpawnRange.x,realisedSpawnRange.x),Random.Range(-realisedSpawnRange.y,realisedSpawnRange.y), 0);
            thisAutotroph = Instantiate(autotroph_prefab,tempSpawnPosition, tempRotation);
            thisAutotroph.GetComponent<Autotroph_main>().nutrientLevel = initialNutrientLevel;
        }
    }
}
