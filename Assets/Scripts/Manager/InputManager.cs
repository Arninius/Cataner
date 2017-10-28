using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class InputManager : MonoBehaviour {

    public WorldManager world_manager;
	IntVector2 sel = new IntVector2(0, 0);
    bool selection_visible;

    void Update()
    {
		Ray ray = Camera.main.ScreenPointToRay (Input.mousePosition);
		RaycastHit hit_info;
		if (Physics.Raycast (ray, out hit_info, Mathf.Infinity)) {
			Vector3 colPoint = transform.worldToLocalMatrix.MultiplyPoint3x4 (hit_info.point);
			if (selection_visible) {
				IntVector2 old_sel = (IntVector2)sel;
				sel.set (colPoint);
				if (!old_sel.equals (sel)) {
					world_manager.redoTexture (old_sel.x, old_sel.z);
					world_manager.tintTexture (sel.x, sel.z, Color.red);
				}
			} else {
				sel.set (colPoint);
				world_manager.tintTexture (sel.x, sel.z, Color.red);
				selection_visible = true;
			}
			if (Input.GetMouseButtonDown (0)) {

			}
		} else if (selection_visible) {
			world_manager.redoTexture (sel.x, sel.z);
			selection_visible = false;
		}
    }
}