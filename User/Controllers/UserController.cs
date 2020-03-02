using Common;
using Common.Config;
using GenerSoft.IndApp.CommonSdk;
using GenerSoft.IndApp.Redis;
using GenerSoft.IndApp.WebApiFilterAttr;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Http;
using UserBLL;
using UserBLL.Model.Parameter.PhoneCode;
using UserBLL.Model.Parameter.User;
using UserBLL.Model.Return.User;
using UserBLL.SMS;
using UserDAL.Model.Parameter.User;
using UserDAL.Model.Return.User;

namespace User.Controllers
{
    public class UserController : BaseApiController
    {
        /// <summary>
        /// 1.手机验证码登录
        /// </summary>
        [HttpPost]
        public IHttpActionResult SendSMS(SendMessageSMS model)
        {
            UserInfoBLL user = new UserInfoBLL();
            var get = user.SendSMS(model);
            return InspurJson<List<UserDAL.U_PhoneCode>>(get);
        }
        /// <summary>
        /// 2.手机验证码登录验证
        /// </summary>
        [HttpPost]
        public IHttpActionResult UserMessageLogin(UserMessageLoginModel model)
        {
            UserInfoBLL user = new UserInfoBLL();
            var get = user.UserMessageLogin(model);
            return InspurJson<RetUserMessageLoginModel>(get);
        }
        /// <summary>
        /// 注册用户
        /// </summary>
        [HttpPost]
        public IHttpActionResult Register(UserRegisterModel model)
        {
            UserInfoBLL user = new UserInfoBLL();
            var get = user.Register(model);
            return InspurJson<RetUserInfo>(get);
        }

        /// <summary>
        /// 查询手机号是否存在
        /// </summary>
       
        [HttpPost]
        public IHttpActionResult CheckUserInfoByAccountId(UserInfoModel model)
        {
            UserInfoBLL user = new UserInfoBLL();
            var get = user.CheckUserInfoByAccountId(model);
            return InspurJson<RetUserInfo>(get);
        }

        /// <summary>
        /// 忘记密码
        /// </summary>
        
        [HttpPost]
        public IHttpActionResult ForgetPwd(UserInfoModel model)
        {
            UserInfoBLL user = new UserInfoBLL();
            var get = user.UpdateUserInfoByAccountId(model);
            if (get.Code == -1)
            {
                get.Msg = "密码修改失败";
            }
            else if (get.Code == 0)
            {
                get.Msg = "密码修改成功";
            }
            return InspurJson<RetUserInfo>(get);
        }

        /// <summary>
        /// 获取用户信息
        /// </summary>
     
        [HttpPost]
        public IHttpActionResult GetUserInfo(UserInfoModel model)
        {
            
            UserInfoBLL user = new UserInfoBLL();
            var get = user.GetUserInfoById(model);
            return InspurJson<RetUserInfo>(get);
        }

        /// <summary>
        /// 用户上传头像
        /// </summary>
        
        [HttpPost]
        public IHttpActionResult UploadHeadImg(UserInfoModel model)
        {
           
            UserInfoBLL user = new UserInfoBLL();
            var get = user.UpdateUserInfo(model);
            if (get.Code == -1)
            {
                get.Msg = "头像上传失败";
            }
            else if (get.Code == 0)
            {
                get.Msg = "头像上传成功";
            }
            return InspurJson<RetUserInfo>(get);
        }

        /// <summary>
        /// 用户修改密码
        /// </summary>
        
        [HttpPost]
        public IHttpActionResult SetPwd(UserInfoModel model)
        {
           
            
            UserInfoBLL user = new UserInfoBLL();
            var get = user.UpdateUserInfo(model);
            if (get.Code == -1)
            {
                get.Msg = "密码修改失败";
            }
            else if (get.Code == 0)
            {
                get.Msg = "密码修改成功";
            }
            return InspurJson<RetUserInfo>(get);
        }

        /// <summary>
        /// 用户修改手机号
        /// </summary>
        
        [HttpPost]
        public IHttpActionResult SetPhone(UserInfoModel model)
        {
            
            UserInfoBLL user = new UserInfoBLL();
            var get = user.UpdateUserInfo(model);
            if (get.Code == -1)
            {
                get.Msg = "手机号修改失败";
            }
            else if (get.Code == 0)
            {
                get.Msg = "手机号修改成功";
            }
            return InspurJson<RetUserInfo>(get);
        }

        /// <summary>
        /// 用户获取用户/组织信息
        /// </summary>
       
        [HttpPost]
        public IHttpActionResult GetInfo(UserOrganizeInfoModel model)
        {
            UserInfoBLL user = new UserInfoBLL();
            var add = user.GetInfo(model);
            return InspurJson<RetUserOrganiseInfo>(add);
        }

