using Common;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UserBLL.Model.Parameter.Message;
using UserBLL.Model.Return.Message;
using UserBLL.Model.Return.User;
using UserDAL;

namespace UserBLL
{
    public class MessageBLL
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        /// <summary>
        /// 新增消息
        /// </summary>
        public ReturnItem<RetMessageInfo> AddMessage(MessageInfoModel parameter)
        {
            ReturnItem<RetMessageInfo> r = new ReturnItem<RetMessageInfo>();
            using (UserEntities user = new UserEntities())
            {
                try
                {
                    //新增消息
                    U_Message message = new U_Message()
                    {
                        Type = parameter.Type,
                        Tittle = parameter.Tittle,
                        Text = parameter.Text,
                        CreateTime = DateTime.Now,
                        CreateUserID = parameter.CreateUserID
                    };
                    user.U_Message.Add(message);
                    user.SaveChanges();

                    //新增用户消息关联表
                    var getinfo = user.U_Message.OrderByDescending(m => m.CreateTime).FirstOrDefault();
                    foreach (var item in parameter.UserID)
                    {
                        U_UserMessageRel newrel = new U_UserMessageRel()
                        {
                            UserID = Convert.ToInt32(item),
                            MessageID = getinfo.ID,
                            Status = "0",
                            CreateTime = DateTime.Now,
                            CreateUserID = parameter.CreateUserID
                        };
                        user.U_UserMessageRel.Add(newrel);
                    }
                    user.SaveChanges();

                    r.Msg = "消息新增成功";
                    r.Code = 0;
                }
                catch (Exception e)
                {
                    r.Msg = "内部错误请重试";
                    log.ErrorFormat("内部错误：{0},{1}", e.Message, e.StackTrace);
                    r.Code = -1;
                }
            }

            return r;
        }

        /// <summary>
        /// 管理员获取信息列表
        /// </summary>
        /// <returns>成功返回用户信息,失败返回Null.</returns>
        public ReturnItem<List<RetMessageInfo>> GetAllMessageInfo(MessageInfoModel parameter)
        {
            ReturnItem<List<RetMessageInfo>> r = new ReturnItem<List<RetMessageInfo>>();
            List<RetMessageInfo> listinfo = new List<RetMessageInfo>();
            using (UserEntities user = new UserEntities())
            {
                try
                {
                    IQueryable<U_Message> messageList = user.U_Message.AsQueryable<U_Message>();
                    if (parameter.Tittle != null && !"".Equals(parameter.Tittle))
                    {
                        messageList = messageList.Where(x => x.Tittle.IndexOf(parameter.Tittle) >= 0);
                    }
                    if (!"".Equals(parameter.Type) && parameter.Type != null)
                    {
                        messageList = messageList.Where(x => x.Type == parameter.Type.ToString());
                    }
                    messageList = messageList.OrderByDescending(x => x.CreateTime);
                    if (messageList == null)
                    {
                        r.Data = null;
                        r.Code = -1;
                        r.Msg = "没有数据";
                        return r;
                    }
                    if (messageList != null)
                    {
                        List<U_Message> list = messageList.ToList<U_Message>();
                        r.Count = messageList.Count();
                        r.Msg = "消息列表获取成功";
                        r.Code = 0;
                        foreach (var item in list)
                        {
                            var info = new RetMessageInfo();
                            info.ID = item.ID.ToString();
                            info.Type = item.Type;
                            info.Tittle = item.Tittle;
                            info.Text = item.Text;
                            info.CreateTime = item.CreateTime;
                            info.CreateUserID = item.CreateUserID;
                            listinfo.Add(info);
                        }
                        r.Data = listinfo;
                    }
                }
                catch (Exception e)
                {
                    r.Msg = "内部错误请重试";
                    log.ErrorFormat("内部错误：{0},{1}", e.Message, e.StackTrace);
                    r.Code = -1;
                    return r;
                }
            }

            return r;
        }

