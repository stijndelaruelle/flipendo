using UnityEngine;
using System.Collections;

public class HealSpell : Spell
{
    public HealSpell() : base() { }

    override public void Execute()
    {
        m_Target.Heal(2);
    }
}
