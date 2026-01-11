using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MailCellViewHolder : MonoBehaviour
{
    public MailView _view;
    [SerializeField]
    private GameObject _views;

    private MailData _data;
    private GameObject _readed;
    private GameObject _notRead;
    private Image _selectedImage;
    private TextMeshProUGUI _dateTimeText;
    private List<GameObject> _itemList = new List<GameObject>();

    public void Init(MailView mailView)
    {
        _view = mailView;

        _readed = UIHelper.FindChild(_views, "Readed");
        _notRead = UIHelper.FindChild(_views, "NotRead");
        _selectedImage = UIHelper.GetImageControl(_views, "Selected");
        _dateTimeText = UIHelper.GetTextMeshControl(_views, "DateTime");

        for (int i = 1; i < 4; i++)
        {
            var item = UIHelper.FindChild(_views, $"Items/Item0{i}");
            _itemList.Add(item);
        }
        
        
        _view.AddButtonClick(gameObject, () =>
        {
            _view.SelectedMail(_data);
        });
    }

    public void UpdateMailData(MailData data)
    {
        _data = data;
        
        _readed.SetActive(data.Info.Status == 1);
        _notRead.SetActive(data.Info.Status == 0);
        _selectedImage.SetActive(data.IsSelected);
        
        _view.SetTextMesh(_readed, data.Info.Title, "Title");
        _view.SetTextMesh(_notRead, data.Info.Title, "Title");
        _view.SetTextMesh(_views, Util.TimeStampToDateTime((int)data.Info.SendTime).ToString("yyyy/MM/dd"), "DateTime");

        for (int i = 0; i < _itemList.Count; i++)
        {
            _view.SetActive(_itemList[i], data.Info.Attachment != null, "Image");
        }
        
        
        if(data.IsSelected)
            _view.ShowMailInfo(data);
    }
}
