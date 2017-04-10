using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(SoldierCreator))]
public class SoldierCreatorEditor : Editor {

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        SoldierCreator script = (SoldierCreator)target;

        if(GUILayout.Button("Create Soldier")) {
            script.CreateSoldier();
        }
    }
}
