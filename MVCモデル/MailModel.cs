
using System;
using System.Collections.Generic;
using System.Linq;
using Google.Protobuf.Collections;
using Messagebean;

public class MailData
{
    public EmailInfo Info;
    public bool IsSelected;
}

public class MailModel : BaseModel
{
    private List<MailData> _mailInfoList = new List<MailData>();
    
    public void UpdateMailList(RepeatedField<EmailInfo> mailInfoList)
    {
        _mailInfoList.Clear();

        for (int i = 0; i < mailInfoList.Count; i++)
        {
            var clone = mailInfoList[i].Clone();
            
            _mailInfoList.Add(new MailData
            {
                Info = clone,
                IsSelected = false
            });
        }
    }

    public List<MailData> GetMailDataList()
    {
        var list = new List<MailData>();
        for (int i = 0; i < _mailInfoList.Count; i++)
        {
            var mailData = _mailInfoList[i];
            if(mailData.Info.IsForever == 0)
            {
                mailData.IsSelected = false;
                list.Add(mailData);
            }
        }
        return list;
    }

    public List<MailData> GetCollectionDataList()
    {
        var list = new List<MailData>();
        for (int i = 0; i < _mailInfoList.Count; i++)
        {
            var mailData = _mailInfoList[i];
            if (mailData.Info.IsForever == 1)
            {
                mailData.IsSelected = false;
                list.Add(mailData);
            }
        }
        return list;
    }
    
    public MailData GetMail(int mailId)
    {
        var data = _mailInfoList.FirstOrDefault(x => x.Info.Eid == mailId);
        return data;
    }
    
    /// <summary>
    /// delete mail
    /// </summary>
    public void UpdateDelMail(int mailId)
    {
        var mailData = GetMail(mailId);
        if(mailData != null)
        {
            mailData.Info.Status = 2;
            _mailInfoList.Remove(mailData);
        }
    }
    
    /// <summary>
    /// read mail
    /// </summary>
    public void UpdateReadMail(int mailId)
    {
        var mailData = GetMail(mailId);
        if (mailData != null)
        {
            mailData.Info.Status = 1;
        }
    }

    /// <summary>
    /// collection mail
    /// </summary>
    public void UpdateCollectionMail(int mailId)
    {
        var mailData = GetMail(mailId);
        if (mailData != null)
        {
            mailData.Info.IsForever = 1;
        }
    }

    /// <summary>
    /// reward mail
    /// </summary>
    public void UpdateRewardMail(int mailId)
    {
        var mailData = GetMail(mailId);
        if (mailData != null)
        {
            mailData.Info.Isget = 1;
            mailData.Info.Status = 1;
        }
    }
    
    /// <summary>
    /// batch delete mail
    /// </summary>
    public void UpdateBatchDelMail(RepeatedField<int> eids)
    {
        for (int i = 0; i < eids.Count; i++)
        {
            var mailData = GetMail(eids[i]);
            if (mailData != null)
            {
                mailData.Info.Status = 2;
                _mailInfoList.Remove(mailData);
            }
        }
    }

    /// <summary>
    /// batch reward mail
    /// </summary>
    public void UpdateBatchRewadMail(RepeatedField<int> eids)
    {
        for (int i = 0; i < eids.Count; i++)
        {
            var mailData = GetMail(eids[i]);
            if (mailData != null)
            {
                mailData.Info.Isget = 1;
                mailData.Info.Status = 1;
            }
        }
    }
    
}