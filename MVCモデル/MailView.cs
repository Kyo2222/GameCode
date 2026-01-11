
using System;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using SuperScrollView;
using UnityEngine;

public partial class MailView
{
    private LoopListView2 _mailListAdapter;
    private List<MailData> _mailList = new List<MailData>();
    private LoopListView2 _collectionAdapter;
    private List<MailData> _collectionList = new List<MailData>();
    private MailData _tempSelectedData;

    public override void OnClick(string name)
    {
        switch (name)
        {
            case "CloseBtn":
                UIManager.ClosePopup(ModuleEnum.Mail);
                break;
            case "DeleteReadButton_Mail":
                var deleteMailList = _mailList.Where(x => x.Info.Status == 1).Select(x => x.Info.Eid).ToList();
                MessageManager.SendShortCutDelMail(deleteMailList);
                break;
            case "GetAllButton_Mail":
                var rewardMailList = _mailList.Select(x => x.Info.Eid).ToList();
                MessageManager.SendShortCutRewardMail(rewardMailList);
                break;
            case "GetAllButton_Collection":
                var rewardCollectionList = _mailList.Where(x => x.Info.IsForever == 1)
                    .Select(x => x.Info.Eid).ToList();
                MessageManager.SendShortCutDelMail(rewardCollectionList);
                break;
            case "PinButton":
                if(_tempSelectedData != null)
                {
                    mCollectionButton.SetActive(true);
                    MessageManager.SendCollectionMail(_tempSelectedData.Info.Eid);
                }
                break;
            case "CollectionButton":
                if(_tempSelectedData != null)
                {
                    mCollectionButton.SetActive(false);
                    MessageManager.SendReadMail(_tempSelectedData.Info.Eid);
                }
                break;
            case "GetButton_Sub":
                if (_tempSelectedData != null)
                {
                    MessageManager.SendRewardMail(_tempSelectedData.Info.Eid);
                }
                break;
            case "DeleteButton_Sub":
                if (_tempSelectedData != null)
                {
                    MessageManager.SendDelMail(_tempSelectedData.Info.Eid);
                }
                break;
        }

        if (_tempSelectedData != null)
        {
            _tempSelectedData = null;
            SelectedMail(_tempSelectedData);
        }
    }

    public override void OnToggleChanged(string name, bool isOn)
    {
        if (_tempSelectedData != null)
        {
            _tempSelectedData = null;
            SelectedMail(null);
        }
        
        switch (name)
        {
            case "MailToggle":
                UIHelper.FindChild(mMailToggle, "Enable").SetActive(isOn);
                UIHelper.FindChild(mMailToggle, "Disable").SetActive(!isOn);
                mMailContent.SetActive(isOn);
                mCollectionContent.SetActive(!isOn);
                if(isOn)
                {
                    UpdateMail();
                }
                break;
            case "CollectionToggle":
                UIHelper.FindChild(mCollectionToggle, "Enable").SetActive(isOn);
                UIHelper.FindChild(mCollectionToggle, "Disable").SetActive(!isOn);
                mCollectionContent.SetActive(isOn);
                mMailContent.SetActive(!isOn);
                if(isOn)
                {
                    UpdateCollection();
                }
                break;
        }
    }

    public void UpdateUI()
    {
        if (mMailToggle.isOn)
            UpdateMail();
        else if (mCollectionToggle.isOn)
            UpdateCollection();
    }
    
    private void UpdateMail()
    {
        _mailList = GameEntry.DataModel.Mail.GetMailDataList();

        SetActive(mMailContent, _mailList.Count > 0, "Message");
        SetActive(mMailContent, _mailList.Count <= 0, "NoMessage");
        SetTextMesh(mMailContent, $"{_mailList.Count}/200", "Message/PageText");
        
        SelectedMail(_tempSelectedData);

        if (_mailListAdapter.IsNullOrUnityNull())
        {
            _mailListAdapter = (LoopListView2) UIHelper.GetUIControl(mMailBox, typeof(LoopListView2));
            _mailListAdapter.InitListView(_mailList.Count, OnGetMailByIndex);
        }
        else
        {
            _mailListAdapter.SetListItemCount(_mailList.Count);
            _mailListAdapter.RefreshAllShownItem();
        }
    }

    private void UpdateCollection()
    {
        _mailList = GameEntry.DataModel.Mail.GetCollectionDataList();

        SetActive(mCollectionContent, _mailList.Count > 0, "Message");
        SetActive(mCollectionContent, _mailList.Count <= 0, "NoMessage");
        SetTextMesh(mCollectionContent, $"{_mailList.Count}/200", "Message/PageText");
        
        SelectedMail(_tempSelectedData);

        if (_collectionAdapter.IsNullOrUnityNull())
        {
            _collectionAdapter = (LoopListView2) UIHelper.GetUIControl(mCollectionBox, typeof(LoopListView2));
            _collectionAdapter.InitListView(_mailList.Count, OnGetMailByIndex);
        }
        else
        {
            _collectionAdapter.SetListItemCount(_mailList.Count);
            _collectionAdapter.RefreshAllShownItem();
        }
    }
    
