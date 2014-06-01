using UnityEngine;
using System.Collections;

public class ShieldSpell : Spell
{
	public ShieldSpell() : base() {}

	override public void OnHit()
	{
		m_Target.AddEffect(new ShieldEffect(m_Target));
		Destroy(gameObject);
	}
}