using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ProjectManager : MonoBehaviour {

    public ButtonCreator buttonCreator;


    /* * * Start-Function
     * Starts when the Scene is built
     * Load all URLs of the Projects on the Website and load them into a Root-Object
     * */
    [RuntimeInitializeOnLoadMethod]
    IEnumerator Start()
    {
        string startingUrl = "https://raw.githubusercontent.com/Vincethevince/CodeCityVR/master/data/projects/index.json";
        WWW myUrl = new WWW(startingUrl);
        yield return myUrl;

        if (myUrl.error != null)
        {
            Debug.Log("ERROR: " + myUrl.error);
        }
        else
        {
            Debug.Log("No Error");
            Debug.Log(myUrl.text);

            Root startProject = ProcessStart(myUrl.text);
            Debug.Log(startProject.projects[0].href);
            //for (int i = 0; i < startProject.projects.Length; i += 1) {
               StartCoroutine( GetUrlForJsonProject(startProject.projects[0].href));
            //}
            //Debug.Log(projectList[0].name);
        }
    }
    
    /* * * GetUrlForJsonProject-Function
     * Create an actual WWW Object from the URL given on the Root Object
     * */
    IEnumerator GetUrlForJsonProject(string myUrl) {
        WWW newUrl = new WWW(myUrl);
        yield return newUrl;
        Debug.Log(newUrl.text);
        LoadProjects(newUrl);
    }

    /* * * LoadProjects-Function
     * Create a JsonProject-Object from a given File on the Internet
     * Start the Function to create a button of that JsonProject-File
     */

    void LoadProjects(WWW url) {
        JsonProject jsonProjectToLoad = ProcessJson(url.text);
        //projectList[counter] = new ProjectList(jsonProject);
        //Debug.Log(projectList[0].name);

        Debug.Log(jsonProjectToLoad.name);
        buttonCreator.createButtons(jsonProjectToLoad);
    }
    
    /* * * ProcessJson-Function
     * Deserialize a JsonFile into a JsonProject-Object
     */
    private JsonProject ProcessJson(string jsonString)
    {
        JsonProject parsejson = JsonUtility.FromJson<JsonProject>(jsonString);
        return parsejson;
    }


    /* * * ProcessStart-Function
     * Deserialize a JsonFile into a Root-Object -- The first Object
     */
    private Root ProcessStart(string jsonString)
    {
        Root startProject = JsonUtility.FromJson<Root>(jsonString);
        return startProject;
    }
}