        /// <summary>
        /// 获取消息信息
        /// </summary>
        public ReturnItem<RetMessageInfo> GetInfo(MessageInfoModel parameter)
        {
            ReturnItem<RetMessageInfo> r = new ReturnItem<RetMessageInfo>();
            using (UserEntities user = new UserEntities())
            {
                try
                {
                    var getinfo = user.U_Message.Where(s => s.ID == parameter.ID).FirstOrDefault();
                    if (getinfo == null)
                    {
                        r.Data = null;
                        r.Code = -1;
                        r.Msg = "未找到消息";
                        return r;
                    }
                    if (getinfo != null)
                    {
                        var model = new RetMessageInfo()
                        {
                            ID = getinfo.ID.ToString(),
                            Type = getinfo.Type,
                            Tittle = getinfo.Tittle,
                            Text = getinfo.Text,
                            CreateTime = getinfo.CreateTime,
                            CreateUserID = getinfo.CreateUserID
                        };

                        var getuserinfo = user.U_User.Join(user.U_UserMessageRel, x => x.ID, x => x.UserID, (a, b) => new { a, b })
                        .Join(user.U_Message, x => x.b.MessageID, x => x.ID, (a, c) => new { a.a, a.b, c })
                        .Where(x => x.c.ID == parameter.ID).ToList();

                        List<RetUserInfo> userinfo = new List<RetUserInfo>();
                        foreach (var item in getuserinfo)
                        {
                            RetUserInfo info = new RetUserInfo()
                            {
                                Id = item.a.ID.ToString(),
                                AccountId = item.a.AccountId.ToString(),
                                Name = item.a.Name,
                                Type = item.a.Type,
                                
                                Email = item.a.Email,
                                ContactPhone = item.a.ContactPhone,
                                PassWord = item.a.PassWord,
                                IsAdmin = item.a.IsAdmin,
                                IsManageAdmin = item.a.IsManageAdmin,
                                HeadImgUrl = item.a.HeadImgUrl == null ? "" : item.a.HeadImgUrl.ToString(),
                                CreateTime = item.a.CreateTime,
                               
                            };
                            userinfo.Add(info);
                        }
                        model.UserInfo = userinfo;
                        r.Msg = "消息信息获取成功";
                        r.Code = 0;
                        r.Data = model;
                    }
                }
                catch (Exception e)
                {
                    r.Msg = "内部错误请重试";
                    log.ErrorFormat("内部错误：{0},{1}", e.Message, e.StackTrace);
                    r.Code = -1;
                }
            }
            return r;
        }

        /// <summary>
        /// 新增报警信息(内部调用)
        /// </summary>
        public ReturnItem<string> AddAlarmMessageInside(GetMessageInfoParameter parameter)
        {
            ReturnItem<string> r = new ReturnItem<string>();
            using (UserEntities user = new UserEntities())
            {
                try
                {
                    //新增报警信息
                    U_Message message = new U_Message()
                    {
                        Type = parameter.Type,
                        Tittle = parameter.Tittle,
                        Text = parameter.Text,
                        CreateTime = DateTime.Now
                    };
                    user.U_Message.Add(message);
                    user.SaveChanges();
                    //查询当前组织下用户id
                    int OrgID = Convert.ToInt32(parameter.OrgID);
                    var getuserinfo = user.U_UserOrganizationRel.Where(m => m.OrgID == OrgID).ToList();
                    List<string> userinfo = new List<string>();
                    foreach (var info in getuserinfo)
                    {
                        userinfo.Add(info.UserID.ToString());
                    }
                    //新增用户消息关联表
                    var getinfo = user.U_Message.OrderByDescending(m => m.CreateTime).FirstOrDefault();
                    foreach (var item in userinfo)
                    {
                        U_UserMessageRel newrel = new U_UserMessageRel()
                        {
                            UserID = Convert.ToInt32(item),
                            MessageID = getinfo.ID,
                            Status = "0",
                            CreateTime = DateTime.Now
                        };
                        user.U_UserMessageRel.Add(newrel);
                    }
                    user.SaveChanges();

                    r.Msg = "消息新增成功";
                    r.Code = 0;
                }
                catch (Exception e)
                {
                    r.Msg = "内部错误请重试";
                    r.Code = -1;
                    r.Data = e.Message;
                    log.ErrorFormat("内部错误：{0},{1}", e.Message, e.StackTrace);
                }
            }

            return r;
        }

        /// <summary>
        /// 用户获取信息列表
        /// </summary>
        /// <returns>成功返回用户信息,失败返回Null.</returns>
        public ReturnItem<List<RetUserMessageRel>> GetAllMessageToUser(UserMessageRelModel parameter,string GetUserID)
        {
            ReturnItem<List<RetUserMessageRel>> r = new ReturnItem<List<RetUserMessageRel>>();
            List<RetUserMessageRel> listinfo = new List<RetUserMessageRel>();
            using (UserEntities user = new UserEntities())
            {
                try
                {
                    var getuserid = Convert.ToInt32(GetUserID);
                    var messageList = user.U_Message.Join(user.U_UserMessageRel, x => x.ID, x => x.MessageID, (a, b) => new { a, b }).Where(x => x.b.UserID == getuserid).AsQueryable();
                    if (parameter.Status != null && !"".Equals(parameter.Status))
                    {
                        messageList = messageList.Where(x => x.b.Status == parameter.Status);
                    }
                    if (!"".Equals(parameter.Type) && parameter.Type != null)
                    {
                        messageList = messageList.Where(x => x.a.Type == parameter.Type.ToString());
                    }
                    messageList = messageList.OrderByDescending(x => x.a.CreateTime);
                    if (messageList == null)
                    {
                        r.Data = null;
                        r.Code = -1;
                        r.Msg = "没有数据";
                        return r;
                    }
                    if (messageList != null)
                    {
                        var list = messageList.ToList();
                        r.Count = messageList.Count();
                        r.Msg = "消息列表获取成功";
                        r.Code = 0;
                        foreach (var item in list)
                        {
                            var info = new RetUserMessageRel();
                            info.ID = item.a.ID;
                            info.Type = item.a.Type;
                            info.Tittle = item.a.Tittle;
                            info.Text = item.a.Text;
                            info.CreateTime = item.a.CreateTime;
                            info.CreateUserID = item.a.CreateUserID;
                            info.Status = item.b.Status;
                            listinfo.Add(info);
                        }
                        r.Data = listinfo;
                    }
                }
                catch (Exception e)
                {
                    r.Msg = "内部错误请重试";
                    log.ErrorFormat("内部错误：{0},{1}", e.Message, e.StackTrace);
                    r.Code = -1;
                    return r;
                }
            }

            return r;
        }

