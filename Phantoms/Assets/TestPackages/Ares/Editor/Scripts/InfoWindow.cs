using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace Ares.Editor {
	public class InfoWindow : EditorWindow {
		struct PatchNotes {
			public string version;
			public string features;
			public string fixes;
			
			public PatchNotes(string version, string features, string fixes){
				this.version = version;
				this.features = features;
				this.fixes = fixes;
			}
		}

		static PatchNotes[] patchNotes;
		static bool[] expanded;

		[MenuItem("Window/ARES Info")]
		public static void ShowWindow(){
			patchNotes = new PatchNotes[]{
				new PatchNotes("V1.1", 
					"- Exposed priority value on abilities. Higher-priority abilities get evaluated before others, regardless of the actor sort order.\n" +
					"- Previously queued actions can now be cancelled (cancel last, last player input or all).\n" +
					"- Animation, Audio and Instantiation effects now have sensible default values.\n" +
					"- Added Preparation and Recovery turns for Items and Abilities.\n" +
					"- Added turn/ round time limits and a fallback action for when time runs out.\n" +
					"- Added Item and Ability categories. \"Add [Item | Ability]\" dropdown menus use these to create nested lists. This makes working on projects with a large amount of assets much more manageable.\n" +
					"- Added assembly definition files."
					,
					"- After missing an action, the next time that same actor landed a hit the battle would fire an error and not progress.\n" +
					"- In certain cases Battle Delay Locks wouldn't free the current actor to end its ability.\n" +
					"- Fallback Abilities now show up in the Actor Components' ability entries. If they are missing from components added in V1.0, click the gear in the Inspector or right-click it to bring up the context menu and click \"Fix Missing Fallback Ability\"."
				)
			};

			expanded = new bool[patchNotes.Length];
			expanded[expanded.Length - 1] = true;

			EditorWindow.GetWindow<InfoWindow>("ARES", true);
		}

		void OnGUI(){
			GUILayout.Label("Support", EditorStyles.boldLabel);
			GUIContent about = new GUIContent("If you have any comments, questions, feedback or concerns, please contact me through any of the following:\n\n" +
				"E-mail: p_boelens@msn.com\n" +
				"Unity Forums: https://forum.unity.com/threads/released-ares-turn-based-battle-system.618160\n" +
				"Discord: https://discord.gg/yYu54Sp");
			EditorGUILayout.SelectableLabel(about.text, GUILayout.Height(EditorStyles.label.CalcHeight(about, position.width)));

			GUILayout.Space(20f);
			GUILayout.Label("Patch Notes", EditorStyles.boldLabel);

			for(int i = patchNotes.Length - 1; i > -1; i--){
				expanded[i] = EditorGUILayout.Foldout(expanded[i], patchNotes[i].version);
				if(expanded[i]){
					GUILayout.BeginHorizontal();
					GUILayout.Space(20f);
					EditorGUILayout.HelpBox("New Features:\n" + patchNotes[i].features, MessageType.Info);
					GUILayout.EndHorizontal();
					GUILayout.BeginHorizontal();
					GUILayout.Space(20f);
					EditorGUILayout.HelpBox("Bug Fixes:\n" + patchNotes[i].fixes, MessageType.Warning);
					GUILayout.EndHorizontal();
				}
			}
		}
	}
}