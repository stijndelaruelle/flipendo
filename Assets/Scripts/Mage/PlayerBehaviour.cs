using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PlayerBehaviour : Mage
{
	//Fields
	public GameObject m_GestureOnScreen;
	public string m_LibraryName;

	private GestureLibrary m_GestureLibrary;
	private List<Vector2> m_Points = new List<Vector2>();

	private LineRenderer m_LineRenderer;
	private int m_VertexCount = 0;
	
	private Rect m_DrawArea;
    private bool m_SpellCharged = false;

    Vector3 m_StartDragPos;

	//Constructor
	public PlayerBehaviour() : base(){}

	override protected void Start()
	{
		Target = GameObject.Find("Enemy").GetComponent<Mage>();

		m_GestureLibrary = new GestureLibrary(m_LibraryName);
		m_LineRenderer = m_GestureOnScreen.GetComponent<LineRenderer>();
		m_DrawArea = new Rect(0, 0, Screen.width, Screen.height);

		base.Start();
	}

	// Update is called once per frame
	override protected void Update ()
	{
		Vector3 virtualKeyPosition = Vector3.zero;

		//Get the input
		if (Application.platform == RuntimePlatform.Android)
		{
			if (Input.touchCount > 0) virtualKeyPosition = new Vector3(Input.GetTouch(0).position.x, Input.GetTouch(0).position.y);
		}
		else
		{
			if (Input.GetMouseButtonDown(0) || Input.GetMouseButtonUp(0) || Input.GetMouseButton(0)) virtualKeyPosition = new Vector3(Input.mousePosition.x, Input.mousePosition.y);
		}

        //Right click = cancel
        if (Input.GetMouseButtonUp(1))
        {
            CancelSpell();
            m_SpellCharged = false;
        }

		//If we are clicking inside of the drawarea
		if (m_DrawArea.Contains(virtualKeyPosition))
		{
            if (m_SpellCharged) AimSpell(virtualKeyPosition);
            else                DrawSpell(virtualKeyPosition);
		}

		base.Update();
	}

	private Vector3 WorldCoordinateForGesturePoint(Vector3 gesturePoint)
	{
		Vector3 worldCoordinate = new Vector3(gesturePoint.x, gesturePoint.y, 10);
		return Camera.main.ScreenToWorldPoint(worldCoordinate);
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
            m_LineRenderer.SetVertexCount(++m_VertexCount);
            m_LineRenderer.SetPosition(m_VertexCount - 1, WorldCoordinateForGesturePoint(virtualKeyPosition));
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
            m_Points.Clear();
            ClearLineRenderer();

            //Start casting a spell
            CreateSpell();
        }
			
        //Hold left mouse button: Keep on drawing the spell
        if (Input.GetMouseButton(0))
        {
            m_Points.Add(new Vector2(virtualKeyPosition.x, -virtualKeyPosition.y));
				
            m_LineRenderer.SetVertexCount(++m_VertexCount);
            m_LineRenderer.SetPosition(m_VertexCount - 1, WorldCoordinateForGesturePoint(virtualKeyPosition));
        }
			
        //Left mouse release: Our spell is drawn, wait for a swipe to cast it!
        if (Input.GetMouseButtonUp(0))
        {
            Gesture gesture = new Gesture(m_Points);
            Result result = gesture.Recognize(m_GestureLibrary, true);

            Debug.Log(result.Name + " score: " + result.Score);

            if (result.Name == "rectangle")
            {
	            m_CurrentSpell.AddComponent<DamageSpell>();
                m_SpellCharged = true;
            }
            else if (result.Name == "circle")
            {
	            m_CurrentSpell.AddComponent<ShieldSpell>();
                m_SpellCharged = true;
            }
            else if (result.Name == "triangle")
            {
                m_CurrentSpell.AddComponent<HealSpell>();
                m_SpellCharged = true;
            }
            else
            {
	            CancelSpell();
                ClearLineRenderer();
                m_SpellCharged = false;
            }
        }
    }

    private void ClearLineRenderer()
    {
        m_LineRenderer.SetVertexCount(0);
        m_VertexCount = 0;
    }
}
