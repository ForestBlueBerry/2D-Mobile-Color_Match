using UnityEngine;
using VContainer;

public class ControlBucket : MonoBehaviour
{
    [SerializeField] private float _speed = 1.2f;
    [SerializeField] private float _minX = -5f;
    [SerializeField] private float _maxX = 5f;

    private IInputService _input;
    private Vector3 _currentPos;

    [Inject]
    public void Construct(IInputService input)
    {
        _input = input;
    }

    private void Start()
    {
        _currentPos = transform.position;
    }

    private void Update()
    {
        MoveBucket();
    }

    private void MoveBucket()
    {
        if (_input == null || !_input.IsPressed) return;

        float normalizedDelta = _input.TouchDelta.x / Screen.width;

        float worldWidth = _maxX - _minX;

        _currentPos.x += normalizedDelta * worldWidth * _speed;
        _currentPos.x = Mathf.Clamp(_currentPos.x, _minX, _maxX);

        transform.position = _currentPos;
    }
}