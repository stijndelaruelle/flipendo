﻿using UnityEngine;
using System.Collections;

public class ShieldEffect : IEffect
{
	public Mage Target { get; set; }
	public bool DeleteMe { get; set; }

	private float m_Timer = 5.0f;
	private int m_ColorDirection = 1;

	//Constructor
	public ShieldEffect(Mage target)
	{
		Target = target;
	}

	public void Update ()
	{
        SpriteRenderer renderer = Target.gameObject.GetComponent<SpriteRenderer>();

		m_Timer -= Time.deltaTime;
		if (m_Timer <= 0.0f)
		{
			DeleteMe = true;
            renderer.color = Color.white;
			return;
		}

		//Shield effect, extracts all the color channels one by one
		Color color = renderer.color;
		
		float striveValue = 1.0f;
		if (m_ColorDirection < 0) striveValue = 0.0f;
		
		if (color.r == striveValue && color.g == striveValue && color.b == striveValue) m_ColorDirection *= -1;
		
		if (color.r == striveValue && color.g == striveValue) 	color.b += m_ColorDirection * Time.deltaTime * 5;
		else if (color.r == striveValue) 						color.g += m_ColorDirection * Time.deltaTime * 5;
		else 													color.r += m_ColorDirection * Time.deltaTime * 5;
		
		if (color.r < 0.0f) color.r = 0.0f;	if (color.r > 1.0f) color.r = 1.0f;
		if (color.g < 0.0f) color.g = 0.0f;	if (color.g > 1.0f) color.g = 1.0f;
		if (color.b < 0.0f) color.b = 0.0f; if (color.b > 1.0f) color.b = 1.0f;

		renderer.color = color;
	}

    //This gets called every time the owner of this effect get's hit by a spell
	public bool ProcessSpell(Spell spell)
	{
        //Reflect the spell
        //Set the target of the spell to the target of our current target (basically the other mage)

        Vector2 targetPos = new Vector2(Target.Target.gameObject.transform.position.x, Target.Target.gameObject.transform.position.y);
        spell.CastAt(targetPos, Target.Target);

        Target.SpawnText("Reflected", Color.yellow);

        return false;
	}

    public void OnDuplicate()
    {
        //This can never happen
        m_Timer = 0.0f;
    }
}