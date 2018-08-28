using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CityObject : MonoBehaviour {

    public GameObject form;

    public CityObject parent;
    public List<CityObject> children;

    public double width;
    public int height;
    public int value;


    public CityObject(GameObject prefab, CityObject cityObjectParent, double newWidth, int newHeight, int newValue)
    {
        form = Instantiate(prefab);
        parent = cityObjectParent;
        children = new List<CityObject>();
        width = newWidth;
        height = newHeight;
        value = newValue;
    }

    public void AppendChild(CityObject cityObject)
    {
        children.Add(cityObject);
        cityObject.form.transform.SetParent(this.form.transform);
    }

    public void DestroyCityObject()
    {
        DestroyImmediate(form);
        DestroyImmediate(this);
    }

    
}
