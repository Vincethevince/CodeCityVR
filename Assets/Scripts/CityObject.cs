using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CityObject : MonoBehaviour {

    private GameObject prefab;

    private CityObject parentCityObject;
    private ArrayList children;


    public CityObject(GameObject gameObject, CityObject cityObjectParent)
    {
        prefab = Instantiate(gameObject);
        parentCityObject = cityObjectParent;
    }

    public void appendChildren(CityObject cityObject)
    {
        this.children.Add(cityObject);
    }

    ~CityObject()
    {

    }
}
