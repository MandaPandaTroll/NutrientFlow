using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class SuperGrid 
{

    public int nCells;
    public float cellSize;
    int width, height;
    Vector3 originPosition;
    public Cell[,] cellArray;
    public struct Cell{
        List<Individual> individuals;
        List<GameteMain> gametes;
        int nutrientContent;
    }


    
    public SuperGrid(int width, int height, float cellSize,Vector3 originPosition){
        this.width = width;
        this.height = height;
        this.cellSize = cellSize;
        this.originPosition = originPosition;
        cellArray = new Cell[width,height];
        
    }

private void GetXY(Vector3 worldPosition, out int x, out int y){

        x = Mathf.FloorToInt((worldPosition - originPosition).x / cellSize);
        y = Mathf.FloorToInt((worldPosition - originPosition).y / cellSize);
    
    }

    
    
}

public class IndividualClass:MonoBehaviour{
    int initAge = 0;
    public int initNutrientLevel;
    public static int globalPopulation{get;set;}
    public int defaultMaximumLifeSpan;
    public float initEnergyLevel;
    public List<Individual> allIndividuals = new List<Individual>();
   


        
    }


     public struct Individual{
        int individualNumber, nutrientLevel, age, maximumLifeSpan;
       float energyLevel;
       Vector2 position;

       public Individual(int individualNumber, int nutrientLevel, int age,int maximumLifeSpan,Vector2 position,float initEnergyLevel){
        this.individualNumber = individualNumber;
        this.nutrientLevel = nutrientLevel;
        this.age = age;
        this.maximumLifeSpan = maximumLifeSpan;
        this.position = position;
        this.energyLevel = initEnergyLevel;}
    }
