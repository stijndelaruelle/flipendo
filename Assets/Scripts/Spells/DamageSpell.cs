using UnityEngine;
using System.Collections;

public class DamageSpell : Spell
{
	public DamageSpell() : base() {}

	override public void OnHit()
	{
		m_Target.Damage(10);
		Destroy (gameObject);
	}
}