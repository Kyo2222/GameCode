using System;

// データモデル（Model）
using UniRx;

public class MVVMModel
{
    private int _data;

    // UniRx の HP（血量）
    public ReactiveProperty<int> Hp { get; } = new ReactiveProperty<int>(100);

    public int Data
    {
        get => _data;
        set
        {
            if (_data != value)
            {
                _data = value;
                OnDataChanged?.Invoke(_data);
            }
        }
    }

    // データ変更通知イベント
    public event Action<int> OnDataChanged;
}