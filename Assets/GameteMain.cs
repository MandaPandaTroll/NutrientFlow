using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameteMain : MonoBehaviour
{
    public static List<GameteMain> gameteScripts = new List<GameteMain>();
    
    public bool motile;
    public bool autoTrophSpawned;
    public bool chemotaxisEnabled;
    public float perceptionRadius;
    
    public bool incompatibilityEnabled;
    public float interactionRadius;
    public float actionFrequency;
   public float actionTimer;
   public int nutrientLevel;
   public int numSteps;
   public int maximumLifeSpan;
   public int generation;
   public GameObject autotroph_prefab;
    public  IntGrid nutrientgrid;

   Collider2D[] otherGametes;
   LayerMask gameteMask;
    // Start is called before the first frame update
    void Awake(){
        if(gameteScripts.Contains(this) == false){
            gameteScripts.Add(this);
        }
    }
    void Start()
    {
         nutrientgrid = GameObject.Find("NutrientGrid").GetComponent<DiscreteGrid>().nutrientGrid;
        autoTrophSpawned = false;
        gameteMask = LayerMask.GetMask("Gametes");
    }
    
    // Update is called once per frame
    int sumNutrients;
    int cellValue;
    void FixedUpdate()
    {
        numSteps += 1;
        
        if(numSteps >= maximumLifeSpan){
            cellValue = nutrientgrid.GetValue(transform.position);
            nutrientgrid.SetValue(transform.position, cellValue+nutrientLevel);
            nutrientLevel = 0;
            Destroy(gameObject, 0.2f);
        }

        actionTimer += Time.fixedDeltaTime;

        //Actions are taken each time the actionTimer reaches a pre-defined value
        if(actionTimer >= actionFrequency){

            if(chemotaxisEnabled == true){
                otherGametes = Physics2D.OverlapCircleAll(transform.position,perceptionRadius, gameteMask);
            }else if(chemotaxisEnabled == false){
                otherGametes = Physics2D.OverlapCircleAll(transform.position,interactionRadius, gameteMask);
            }

            if(incompatibilityEnabled == false){
                GameteMain otherGamete_script;
                
                if(otherGametes.Length > 1 && otherGametes[1] != this.GetComponent<Collider2D>()){
                    otherGamete_script = otherGametes[1].gameObject.GetComponent<GameteMain>();
                    if(otherGamete_script.autoTrophSpawned == false){
                        Vector2 zygotePosition = (transform.position + otherGamete_script.transform.position)/2f;
                            sumNutrients = nutrientLevel; //+ gamete.gameObject.GetComponent<GameteMain>().nutrientLevel;
                            GameObject thisAutotroph = Instantiate (autotroph_prefab, zygotePosition, transform.rotation);
                            thisAutotroph.GetComponent<Autotroph_main>().nutrientLevel = sumNutrients;
                            thisAutotroph.GetComponent<Autotroph_main>().generation = generation;
                            autoTrophSpawned = true;
                    }
                    
                            Destroy(gameObject, 0.2f);
                }
                        
                            
                        
                
            }

            


            actionTimer = 0;
        }
    }

    void OnDestroy(){
        
             
            gameteScripts.Remove(this);
    }
}


public static class GameteStats{
    public static int GetNGamete(){
        int popSize = GameteMain.gameteScripts.Count;
        return popSize;
    }
    public static int GetSumNutrients(){
        int output = 0;
        foreach(GameteMain gameteScript in GameteMain.gameteScripts){
            output += gameteScript.nutrientLevel;
        }
        return output;
    }

}