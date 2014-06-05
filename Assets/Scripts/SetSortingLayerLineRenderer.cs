using UnityEngine;
using System.Collections;

public class SetSortingLayerLineRenderer : MonoBehaviour
{
	void Start ()
	{
		gameObject.GetComponent<LineRenderer>().sortingLayerName = "Foreground";
	}
}
