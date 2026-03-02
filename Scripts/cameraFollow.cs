using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using TMPro;

public class cameraFollow : MonoBehaviour
{
    public float timeRemaining = 5;
    public bool timerIsRunning = false;
    public GameObject startScreen;
    public GameObject tutorialScreen;
    public GameObject creditsScreen;
    public GameObject player; 
    public CinemachineCamera cf; 
    public Button start1Button;
    public Button backButton1;
    public Button backButton2;
    public Button backButton3;
    public Button tutorialButton;
    public Button creditsButton;
    public Canvas uiCanvasLive;
    public TMP_Text timerText;
    public TMP_Text dieText;
    public TMP_Text winText;
    public Button menuButton;
    public GameObject ff;

    public GameObject menuScreen;

    public Rigidbody2D ffrb;

    public GameObject[] fishes;

    void Start()
    {
        player.GetComponent<movement>().enabled = false;
        start1Button.onClick.AddListener(start1ButtonClicked);
        backButton1.onClick.AddListener(backButtonClicked);
        backButton2.onClick.AddListener(backButtonClicked);
        backButton3.onClick.AddListener(backButtonClicked);
        tutorialButton.onClick.AddListener(tutorialButtonClicked);
        creditsButton.onClick.AddListener(creditsButtonClicked);
        menuButton.onClick.AddListener(menuButtonClicked);
        cf.Follow = startScreen.transform;
        uiCanvasLive.enabled = false;
        ffrb.gravityScale = 0;
        dieText.enabled = false;
        winText.enabled = false;
        GameObject fishContainer = GameObject.Find("FISHESSS");
        if (fishContainer != null)
        {
            fishes = new GameObject[fishContainer.transform.childCount];
            for (int i = 0; i < fishContainer.transform.childCount; i++)
            {
                fishes[i] = fishContainer.transform.GetChild(i).gameObject;
            }
        }
        //disable all fish at start
        foreach (GameObject fish in fishes)
        {
            fish.SetActive(false);
        }
    }

    private void Update()
    {
        if (timerIsRunning)
        {
            if (timeRemaining > 0)
            {
                timeRemaining -= Time.deltaTime;
                //turn seconds to seconds and minutes
                int minutes = Mathf.FloorToInt(timeRemaining / 60f);
                int seconds = Mathf.FloorToInt(timeRemaining % 60f);
                timerText.text = minutes + ":" + seconds;
            } else
            {
                timeRemaining = 0;
                timerIsRunning = false;
                player.GetComponent<movement>().enabled = false;
                //get text component of the ui canvas live and set it to "You Died"
                StartCoroutine(dieCoroutine());
            }
        }
    }
    void start1ButtonClicked()
    {
        player.transform.position = new Vector3(0, 0, 0);
        cf.Follow = ff.transform;
        //do this without stalling the whole game, maybe with a coroutine?
        ffrb.gravityScale = 0.10f;
        StartCoroutine(cameraFollowCoroutine0());
    }
    void backButtonClicked()
    {
        cf.Follow = startScreen.transform;
    }
    void tutorialButtonClicked()
    {
        cf.Follow = tutorialScreen.transform;
    }
    void creditsButtonClicked()
    {
        cf.Follow = creditsScreen.transform;
    }

    void menuButtonClicked()
    {
        cf.Follow = menuScreen.transform;
    }
    IEnumerator cameraFollowCoroutine()
    {
        while (Vector3.Distance(cf.transform.position, player.transform.position) > 10.1f) 
        { 
            //debug the distance
            Debug.Log(Vector3.Distance(cf.transform.position, player.transform.position));
            yield return null; 
        }
        yield return new WaitForSeconds(1f);

        player.GetComponent<movement>().enabled = true;
        timerIsRunning = true;
        uiCanvasLive.enabled = true;
        //enable all fish
        foreach (GameObject fish in fishes)
        {
            fish.SetActive(true);
        }
        
    }

    IEnumerator cameraFollowCoroutine0()
    {
        while (Vector3.Distance(cf.transform.position, ff.transform.position) > 10.1f) 
        { 
            //debug the distance
            Debug.Log(Vector3.Distance(cf.transform.position, ff.transform.position));
            yield return null; 
        }
        yield return new WaitForSeconds(3f);
        cf.Follow = player.transform;
        StartCoroutine(cameraFollowCoroutine());
        yield return new WaitForSeconds(1f);
        ffrb.gravityScale = 0;
        
    }
    IEnumerator dieCoroutine()
    {
        //disable all fish, skip if gone already
        for (int i = 0; i < fishes.Length; i++)
        {
            if (fishes[i] != null)
            {
                fishes[i].SetActive(false);
            }
        }
        dieText.enabled = true;
        yield return new WaitForSeconds(3f);
        cf.Follow = startScreen.transform;
        dieText.enabled = false;
        uiCanvasLive.enabled = false;
    }

    IEnumerator winCoroutine()
    {
        //disable all fish, skip if gone already
        for (int i = 0; i < fishes.Length; i++)
        {
            if (fishes[i] != null)
            {
                fishes[i].SetActive(false);
            }
        }
        winText.enabled = true;
        yield return new WaitForSeconds(5f);
        cf.Follow = startScreen.transform;
        winText.enabled = false;
        uiCanvasLive.enabled = false;
    }

    public void dieStart()
    {
        dieText.enabled = true;
        timeRemaining = 0;
        timerIsRunning = false;
        player.GetComponent<movement>().enabled = false;
        StartCoroutine(dieCoroutine());
    }
    public void winStart()
    {
        winText.enabled = true;
        timeRemaining = 0;
        timerIsRunning = false;
        player.GetComponent<movement>().enabled = false;
        StartCoroutine(winCoroutine());
    }
}