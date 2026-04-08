using UnityEngine;

// MVVMのデモ初期化
public class MVVMDemo : MonoBehaviour
{
    void Start()
    {
        MVVMModel model = new MVVMModel();
        ViewModel viewModel = new ViewModel(model);
        MVVMView view = gameObject.AddComponent<MVVMView>();
        view.Initialize(viewModel);

        Debug.Log("MVVMが初期化されました。上下キーで値を変更できます。");
    }
}