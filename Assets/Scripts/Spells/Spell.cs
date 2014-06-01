using UnityEngine;
using System.Collections;

public class Spell : MonoBehaviour
{
	protected Mage m_Target;

	private bool m_IsMoving = false;
	private Vector2 m_TargetPosition;
	private Vector2 m_Direction;

	//Constructor
	public Spell() {}

	// Update is called once per frame
	protected void Update ()
	{
		if (!m_IsMoving) return;

		transform.Translate(m_Direction.x * 10.0f * Time.deltaTime, m_Direction.y * 10.0f * Time.deltaTime, 0.0f);

		if (m_Direction.x > 0.0f)
		{
			if (transform.position.x > m_TargetPosition.x) OnHit();
		}
		else
		{
			if (transform.position.x < m_TargetPosition.x) OnHit();
		}
	}

	public void Cast(Mage mage)
	{
		m_Target = mage;
		OnHit();
	}

	public void CastAt(Vector2 targetPos, Mage mage)
	{
		m_Target = mage;

		m_TargetPosition = targetPos;
		m_Direction.x = targetPos.x - transform.position.x;
		m_Direction.y = targetPos.y - transform.position.y;
		m_Direction.Normalize();

		m_IsMoving = true;
	}

	virtual public void OnHit()
	{
		Destroy (gameObject);
	}
}