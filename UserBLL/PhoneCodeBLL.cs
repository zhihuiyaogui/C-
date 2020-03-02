using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UserBLL.Model.Parameter.PhoneCode;
using UserDAL;

namespace UserBLL
{
    public class PhoneCodeBLL
    {
        public int InsertPhoneCode(PhoneCodeModel model)
        {
            using (UserEntities user = new UserEntities())
            {
                try
                {
                    //新增PhoneCode
                    U_PhoneCode newcode = new U_PhoneCode()
                    {
                        Phone = model.Phone,
                        Code = model.Code.ToString(),
                        StartTime = model.StartTime,
                        EndTime = model.EndTime,
                        ClientIp = model.ClientIp.ToString(),
                        SmsType = model.SmsType.ToString(),
                        SmsContent = model.SmsContent.ToString()
                    };
                    user.U_PhoneCode.Add(newcode);
                    user.SaveChanges();
                }
                catch (Exception e)
                {
                    return 0;
                }
            }

            return 1;
        }

        public PhoneCodeModel GetPhoneCode(string Phone, int smstype)
        {
            PhoneCodeModel data = new PhoneCodeModel();
            using (UserEntities user = new UserEntities())
            {
                try
                {
                    var getinfo = user.U_PhoneCode.Where(s => s.Phone == Phone && s.EndTime > DateTime.Now && s.SmsType == smstype.ToString()).FirstOrDefault();
                    if (getinfo == null)
                    {
                        return null;
                    }
                    if (getinfo != null)
                    {
                        data.Id = getinfo.ID.ToString();
                        data.Phone = getinfo.Phone;
                        data.Code = getinfo.Code;
                        data.StartTime = Convert.ToDateTime(getinfo.StartTime);
                        data.EndTime = Convert.ToDateTime(getinfo.EndTime);
                        data.ClientIp = getinfo.ClientIp;
                        data.SmsType = getinfo.SmsType;
                        data.SmsContent = getinfo.SmsContent;
                        return data;
                    }
                }
                catch (Exception e)
                {
                    return null;
                }
            }
            return data;
        }

        public int GetCountByClientIpEveryDay(string clientip)
        {
            using (UserEntities user = new UserEntities())
            {
                try
                {
                    DateTime datetime = Convert.ToDateTime(DateTime.Now.Date.ToString());
                    var getinfo = user.U_PhoneCode.Where(s => s.ClientIp == clientip && s.EndTime > datetime).ToList();
                    if (getinfo == null)
                    {
                        return 0;
                    }
                    if (getinfo != null)
                    {
                        return getinfo.Count;
                    }
                }
                catch (Exception e)
                {
                    return 0;
                }
            }
            return 0;
        }

        public int GetCountByPhoneEveryDay(string Phone)
        {
            using (UserEntities user = new UserEntities())
            {
                try
                {
                    DateTime datetime = Convert.ToDateTime(DateTime.Now.Date.ToString());
                    var getinfo = user.U_PhoneCode.Where(s => s.Phone == Phone && s.EndTime > datetime).ToList();
                    if (getinfo == null)
                    {
                        return 0;
                    }
                    if (getinfo != null)
                    {
                        return getinfo.Count;
                    }
                }
                catch (Exception e)
                {
                    return 0;
                }
            }
            return 0;
        }

        /// <summary>
        /// Code作废
        /// </summary>
        /// <param name="Phone"></param>
        /// <returns></returns>
        public int UpdatePhoneCode(string Phone)
        {
            using (UserEntities user = new UserEntities())
            {
                try
                {
                    var getinfo = user.U_PhoneCode.Where(s => s.Phone == Phone && s.EndTime > DateTime.Now).FirstOrDefault();
                    if (getinfo == null)
                    {
                        return 0;
                    }
                    if (getinfo != null)
                    {
                        getinfo.EndTime = DateTime.Now;
                        user.SaveChanges();
                        return 1;
                    }
                }
                catch (Exception e)
                {
                    return 0;
                }
            }
            return 0;
        }
    }
}
