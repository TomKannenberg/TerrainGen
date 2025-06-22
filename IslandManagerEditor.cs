// FILE: Editor/IslandManagerEditor.cs
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(IslandManager))]
public class IslandManagerEditor : Editor {
	public override void OnInspectorGUI() {
		DrawDefaultInspector();
		IslandManager manager = (IslandManager)target;

		EditorGUILayout.Space(10);

		GUI.backgroundColor = new Color(0.6f, 1f, 0.6f); // Green
		if (GUILayout.Button("Generate New Island", GUILayout.Height(40))) {
			EditorApplication.delayCall += () => {
				manager.GenerateNewIsland();
			};
		}

		EditorGUILayout.Space(5);

		if (manager.transform.childCount > 0) {
			GUI.backgroundColor = new Color(1f, 0.6f, 0.6f); // Red
			if (GUILayout.Button("Delete Generated Island")) {
				if (EditorUtility.DisplayDialog("Delete Island?", "Are you sure you want to delete the generated child island? This can be undone.", "Delete", "Cancel")) {
					EditorApplication.delayCall += () => {
						manager.DeleteAllChildren();
					};
				}
			}
		}
		GUI.backgroundColor = Color.white;
	}
}