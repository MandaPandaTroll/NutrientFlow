using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class AutoController : MonoBehaviour{
    //List<Autotroph_main> indsToShuffle = new List<Autotroph_main>();
    //List<GameteMain> gamsToShuffle = new List<GameteMain>();
    void Start(){
    
        var indsToShuffle = FindObjectsByType(typeof(Autotroph_main),FindObjectsSortMode.None);
        var gamsToShuffle = FindObjectsByType(typeof(GameteMain),FindObjectsSortMode.None);

        gamsToShuffle =  gamsToShuffle.OrderBy(c => _rand.Next()).ToArray();
        indsToShuffle = indsToShuffle.OrderBy(c => _rand.Next()).ToArray();

        foreach(Autotroph_main ind in indsToShuffle){
            ind.gameObject.SendMessage("DoFixedUpdate");
        }
    }
    int shuffleFreq = 8, shuffletimer = 0;
    void OWOFixedUpdate(){
        shuffletimer += 1;
        //indsToShuffle = FindObjectsByType<Autotroph_main>(FindObjectsSortMode.None).ToList<Autotroph_main>();
        //indsToShuffle = GenerateRandomOrderBy(indsToShuffle);
        var indsToShuffle = FindObjectsByType(typeof(Autotroph_main),FindObjectsSortMode.None);
        var gamsToShuffle = FindObjectsByType(typeof(GameteMain),FindObjectsSortMode.None);
        if(shuffletimer >= shuffleFreq){
            shuffletimer = 0;
            gamsToShuffle =  gamsToShuffle.OrderBy(c => _rand.Next()).ToArray();
        indsToShuffle = indsToShuffle.OrderBy(c => _rand.Next()).ToArray();
        }
        
        //gamsToShuffle = GenerateRandomOrderBy(gamsToShuffle);
        foreach(Autotroph_main ind in indsToShuffle){
            if(ind != null){
                ind.SendMessage("DoFixedUpdate");
            }
            

        }
        foreach(GameteMain gam in gamsToShuffle){
            if(gam != null){
                gam.SendMessage("DoFixedUpdate");
            }
            
        }
    }

    void Update(){

        //foreach(Autotroph_main ind in indsToShuffle){
        //    if(ind != null){
        //        ind.SendMessage("DoUpdate");
        //    }
            

        //}
        //foreach(GameteMain gam in gamsToShuffle){
         //   gam.SendMessage("DoUpdate");
       // }


    }
    public System.Random _rand = new System.Random();
    public List<Autotroph_main> GenerateRandomLoop(List<Autotroph_main> listToShuffle)
{
    for (int i = listToShuffle.Count - 1; i > 0; i--)
    {
        var k = _rand.Next(i + 1);
        var value = listToShuffle[k];
        listToShuffle[k] = listToShuffle[i];
        listToShuffle[i] = value;
    }
    return listToShuffle;
}

public List<GameteMain> GenerateRandomLoop(List<GameteMain> listToShuffle)
{
    for (int i = listToShuffle.Count - 1; i > 0; i--)
    {
        var k = _rand.Next(i + 1);
        var value = listToShuffle[k];
        listToShuffle[k] = listToShuffle[i];
        listToShuffle[i] = value;
    }
    return listToShuffle;
}



public  List<Autotroph_main> GenerateRandomOrderBy(List<Autotroph_main> listToShuffle)
{
    var shuffledList = listToShuffle.OrderBy(_ => _rand.Next()).ToList();
    return shuffledList;
}

public  List<GameteMain> GenerateRandomOrderBy(List<GameteMain> listToShuffle)
{
    var shuffledList = listToShuffle.OrderBy(_ => _rand.Next()).ToList();
    return shuffledList;
}

}
