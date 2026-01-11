
using System;
using System.Collections.Generic;
using Google.Protobuf;
using Messagebean;
using UnityEngine;

public class MailCtrl : BaseCtrl
{
    private int _uiLayer = CoreUILayer.Mid;

    public override void OnInit()
    {
        EventManager.AddListener(Define.NetCmd.MsgStcEmailList, OnMailList);
        EventManager.AddListener(Define.NetCmd.MsgStcEmailOp, OnMailOp);
        EventManager.AddListener(Define.NetCmd.MsgStcEmailShortCut, OnMailShortCut);
    }

    public override void OnCreate()
    {
        MessageManager.SendGetMailList();
    }

    private void OnMailList(object sender, GameEventArgs e)
    {
        using (var rsp = MessagePool.Instance.Fetch<EmailListRsp>())
        {
            NetPacketEventArgs packet = (NetPacketEventArgs)e;

            rsp.MergeFrom(packet.Buffer);

            _mailModel.UpdateMailList(rsp.EmailInfos);

            _mailView?.UpdateUI();
        }
    }

    private void OnMailOp(object sender, GameEventArgs e)
    {
        using (var rsp = MessagePool.Instance.Fetch<EmailOpRsp>())
        {
            NetPacketEventArgs packet = (NetPacketEventArgs)e;

            rsp.MergeFrom(packet.Buffer);
            
            if (rsp.Op == 1)
            {
                _mailModel.UpdateDelMail(rsp.Eid);
            }
            else if(rsp.Op == 2)
            {
                _mailModel.UpdateReadMail(rsp.Eid);
            }
            else if(rsp.Op == 3)
            {
                _mailModel.UpdateRewardMail(rsp.Eid);
            }
            else if (rsp.Op == 4)
            {
                _mailModel.UpdateCollectionMail(rsp.Eid);
            }
            
            if (_mailView != null)
            {
                _mailView.UpdateOptMailUI(rsp.Op);
            }
        }
    }

    private void OnMailShortCut(object sender, GameEventArgs e)
    {
        using (var rsp = MessagePool.Instance.Fetch<EmailShortCutRsp>())
        {
            NetPacketEventArgs packet = (NetPacketEventArgs)e;

            rsp.MergeFrom(packet.Buffer);

            if (rsp.Op == 1)
            {
                _mailModel.UpdateBatchDelMail(rsp.Eids);
            }
            else if (rsp.Op == 2)
            {
                _mailModel.UpdateBatchRewadMail(rsp.Eids);
            }
            
            if (_mailView != null)
            {
                _mailView.UpdateBatchUI(rsp.Op);
            }
        }
    }

    #region NO_ATTENTION
    private MailView _mailView;
    private MailModel _mailModel => GameEntry.DataModel.Mail;

    public MailCtrl(ModuleEnum moduleEnum) : base(moduleEnum) {}

    public override string GetViewPath()
    {
        return "UIMailView";
    }

    public override void OnShow(GameObject viewGameObject, object userData)
    {
        if (_mailView == null)
        {
            moduleView = _mailView = new MailView();
        }
        _mailView.CreateView(viewGameObject, _uiLayer, userData);
    }

    public override void OnDestroy(object userData)
    {
        if(_mailView != null)
        {
            _mailView.Close(userData);
            moduleView = _mailView = null;
        }
    }
    #endregion
}