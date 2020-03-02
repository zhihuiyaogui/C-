using GenerSoft.IndApp.CommonSdk;
using GenerSoft.IndApp.WebApiFilterAttr;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using UserBLL;
using UserBLL.Model.Parameter.Message;
using UserBLL.Model.Return.Message;

namespace User.Controllers
{
    public class MessageController : BaseApiController
    {
        /// <summary>
        /// 管理员新增消息
        /// </summary>
        [DecipfilterFilterAttribute(NeedLogin = true)]
        [HttpPost]
        public IHttpActionResult AddMessage(MessageInfoModel model)
        {
            UserApi api = new UserApi();
            var userApi = api.GetUserInfoByToken();
            model.CreateUserID =Convert.ToInt32(userApi.Data.UserId);
            MessageBLL msg = new MessageBLL();
            var get = msg.AddMessage(model);
            return InspurJson<RetMessageInfo>(get);
        }

        /// <summary>
        /// 管理员获取信息列表
        /// </summary>
        [DecipfilterFilterAttribute(NeedLogin = true)]
        [HttpPost]
        public IHttpActionResult GetAllMessageInfo(MessageInfoModel model)
        {
            MessageBLL msg = new MessageBLL();
            var get = msg.GetAllMessageInfo(model);
            return InspurJson<List<RetMessageInfo>>(get);
        }

        /// <summary>
        /// 管理员获取消息信息
        /// </summary>
        [DecipfilterFilterAttribute(NeedLogin = true)]
        [HttpPost]
        public IHttpActionResult GetInfoByAdmin(MessageInfoModel paramter)
        {
            MessageBLL user = new MessageBLL();
            var add = user.GetInfo(paramter);
            return InspurJson<RetMessageInfo>(add);
        }

        /// <summary>
        /// 新增报警信息(内部调用)
        /// </summary>
        [InnerCallFilter]
        [HttpPost]
        public IHttpActionResult AddAlarmMessageInside(GetMessageInfoParameter parm)
        {
            MessageBLL msg = new MessageBLL();
            var get = msg.AddAlarmMessageInside(parm);
            return InspurJson<string>(get, true);
        }

        /// <summary>
        /// 用户获取信息列表
        /// </summary>
        [DecipfilterFilterAttribute(NeedLogin = true, NeedPlatformAdmin = false)]
        [HttpPost]
        public IHttpActionResult GetAllMessageToUser(UserMessageRelModel model)
        {
            UserApi api = new UserApi();
            var userApi = api.GetUserInfoByToken();
            string GetUserID = userApi.Data.UserId;
            MessageBLL msg = new MessageBLL();
            var get = msg.GetAllMessageToUser(model,GetUserID);
            return InspurJson<List<RetUserMessageRel>>(get);
        }

        /// <summary>
        /// 用户获取信息
        /// </summary>
        [DecipfilterFilterAttribute(NeedLogin = true, NeedPlatformAdmin = false)]
        [HttpPost]
        public IHttpActionResult GetInfoByUser(MessageInfoModel paramter)
        {
            MessageBLL user = new MessageBLL();
            var add = user.GetInfo(paramter);
            return InspurJson<RetMessageInfo>(add);
        }

        /// <summary>
        /// 批量删除消息信息
        /// </summary>
        [DecipfilterFilterAttribute(NeedLogin = true, NeedPlatformAdmin = false)]
        [HttpPost]
        public IHttpActionResult DelMessageInfo(List<MessageInfoModel> model)
        {
            UserApi api = new UserApi();
            var userApi = api.GetUserInfoByToken();
            string userid = userApi.Data.UserId;
            MessageBLL msg = new MessageBLL();
            var get = msg.DelMessageInfo(model,userid);
            return InspurJson<RetMessageInfo>(get);
        }

        /// <summary>
        /// 批量更新消息为已读状态
        /// </summary>
        [DecipfilterFilterAttribute(NeedLogin = true, NeedPlatformAdmin = false)]
        [HttpPost]
        public IHttpActionResult UpdateMessageInfo(List<MessageInfoModel> model)
        {
            UserApi api = new UserApi();
            var userApi = api.GetUserInfoByToken();
            string userid = userApi.Data.UserId;
            MessageBLL msg = new MessageBLL();
            var get = msg.UpdateMessageInfo(model,userid);
            return InspurJson<RetMessageInfo>(get);
        }

        /// <summary>
        /// 首页展示获取未读信息列表
        /// </summary>
        [DecipfilterFilterAttribute(NeedLogin = true, NeedPlatformAdmin = false)]
        [HttpPost]
        public IHttpActionResult GetAllUnreadMessage(UserMessageRelModel model)
        {
            UserApi api = new UserApi();
            var userApi = api.GetUserInfoByToken();
            string GetUserID = userApi.Data.UserId;
            MessageBLL msg = new MessageBLL();
            var get = msg.GetAllUnreadMessage(model, GetUserID);
            return InspurJson<List<RetUserMessageRel>>(get);
        }
    }
}