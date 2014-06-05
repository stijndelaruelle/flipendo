using UnityEngine;
using System.Collections;

public interface IEffect
{
	Mage Target { get; set; }
	bool DeleteMe { get; set; }

	void Update();
	bool ProcessSpell(Spell spell);
}