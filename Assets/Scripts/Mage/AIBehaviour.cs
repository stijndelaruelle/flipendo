using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AIBehaviour : Mage
{
	//Fields
	private bool m_IsCasting = false;
	private float m_CastTimer;
	private float m_ReleaseTimer;
    private Mage m_CurrentTarget;

    //Drawing
    public Transform m_DrawingTransform; //Otherwise we can't get them in the inspector
    private List<Gesture> m_Gestures;
    private int m_CurrentPointId;

	//Constructor
	public AIBehaviour() : base(){}
	
	override protected void Start()
	{
        base.Start();

        //As there will always be only 1 player, set him as the target immediatly
		Target = GameObject.Find("Player").GetComponent<Mage>();

        //Get all the gestures
        m_Gestures = m_GestureLibrary.Library;
	}

	// Update is called once per frame
	override protected void Update()
	{
        base.Update();

        if (m_CastTimer > 0.0f)    m_CastTimer -= Time.deltaTime;
		if (m_ReleaseTimer > 0.0f) m_ReleaseTimer -= Time.deltaTime;

        if (!m_IsCasting && m_CastTimer <= 0.0f)
		{
			//Start casting a spell
			CreateSpell();

            //Determine which spell it is
            if (Target.HasEffect(new ShieldEffect(Target)))
            {
                m_CurrentSpell.AddComponent<HealSpell>();
                m_CurrentTarget = this;
            }
            else
            {
                m_CurrentSpell.AddComponent<DamageSpell>();
                m_CurrentTarget = Target;
            }

            //Depending on the chosen spell set the shape we're going to draw
            SetDrawingPoints(m_CurrentSpell.GetComponent<Spell>().Drawing);
            m_CurrentPointId = 0;

			//Release it afer a second (so the player has time to react)
			m_ReleaseTimer = 1.5f;
			m_IsCasting = true;
		}

		if (m_IsCasting && m_ReleaseTimer <= 0.0f)
		{
            //Cast the spell
            CastSpell(m_CurrentTarget);

            //Reset some stuff
            m_CastTimer = Random.Range(2, 5);
            ClearLineRenderer();
			m_IsCasting = false;

		}

        //We are still casting, keep on drawing
        else if (m_IsCasting && m_ReleaseTimer > 0.0f)
        {
            if (m_CurrentPointId < m_Points.Count)
            {
                Vector3 point = new Vector3(m_Points[m_CurrentPointId].x, m_Points[m_CurrentPointId].y, 0.0f);
                AddLinePoint(WorldCoordinateForGesturePoint(point));
                ++m_CurrentPointId;
            }
        }
	}

    private void SetDrawingPoints(string name)
    {
        for (int i = 0; i < m_Gestures.Count; ++i)
        {
            if (m_Gestures[i].Name == name)
            {
                //Take a copy of the points
                m_Points = new List<Vector2>(m_Gestures[i].Points);

                //Create a transformation matrix for them
                Vector3 location = GesturePointForWorldCoordinate(m_DrawingTransform.position);

                Matrix4x4 matTransform = Matrix4x4.TRS(location,
                                                       Quaternion.identity,
                                                       new Vector3(0.5f, 0.5f, 1.0f));

                //Transform all the points
                for (int j = 0; j < m_Points.Count; ++j)
                {
                    Vector3 temp = matTransform.MultiplyPoint(new Vector3(m_Points[j].x, m_Points[j].y, 0.0f));
                    m_Points[j] = new Vector2(temp.x, temp.y);
                }

                return;
            }
        }
    }
}
