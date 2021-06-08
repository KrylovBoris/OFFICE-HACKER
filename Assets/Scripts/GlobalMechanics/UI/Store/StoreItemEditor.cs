using UnityEditor;

namespace GlobalMechanics.UI.Store
{
    [CustomEditor(typeof(StoreItem))]
    public class StoreItemEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            var property = serializedObject.GetIterator();
            property.Next(true);
            do
            {
                property.NextVisible(false);
                if (property.name != "m_Script") EditorGUILayout.PropertyField(property, true);
                
            } while (property.name != "hasSharedStock");

            var drawAddition = property.boolValue;
            if (drawAddition)
            {
                property.NextVisible(false);
                EditorGUILayout.PropertyField(property, true);
            }

            serializedObject.ApplyModifiedProperties();
        }
    }
}