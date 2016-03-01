using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class UpgradeMenu : MonoBehaviour
{
    public class Price
    {
        public int nitro = 10;
        public int engine = 80;
        public int tires = 40;
        public int speedometer = 100;
        public int shocks = 60;
    }

    public Price price;
   // public static UpgradeMenu instance;
    public static RaceManager race_manager_instance;

    public GameObject[] upgrades;
    public GameObject selectionMarker;

    public int menu_index = 0;
    public int current_track_index;
   
   
    public bool isEnabled = false;


    void Awake()
    {
        //create an instance
       // instance = this;
        current_track_index = SceneManager_.instance.GetTrackIndex();
        price = new Price();
        race_manager_instance = RaceManager.instance;
        
    }

    // Use this for initialization
    void Start()
    {
        InitializeSelectionMarker();
        
        race_manager_instance.raceCompleted = false;
        Debug.Log("Starting budget: " + PlayerData.currency);
        Debug.Log("Current track index: " + current_track_index);



    }

    // Update is called once per frame
    void Update()
    {
        if (isEnabled)
        {
            MenuInput();
            
        }
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

    public void MoveMarker()
    {

        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            menu_index -= 2;

            if (menu_index == -2)
            {
                menu_index = 5;
                selectionMarker.transform.position = upgrades[menu_index].transform.position;
            }
            else
            if (menu_index == -1)
            {
                menu_index = 4;
                selectionMarker.transform.position = upgrades[menu_index].transform.position;
            }
            else
                selectionMarker.transform.position = upgrades[menu_index].transform.position;

            
        }

        if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            menu_index += 2;

            if (menu_index == 6)
            {
                menu_index = 1;
                selectionMarker.transform.position = upgrades[menu_index].transform.position;
            }
            else
            if (menu_index == 7)
            {
                menu_index = 0;
                selectionMarker.transform.position = upgrades[menu_index].transform.position;
            }
            else
                selectionMarker.transform.position = upgrades[menu_index].transform.position;
        }


        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {

            menu_index -= 1;

            if (menu_index == -1)
            {
                menu_index = 5;
                selectionMarker.transform.position = upgrades[menu_index].transform.position;
            }
            else
                selectionMarker.transform.position = upgrades[menu_index].transform.position;
        }

        if (Input.GetKeyDown(KeyCode.RightArrow))
        {

            menu_index += 1;

            if (menu_index == 6)
            {
                menu_index = 0;
                selectionMarker.transform.position = upgrades[menu_index].transform.position;
            }
            else
                selectionMarker.transform.position = upgrades[menu_index].transform.position;
        }
    }

    public int GetIndexNumber()
    {
        return menu_index;
    }

    public void MenuInput()
    {
        MoveMarker();

        if (Input.GetKeyDown(KeyCode.Return))
        {
            if (PlayerData.currency >= 0)
            {
                switch (menu_index)
                {
                    case 0:
                        if (PlayerData.currency >= price.nitro)
                            PlayerData.DeductCurrency(price.nitro);
                        else
                            Debug.Log("You have insufficient budget to buy any upgrade");

                        break;
                    case 1:
                        if (PlayerData.currency >= price.engine)
                            PlayerData.DeductCurrency(price.engine);
                        else
                            Debug.Log("You have insufficient budget to buy any upgrade");

                        //Debug.Log("Budget: " + PlayerData.currency);
                        break;
                    case 2:
                        if (PlayerData.currency >= price.tires)
                            PlayerData.DeductCurrency(price.tires);
                        else
                            Debug.Log("You have insufficient budget to buy any upgrade");

                        //Debug.Log("Budget: " + PlayerData.currency);
                        break;
                    case 3:
                        if (PlayerData.currency >= price.speedometer)
                            PlayerData.DeductCurrency(price.speedometer);
                        else
                            Debug.Log("You have insufficient budget to buy any upgrade");

                        //Debug.Log("Budget: " + PlayerData.currency);
                        break;
                    case 4:
                        if (PlayerData.currency >= price.shocks)
                            PlayerData.DeductCurrency(price.shocks);
                        else
                            Debug.Log("You have insufficient budget to buy this upgrade");

                        //Debug.Log("Budget: " + PlayerData.currency);
                        break;
                    case 5:
                        StartCoroutine(SceneManager_.instance.LoadNextTrack());
                        break;
                }
                
                Debug.Log("Budget: " + PlayerData.currency);
            }
            else
                Debug.Log("You have insufficient budget to buy any upgrades");
        }
    }

    public void BuyUpgrade()
    {

    } 
}
