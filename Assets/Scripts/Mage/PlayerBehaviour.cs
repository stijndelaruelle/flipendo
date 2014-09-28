using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PlayerBehaviour : Mage
{
    private Rect m_DrawArea;

    private bool m_SpellCharged = false;
    Vector3 m_StartDragPos;

    private string m_NewGestureName;
      
	//Constructor
	public PlayerBehaviour() : base(){}

	override protected void Start()
	{
        base.Start();

        //As there will always be only 1 enemy, set him as the target immediatly
		Target = GameObject.Find("Enemy").GetComponent<Mage>();

        //Create a draw area
        m_DrawArea = new Rect(400, 0, Screen.width - 800, Screen.height);
        m_NewGestureName = "";
	}

	override protected void Update()
	{
        base.Update();

		Vector3 virtualKeyPosition = Vector3.zero;

		//Get the input
		if (Application.platform == RuntimePlatform.Android)
		{
            if (Input.touchCount > 0)
            {
                virtualKeyPosition = new Vector3(Input.GetTouch(0).position.x, Input.GetTouch(0).position.y);
            }
		}
		else
		{
            if (Input.GetMouseButtonDown(0) || Input.GetMouseButtonUp(0) || Input.GetMouseButton(0))
            {
                virtualKeyPosition = new Vector3(Input.mousePosition.x, Input.mousePosition.y);
            }
		}

        //Right click = cancel
        if (Input.GetMouseButtonUp(1))
        {
            CancelSpell();
            m_SpellCharged = false;
        }

		//If we are clicking inside of the drawarea either cast or draw the spell
		if (m_DrawArea.Contains(virtualKeyPosition))
		{
            if (m_SpellCharged) AimSpell(virtualKeyPosition);
            else                DrawSpell(virtualKeyPosition);
		}
	}

    private void AimSpell(Vector3 virtualKeyPosition)
    {
        //if (!m_SpellCharged) return;

        Vector2 minSwipeDist = new Vector2(100, 100);

        //Left mouse down: Start swiping
        if (Input.GetMouseButtonDown(0))
        {
            ClearLineRenderer();
            m_StartDragPos = virtualKeyPosition;
        }

        //Hold left mouse button: Draw a trail!
        if (Input.GetMouseButton(0))
        {
            AddLinePoint(WorldCoordinateForGesturePoint(virtualKeyPosition));
        }

        //Left mouse up: stop swiping & cast spell
        if (Input.GetMouseButtonUp(0))
        {
            ClearLineRenderer();

            float swipeDistHorizontal = (new Vector3(virtualKeyPosition.x, 0, 0) - new Vector3(m_StartDragPos.x, 0, 0)).magnitude;

            if (swipeDistHorizontal > minSwipeDist.x)
            {
                float swipeValue = Mathf.Sign(virtualKeyPosition.x - m_StartDragPos.x);

                //left swipe
                if (swipeValue < 0)
                {
                    Debug.Log("SWIPE LEFT");
                    CastSpell(this);
                }

                //right swipe
                else
                {
                    Debug.Log("SWIPE RIGHT");
                    CastSpell(Target);
                }

                m_SpellCharged = false;
                return;
            }

            float swipeDistVertical = (new Vector3(0, virtualKeyPosition.y, 0) - new Vector3(0, m_StartDragPos.y, 0)).magnitude;

            if (swipeDistVertical > minSwipeDist.y)
            {
                float swipeValue = Mathf.Sign(virtualKeyPosition.y - m_StartDragPos.y);

                //up swipe
                if (swipeValue > 0)
                {
                    Debug.Log("SWIPE UP");
                }

                //down swipe
                else
                {
                    Debug.Log("SWIPE DOWN");
                }

                m_SpellCharged = false;
                return;
            }
        }
    }

    private void DrawSpell(Vector3 virtualKeyPosition)
    {
        //if (m_SpellCharged) return;

        //Left mouse down: Start drawing a new spell
        if (Input.GetMouseButtonDown(0))
        {
            //Clear all data
            ClearLineRenderer();

            //Create the spell -> shows visuals on the player's staff
            CreateSpell();
        }
			
        //Hold left mouse button: Keep on drawing the spell
        if (Input.GetMouseButton(0))
        {
            m_Points.Add(new Vector2(virtualKeyPosition.x, virtualKeyPosition.y));
            AddLinePoint(WorldCoordinateForGesturePoint(virtualKeyPosition));
        }
			
        //Left mouse release: Our spell is drawn, wait for a swipe to cast it!
        if (Input.GetMouseButtonUp(0))
        {
            Gesture gesture = new Gesture(m_Points);
            Result result = gesture.Recognize(m_GestureLibrary, false);

            Debug.Log(result.Name + " score: " + result.Score + " (" + m_Points.Count + " points)");

            m_SpellCharged = true;
            if (result.Name == "rectangle")     m_CurrentSpell.AddComponent<DamageSpell>();
            else if (result.Name == "circle")   m_CurrentSpell.AddComponent<ShieldSpell>();
            else if (result.Name == "triangle") m_CurrentSpell.AddComponent<HealSpell>();
            else if (result.Name == "No match")  return;
            else
            {
	            CancelSpell();
                ClearLineRenderer();
                m_SpellCharged = false;
            }

            //For debugging, show the spell we drew
            SpawnText(result.Name, Color.grey);
        }
    }

    //TEMP TEMP TEMP
    void OnGUI()
    {
        GUI.Label(new Rect(Screen.width - 340, 10, 70, 30), "Add as: ");
        m_NewGestureName = GUI.TextField(new Rect(Screen.width - 270, 10, 200, 30), m_NewGestureName);

        if (GUI.Button(new Rect(Screen.width - 60, 10, 50, 30), "Add"))
        {
            Gesture newGesture = new Gesture(m_Points, m_NewGestureName);
            m_GestureLibrary.AddGesture(newGesture);
        }
    }
}
