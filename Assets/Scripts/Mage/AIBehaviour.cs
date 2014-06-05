using UnityEngine;
using System.Collections;

public class AIBehaviour : Mage
{
	//Fields
	private bool m_IsCasting = false;
	private float m_CastTimer;
	private float m_ReleaseTimer;

	//Constructor
	public AIBehaviour() : base(){}
	
	override protected void Start ()
	{
		Target = GameObject.Find("Player").GetComponent<Mage>();
		base.Start();
	}

	// Update is called once per frame
	override protected void Update ()
	{
		m_CastTimer -= Time.deltaTime;
		if (m_ReleaseTimer > 0.0f) m_ReleaseTimer -= Time.deltaTime;

		if (m_CastTimer <= 0.0f)
		{
			//Start casting a spell
			CreateSpell();

			//Release it afer a second (so the player has time to react)
			m_ReleaseTimer = 1.0f;

			//Reset the cast timer
			m_CastTimer = Random.Range (2, 5);
			m_IsCasting = true;
		}

		if (m_IsCasting && m_ReleaseTimer <= 0.0f)
		{
			m_CurrentSpell.AddComponent<DamageSpell>();
			CastSpellAt();
			m_IsCasting = false;
		}

		base.Update();
	}
}
