using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckPointScript : MonoBehaviour
{

    private List<Transform> checkPoints = new List<Transform>();
    private GameObject checkPointHolder;

    private void Awake()
    {
        checkPointHolder = GameObject.FindGameObjectWithTag("CPholder");

        for (int i = 0; i < checkPointHolder.transform.childCount; i++)
        {
            Debug.Log("test: " + i);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
