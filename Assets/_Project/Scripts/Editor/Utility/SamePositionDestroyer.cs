using System;
using System.Linq;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

public class SamePositionDestroyer
{
    [MenuItem("Monster Farm/Destory selected objects in the same location")]
    public static void DestroySameObjects()
    {
        var allObjects = Selection.gameObjects;

        var samePositionObjects = allObjects.GroupBy(x => x.transform.position).Where(x => x.Count() > 1);

        var deletedCount = 0;
        foreach (var samePositionObject in samePositionObjects)
        {
            foreach (var obj in samePositionObject.Skip(1))
            {
                try
                {
                    if (samePositionObject.Any(x => x.transform == obj.transform.parent))
                        break;
                    Object.DestroyImmediate(obj.gameObject);
                }
                catch (Exception e)
                {
                    // ignored
                }

                deletedCount++;
            }
        }

        Debug.Log(deletedCount);
    }


    [MenuItem("Monster Farm/Destroy enemies in the same position")]
    public static void DestroyEnemies()
    {
        var allObjects = Object.FindObjectsOfType<EnemyDummy>(true);

        var samePositionObjects = allObjects.GroupBy(x => x.transform.position).Where(x => x.Count() > 1);

        var deletedCount = 0;
        foreach (var samePositionObject in samePositionObjects)
        {
            foreach (var obj in samePositionObject.Skip(1))
            {
                Object.DestroyImmediate(obj.gameObject);
                deletedCount++;
            }
        }

        Debug.Log(deletedCount);
    }
}