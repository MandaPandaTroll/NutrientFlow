using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
public class InputFieldToFloat : MonoBehaviour
{

    public InputField inputField;
    public static string fieldText{get;set;}

    public static float value{get;set;}
    public float defaultValue = 0;
    // Start is called before the first frame update
    void Start()
    {
        value = defaultValue;
        inputField.text = defaultValue.ToString();

        if(fieldText != null){
            value = float.Parse(fieldText);
        }
        
    }

    // Update is called once per frame
    void Update()
    {
        fieldText = inputField.text;
        if(fieldText != null){
            value = float.Parse(fieldText);
        }
    }
}
