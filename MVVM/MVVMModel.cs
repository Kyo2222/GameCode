using System;

// データモデル
public class MVVMModel
{
    private int _data;

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

    // データ変更通知
    public event Action<int> OnDataChanged;
}