        /// <summary>
        /// 用户更新用户/组织信息
        /// </summary>
       
        [HttpPost]
        public IHttpActionResult UpdateInfo(UserOrganizeInfoModel model)
        {
            
            UserInfoBLL user = new UserInfoBLL();
            var add = user.UpdateInfo(model);
            return InspurJson<RetUserOrganiseInfo>(add);
        }

        /// <summary>
        /// 管理员获取用户/组织信息
        /// </summary>
       
        [HttpPost]
        public IHttpActionResult GetInfoByAdmin(UserOrganizeInfoModel paramter)
        {
            UserInfoBLL user = new UserInfoBLL();
            var add = user.GetInfo(paramter);
            return InspurJson<RetUserOrganiseInfo>(add);
        }

        /// <summary>
        /// 管理员新增用户/组织信息
        /// </summary>
        
        [HttpPost]
        public IHttpActionResult AddInfoByAdmin(UserOrganizeInfoModel model)
        {
            UserInfoBLL user = new UserInfoBLL();
            var add = user.AddInfo(model);
            return InspurJson<RetUserOrganiseInfo>(add);
        }

        /// <summary>
        /// 管理员更新用户/组织信息
        /// </summary>
       
        [HttpPost]
        public IHttpActionResult UpdateInfoByAdmin(UserOrganizeInfoModel model)
        {
           
            UserInfoBLL user = new UserInfoBLL();
            var add = user.UpdateInfo(model);
            return InspurJson<RetUserOrganiseInfo>(add);
        }

        /// <summary>
        /// 管理员获取用户信息列表数据
        /// </summary>
       
        [HttpPost]
        public IHttpActionResult GetUserAllInfo(UserInfoModel model)
        {
            UserInfoBLL user = new UserInfoBLL();
            var get = user.GetUserAllInfo(model);
            return InspurJson<List<RetUserInfo>>(get);
        }

        /// <summary>
        /// 管理员删除用户信息
        /// </summary>
        
        [HttpPost]
        public IHttpActionResult DeleteUserInfo(UserInfoModel model)
        {
            UserInfoBLL user = new UserInfoBLL();
            var get = user.DeleteUserInfo(model);
            return InspurJson<RetUserInfo>(get);
        }

        /// <summary>
        /// 管理员启用/禁用用户
        /// </summary>
        
        [HttpPost]
        public IHttpActionResult EnabledUser(UserInfoModel model)
        {
            UserInfoBLL user = new UserInfoBLL();
            var add = user.EnabledUser(model);
            return InspurJson<RetUserInfo>(add);
        }

        /// <summary>
        /// 管理员审核用户
        /// </summary>
        
        [HttpPost]
        public IHttpActionResult PassUser(UserInfoModel model)
        {
            UserInfoBLL user = new UserInfoBLL();
            var add = user.PassUser(model);
            return InspurJson<RetUserInfo>(add);
        }

        /// <summary>
        /// 管理员重置密码
        /// </summary>
        
