using System;

// ViewModel：ModelとViewの仲介
public class ViewModel
{
    private readonly MVVMModel _model;
    private int _data;

    public int Data
    {
        get => _data;
        private set
        {
            if (_data != value)
            {
                _data = value;
                OnDataChanged?.Invoke(_data);
                OnDisplayTextChanged?.Invoke(DisplayText);
            }
        }
    }

    public string DisplayText => $"Data: {Data}";

    public event Action<int> OnDataChanged;
    public event Action<string> OnDisplayTextChanged;

    public ViewModel(MVVMModel model)
    {
        _model = model;
        _model.OnDataChanged += HandleModelChanged;
        Data = _model.Data;
    }

    private void HandleModelChanged(int value)
    {
        Data = value;
    }

    public void Increment()
    {
        _model.Data++;
    }

    public void Decrement()
    {
        _model.Data--;
    }

    public void SetData(int value)
    {
        _model.Data = value;
    }
}