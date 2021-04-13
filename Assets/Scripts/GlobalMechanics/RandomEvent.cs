using Random = UnityEngine.Random;

public class RandomEvent
{
    public delegate float ProbabilityFunction();

    private ProbabilityFunction _probability;
    private float _maxProbabilityFunctionValue;
    private float _minProbabilityFunctionValue;

    public RandomEvent(ProbabilityFunction f, float maxValue = 1.0f, float minValue = 0.0f)
    {
        _probability = f;
        _maxProbabilityFunctionValue = maxValue;
        _minProbabilityFunctionValue = minValue;
    }

    public bool HasEventHappened()
    {
        var baselineVal = RandomVal();
        var probability = EventProbability();
        return baselineVal <= probability;
    }

    public float EventProbability()
    {
        return _probability.Invoke();
    }

    private float RandomVal()
    {
        return (float) Random.Range(_minProbabilityFunctionValue, _maxProbabilityFunctionValue);
    }
}
