using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class SubtitleController : MonoBehaviour
{

    public TextMeshProUGUI textHolder;
    private TextMeshPro _tmPro;
    public float fadeTimeOut = 3.0f;
    private float _timeSinceLastSubtitle;
    public float fadeTime = 1.0f;
    private Color _startingColor;
    private Queue<string> _subtitleQueue;
    // Start is called before the first frame update
    void Start()
    {
        _startingColor = textHolder.color;
        _subtitleQueue = new Queue<string>();
    }

    // Update is called once per frame
    void Update()
    {
        if (_timeSinceLastSubtitle + fadeTimeOut < Time.time)
        {
            if (_subtitleQueue.Any())
            {
                textHolder.color = _startingColor;
                textHolder.text = _subtitleQueue.Dequeue();
                _timeSinceLastSubtitle = Time.time;
            }
            else
            {
                var color = new Color(textHolder.color.r,
                    textHolder.color.g, textHolder.color.b, textHolder.color.a - 1 / fadeTime * Time.deltaTime);
                textHolder.color = color;
            }
        }
    }
}