    private LoopListViewItem2 OnGetMailByIndex(LoopListView2 listView, int index)
    {
        LoopListViewItem2 cell = listView.NewListViewItem("MailCell");
        var holder = cell.GetComponent<MailCellViewHolder>();
        if (cell.IsInitHandlerCalled == false)
        {
            cell.IsInitHandlerCalled = true;
            holder.Init(this);
        }
        
        holder.UpdateMailData(_mailList[index]);
        return cell;
    }
    
    public void UpdateOptMailUI(int op)
    {
        if (op== 1 || op == 4)
        {
            if (mMailToggle.isOn)
            {
                UpdateMail();
            }
            else if (mCollectionToggle.isOn)
            {
                UpdateCollection();
            }
        }
        else
        {
            if (mMailToggle.isOn)
            {
                UpdateMail();
            }
            else if (mCollectionToggle.isOn)
            {
                UpdateCollection();
            }
        }
    }
    
    public void UpdateBatchUI(int op)
    {
        if(op == 1)
        {
            if (mMailToggle.isOn)
            {
                UpdateMail();
            }
            else if (mCollectionToggle.isOn)
            {
                UpdateCollection();
            }
        }
        else
        {
            if (mMailToggle.isOn)
            {
                UpdateMail();
            }
            else if (mCollectionToggle.isOn)
            {
                UpdateCollection();
            }
        }
    }
    
    public void SelectedMail(MailData data)
    {
        for (int i = 0; i < _mailList.Count; i++)
        {
            _mailList[i].IsSelected = false;
        }
        
        if(data == null)
        {
            MoveMailInfo(false);
            return;
        }

        MoveMailInfo(true);
        data.IsSelected = true;
        _tempSelectedData = data;

        if (_tempSelectedData.Info.Status == 0)
            MessageManager.SendReadMail(_tempSelectedData.Info.Eid);
        
        if(data.Info.IsForever == 0)
            _mailListAdapter.RefreshAllShownItem();
        else
            _collectionAdapter.RefreshAllShownItem();
    }

    public void ShowMailInfo(MailData data)
    {
        var exipreTimeStr = Util.TimeStampToDateTime((int)data.Info.ExipreTime).ToString("yyyy/MM/dd");
        SetTextMesh(mSub, exipreTimeStr, "Mid/DateTime");
        SetTextMesh(mSub, data.Info.Title, "Top/Title");
        SetTextMesh(mSub, data.Info.Content, "Mid/Content");
        SetActive(mGetButton_Sub, !string.IsNullOrEmpty(data.Info.Attachment));
        SetActive(mPinButton, data.Info.IsForever == 0);

        mGetButton_Sub.SetActive(data.Info.IsForever == 0);
        mDeleteButton_Sub.SetActive(data.Info.IsForever != 0);

        // if (!string.IsNullOrEmpty(data.Info.Attachment))
        // {
        //     var rewardGridAdapter = (LoopGridView)UIHelper.GetUIControl(mItemScrollView, typeof(LoopGridView));
        //     rewardGridAdapter.InitGridView(data.Info., (LoopGridView gridView, int itemIndex, int row, int column) =>
        //     {
        //         LoopGridViewItem item = gridView.NewListViewItem("MailRewardItem");
        //         var itemHolder = item.GetComponent<MailRewardCellViewsHolder>();
        //         if (item.IsInitHandlerCalled == false)
        //         {
        //             item.IsInitHandlerCalled = true;
        //             itemHolder.Init(this);
        //         }
        //     
        //         var itemData = _mailList[itemIndex];
        //         itemHolder.UpdateTitleByItemIndex(itemData);
        //         return item;
        //     });
        // }
    }

    private void MoveMailInfo(bool isShow)
    {
        if (isShow)
        {
            mMain.transform.DOLocalMoveX(-450f, 0.5f).SetEase(Ease.Linear).SetAutoKill();
            mSub.SetActive(true);
            mSub.transform.DOLocalMoveX(460f, 0.5f).SetEase(Ease.Linear).SetAutoKill();
        }
        else
        {
            mMain.transform.DOLocalMoveX(0, 0.5f).SetEase(Ease.Linear).SetAutoKill();
            mSub.SetActive(false);
            mSub.transform.DOLocalMoveX(0, 0.5f).SetEase(Ease.Linear).SetAutoKill();
        }
    }

    #region NO_ATTENTION
    private MailModel _mailModel => GameEntry.DataModel.Mail;
    #endregion
}