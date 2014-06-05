using UnityEngine;
using System.Collections;

public class ShieldSpell : Spell
{
	public ShieldSpell() : base() {}

	override public void Execute()
	{
		m_Target.AddEffect(new ShieldEffect(m_Target));
	}
}