﻿using UnityEngine;
using System.Collections;

public class DamageSpell : Spell
{
	public DamageSpell() : base() {}

    override public void Execute()
	{
		m_Target.Damage(10);
	}
}
