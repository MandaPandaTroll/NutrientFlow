using UnityEngine;
using System.Linq;


public class IntGrid{
    static int[,] internalKernel = new int[3,3];
    public  int[,] zeroKernel = new int[3,3]{{0,0,0},{0,0,0},{0,0,0}};
    public int[,] gridArray;
    private int width;
    private int height;
    private float cellSize;
    
    private Vector3 originPosition;
    

    public IntGrid(int width, int height, float cellSize, Vector3 originPosition){
        this.width = width;
        this.height = height;
        this.cellSize = cellSize;
        
        this.originPosition = originPosition;
        
        gridArray = new int[width,height];

        for (int x = 0; x < gridArray.GetLength(0); x++)
        {
            for(int y = 0; y < gridArray.GetLength(1); y++)
            {
               
                Debug.DrawLine(GetWorldPosition(x, y), GetWorldPosition(x, y + 1), Color.white, 10f);
                Debug.DrawLine(GetWorldPosition(x, y), GetWorldPosition(x +1, y), Color.white, 10f);
            }
        }
        Debug.DrawLine(GetWorldPosition(0, height), GetWorldPosition(width, height), Color.white, 10f);
        Debug.DrawLine(GetWorldPosition(width, 0), GetWorldPosition(width, height), Color.white, 10f);
    }
    private Vector3 GetWorldPosition(int x, int y){
        return new Vector3 (x, y)*cellSize + originPosition;
    }

    private void GetXY(Vector3 worldPosition, out int x, out int y){

        x = Mathf.FloorToInt((worldPosition - originPosition).x / cellSize);
        if(x >= width-1){
            x = width-1;
        }else if(x <= 0){
            x = 0;
        }
        y = Mathf.FloorToInt((worldPosition - originPosition).y / cellSize);
        if(y >= height-1){
            y = height-1;
        }else if(y <= 0){
            y = 0;
        }

    }

    public void SetValue(int x, int y, int value){
        if(x >= 0 && y >= 0 && x < width && y < height){
            if(value < 0){value = 0;}
            gridArray[x, y] = value;
        }
        else {
            int tempx = 0;
            int tempy = 0;
            if (x < 0){
                tempx = 0;
            } else if (x >= width){
                tempx = width-1;
            }

            if (y < 0){
                tempy = 0;
            } else if (y >= height){
                tempy = height-1;
            }
            
             gridArray[tempx,tempy] = value;
        }
    }

    public void SetValue(Vector3 worldPosition, int value){
        int x, y;
        GetXY(worldPosition, out x, out y);
        SetValue(x, y, value);

    }

    public int GetValue(int x, int y) {
        if(x >= 0 && y >= 0 && x < width && y < height){
            return gridArray[x , y];
        }else {
            /*
            int tempx = 0;
            int tempy = 0;
            if (x < 0){
                tempx = 0;
            } else if (x >= width){
                tempx = width-1;
            }

            if (y < 0){
                tempy = 0;
            } else if (y >= height){
                tempy = height-1;
            }
            */
            return 0;
        }
    }

    public int[,] GetKernel(int x, int y){
        if(x >= 1 && y >= 1 && x < width-1 && y < height-1){
            //int[,] internalKernel = new int[3,3];

    internalKernel[0, 2] = gridArray[x - 1, y + 1];  // Top left
    internalKernel[1, 2] = gridArray[x + 0, y + 1];  // Top center
    internalKernel[2, 2] = gridArray[x + 1, y + 1];  // Top right
    internalKernel[0, 1] = gridArray[x - 1, y + 0];  // Mid left
    internalKernel[1, 1] = gridArray[x + 0, y + 0];  // Current pixel
    internalKernel[2, 1] = gridArray[x + 1, y + 0];  // Mid right
    internalKernel[0, 0] = gridArray[x - 1, y - 1];  // Low left
    internalKernel[1, 0] = gridArray[x + 0, y - 1];  // Low center
    internalKernel[2, 0] = gridArray[x + 1, y - 1];  // Low right
    
     
    return internalKernel;

        }else {
            return zeroKernel;
        }
    }
    public int GetValue(Vector3 worldPosition) {
        int x, y;
        GetXY(worldPosition, out x, out y);
        return GetValue(x, y);
    }
    

    public int GetSum(){
        int freeNutes = gridArray.Cast<int>().Sum();
        return freeNutes;
    }

   
public Vector2 GetCellCenter(int x, int y){
    return GetWorldPosition(x,y);
}
   

}