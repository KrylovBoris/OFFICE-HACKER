using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace GlobalMechanics
{
    [CustomEditor(typeof(ProbabilityData))]
    public class ProbabilityDataEditor : UnityEditor.Editor
    {
        private ProbabilityData _matrixData;

        private List<bool> _expandFlags = new List<bool>();
        private Vector2 _handlePos = Vector2.zero;

        public void OnEnable()
        {
            _matrixData = (ProbabilityData) target;
            _expandFlags = new List<bool>();
            for (int i = 0; i < _matrixData.matrix.Count; i++)
            {
                _expandFlags.Add(false);
            }
        }

        public override void OnInspectorGUI()
        {
            var listOfRows = _matrixData.matrix.ToList();
            if (listOfRows.Count != _expandFlags.Count)
            {
                _expandFlags = new List<bool>();
                for (int i = 0; i < listOfRows.Count; i++)
                {
                    _expandFlags.Add(false);
                }
            }
            
            using (var scrollScope = new GUILayout.ScrollViewScope(_handlePos))
            {
                for (int i =0; i< listOfRows.Count; i++)
                {
                    var row = listOfRows[i];
                    var caption = _expandFlags[i] ? "-" : "+" + row.key;
                    _expandFlags[i] = EditorGUILayout.Foldout(_expandFlags[i], caption, true);
                    if (!_expandFlags[i])
                        continue;
                    using (new GUILayout.HorizontalScope())
                    {
                        row.key = EditorGUILayout.TextField(row.key);
                        if (GUILayout.Button("-"))
                        {
                            _matrixData.matrix.Remove(row);
                        }
                    }

                    var maxPossibleValue = 1 - (row.Fearfulness + row.Greed + row.Negligence + row.Trust +
                                                row.TechnicalKnowledge + row.WillingnessToHelp);
                    row.Curiosity =
                        Mathf.Clamp(EditorGUILayout.Slider(new GUIContent("Curiosity"), row.Curiosity, -1, 1), -1,
                            maxPossibleValue);

                    maxPossibleValue = 1 - (row.Curiosity + row.Greed + row.Negligence + row.Trust +
                                            row.TechnicalKnowledge + row.WillingnessToHelp);
                    row.Fearfulness =
                        Mathf.Clamp(EditorGUILayout.Slider(new GUIContent("Fearfulness"), row.Fearfulness, -1, 1), -1,
                            maxPossibleValue);

                    maxPossibleValue = 1 - (row.Curiosity + row.Fearfulness + row.Negligence + row.Trust +
                                            row.TechnicalKnowledge + row.WillingnessToHelp);
                    row.Greed = Mathf.Clamp(EditorGUILayout.Slider(new GUIContent("Greed"), row.Greed, -1, 1), -1,
                        maxPossibleValue);

                    maxPossibleValue = 1 - (row.Curiosity + row.Fearfulness + row.Greed + row.Trust +
                                            row.TechnicalKnowledge + row.WillingnessToHelp);
                    row.Negligence =
                        Mathf.Clamp(EditorGUILayout.Slider(new GUIContent("Negligence"), row.Negligence, -1, 1), -1,
                            maxPossibleValue);

                    maxPossibleValue = 1 - (row.Curiosity + row.Fearfulness + row.Greed + row.Negligence +
                                            row.TechnicalKnowledge + row.WillingnessToHelp);
                    row.Trust = Mathf.Clamp(EditorGUILayout.Slider(new GUIContent("Trust"), row.Trust, -1, 1), -1,
                        maxPossibleValue);

                    maxPossibleValue = 1 - (row.Curiosity + row.Fearfulness + row.Greed + row.Negligence + row.Trust +
                                            row.WillingnessToHelp);
                    row.TechnicalKnowledge =
                        Mathf.Clamp(
                            EditorGUILayout.Slider(new GUIContent("Technical Knowledge"), row.TechnicalKnowledge, -1, 1),
                            -1, maxPossibleValue);

                    maxPossibleValue = 1 - (row.Curiosity + row.Fearfulness + row.Greed + row.Negligence + row.Trust +
                                            row.TechnicalKnowledge);
                    row.WillingnessToHelp =
                        Mathf.Clamp(
                            EditorGUILayout.Slider(new GUIContent("Willingness To Help"), row.WillingnessToHelp, -1, 1),
                            -1, maxPossibleValue);
                }
                
                _handlePos = scrollScope.scrollPosition;
            }

            if (GUILayout.Button("+"))
            {
                _matrixData.Add();
                _expandFlags.Add(true);
            }
            
            EditorUtility.SetDirty(_matrixData);
        }
    }
}