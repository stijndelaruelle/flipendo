using UnityEngine;
using System.Collections;

//Is able to be influenced by effects
public interface IAffectable
{
	void AddEffect(IEffect effect);
	void RemoveEffect(IEffect effect);
    bool HasEffect(IEffect effect);
}