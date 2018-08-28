using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ValueManager : MonoBehaviour {

    public Material value1;
    public Material value2;
    public Material value3;
    public Material value4;
    public Material value5;
    public Material value6;
    public Material value7;
    public Material value8;
    public Material value9;
    public Material valueDefault;

    public void SetValue(CityObject node)
    {
        Material valMaterial; 
        switch (node.value)
        {
            case 1: valMaterial = value1; break;
            case 2: valMaterial = value2; break;
            case 3: valMaterial = value3; break;
            case 4: valMaterial = value4; break;
            case 5: valMaterial = value5; break;
            case 6: valMaterial = value6; break;
            case 7: valMaterial = value7; break;
            case 8: valMaterial = value8; break;
            case 9: valMaterial = value9; break;
            default: valMaterial = valueDefault; break;
        }
        node.form.GetComponent<Renderer>().material = valMaterial;
    }
}
