using UnityEngine;
using System.Collections;

public class CheckInternetConnection : MonoBehaviour
{
    public GameObject conxStatusPanel;
    private bool conxActive;


    public static CheckInternetConnection Instance
    {
        get;
        set;
    }

    // Initialize levels
    void Awake()
    {
        // Stop this game object from being Destroyed when the scene loads
        if(Instance == null)
        {
            DontDestroyOnLoad(gameObject);
            Instance = this;
        }
        else if(Instance != null)
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        conxActive = true;
        StartCoroutine("PeriodicConnectionCheck");
    }

    // Check for internet connection and return it's status
    private IEnumerator PeriodicConnectionCheck()
    {
        if(NetworkAvailable())
        {
            conxActive = true;
            conxStatusPanel.SetActive(false);
            // pause the game here
        }
        else
        {
            conxActive = false;
            conxStatusPanel.SetActive(true);
            // resume the game here
        }

        yield return new WaitForSeconds(2.0f);

        StartCoroutine("PeriodicConnectionCheck");
    }

    public bool NetworkAvailable()
    {
        if(Application.internetReachability == NetworkReachability.NotReachable)
        {
            return false;
        }

        return true;
    }

}
