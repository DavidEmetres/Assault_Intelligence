using UnityEngine;
using System.Collections;

public static class Hacks {

	public static bool isMouseOver(GameObject obj) {
		Ray ray = Camera.main.ScreenPointToRay (Input.mousePosition);
		RaycastHit hit;

		if (Physics.Raycast (ray, out hit)) {
			if (hit.collider.gameObject == obj)
				return true;
			else
				return false;
		}

		return false;
	}

	public static void Outline(GameObject obj) {

	}
}
