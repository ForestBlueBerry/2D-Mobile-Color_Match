using DG.Tweening;
using TMPro;
using UnityEngine;
using VContainer;

public class ScoreView : MonoBehaviour
{
    [SerializeField] private TMP_Text _scoreText;

    [Header("Colors")]
    [SerializeField] private Color _defaultColor = Color.yellow;
    [SerializeField] private Color _plusColor = Color.green;
    [SerializeField] private Color _minusColor = Color.red;

    [Header("Settings the animations")]
    [SerializeField] private float _duration = 0.25f;
    [SerializeField] private Vector3 _plusPunchScale = new Vector3(0.35f, 0.35f, 0f);
    [SerializeField] private Vector3 _minusPunchScale = new Vector3(-0.2f, -0.2f, 0f); 

    private ScoreModel _scoreModel;
    private int _currentScore;
    private Sequence _scoreSequence;

    [Inject]
    public void Construct(ScoreModel scoreModel)
    {
        _scoreModel = scoreModel;
        _currentScore = _scoreModel.CurrentScore;

        if (_scoreText != null)
        {
            _scoreText.text = _currentScore.ToString();
            _scoreText.color = _defaultColor;
        }

        _scoreModel.OnScoreChanged += UpdateScoreText;
    }

    private void OnDestroy()
    {
        if (_scoreModel != null)
        {
            _scoreModel.OnScoreChanged -= UpdateScoreText;
        }

        ResetState();
    }

    private void UpdateScoreText(int newScore)
    {
        if (_scoreText == null) return;

        int diff = newScore - _currentScore;
        _currentScore = newScore;

        _scoreText.text = _currentScore.ToString();

        ResetState();

        _scoreSequence = DOTween.Sequence();

        if (diff > 0)
        {
            _scoreSequence.Join(_scoreText.DOColor(_plusColor, _duration * 0.5f).SetLoops(2, LoopType.Yoyo));
            _scoreSequence.Join(_scoreText.transform.DOPunchScale(_plusPunchScale, _duration, 7, 0.5f));
        }
        else if (diff < 0)
        {
            _scoreSequence.Join(_scoreText.DOColor(_minusColor, _duration * 0.5f).SetLoops(2, LoopType.Yoyo));
            _scoreSequence.Join(_scoreText.transform.DOPunchScale(_minusPunchScale, _duration, 8, 0.5f));
            _scoreSequence.Join(_scoreText.transform.DOPunchRotation(new Vector3(0, 0, 12f), _duration, 8, 0.5f));
        }

        _scoreSequence.OnKill(ResetVisuals);
    }

    private void ResetState()
    {
        _scoreSequence?.Kill();
        ResetVisuals();
    }

    private void ResetVisuals()
    {
        if (_scoreText != null)
        {
            _scoreText.color = _defaultColor;
            _scoreText.transform.localScale = Vector3.one;
            _scoreText.transform.localRotation = Quaternion.identity;
        }
    }
}