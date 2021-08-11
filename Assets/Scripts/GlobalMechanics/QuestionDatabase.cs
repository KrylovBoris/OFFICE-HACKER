// MIT License
// Copyright (c) 2020 KrylovBoris
// License information: https://github.com/KrylovBoris/OFFICE-HACKER/blob/main/LICENSE

using System;
using System.Collections.Generic;
using UnityEngine;

namespace GlobalMechanics
{
    [Serializable]
    public struct Question
    {
        public string text;
        public string correctAnswerVariant;
        public AnswerVariant[] incorrectAnswerVariants;
    }

    [Serializable]
    public struct AnswerVariant
    {
        public string answer;
        public string clarification;
    }

    [CreateAssetMenu(fileName = "Questions for test", menuName = "ScriptableObjects/Test Questions", order = 3)]
    public class QuestionDatabase : ScriptableObject
    {
        [SerializeField]
        private List<Question> generalQuestions;
        public Question[] GeneralQuestions => generalQuestions.ToArray();
    }

    // [CustomEditor(typeof(QuestionDatabase))]
    // public class QuestionDatabaseEditor : Editor
    // {
    //     private QuestionDatabase _database;
    //     
    //
    //     private void OnEnable()
    //     {
    //         _database = serializedObject.targetObject as QuestionDatabase;
    //     }
    //
    //     public override void OnInspectorGUI()
    //     {
    //         var generalQuestions = _database.GeneralQuestions;
    //         for (int i = 0; i < generalQuestions.Length; i++)
    //         {
    //             ShowQuestion(ref generalQuestions[i]);
    //         }
    //
    //         if (GUILayout.Button("+"))
    //         {
    //             _database.AddQuestion(new Question());
    //         }
    //     }
    //
    //     private void ShowQuestion(ref Question question)
    //     {
    //         question.text = EditorGUILayout.TextArea(question.text);
    //         GUILayout.Label("Correct answer:");
    //         question.CorrectAnswerVariant.Answer = EditorGUILayout.TextField(question.CorrectAnswerVariant.Answer);
    //     }
    //}
    
}