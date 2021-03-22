using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

[CustomPropertyDrawer(typeof(SceneAttribute))]
public class SceneDrawer : PropertyDrawer
{

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {

        if (property.propertyType == SerializedPropertyType.String)
        {
            var sceneObject = GetSceneObject(property.stringValue);
            var scene = EditorGUI.ObjectField(position, label, sceneObject, typeof(SceneAsset), true);
            if (scene == null)
            {
                property.stringValue = "";
            }
            else
            {
                string scenePath = AssetDatabase.GetAssetOrScenePath(scene);

                if (scenePath != property.stringValue)
                {
                    var sceneObj = GetSceneObject(scenePath);
                    if (sceneObj == null)
                    {
                        Debug.LogWarning("The scene " + scene.name + " cannot be used. To use this scene add it to the build settings for the project");
                    }
                    else
                    {
                        property.stringValue = scenePath;
                    }
                }
            }
        }
        else
        {
            EditorGUI.LabelField(position, label.text, "Use [Scene] with strings.");
        }
    }

    protected SceneAsset GetSceneObject(string sceneObjectPath)
    {
        if (string.IsNullOrEmpty(sceneObjectPath))
        {
            return null;
        }

        int buildIndex = SceneUtility.GetBuildIndexByScenePath(sceneObjectPath);

        if (buildIndex != -1)
        {
            return AssetDatabase.LoadAssetAtPath(sceneObjectPath, typeof(SceneAsset)) as SceneAsset;
        }

        Debug.LogWarning("Scene [" + sceneObjectPath + "] cannot be used. Add this scene to the 'Scenes in the Build' in build settings.");
        return null;
    }
}