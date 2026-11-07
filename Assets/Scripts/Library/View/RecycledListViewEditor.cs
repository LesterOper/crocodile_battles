using UnityEditor;

namespace Library.View
{
#if UNITY_EDITOR
    [CustomEditor(typeof(RecycleListView))]
    public class RecycledListViewEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            var recycleListView = (RecycleListView) target;
            if (recycleListView.LayoutType == RecycleListView.PositionSourceType.LINEAR)
            {
                serializedObject.Update();
                DrawPropertiesExcluding(serializedObject, "_lineSize");
                serializedObject.ApplyModifiedProperties();
            }
            else
            {
                DrawDefaultInspector();
            }
        }
    }
#endif
}