        /// <summary>
        /// 批量删除消息信息
        /// </summary>
        public ReturnItem<RetMessageInfo> DelMessageInfo(List<MessageInfoModel> parameter,string UserID)
        {
            ReturnItem<RetMessageInfo> r = new ReturnItem<RetMessageInfo>();
            using (UserEntities user = new UserEntities())
            {
                try
                {
                    var list = new List<string>();
                    foreach (var item in parameter)
                    {
                        list.Add(item.ID.ToString());
                    }
                    var userid = Convert.ToInt32(UserID);
                    var deluser = user.U_UserMessageRel.Where(s => s.UserID == userid && list.Contains(s.MessageID.ToString())).AsQueryable<U_UserMessageRel>();
                    if (deluser != null)
                    {
                        foreach (var item in deluser)
                        {
                            var entry = user.Entry(item);
                            //设置该对象的状态为删除  
                            entry.State = EntityState.Deleted;
                        }
                        user.SaveChanges();
                        //保存修改  
                        r.Msg = "信息删除成功";
                        r.Code = 0;
                    }
                }
                catch (Exception e)
                {
                    r.Msg = "内部错误请重试";
                    log.ErrorFormat("内部错误：{0},{1}", e.Message, e.StackTrace);
                    r.Code = -1;
                }
            }

            return r;
        }

        /// <summary>
        /// 批量更新消息为已读状态
        /// </summary>
        public ReturnItem<RetMessageInfo> UpdateMessageInfo(List<MessageInfoModel> parameter,string UserID)
        {
            ReturnItem<RetMessageInfo> r = new ReturnItem<RetMessageInfo>();
            using (UserEntities user = new UserEntities())
            {
                try
                {
                    var list = new List<string>();
                    foreach (var item in parameter)
                    {
                        list.Add(item.ID.ToString());
                    }
                    var userid = Convert.ToInt32(UserID);
                    var messageinfo = user.U_UserMessageRel.Where(s => s.UserID == userid && list.Contains(s.MessageID.ToString())).AsQueryable<U_UserMessageRel>();
                    if (messageinfo == null)
                    {
                        r.Data = null;
                        r.Code = -1;
                        r.Msg = "未找到该用户";
                        return r;
                    }
                    if (messageinfo != null)
                    {
                        List<U_UserMessageRel> rellist = messageinfo.ToList<U_UserMessageRel>();
                        r.Count = messageinfo.Count();
                        foreach (var item in rellist)
                        {
                            item.Status = "1";
                            user.SaveChanges();
                        }
                        r.Msg = "消息状态更新成功";
                        r.Code = 0;
                        r.Data = new RetMessageInfo()
                        {

                        };
                    }
                }
                catch (Exception e)
                {
                    r.Msg = "内部错误请重试";
                    log.ErrorFormat("内部错误：{0},{1}", e.Message, e.StackTrace);
                    r.Code = -1;
                }
            }

            return r;
        }

        /// <summary>
        /// 首页展示获取未读信息列表
        /// </summary>
        /// <returns>成功返回消息信息,失败返回Null.</returns>
        public ReturnItem<List<RetUserMessageRel>> GetAllUnreadMessage(UserMessageRelModel parameter, string GetUserID)
        {
            ReturnItem<List<RetUserMessageRel>> r = new ReturnItem<List<RetUserMessageRel>>();
            List<RetUserMessageRel> listinfo = new List<RetUserMessageRel>();
            using (UserEntities user = new UserEntities())
            {
                try
                {
                    var getuserid = Convert.ToInt32(GetUserID);
                    var messageList = user.U_Message.Join(user.U_UserMessageRel, x => x.ID, x => x.MessageID, (a, b) => new { a, b })
                        .Where(x => x.b.UserID == getuserid && x.b.Status == "0").AsQueryable();
                    messageList = messageList.OrderByDescending(x => x.a.CreateTime);
                    if (messageList == null)
                    {
                        r.Data = null;
                        r.Code = -1;
                        r.Msg = "没有数据";
                        return r;
                    }
                    if (messageList != null)
                    {
                        var list = messageList.ToList();
                        r.Count = messageList.Count();
                        r.Msg = "消息列表获取成功";
                        r.Code = 0;
                    }
                }
                catch (Exception e)
                {
                    r.Msg = "内部错误请重试";
                    log.ErrorFormat("内部错误：{0},{1}", e.Message, e.StackTrace);
                    r.Code = -1;
                    return r;
                }
            }

            return r;
        }
    }
}
