using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CityObject : MonoBehaviour {

    public GameObject form;

    private CityObject parent;
    public ArrayList children;

    public int width;


    public CityObject(GameObject prefab, CityObject cityObjectParent, int newWidth)
    {
        form = Instantiate(prefab);
        parent = cityObjectParent;
        children = new ArrayList();
        width = newWidth;
    }

    public void appendChild(CityObject cityObject)
    {
        children.Add(cityObject);
        cityObject.form.transform.SetParent(this.form.transform);
    }

    public void destroyCityObject()
    {
        DestroyImmediate(gameObject);
        DestroyImmediate(this);
    }

    
}
