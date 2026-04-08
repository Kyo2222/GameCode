using System;

// ViewModel：ModelとViewの仲介
using UniRx;

public class ViewModel
{
    private readonly MVVMModel _model;
    private int _data;

    // UniRx 血量
    public ReactiveProperty<int> Hp => _model.Hp;

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

    public string DisplayText => $"Data: {Data} / HP: {Hp.Value}";

    public event Action<int> OnDataChanged;
    public event Action<string> OnDisplayTextChanged;

    private readonly CompositeDisposable _disposables = new CompositeDisposable();

    public ViewModel(MVVMModel model)
    {
        _model = model;
        _model.OnDataChanged += HandleModelChanged;
        Data = _model.Data;

        // UniRx 血量監聽
        Hp.Subscribe(hp =>
        {
            OnDisplayTextChanged?.Invoke(DisplayText);
        }).AddTo(_disposables);
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

    // 血量操作
    public void AddHp(int value)
    {
        Hp.Value += value;
    }

    public void SubHp(int value)
    {
        Hp.Value -= value;
    }

    // 解放資源
    public void Dispose()
    {
        _disposables.Dispose();
    }
}