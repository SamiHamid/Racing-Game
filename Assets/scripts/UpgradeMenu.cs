using UnityEngine;
using System.Collections;

public class UpgradeMenu : MonoBehaviour
{
    public static UpgradeMenu instance;
    public GameObject[] upgrades;
    public GameObject selectionMarker;
    int index = 0;
    bool isEnabled = false;

    void Awake()
    {
        //create an instance
        instance = this;
    }

    // Use this for initialization
    void Start()
    {
        InitializeSelectionMarker();
        
    }

    // Update is called once per frame
    void Update()
    {
        if(isEnabled)
        EnableSelection();
    }

    // This function is called when the object becomes enabled and active
    public void OnEnable()
    {
        isEnabled = true;
    }

    public void InitializeSelectionMarker()
    {
        selectionMarker.transform.position = upgrades[0].transform.position;
    }

    public void EnableSelection()
    {

        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            index -= 2;

            if (index == -2)
            {
                index = 5;
                selectionMarker.transform.position = upgrades[index].transform.position;
            }
            else
            if (index == -1)
            {
                index = 4;
                selectionMarker.transform.position = upgrades[index].transform.position;
            }
            else
                selectionMarker.transform.position = upgrades[index].transform.position;
        }

        if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            index += 2;

            if (index == 6)
            {
                index = 1;
                selectionMarker.transform.position = upgrades[index].transform.position;
            }
            else
            if (index == 7)
            {
                index = 0;
                selectionMarker.transform.position = upgrades[index].transform.position;
            }
            else
                selectionMarker.transform.position = upgrades[index].transform.position;
        }


        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
   
            index -= 1;

            if (index == -1)
            {
                index = 5;
                selectionMarker.transform.position = upgrades[index].transform.position;
            }
            else
                selectionMarker.transform.position = upgrades[index].transform.position;
        }

        if (Input.GetKeyDown(KeyCode.RightArrow))
        {

            index += 1;

            if (index == 6)
            {
                index = 0;
                selectionMarker.transform.position = upgrades[index].transform.position;
            }
            else
                selectionMarker.transform.position = upgrades[index].transform.position;
        }
    }
}
