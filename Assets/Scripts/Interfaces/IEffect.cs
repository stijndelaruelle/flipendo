using UnityEngine;
using System.Collections;

public interface IEffect
{
	Mage Target { get; set; }
	bool DeleteMe { get; set; }

	void OnUpdate();
	void OnHit();
	//void OnHeal()...
}