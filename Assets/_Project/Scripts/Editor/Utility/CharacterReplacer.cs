using System.Linq;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

public class CharacterReplacer : EditorWindow
{
    private Object _oldGameObject;
    private Object _newGameObject;

    [MenuItem("Monster Farm/Character Replacer")]
    private static void Init() => ((CharacterReplacer)GetWindow(typeof(CharacterReplacer))).Show();

    private void OnGUI()
    {
        _oldGameObject = EditorGUILayout.ObjectField("Old GameObject", _oldGameObject, typeof(GameObject), true);
        _newGameObject = EditorGUILayout.ObjectField("New GameObject", _newGameObject, typeof(GameObject), true);
        if (GUILayout.Button("Replace"))
        {
            SetColliders(_oldGameObject as GameObject, _newGameObject as GameObject);
        }
    }

    private void SetColliders(GameObject oldGameObject, GameObject newGameObject)
    {
        foreach (var oldTransform in oldGameObject.GetComponentsInChildren<Transform>().Skip(1))
        {
            var childObjectInNew = FindChildWithName(newGameObject.transform, oldTransform.gameObject.name);
            if (childObjectInNew != null)
            {
                var childObjectInOld = FindChildWithName(oldGameObject.transform, oldTransform.gameObject.name);
                var targetComponentsC = childObjectInOld.GetComponents<Component>();
                foreach (var component in targetComponentsC)
                {
                    ComponentUtility.CopyComponent(component);
                    ComponentUtility.PasteComponentAsNew(childObjectInNew);
                }

                var parentObjectInNew = FindChildWithName(newGameObject.transform, oldTransform.parent.name, true);
                childObjectInNew.transform.parent = parentObjectInNew.transform;

                var characterJoint = childObjectInOld.GetComponent<CharacterJoint>();
                if (characterJoint != null)
                {
                    childObjectInNew.GetComponent<CharacterJoint>().connectedBody =
                        FindChildWithName(newGameObject.transform, characterJoint.connectedBody.name)
                            .GetComponent<Rigidbody>();
                }
            }
            else
            {
                var parentObjectInNew = FindChildWithName(newGameObject.transform, oldTransform.parent.name, true);
                if (parentObjectInNew == null)
                {
                    parentObjectInNew = new GameObject(oldTransform.parent.name);
                    parentObjectInNew.transform.parent = newGameObject.transform;
                }

                childObjectInNew = new GameObject(oldTransform.name)
                {
                    transform =
                    {
                        parent = parentObjectInNew.transform
                    }
                };
            }
        }
    }


    private GameObject FindChildWithName(Component currentTransform, string name, bool includeParent = false)
    {
        var skipCount = includeParent ? 0 : 1;
        var childs = currentTransform.GetComponentsInChildren<Transform>().Skip(skipCount);
        return (from t in childs where t.name == name select t.gameObject).FirstOrDefault();
    }
}