using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class QuestionDisplay : MonoBehaviour
{
    public TextMeshProUGUI questionTmp;
    public Button next;
    public int possibleAnswersCount;
    public GameObject answersHolder;
    public GameObject nextLineButton;

    private TestSystem _testSystem;
    private TextMeshProUGUI[] _answerTmps;
    private Animator[] _answerAnimators;
    private bool answeredCorrectly;
    private bool[] _answerChosen;
    private const string Reset = "Reset";
    private const string Wrong = "AnswerIsWrong";
    private const string Right = "AnswerIsRight";

    private TestSystem.QuestionInfo _currentQuestion;
    // Start is called before the first frame update
    void Start()
    {
        _answerChosen = new bool[possibleAnswersCount];
        _answerTmps = answersHolder.GetComponentsInChildren<TextMeshProUGUI>().Take(possibleAnswersCount).ToArray();
        _answerAnimators = answersHolder.GetComponentsInChildren<Animator>().Take(possibleAnswersCount).ToArray();;
        _testSystem = GetComponent<TestSystem>();
        _currentQuestion = null;
    }

    public void DisplayQuestion(TestSystem.QuestionInfo newQuestion)
    {
        if (_currentQuestion != null)
        {
            foreach (var animator in _answerAnimators)
            {
                animator.SetTrigger(Reset);
            }
        }
        _currentQuestion = newQuestion;
        
        questionTmp.text = newQuestion.Text;
        nextLineButton.SetActive(false);
        TurnOnButtons(_currentQuestion.Answers.Length);
        for (int i = 0; i < _currentQuestion.Answers.Length; i++)
        {
            _answerAnimators[i].gameObject.SetActive(true);
            _answerTmps[i].text = _currentQuestion.Answers[i];
            
        _answerChosen[i] = false;
        }

        next.interactable = false;

        answeredCorrectly = true;
    }

    public void SubmitAnswer(int i)
    {
        if (!_answerChosen[i])
        {
            if (_currentQuestion.IsAnswerCorrect(i))
            {
                _answerAnimators[i].SetTrigger(Right);
                next.interactable = true;
                _testSystem.QuestionAnswered(false);
            }
            else
            {
                _answerAnimators[i].SetTrigger(Wrong);
                _answerTmps[i].text = _currentQuestion.Clarification[i];
                _testSystem.QuestionAnswered(true);
            }
            _answerChosen[i] = true;
        }
    }

    public void ShowLine(string line)
    {
        TurnOffButtons();
        questionTmp.text = line;
    }

    private void TurnOffButtons() => TurnOnButtons(0);

    private void TurnOnButtons(int number = 4)
    {
        if (number == 0)
        {
            answersHolder.SetActive(false);
            nextLineButton.SetActive(true);
        }
        else
        {
            answersHolder.SetActive(true);

            for (var i = 0; i < _answerAnimators.Length; i++)
            {
                _answerAnimators[i].gameObject.SetActive(i < number);
            }
        }
    }
    
    
}
