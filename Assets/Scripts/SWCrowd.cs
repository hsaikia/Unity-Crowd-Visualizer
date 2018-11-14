using System.Collections;
using System.IO;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Agent {
    public float r;
    public float g;
    public float b;
    public float radius;
    public List<Vector3> pos;
    public List<Vector3> tar;

    Vector3 forw_;

    public Agent(){
        pos = new List<Vector3>();
        tar = new List<Vector3>();
    }

    public Vector3 getForward(int t){
        Vector3 newforw;
        if(t == 0){
           newforw = tar[0] - pos[0];
        } else {
            newforw = pos[t] - pos[t - 1];
        }
        if(newforw.magnitude < 0.00001){
            return forw_;
        }
        forw_ = newforw;
        return forw_;
    }
}

public class SWCrowd : MonoBehaviour
{
    public bool showInfo;
    public GameObject walkerPrefab_;
    public GameObject targetPrefab_;
    public TextAsset csvFile_;
    private int numAgents_;
    private List<Agent> agents_;
    private List<GameObject> walkerAgents_;
    private List<GameObject> targetAgents_;
    public Text info_;
    private int columns_;
    private int timestep;
    private int maxTimeStep;

    private int totalCollisions;
    public bool animate;

    [Range (0, 100)]
    public int trailLength;

    void Start()
    {
        agents_ = new List<Agent>();
        walkerAgents_ = new List<GameObject>();
        targetAgents_ = new List<GameObject>();
        readCSV();

        if(animate){
            InvokeRepeating("moveAgents", 0.0f, 0.1f);
        }
        //InvokeRepeating("moveAgents", 0.0f, 0.1f);
    }

    void Update()
    {
        
    }

    void OnDrawGizmos(){

    }

    void readCSV(){
        char lineSeparator = '\n'; // It defines line seperate character
        char fieldSeparator = ','; // It defines field seperate chracter

        List<List<float> > dataitems = new List<List<float> >(); 

        string[] records = csvFile_.text.Split(lineSeparator);

        bool ignoreFirst = true;

        foreach (string record in records)
        {
            string[] fields = record.Split(fieldSeparator);
            //Debug.Log(record);
            if(ignoreFirst){
                columns_ = fields.GetLength(0);
                ignoreFirst = false;
                continue;
            }

            //Debug.Log("Columns extracted " + fields.GetLength(0));

            if(fields.GetLength(0) != columns_){
                continue;
            }

            List<float> row = new List<float>();

            int c = 0;
            foreach(string field in fields)
            {
                if(c >= columns_){
                    break;
                }
                //Debug.Log("Field " + field);
                row.Add(float.Parse(field));       
                c++;
            }

            dataitems.Add(row);
        }
        
        for(int i = 0; i < dataitems.Count; i++){
            // TIME ,ID ,POS_X ,POS_Y, TAR_X, TAR_Y, AGENT_RADIUS, COLOR_R, COLOR_G, COLOR_B 
            int id = Convert.ToInt32(dataitems[i][1]);
            int time = Convert.ToInt32(dataitems[i][0]);

            maxTimeStep = Mathf.Max(maxTimeStep, time);

            while(id + 1 > agents_.Count){
                agents_.Add(new Agent());
            }
            
            agents_[id].pos.Add(new Vector3(dataitems[i][2], 0, dataitems[i][3]));
            agents_[id].tar.Add(new Vector3(dataitems[i][4], 0, dataitems[i][5]));
            agents_[id].radius = dataitems[i][6];
            agents_[id].r = dataitems[i][7];
            agents_[id].g = dataitems[i][8];
            agents_[id].b = dataitems[i][9];
        }

        numAgents_ = agents_.Count;

        // set agents initial position

        SetGameObjects();
    }

    void UpdateInfo(){
        info_.text = "B.o.B Do something!";
    }

    void ClearScene(){

        foreach(var wa in walkerAgents_){
            Destroy(wa);
        }

        walkerAgents_.Clear();

        foreach(var tar in targetAgents_){
            Destroy(tar);
        }

        targetAgents_.Clear();
    }

    void SetGameObjects()
    {
        ClearScene();
        for(int i = 0; i < numAgents_; i++)
        {
            GameObject walkerClone = Instantiate(walkerPrefab_, agents_[i].pos[0] , Quaternion.identity);
            walkerClone.GetComponent<MeshRenderer>().material.color = new Color(agents_[i].r, agents_[i].g, agents_[i].b);
            walkerClone.transform.localScale = new Vector3(2 * agents_[i].radius, 1, 2 * agents_[i].radius);
            walkerClone.transform.forward = agents_[i].getForward(0);
            walkerClone.SetActive(true);
            //line.startColor = new Color(agents_[i].r, agents_[i].g, agents_[i].b);
            //line.endColor = new Color(agents_[i].r, agents_[i].g, agents_[i].b);
            walkerClone.AddComponent(typeof(LineRenderer));
            walkerClone.GetComponent<LineRenderer>().material = new Material(Shader.Find("Sprites/Default"));
            walkerClone.GetComponent<LineRenderer>().startColor = new Color(agents_[i].r, agents_[i].g, agents_[i].b);
            walkerClone.GetComponent<LineRenderer>().widthMultiplier = 0.2f;

            GameObject targetClone = Instantiate(targetPrefab_, agents_[i].tar[0], Quaternion.identity);
            targetClone.GetComponent<MeshRenderer>().material.color = new Color(agents_[i].r, agents_[i].g, agents_[i].b);
            targetClone.transform.localScale = new Vector3(2 * agents_[i].radius, 1, 2 * agents_[i].radius);
            targetClone.SetActive(true);

            walkerAgents_.Add(walkerClone);
            targetAgents_.Add(targetClone);
        }
        totalCollisions = 0;
    }

    void moveAgents(){
        info_.text = "Timestep " + timestep;
        timestep = (timestep + 1) % maxTimeStep;
        for(int i = 0; i < numAgents_; i++)
        {
            walkerAgents_[i].transform.SetPositionAndRotation(agents_[i].pos[timestep], Quaternion.identity);
            walkerAgents_[i].transform.forward = agents_[i].getForward(timestep);
            targetAgents_[i].transform.SetPositionAndRotation(agents_[i].tar[timestep], Quaternion.identity);

            List<Vector3> poss = new List<Vector3>();

            for(int t = timestep; t >= 0; t--){
                if(poss.Count >= trailLength){
                    break;
                }
                poss.Add(agents_[i].pos[t]);
            }

            walkerAgents_[i].GetComponent<LineRenderer>().positionCount = poss.Count;
            walkerAgents_[i].GetComponent<LineRenderer>().SetPositions(poss.ToArray());
        }

        // find number of collisions
        int collisions = 0;
        for(int i = 0; i < numAgents_; i++)
        {
            for(int j = i + 1; j < numAgents_; j++)
            {
                if((walkerAgents_[i].transform.position - walkerAgents_[j].transform.position).magnitude < 2 * agents_[i].radius){
                    collisions++;
                    totalCollisions++;
                }
            }
        }

        info_.text = "Timestep : " + timestep + "\nCollisions : " + collisions + "\nTotal Collisions : "  + totalCollisions;
    }

}