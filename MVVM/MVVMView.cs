using UnityEngine;
using TMPro;

public class MVVMView : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI displayText;

    private ViewModel _viewModel;

    // ViewModelを初期化してバインド
    public void Initialize(ViewModel viewModel)
    {
        _viewModel = viewModel;
        _viewModel.OnDisplayTextChanged += UpdateDisplay;
        UpdateDisplay(_viewModel.DisplayText);
    }

    private void UpdateDisplay(string text)
    {
        if (displayText != null)
        {
            displayText.text = text;
        }
        else
        {
            Debug.Log(text);
        }
    }

    private void OnDestroy()
    {
        if (_viewModel != null)
        {
            _viewModel.OnDisplayTextChanged -= UpdateDisplay;
        }
    }

    void Update()
    {
        if (_viewModel == null)
            return;

        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            _viewModel.Increment();
        }
        else if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            _viewModel.Decrement();
        }
    }
}