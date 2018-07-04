using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CityObject : MonoBehaviour {

    private GameObject prefab;

    private CityObject parentCityObject;
    public ArrayList children;


    public CityObject(GameObject gameObject, CityObject cityObjectParent)
    {
        prefab = Instantiate(gameObject);
        prefab.transform.SetParent(cityObjectParent.transform);
        parentCityObject = cityObjectParent;
        children = new ArrayList();
    }

    public void appendChildren(CityObject cityObject)
    {
        children.Add(cityObject);
    }

    public void destroyCityObject()
    {
        DestroyImmediate(prefab);
        DestroyImmediate(this);
    }
}
