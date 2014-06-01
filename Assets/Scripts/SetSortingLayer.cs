using UnityEngine;
using System.Collections;

public class SetSortingLayer : MonoBehaviour
{
	void Start ()
	{
		gameObject.GetComponent<LineRenderer>().sortingLayerName = "Foreground";
	}
}
