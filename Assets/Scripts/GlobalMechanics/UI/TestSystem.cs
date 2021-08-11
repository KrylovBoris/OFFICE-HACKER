// MIT License
// Copyright (c) 2020 KrylovBoris
// License information: https://github.com/KrylovBoris/OFFICE-HACKER/blob/main/LICENSE

using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

namespace GlobalMechanics.UI
{
    public class TestSystem : MonoBehaviour
    {

        //public TextAsset questions;
        public QuestionDatabase questions;
        public int testLength;
        public TextMesh questionCounter;
        public TextMesh errorCounter;
        public string[] startingLines;
        public string[] endingLines;
    
        private int _questionsRemained;
        private int _errors;
        private int _currentLine;
        private bool _isTestStarted;
        private bool _isErrorCounted;
        private string _questionCounterText;
        private string _errorCounterText;
    
        private QuestionDisplay _questionComponent;

        private List<QuestionInfo> _questionList;
        // Start is called before the first frame update

        private void Start()
        {
            _currentLine = -1;
            _questionList = new List<QuestionInfo>();
            _questionComponent = this.GetComponent<QuestionDisplay>();
            ExtractGeneralQuestions(questions);
            _errorCounterText = errorCounter.text;
            errorCounter.text += _errors;
            _questionCounterText = questionCounter.text;
            questionCounter.text += testLength;
            //ExtractGeneralQuestions(JObject.Parse(questions.text));
            NextLine();
        }

        void TestStart()
        {
            _questionsRemained = testLength;
            _errors = 0;
            _isTestStarted = true;
        }

        private void ExtractGeneralQuestions(JObject questionData)
        {
            var generalQuestions = questionData["generalQuestions"];
            if (generalQuestions.Any())
            {
                foreach (var questionToken in generalQuestions)
                {
                    _questionList.Add(ExtractQuestion(questionToken));
                }
            }
        }

        private QuestionInfo ExtractQuestion(JToken jQuestion)
        {
            string text = jQuestion["questionText"].Value<string>();
            List<(string, string)> answersAndClarifications = new List<(string, string)>();
            answersAndClarifications.Add((jQuestion["correctAnswer"].Value<string>(), String.Empty));
            answersAndClarifications.Add((jQuestion["wrongAnswer1"].Value<string>(), jQuestion["wrongAnswerCorrection1"].Value<string>()));
            var wrongAnswer = jQuestion["wrongAnswer2"].Value<string>();
            var clarification = jQuestion["wrongAnswerCorrection2"].Value<string>();
            if (wrongAnswer != string.Empty)
            {
                answersAndClarifications.Add((wrongAnswer, clarification));
            }
                
            wrongAnswer = jQuestion["wrongAnswer3"].Value<string>();
            clarification = jQuestion["wrongAnswerCorrection3"].Value<string>();
            if (wrongAnswer != string.Empty)
            {
                answersAndClarifications.Add((wrongAnswer, clarification));
            }

            var answerList = new List<string>();
            var clarificationList = new List<string>();

            while (answersAndClarifications.Any())
            {
                var randomNumber = Random.Range(0, answersAndClarifications.Count);
                var randomAnswer = answersAndClarifications[randomNumber];
                answersAndClarifications.RemoveAt(randomNumber);
                answerList.Add(randomAnswer.Item1);
                clarificationList.Add(randomAnswer.Item2);
            }
            return new QuestionInfo(text, answerList.ToArray(), clarificationList.ToArray());
        }
    
        private void ExtractGeneralQuestions(QuestionDatabase questionData)
        {
            var generalQuestions = questionData.GeneralQuestions;
            if (generalQuestions.Any())
            {
                foreach (var question in generalQuestions)
                {
                    _questionList.Add(ExtractQuestion(question));
                }
            }
        }

        private QuestionInfo ExtractQuestion(Question question)
        {
            var answerVariants = new List<string> {question.correctAnswerVariant};
            var clarificationVariants = new List<string> {String.Empty}; 
            foreach (var incorrectAnswerVariant in question.incorrectAnswerVariants)
            {
                answerVariants.Add(incorrectAnswerVariant.answer);
                clarificationVariants.Add(incorrectAnswerVariant.clarification);
            }
            //Shuffle list
            var last = answerVariants.Count - 1;
            for (int i = 0; i < last; i++)
            {
                var randIndex = Random.Range(i, last);
                var tmpQuestionVariant = answerVariants[randIndex];
                var tmpClarification = clarificationVariants[randIndex];
                answerVariants[randIndex] = answerVariants[i];
                clarificationVariants[randIndex] = clarificationVariants[i];
                answerVariants[i] = tmpQuestionVariant;
                clarificationVariants[i] = tmpClarification;
            }

            return new QuestionInfo(question.text, answerVariants.ToArray(), clarificationVariants.ToArray());
        }

        public void NextQuestion()
        {
            if (_isTestStarted && _questionsRemained > 0)
            {
                var randomQuestionIndex = Random.Range(0, _questionList.Count);
                _questionComponent.DisplayQuestion(_questionList[randomQuestionIndex]);
                _questionList.RemoveAt(randomQuestionIndex);
                _isErrorCounted = false;
                questionCounter.text = _questionCounterText + _questionsRemained;
                _questionsRemained--;
            }
            else
            {
                NextLine();
            }
        }

        private void NextLine()
        {
            _currentLine++;
            if (_currentLine < startingLines.Length && !_isTestStarted)
            {
                _questionComponent.ShowLine(startingLines[_currentLine]);
                return;
            }
        
            if (_isTestStarted && _currentLine < endingLines.Length)
            {
                _questionComponent.ShowLine(endingLines[_currentLine]);
                return;
            }

            if (!_isTestStarted && _currentLine >= startingLines.Length || _isTestStarted && _currentLine >= endingLines.Length)
            {
                TestStart();
                _currentLine = -1;
                NextQuestion();
            }

        }

        public void QuestionAnswered(bool answerCorrect)
        {
            if (!_isErrorCounted)
            {
                _errors += (answerCorrect) ? 1 : 0;
                _isErrorCounted = true;
            }

            errorCounter.text = _errorCounterText + _errors;
        }
    
        //This is needed to erase the information of the correct answer from the structure
        public class QuestionInfo
        {
            public readonly string Text;
            public readonly string[] Answers;
            public readonly string[] Clarification;

            public QuestionInfo(string text, string[] answers, string[] clarifications)
            {
                Text = text;
                Answers = answers;
                Clarification = clarifications;
            }

            public bool IsAnswerCorrect(int answerNum)
            {
                if (answerNum >= Clarification.Length) return false;
                return Clarification[answerNum] == String.Empty;
            }
        
        
        }
    }
}
