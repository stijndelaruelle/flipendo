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

	//Constructor
	public PlayerBehaviour() : base(){}

	override protected void Start()
	{
		Target = GameObject.Find("Enemy");

		m_GestureLibrary = new GestureLibrary(m_LibraryName);
		m_LineRenderer = m_GestureOnScreen.GetComponent<LineRenderer>();
		m_DrawArea = new Rect(0, 0, Screen.width, Screen.height);

		base.Start();
	}

	// Update is called once per frame
	override protected void Update ()
	{
		Vector3 virtualKeyPosition = Vector2.zero;

		//Get the input
		if (Application.platform == RuntimePlatform.Android)
		{
			if (Input.touchCount > 0) virtualKeyPosition = new Vector3(Input.GetTouch(0).position.x, Input.GetTouch(0).position.y);
		}
		else
		{
			if (Input.GetMouseButton(0)) virtualKeyPosition = new Vector3(Input.mousePosition.x, Input.mousePosition.y);
		}
		
		//If we are clicking inside of the drawarea
		if (m_DrawArea.Contains(virtualKeyPosition))
		{
			//Mousedown = reset gesture
			if (Input.GetMouseButtonDown(0))
			{
				m_Points.Clear();
				m_LineRenderer.SetVertexCount(0);
				m_VertexCount = 0;

				//Start casting a spell
				CreateSpell();
			}
			
			//Dragging = adding new points to the vector
			if (Input.GetMouseButton(0))
			{
				m_Points.Add(new Vector2(virtualKeyPosition.x, -virtualKeyPosition.y));
				
				m_LineRenderer.SetVertexCount(++m_VertexCount);
				m_LineRenderer.SetPosition(m_VertexCount - 1, WorldCoordinateForGesturePoint(virtualKeyPosition));
			}
			
			//Release = recognise gesture
			if (Input.GetMouseButtonUp(0))
			{
				Gesture gesture = new Gesture(m_Points);
				Result result = gesture.Recognize(m_GestureLibrary, true);

				if (result.Name == "rectangle")
				{
					m_CurrentSpell.AddComponent<DamageSpell>();
					CastSpellAt();
				}
				else if (result.Name == "circle")
				{
					m_CurrentSpell.AddComponent<ShieldSpell>();
					CastSpellAt();
				}
				else
				{
					CancelSpell();
				}

				Debug.Log(result.Name);
			}
		}

		base.Update();
	}

	private Vector3 WorldCoordinateForGesturePoint(Vector3 gesturePoint)
	{
		Vector3 worldCoordinate = new Vector3(gesturePoint.x, gesturePoint.y, 10);
		return Camera.main.ScreenToWorldPoint(worldCoordinate);
	}
}
