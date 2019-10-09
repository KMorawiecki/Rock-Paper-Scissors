using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.PostProcessing;
using UnityEngine;
using System;

public class GameManager : MonoBehaviour
{
    public GameObject p1prefab;     //variables to store player1 and player2 icon prefabs
    public GameObject p2prefab;
    public GameObject gestureSelector;      //variable to store prefab for panel for selecting gestures
    public GameObject continueButton;      
    public GameObject resetButton;
    public Canvas myCanvas;
    public PostProcessingProfile profile;   
    public Text p1pointsInfo;
    public Text p2pointsInfo;  

    private int p1points;
    private int p2points;
    private int activePlayer;           //flag determining whose round it is
    private bool ungrainSceneFlag = false;      //flags to change post processing settings of the scene
    private bool sharpenSceneFlag = false;
    private Gesture p1play;             //player1 and player2 play choices
    private Gesture p2play;
    private GameObject gestureSelectorInstance;     //variable to store panel for selecting gestures
    private Text instruction;                       //selection panel text
    private GameObject p1instance;      //variables to store player1 and player2 icons
    private GameObject p2instance;
    private Animator p1animator;
    private Animator p2animator;

    private void Start()
    {
        //create player icons
        p1instance = Instantiate(p1prefab, new Vector3(-70, 0, 0), Quaternion.identity);
        p2instance = Instantiate(p2prefab, new Vector3(70, 0, 0), Quaternion.identity);

        //point icons' animators to variables
        p1animator = p1instance.GetComponentInChildren<Animator>();
        p2animator = p2instance.GetComponentInChildren<Animator>();

        activePlayer = 1;
        CreateSelectionPanel();

        p1points = 0;
        p2points = 0;

        //set starting post proccessing settings
        BluntScene();
    }

    private void Update()
    {
        //if animation is played, gradualy sharpen the view by changing depth of field and grain settings from post proccessing
        if(ungrainSceneFlag)
        {
            var newGrainValue = profile.grain.settings;
            newGrainValue.intensity -= 0.01f;
            profile.grain.settings = newGrainValue;

            //stop when the scene is clear
            if (newGrainValue.intensity <= 0)
                ungrainSceneFlag = false;
        }

        if (sharpenSceneFlag)
        {
            var newDepthValue = profile.depthOfField.settings;
            newDepthValue.aperture += 0.0475f;
            profile.depthOfField.settings = newDepthValue;

            //stop when the scene is clear
            if (newDepthValue.aperture >= 2f)
                sharpenSceneFlag = false;
        }
    }

    private void CreateSelectionPanel()
    {
        Text instruction;

        //instantiate gesture selection panel and set its transform parent to canvas
        gestureSelectorInstance = Instantiate(gestureSelector, new Vector3(0, 0, 0), Quaternion.identity);
        gestureSelectorInstance.transform.SetParent(myCanvas.transform);
        gestureSelectorInstance.transform.localPosition = Vector3.zero;

        for (int i = 0; i < gestureSelectorInstance.transform.childCount; i++)
        {
            //for each button on panel, pin this game manager to its GameManager variable
            GestureButton button;
            if (gestureSelectorInstance.transform.GetChild(i).gameObject.GetComponent<GestureButton>() != null)
            {
                button = gestureSelectorInstance.transform.GetChild(i).gameObject.GetComponent<GestureButton>();
                button.SetGameManager(this);
            }
        }

        //update instruction text
        instruction = gestureSelectorInstance.GetComponentInChildren<Text>();
        instruction.text = "Player " + activePlayer + ", please choose your play.";

        //disable continue and reset buttons
        HideUtilityButtons();
    }

    //after choosing to continue, enable selection panel again
    public void ShowSelectionPanel()
    {
        gestureSelectorInstance.SetActive(true);

        //reset icons' positions
        p1animator.SetTrigger("goToStart");
        p2animator.SetTrigger("goToStart");

        //update instruction text
        instruction = gestureSelectorInstance.GetComponentInChildren<Text>();
        instruction.text = "Player " + activePlayer + ", please choose your play.";

        //disable continue and reset buttons
        HideUtilityButtons();
        //set default post processing settings
        BluntScene();
    }

    //after choosing gestures disable selection panel
    private void HideSelectionPanel()
    {
        gestureSelectorInstance.SetActive(false);
    }

    //enable continue and reset buttons
    private void ShowUtilityButtons()
    {
        continueButton.SetActive(true);
        resetButton.SetActive(true);
    }

    //disable continue and reset buttons
    private void HideUtilityButtons()
    {
        continueButton.SetActive(false);
        resetButton.SetActive(false);
    }

    public void SetGesture(Gesture selectedGesture)
    {
        switch(activePlayer)
        {
            case 1:
                p1play = selectedGesture;
                break;
            case 2:
                p2play = selectedGesture;
                break;
            default:
                Debug.Log("Error: wrong player number");
                break;
        }

        UpdateTurn();
    }

    private void UpdateTurn()
    {
        if (activePlayer == 1)
        {
            //if first person played, enable panel for the second player
            activePlayer = 2;
            ShowSelectionPanel();
        }
        else
        {
            //if second person played, disable panel and animate icons
            activePlayer = 1;
            AnimateTurn();
            SettleTheGame();
        }
    }

    private void AnimateTurn()
    {
        //reset triggers, so icons won't go back to starting positions after animation is finished
        p1animator.ResetTrigger("goToStart");
        p2animator.ResetTrigger("goToStart");

        //set flags to clear the scene by gradually changing post proccessing settings
        ungrainSceneFlag = true;
        sharpenSceneFlag = true;

        //play animation depending on previously selected gesture
        switch (p1play)
        {
            case Gesture.Rock:
                p1animator.SetTrigger("playRock");
                break;
            case Gesture.Paper:
                p1animator.SetTrigger("playPaper");
                break;
            case Gesture.Scissors:
                p1animator.SetTrigger("playScissors");
                break;
        }

        switch (p2play)
        {
            case Gesture.Rock:
                p2animator.SetTrigger("playRock");
                break;
            case Gesture.Paper:
                p2animator.SetTrigger("playPaper");
                break;
            case Gesture.Scissors:
                p2animator.SetTrigger("playScissors");
                break;
        }
    }

    //function to reset post proccesing settings of the scene 
    private void BluntScene()
    {
        ungrainSceneFlag = false;
        sharpenSceneFlag = false;

        var newGrainValue = profile.grain.settings;
        newGrainValue.intensity = 0.4f;
        profile.grain.settings = newGrainValue;

        var newDepthValue = profile.depthOfField.settings;
        newDepthValue.aperture = 0.1f;
        profile.depthOfField.settings = newDepthValue;
    }

    private void SettleTheGame()
    {
        //calculate the winner of the active round
        if (p1play != p2play)
        {
            if (p1play - p2play == 1 || p1play - p2play == -2)
                p1points++;
            else
                p2points++;
        }

        HideSelectionPanel();
        ShowUtilityButtons();
        UpdateScore();
    }

    //update information on the screen
    private void UpdateScore()
    {
        p1pointsInfo.text = "P1 points: " + p1points;
        p2pointsInfo.text = "P2 points: " + p2points;
    }

    //reset icons positions and player points
    public void ResetGame()
    {
        p1points = 0;
        p2points = 0;

        p1animator.SetTrigger("goToStart");
        p2animator.SetTrigger("goToStart");

        UpdateScore();
    }

    public int GetActivePlayer()
    {
        return activePlayer;
    }
}