        [HttpPost]
        public IHttpActionResult ResetPassword(UserInfoModel model)
        {
            UserInfoBLL user = new UserInfoBLL();
            var add = user.ResetPassword(model);
            return InspurJson<RetUserInfo>(add);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="parameter"></param>
        /// <returns></returns>
        [InnerCallFilterAttribute]
        [HttpPost]
        public IHttpActionResult GetTokenEntity(GetUserLoginInfoParameter parameter)
        {
            UserLoginBLL user = new UserLoginBLL();
            return InspurJson(user.GetUserLoginInfo(parameter), true);
        }

        [InnerCallFilterAttribute]
        [HttpPost]
        public IHttpActionResult GetUserInfoInside(GetUserLoginInfoParameter parameter)
        {
            UserLoginBLL user = new UserLoginBLL();
            return InspurJson(user.GetUserLoginInfo(parameter), true);
        }

        /// <summary>
        /// 图片验证码
        /// </summary>
        [HttpPost]
        public IHttpActionResult SecurityCode()
        {
            string code = CreateRandomCode(4); //验证码的字符为4个
            String strbaser64 = Convert.ToBase64String(CreateValidateGraphic(code));
            string VerificationCode = CodeSaveToRedis(code);
            RetVerificationCode model = new RetVerificationCode();
            model.ImgBase64 = strbaser64;
            model.VerificationCode = VerificationCode;
            var r = new ReturnItem<RetVerificationCode>();
            r.Data = model;
            return InspurJson<RetVerificationCode>(r);
        }

        /// <summary>
        /// 生成随机的字符串
        /// </summary>
        /// <param name="codeCount"></param>
        /// <returns></returns>
        public string CreateRandomCode(int codeCount)
        {
            string allChar = "0,1,2,3,4,5,6,7,8,9,A,B,C,D,E,a,b,c,d,e,f,g,h,i,g,k,l,m,n,o,p,q,r,F,G,H,I,G,K,L,M,N,O,P,Q,R,S,T,U,V,W,X,Y,Z,s,t,u,v,w,x,y,z";
            string[] allCharArray = allChar.Split(',');
            string randomCode = "";
            int temp = -1;
            Random rand = new Random();
            for (int i = 0; i < codeCount; i++)
            {
                if (temp != -1)
                {
                    rand = new Random(i * temp * ((int)DateTime.Now.Ticks));
                }
                int t = rand.Next(35);
                if (temp == t)
                {
                    return CreateRandomCode(codeCount);
                }
                temp = t;
                randomCode += allCharArray[t];
            }
            return randomCode;
        }

        /// <summary>
        /// 创建验证码图片
        /// </summary>
        /// <param name="validateCode"></param>
        /// <returns></returns>
        public byte[] CreateValidateGraphic(string validateCode)
        {
            Bitmap image = new Bitmap((int)Math.Ceiling(validateCode.Length * 16.0), 27);
            Graphics g = Graphics.FromImage(image);
            try
            {
                //生成随机生成器
                Random random = new Random();
                //清空图片背景色
                g.Clear(Color.White);
                //画图片的干扰线
                for (int i = 0; i < 25; i++)
                {
                    int x1 = random.Next(image.Width);
                    int x2 = random.Next(image.Width);
                    int y1 = random.Next(image.Height);
                    int y2 = random.Next(image.Height);
                    g.DrawLine(new Pen(Color.Silver), x1, x2, y1, y2);
                }
                Font font = new Font("Arial", 13, (FontStyle.Bold | FontStyle.Italic));
                LinearGradientBrush brush = new LinearGradientBrush(new Rectangle(0, 0, image.Width, image.Height), Color.Blue, Color.DarkRed, 1.2f, true);
                g.DrawString(validateCode, font, brush, 3, 2);

                //画图片的前景干扰线
                for (int i = 0; i < 100; i++)
                {
                    int x = random.Next(image.Width);
                    int y = random.Next(image.Height);
                    image.SetPixel(x, y, Color.FromArgb(random.Next()));
                }
                //画图片的边框线
                g.DrawRectangle(new Pen(Color.Silver), 0, 0, image.Width - 1, image.Height - 1);

                //保存图片数据
                MemoryStream stream = new MemoryStream();
                image.Save(stream, ImageFormat.Jpeg);

                //输出图片流
                return stream.ToArray();
            }
            finally
            {
                g.Dispose();
                image.Dispose();
            }
        }

        /// <summary>
        /// 图片验证码存储到redis
        /// </summary>
        /// <param name="codeCount"></param>
        /// <returns></returns>
        public string CodeSaveToRedis(string code)
        {
            string id = System.Guid.NewGuid().ToString("N");
            new RedisClient(CustomConfigParam.RedisDbNumber).StringSet<string>("VerificationCode:" + id.ToLower(), code);
            return id.ToLower();
        }

        /// <summary>
        /// 验证图片验证码
        /// </summary>
        [DecipfilterFilterAttribute(NeedLogin = false, NeedPlatformAdmin = false)]
        [HttpPost]
        public IHttpActionResult ValidateSecurityCode(VerificationCodeModel model)
        {
            var r = new ReturnItem<RetVerificationCode>();
            if (CustomConfigParam.IsUseRedis)
            {
                string key = "VerificationCode:" + model.VerificationCode;
                string realCode = new RedisClient(CustomConfigParam.RedisDbNumber).GetString(key);
                if (realCode != null)
                {
                    if (realCode.ToLower() == model.Code.ToLower())
                    {
                        r.Code = 1;
                    }
                    else
                    {
                        r.Code = 0;
                    }
                }
                else
                {
                    r.Code = -1;
                }
            }
            return InspurJson<RetVerificationCode>(r);
        }

        /// <summary>
        /// 发送短信
        /// </summary>
        [DecipfilterFilterAttribute(NeedLogin = false, NeedPlatformAdmin = false)]
        [HttpPost]
        public IHttpActionResult SendMessage(PhoneCodeModel model)
        {
            if (model.Phone == null || model.Phone == "")
            {
                return InspurJson<string>(new ReturnItem<string>() { Code = 0, Msg = "手机号不正确" });
            }
            //
            string regex = @"^0?(13[0-9]|15[012356789]|17[013678]|18[0-9]|14[57])[0-9]{8}$";
            Match m = Regex.Match(model.Phone, regex);
            if (!m.Success)
            {
                return InspurJson<string>(new ReturnItem<string>() { Code = 0, Msg = "手机号格式不正确" });
            }
            ShortMessageBLL bll = new ShortMessageBLL();
            ReturnItem<string> ret = bll.SendMessage(model.Phone, getClientIp(), (int)SmsType.IdentifyingCode_Reg);
            return InspurJson<string>(new ReturnItem<string>() { Code = ret.Code, Msg = ret.Msg });
        }

        private static string getClientIp()
        {
            string ip = getIp();
            string regex = @"((2[0-4]\d|25[0-5]|[01]?\d\d?)\.){3}(2[0-4]\d|25[0-5]|[01]?\d\d?)";
            Match m = Regex.Match(ip, regex);
            string s = "";
            if (m.Success)
            {
                s = m.Value;
            }
            return s;
        }

        private static string getIp()
        {
            string retip = "";
            if (System.Web.HttpContext.Current.Request.ServerVariables["HTTP_VIA"] != null)
            {
                retip = HttpContext.Current.Request.ServerVariables["HTTP_X_FORWARDED_FOR"].Split(new char[] { ',' })[0];
            }
            else
            {
                retip = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];
            }
            if (retip == "::1")
            {
                retip = "127.0.0.1";
            }

            return retip;
        }

