using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CityObject : MonoBehaviour {

    public GameObject form;

    public CityObject parent;
    public ArrayList children;

    public int width;
    public int height;
    public int value;


    public CityObject(GameObject prefab, CityObject cityObjectParent, int newWidth, int newHeight, int newValue)
    {
        form = Instantiate(prefab);
        parent = cityObjectParent;
        children = new ArrayList();
        width = newWidth;
        height = newHeight;
        value = newValue;
    }

    public void appendChild(CityObject cityObject)
    {
        children.Add(cityObject);
        cityObject.form.transform.SetParent(this.form.transform);
    }

    public void destroyCityObject()
    {
        DestroyImmediate(form);
        DestroyImmediate(this);
    }

    
}
