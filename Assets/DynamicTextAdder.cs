
using UnityEngine;
using UnityEngine.UI;

public class DynamicTextAdder : MonoBehaviour
{
    Text text;
    // Start is called before the first frame update
    void Start()
    {
        text = GetComponent<Text>();
    }

    // Update is called once per frame
    void Update()
    {
        
            text.text = nutrientStats.totalNutrients.ToString();
        
        
    }
}