        /// <summary>
        /// 验证短信验证码
        /// </summary>
      
        [HttpPost]
        public IHttpActionResult ValidatePhoneCode(PhoneCodeModel model)
        {
            var r = new ReturnItem<string>();
            PhoneCodeBLL bll = new PhoneCodeBLL();

            PhoneCodeModel code = bll.GetPhoneCode(model.Phone, Convert.ToInt32(model.SmsType));

            if (code == null || code.Code != model.Code)
            {
                r.Code = 0;
            }
            else if (code.Code == model.Code)
            {
                r.Code = 1;
            }
            else
            {
                r.Code = -1;
            }
            return InspurJson<string>(r);
        }

        /// <summary>
        /// 短信验证码过期
        /// </summary>
       
        [HttpPost]
        public IHttpActionResult UpdatePhoneCode(PhoneCodeModel model)
        {
            var r = new ReturnItem<string>();
            PhoneCodeBLL bll = new PhoneCodeBLL();
            bll.UpdatePhoneCode(model.Phone);
            return InspurJson<string>(r);
        }

        /// <summary>
        /// 管理员获取组织信息列表数据
        /// </summary>
     
        [HttpPost]
        public IHttpActionResult GetOrganizationListInfo(OrganizationInfoModel model)
        {
            UserInfoBLL user = new UserInfoBLL();
            var get = user.GetOrganizationListInfo(model);
            return InspurJson<List<RetOrganizationInfo>>(get);
        }

        /// <summary>
        /// 管理员获取组织信息
        /// </summary>
       
        [HttpPost]
        public IHttpActionResult GetOrganizationInfo(OrganizationInfoModel paramter)
        {
            UserInfoBLL user = new UserInfoBLL();
            var add = user.GetOrganizationInfo(paramter);
            return InspurJson<RetOrganizationInfo>(add);
        }

        /// <summary>
        /// 管理员更新组织信息
        /// </summary>
        
        [HttpPost]
        public IHttpActionResult UpdateOrganizationInfo(OrganizationInfoModel paramter)
        {
            UserInfoBLL user = new UserInfoBLL();
            var add = user.UpdateOrganizationInfo(paramter);
            return InspurJson<RetOrganizationInfo>(add);
        }

        /// <summary>
        /// 管理员新增组织信息
        /// </summary>
      
        [HttpPost]
        public IHttpActionResult AddOrganizationInfo(OrganizationInfoModel paramter)
        {
            UserInfoBLL user = new UserInfoBLL();
            var add = user.AddOrganizationInfo(paramter);
            return InspurJson<RetOrganizationInfo>(add);
        }

        /// <summary>
        /// 管理员删除组织信息
        /// </summary>
        
        [HttpPost]
        public IHttpActionResult DeleteOrganizationInfo(OrganizationInfoModel model)
        {
            UserInfoBLL user = new UserInfoBLL();
            var get = user.DeleteOrganizationInfo(model);
            return InspurJson<RetOrganizationInfo>(get);
        }

        /// <summary>
        /// 管理员特定组织下新增用户信息
        /// </summary>
      
        [HttpPost]
        public IHttpActionResult AddUserInfo(UserOrganizeInfoModel model)
        {
            UserInfoBLL user = new UserInfoBLL();
            var get = user.AddUserInfo(model);
            return InspurJson<RetUserOrganiseInfo>(get);
        }
    }
}
