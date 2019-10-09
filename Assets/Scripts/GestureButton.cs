using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class GestureButton : Button, IPointerEnterHandler, IPointerExitHandler
{
    private GameManager gameManager;
    private Gesture gesture;
    private Image sprite;

    protected override void Start()
    {
        sprite = gameObject.GetComponentInChildren<Image>();

        //set gesture which corresponds with this button
        if (name == "rockButton")
            gesture = Gesture.Rock;
        if (name == "paperButton")
            gesture = Gesture.Paper;
        if (name == "scissorsButton")
            gesture = Gesture.Scissors;
    }

    public void SetGameManager(GameManager manager)
    {
        gameManager = manager;
    }

    public void OnMouseClick()
    {
        //transfer information about chosen gesture to game manager
        gameManager.SetGesture(gesture);
        EventSystem.current.SetSelectedGameObject(null);

        if (gameManager.GetActivePlayer() == 1)
        {
            //change button color and scale to normal after selection panel is disabled (so it isn't highlighted after selection panel is enabled again)
            gameObject.transform.localScale = new Vector3(2f, 2f, 2f);
            sprite.color = Color.white;
        }
    }

    public override void OnPointerEnter(PointerEventData eventData)
    {
        //highlight and enlarge button when mouse pointer is floating above it
        gameObject.transform.localScale = new Vector3(2.2f, 2.2f, 2.2f);
        sprite.color = Color.green;
    }

    public override void OnPointerExit(PointerEventData eventData)
    {
        //go back to normal scale and color after mouse pointer is moved away from button
        gameObject.transform.localScale = new Vector3(2f, 2f, 2f);
        sprite.color = Color.white;
    }

}
