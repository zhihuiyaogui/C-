using Common;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UserBLL.Model.Parameter.EnergyReport;
using UserBLL.Model.Return.EnergyReport;
using UserDAL;

namespace UserBLL
{
    public class EquipmentReportBLL
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        /// <summary>
        /// 新增/更新能源信息数据
        /// </summary>
        public ReturnItem<RetEquipmentEnergyList> AddChart(EquipmentEnergyModel parameter)
        {
            ReturnItem<RetEquipmentEnergyList> r = new ReturnItem<RetEquipmentEnergyList>();
            using (UserEntities user = new UserEntities())
            {
                try
                {
                    var OrgID = Convert.ToInt32(parameter.OrgID);
                    var DashBoardType = Convert.ToInt32(parameter.DashBoardType);
                    var addchart = user.U_HomeConfiguration.Where(s => s.OrgID == OrgID && s.DashBoardType == DashBoardType).FirstOrDefault();
                    if (addchart != null)
                    {
                        addchart.ChartConfig = parameter.ChartConfig;
                        user.SaveChanges();
                        r.Msg = "能源报表配置信息更新成功";
                        r.Code = 0;
                    }
                    if (addchart == null)
                    {
                        U_HomeConfiguration newChart = new U_HomeConfiguration()
                        {
                            DashBoardType = Convert.ToInt32(parameter.DashBoardType),
                            ChartConfig = parameter.ChartConfig,
                            SortID = Convert.ToInt32(parameter.SortID),
                            CreateTime = DateTime.Now,
                            CreateUserID = Convert.ToInt32(parameter.CreateUserID),
                            OrgID = Convert.ToInt32(parameter.OrgID)
                        };
                        user.U_HomeConfiguration.Add(newChart);
                        user.SaveChanges();
                        r.Msg = "能源报表配置信息新增成功";
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
        /// 获取设备能源数据信息
        /// </summary>
        /// <returns>成功返回图表列表,失败返回Null.</returns>
        public ReturnItem<RetEquipmentEnergyList> GetEquipmentEnergyChartList(EquipmentEnergyModel parameter)
        {
            ReturnItem<RetEquipmentEnergyList> r = new ReturnItem<RetEquipmentEnergyList>();
            using (UserEntities user = new UserEntities())
            {
                try
                {
                    var dashboardtype = Convert.ToInt32(parameter.DashBoardType);
                    var OrgID = Convert.ToInt32(parameter.OrgID);
                    var getchartlist = user.U_HomeConfiguration.Where(s => s.DashBoardType == dashboardtype && s.OrgID == OrgID).FirstOrDefault();
                    if (getchartlist == null)
                    {
                        r.Data = null;
                        r.Code = -1;
                        r.Msg = "未找到设备能源配置信息";
                        return r;
                    }
                    if (getchartlist != null)
                    {
                        r.Msg = "设备能源配置信息获取成功";
                        r.Code = 0;
                        RetEquipmentEnergyList info = new RetEquipmentEnergyList();
                        info.ID = getchartlist.ID;
                        info.DashBoardType = getchartlist.DashBoardType.ToString();
                        // ChartConfig赋值
                        info.ChartConfig = getchartlist.ChartConfig;
                        DashBoardEnergyConfigModel ChartConfig = new DashBoardEnergyConfigModel();
                        if (getchartlist.ChartConfig != "" && getchartlist.ChartConfig != null)
                        {
                            ChartConfig = JsonConvert.DeserializeObject<DashBoardEnergyConfigModel>(getchartlist.ChartConfig);
                        }
                        info.DatabaseType = ChartConfig.DatabaseType;
                        if (ChartConfig.StartTime != "")
                        {
                            info.StartTime = Convert.ToDateTime(ChartConfig.StartTime);
                        }
                        if (ChartConfig.EndTime != "")
                        {
                            info.EndTime = Convert.ToDateTime(ChartConfig.EndTime);
                        }
                        info.StatisticalInterval = ChartConfig.StatisticalInterval;
                        info.IntervalUnit = ChartConfig.IntervalUnit;
                        info.Type = ChartConfig.Type;
                        List<BaseModel> Property = new List<BaseModel>();
                        foreach (var item in ChartConfig.HomeDeviceInfoList)
                        {
                            BaseModel model = new BaseModel();
                            List<string> data = new List<string>();
                            data.Add(item.DeviceID);
                            data.Add(item.DeviceItemID);
                            model.data = data;
                            Property.Add(model);
                        }
                        info.Property = Property;
                        info.CreateTime = getchartlist.CreateTime;
                        info.CreateUserID = getchartlist.CreateUserID.ToString();
                        info.OrgID = getchartlist.OrgID.ToString();

                        r.Data = info;
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
    }
}
