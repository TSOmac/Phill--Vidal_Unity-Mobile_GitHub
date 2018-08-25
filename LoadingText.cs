using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class LoadingText : MonoBehaviour
{
    private bool loadDone;
    private Text connText;
    public Text [] Digit;
    private bool isLoading;
    private float delay;

    void Start()
    {
        loadDone = false;
        isLoading = true;
        connText = gameObject.GetComponent<Text>();
        Digit = GetComponent<LoadingText>().Digit;
        delay = 0f;
    }

    void Update()
    {
        if(delay > 0.5f)
        {
            // Reset Delay
            delay = 0;

            // Update Header Text
            if(isLoading)
                connText.text = "Loading";
            else
                connText.text = "Saving";

            // Update Digit Text with Random Numbers Between 0 && 1
            for(int i = 0; i < Digit.Length - 1; i++)
            {
                Digit[i].text = Digit[i + 1].text;
                Digit[Digit.Length - 1].text = Random.Range(0, 2).ToString();
            }
        }

        // Update Delay
        delay += Time.deltaTime * 10.0f;
    }

    /// <summary>
    /// Set status of Load / Save
    /// </summary>
    /// <param name="done">true == Complete | false == Incomplete</param>
    public void LoadDone(bool done)
    {
        loadDone = done;
    }

    /// <summary>
    /// Set whether to display Loading / Saving text... 
    /// <param name="IsLoading">true == Loading | false == Saving.</param>
    /// </summary>
    public void SetLoadState(bool IsLoading)
    {
        isLoading = IsLoading;
    }
}
