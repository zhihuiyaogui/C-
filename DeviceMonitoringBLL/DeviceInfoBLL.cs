using Common;
using Common.Config;
using DeviceMonitoringBLL.Model.Parameter.DeviceMonitoring;
using DeviceMonitoringBLL.Model.Return.DeviceMonitoring;
using DeviceMonitoringDAL;
using DeviceMonitoringDAL.Model.Parameter.DeviceMonitoring;
using GenerSoft.IndApp.Redis;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Security;
using System.Reflection;
using System.Security.Cryptography.X509Certificates;
using System.Text;

namespace DeviceMonitoringBLL
{

    public class DeviceInfoBLL
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        /// <summary>
        /// 1.存入接收的RetDeviceEnvironmentDataInfo温湿度数据
        /// </summary>

        public ReturnItem<RetDeviceEnvironmentDataInfo> SubmitTemAndHumData(DeviceEnvironmentDataInfoModel parameter)
        {
            ReturnItem<RetDeviceEnvironmentDataInfo> r = new ReturnItem<RetDeviceEnvironmentDataInfo>();
            using (MonitoringEntities device = new MonitoringEntities())
            {
                try
                {
                    {
                        D_DeviceEnvironmentt newdevice = new D_DeviceEnvironmentt()

                        {
                            tempc = parameter.tempc,
                            hum = parameter.hum,
                            mac = parameter.mac,
                            status = parameter.status,
                            creattime = DateTime.Now

                        };

                        device.D_DeviceEnvironmentt.Add(newdevice);

                        device.SaveChanges();

                        r.Msg = "数据写入成功";
                        r.Code = 0;
                        r.mac = parameter.mac;
                        r.Data = new RetDeviceEnvironmentDataInfo()
                        {
                            mac = parameter.mac,
                            hum = parameter.hum,
                            tempc = parameter.tempc,
                            status = parameter.status

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
        /// 2.获取的RetEnvironmentInfo温湿度数据
        /// </summary>
        public ReturnItem<RetEnvironmentInfo> GetTemAndHumData(GetDeviceEnvironmentDataInfoModel parameter)
        {
            ReturnItem<RetEnvironmentInfo> r = new ReturnItem<RetEnvironmentInfo>();
            using (MonitoringEntities device = new MonitoringEntities())
            {
                try
                {
                    
                    IQueryable<D_DeviceBuilding> n = device.D_DeviceBuilding.AsQueryable<D_DeviceBuilding>();

                    n = device.D_DeviceBuilding.Where(x => x.IMEI == parameter.IMEI && x.tableId == parameter.tableId);
                    foreach (var s in n)
                    {
                        var deviceinfo = new RetMedicineDetails();
                        deviceinfo.mac = s.mac;
                        parameter.mac = deviceinfo.mac;
                    }
                    if (n.Count() == 0)
                    {
                        r.Data = null;
                        r.Code = -1;
                        r.Msg = "未获取到数据";
                        return r;
                    }
                    else
                    {

                        var getinfo = device.D_DeviceEnvironmentt.Where(s => s.mac == parameter.mac).OrderByDescending(s => s.creattime).FirstOrDefault();

                        if (getinfo == null)
                        {
                            r.Data = null;
                            r.Code = 0;
                            r.Msg = "该设备无信息";
                            return r;
                        }
                        if (getinfo != null)
                        {
                            r.Msg = "设备信息获取成功";
                            r.Code = 0;

                            r.Data = new RetEnvironmentInfo()
                            {
                                mac = getinfo.mac,
                                hum = getinfo.hum,
                                tempc = getinfo.tempc,
                                status = (int)getinfo.status

                            };
                           
                        }
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
        /// 3.药品信息录入到数据库
        /// </summary>
        String MCabintId;
        String MTag;
        public ReturnItem<RetMedicineInfo> AddMedicineInfo(MedicineInfoModel parameter)
        {
            ReturnItem<RetMedicineInfo> r = new ReturnItem<RetMedicineInfo>();
            using (MonitoringEntities device = new MonitoringEntities())
            {
                try
                {
                    IQueryable<D_DeviceBuilding> n = device.D_DeviceBuilding.AsQueryable<D_DeviceBuilding>();
                    IQueryable<D_MedicineListInfo> m = device.D_MedicineListInfo.AsQueryable<D_MedicineListInfo>();
                    n = device.D_DeviceBuilding.Where(x => x.IMEI == parameter.IMEI && x.tableId == parameter.tableId);
                    foreach (var s in n)
                    {
                        var deviceinfo = new RetMedicineDetails();
                        deviceinfo.mac = s.mac;
                        parameter.mac = deviceinfo.mac;

                    }
                    if (n.Count() == 0)
                    {
                        r.Data = null;
                        r.Code = -1;
                        r.Msg = "未找到相对应的设备";
                        return r;
                    }
                    else
                    {
                      
                            D_MedicineListInfo medicineinfo = new D_MedicineListInfo()

                            {
                                MedicineId = parameter.MedicineId,
                                MedicineName = parameter.MedicineName,
                                MedicinePrice = parameter.MedicinePrice,
                                Count = parameter.Count,
                                CreatTime = DateTime.Now,
                                Description = parameter.Description,
                                StartDateTime = parameter.StartDateTime,
                                EndDateTime = parameter.EndDateTime,
                                mac = parameter.mac,
                                CabintId = parameter.CabintId,
                                manuName = parameter.manuName,
                                spec = parameter.spec,
                                Image = parameter.Image,
                                tableId = parameter.tableId,



                            };

                            device.D_MedicineListInfo.Add(medicineinfo);

                            device.SaveChanges();

                            r.Msg = "药品存入成功";
                            r.Code = 0;
                            r.mac = parameter.mac;
                            r.Data = new RetMedicineInfo()
                            {
                                mac = medicineinfo.mac,
                                MedicineName = medicineinfo.MedicineName,
                                CabintId = medicineinfo.CabintId
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
        ///3.1.删除药品信息
        /// </summary>



        /// <summary>
        ///4.药品信息展示
        /// </summary>
        public ReturnItem<List<RetMedicineDetails>> GetMedicineInfo(GetMedicineInfoModel parameter)
        {
            ReturnItem<List<RetMedicineDetails>> r = new ReturnItem<List<RetMedicineDetails>>();
            List<RetMedicineDetails> listinfo = new List<RetMedicineDetails>();
            using (MonitoringEntities device = new MonitoringEntities())
            {

                try
                {

                    IQueryable<D_MedicineListInfo> m = device.D_MedicineListInfo.AsQueryable<D_MedicineListInfo>();
                    IQueryable<D_DeviceBuilding> n = device.D_DeviceBuilding.AsQueryable<D_DeviceBuilding>();

                    n = device.D_DeviceBuilding.Where(x => x.IMEI == parameter.IMEI && x.tableId == parameter.tableId);
                    foreach (var s in n)
                    {
                        var deviceinfo = new RetMedicineDetails();
                        deviceinfo.mac = s.mac;
                        parameter.mac = deviceinfo.mac;
                    }
                    if (n.Count() == 0)
                    {
                        r.Data = null;
                        r.Code = -1;
                        r.Msg = "未找到药品柜信息";
                        return r;
                    }
                    else
                    {
   
                        m = device.D_MedicineListInfo.Where(x => x.mac.Equals(parameter.mac)).OrderBy(x => x.CabintId);
                        log.Info(m.Count());
                        
                        if (m.Count() == 0)
                        {
                            r.Data = null;
                            r.Code = 0;
                            r.Msg = "未找到药品信息";
                            return r;
                        }
                        else
                        {

                            List<D_MedicineListInfo> list = m.ToList<D_MedicineListInfo>();
                            r.Msg = "药品信息信息获取成功";
                            r.Code = 0;
                            r.mac = parameter.mac;
                            foreach (var item in list)
                            {
                                var deviceinfo = new RetMedicineDetails();
                                deviceinfo.MedicineName = item.MedicineName;
                                deviceinfo.Count = (int)item.Count;
                                deviceinfo.Image = item.Image;
                                deviceinfo.manuName = item.manuName;
                                deviceinfo.spec = item.spec;
                                deviceinfo.MedicineId = (long)item.MedicineId;
                                deviceinfo.CabintId = item.CabintId;
                                deviceinfo.tableId = item.tableId;
                                listinfo.Add(deviceinfo);
                            }
                            r.Data = listinfo;
                        }
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
        ///5.获取（查询）药品信息 药品名称查询
        /// </summary>
        public ReturnItem<List<RetDisplayMedicineInfo>> DisplayMedicineInfo(DisplayMedicineInfo parameter)
        {
            ReturnItem<List<RetDisplayMedicineInfo>> r = new ReturnItem<List<RetDisplayMedicineInfo>>();
            List<RetDisplayMedicineInfo> listinfo = new List<RetDisplayMedicineInfo>();
            using (MonitoringEntities device = new MonitoringEntities())
            {

                try
                {

                    IQueryable<D_MedicineInfo> m = device.D_MedicineInfo.AsQueryable<D_MedicineInfo>();
  
                        m = device.D_MedicineInfo.Where(x => x.MedicineId == parameter.MedicineId);             
                        if (m.Count() == 0)
                        {
                            r.Data = null;
                            r.Code = -1;
                            r.Msg = "未找到药品信息2222";
                            return r;
                        }
                        else
                        {
                            List<D_MedicineInfo> list = m.ToList<D_MedicineInfo>();
                            r.Msg = "药品信息信息获取成功";
                            r.Code = 0;
                          
                            foreach (var item in list)
                            {
                                var deviceinfo = new RetDisplayMedicineInfo();
                                deviceinfo.MedicineName = item.MedicineName;                                
                                deviceinfo.MedicineId = (long)item.MedicineId;
                                deviceinfo.manuName = item.manuName;
                                deviceinfo.spec = item.spec;
                                deviceinfo.Image = item.Image;
                                
                                listinfo.Add(deviceinfo);
                            }
                            r.Data = listinfo;
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
        ///5.2获取（查询）药品信息 药品名称查询
        /// </summary>


        public ReturnItem<List<RetFindMedicineInfo>> FindMedicineInfo(FindMedicineInfo parameter)
        {
            ReturnItem<List<RetFindMedicineInfo>> r = new ReturnItem<List<RetFindMedicineInfo>>();
            List<RetFindMedicineInfo> listinfo = new List<RetFindMedicineInfo>();
            using (MonitoringEntities device = new MonitoringEntities())
            {

                try
                {

                    IQueryable<D_MedicineListInfo> m = device.D_MedicineListInfo.AsQueryable<D_MedicineListInfo>();
                    IQueryable<D_DeviceBuilding> u = device.D_DeviceBuilding.AsQueryable<D_DeviceBuilding>();

                    u = device.D_DeviceBuilding.Where(x => x.IMEI == parameter.IMEI && x.tableId == parameter.tableId);
                    foreach (var s in u)
                    {
                        var deviceinfo = new RetMedicineDetails();
                        deviceinfo.mac = s.mac;
                        parameter.mac = deviceinfo.mac;
                    }
                    // log.Info(n.Count());
                    if (u.Count() == 0)
                    {
                        r.Data = null;
                        r.Code = -1;
                        r.Msg = "未找到药品信息111";
                        return r;
                    }
                    else
                    {
                        
                        m = device.D_MedicineListInfo.Where(x => x.mac.Equals(parameter.mac) && x.MedicineName.IndexOf(parameter.MedicineName) >= 0);

                        if (m.Count() == 0)
                        {
                            r.Data = null;
                            r.Code = -1;
                            r.Msg = "未找到药品信息2222";
                            return r;
                        }
                        else
                        {
                            List<D_MedicineListInfo> list = m.ToList<D_MedicineListInfo>();
                            r.Msg = "药品信息信息获取成功";
                            r.Code = 0;
                            r.mac = parameter.mac;
                            foreach (var item in list)
                            {
                                var deviceinfo = new RetFindMedicineInfo();
                                deviceinfo.MedicineName = item.MedicineName;
                                deviceinfo.Count = (int)item.Count;
                                //deviceinfo.MedicinePrice = (int)item.MedicinePrice;
                                deviceinfo.MedicineId = (long)item.MedicineId;
                                deviceinfo.manuName = item.manuName;
                                deviceinfo.spec = item.spec;
                                deviceinfo.Image = item.Image;
                                deviceinfo.CabintId = item.CabintId;
                                deviceinfo.tableId = item.tableId;
                                listinfo.Add(deviceinfo);
                            }
                            r.Data = listinfo;
                        }
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
        ///6.获取（查询）药品信息 扫码查询
        /// </summary>
        public ReturnItem<List<RetDisplayMedicineInfo>> ScanMedicineInfo(DisplayMedicineInfo parameter)
        {
            ReturnItem<List<RetDisplayMedicineInfo>> r = new ReturnItem<List<RetDisplayMedicineInfo>>();
            List<RetDisplayMedicineInfo> listinfo = new List<RetDisplayMedicineInfo>();
            using (MonitoringEntities device = new MonitoringEntities())
            {

                try
                {

                    IQueryable<D_MedicineInfo> MedicineList = device.D_MedicineInfo.AsQueryable<D_MedicineInfo>();
                     
                    MedicineList = MedicineList.Where(x =>  x.MedicineId ==parameter.MedicineId ||x.MedicineName.IndexOf(parameter.MedicineName) >= 0);
                    //MedicineList = MedicineList.Where(x => x.MedicineId == parameter.MedicineId || x.MedicineName.IndexOf(parameter.MedicineName) >= 0);
                    if (MedicineList.Count() == 0)
                        {
                            r.Data = null;
                            r.Code = -1;
                            r.Msg = "未找到药品信息";
                            return r;
                        }
                        else
                        {
                            List<D_MedicineInfo> list = MedicineList.ToList<D_MedicineInfo>();
                            r.Msg = "药品信息信息获取成功";
                            r.Code = 0;
                            r.mac = parameter.mac;
                            foreach (var item in list)
                            {
                                var deviceinfo = new RetDisplayMedicineInfo();
                                deviceinfo.MedicineName = item.MedicineName;
                                deviceinfo.Count = (int)item.Count;
                               // deviceinfo.MedicinePrice = (int)item.MedicinePrice;
                                deviceinfo.MedicineId = (long)item.MedicineId;
                                deviceinfo.manuName = item.manuName;
                                deviceinfo.spec = item.spec;
                                deviceinfo.Image = item.Image;                           
                                listinfo.Add(deviceinfo);
                            }
                            r.Data = listinfo;
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
        /// 7.药品存入（1.新药品信息录入，2已有药品的库存更改，本部分实现对已有药品进行库存进行更改）
        /// </summary>
        /// <returns>成功返回用户信息,失败返回Null.</returns>
        public ReturnItem<RetMedicineInfo> UpdateMedicineInfo(MedicineInfoModel parameter)
        {
            ReturnItem<RetMedicineInfo> r = new ReturnItem<RetMedicineInfo>();
            using (MonitoringEntities data = new MonitoringEntities())
            {
                try
                {
                    IQueryable<D_DeviceBuilding> n = data.D_DeviceBuilding.AsQueryable<D_DeviceBuilding>();

                    n = data.D_DeviceBuilding.Where(x => x.IMEI == parameter.IMEI && x.tableId == parameter.tableId);
                    foreach (var s in n)
                    {
                        var deviceinfo = new RetMedicineDetails();
                        deviceinfo.mac = s.mac;
                        parameter.mac = deviceinfo.mac;
                    }
                    if (n.Count() == 0)
                    {
                        r.Data = null;
                        r.Code = -1;
                        r.Msg = "未找到药品信息";
                        return r;
                    }
                    else
                    {
                        var medicineinfo = data.D_MedicineListInfo.Where(x => x.MedicineId == parameter.MedicineId && x.mac == parameter.mac && x.CabintId == parameter.CabintId).OrderByDescending(s => s.CreatTime).FirstOrDefault(); 
                        if (medicineinfo == null)
                        {
                            r.Data = null;
                            r.Code = -1;
                            r.Msg = "未找到该药品信息";
                            return r;
                        }
                        if (medicineinfo != null)
                        {
                            medicineinfo.MedicineId = parameter.MedicineId;
                            medicineinfo.Count = medicineinfo.Count + parameter.Count;
                        };
                        data.SaveChanges();
                        r.mac = parameter.mac;
                        r.Msg = "药品存入成功";
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
        /// 8.药品取出 最低不能低于其库存量
        /// </summary>
        /// <returns>成功返回用户信息,失败返回Null.</returns>
        public ReturnItem<RetMedicineInfo> DeleteMedicineInfo(MedicineInfoModel parameter)
        {
            ReturnItem<RetMedicineInfo> r = new ReturnItem<RetMedicineInfo>();
            using (MonitoringEntities data = new MonitoringEntities())
            {
                try
                {
                    IQueryable<D_DeviceBuilding> n = data.D_DeviceBuilding.AsQueryable<D_DeviceBuilding>();

                    n = data.D_DeviceBuilding.Where(x => x.IMEI == parameter.IMEI && x.tableId == parameter.tableId);
                    foreach (var s in n)
                    {
                        var deviceinfo = new RetMedicineDetails();
                        deviceinfo.mac = s.mac;
                        parameter.mac = deviceinfo.mac;
                    }
                    if (n.Count() == 0)
                    {
                        r.Data = null;
                        r.Code = -1;
                        r.Msg = "未找到药品信息";
                        return r;
                    }
                    else
                    {
                        var medicineinfo = data.D_MedicineListInfo.Where(x => x.MedicineId == parameter.MedicineId && x.mac == parameter.mac && x.CabintId == parameter.CabintId).OrderByDescending(s => s.CreatTime).FirstOrDefault();
                        if (medicineinfo == null)
                        {
                            r.Data = null;
                            r.Code = -1;
                            r.Msg = "未找到该药品信息";
                            return r;
                        }
                        if (medicineinfo != null)
                        {
                            medicineinfo.MedicineId = parameter.MedicineId;
                            medicineinfo.Count = medicineinfo.Count - parameter.Count;
                        };
                        if (medicineinfo.Count >= 0)
                        {
                            data.SaveChanges();

                            r.Msg = "药品取出成功";
                            r.Code = 0;
                        }
                        else
                        {

                            r.Msg = "库存量不足，药品取出失败";
                            r.Code = -1;
                        }
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
        ///删除药柜中的格子
        /// </summary>
        String CabintIdMsg;
        int MCount;
        public ReturnItem<RetMedicineInfo> DeleteMedicineCabintId(MedicineInfoModel parameter)
        {
            ReturnItem<RetMedicineInfo> r = new ReturnItem<RetMedicineInfo>();
            using (MonitoringEntities device = new MonitoringEntities())
            {
                try
                {

                    IQueryable<D_DeviceBuilding> n = device.D_DeviceBuilding.AsQueryable<D_DeviceBuilding>();
                    IQueryable<D_MedicineListInfo> m = device.D_MedicineListInfo.AsQueryable<D_MedicineListInfo>();
                    n = device.D_DeviceBuilding.Where(x => x.IMEI == parameter.IMEI && x.tableId == parameter.tableId);
                    
                    foreach (var s in n)
                    {
                        var deviceinfo = new RetMedicineDetails();
                        deviceinfo.mac = s.mac;
                        parameter.mac = deviceinfo.mac;
                    }
                    if (n.Count() == 0)
                    {
                        r.Data = null;
                        r.Code = -1;
                        r.Msg = "未找到药品信息";
                        return r;
                    }
                    else
                    {
                        m = device.D_MedicineListInfo.Where(x => x.mac == parameter.mac && x.CabintId == parameter.CabintId).OrderByDescending(s => s.CreatTime);
                        if (m.Count() == 0)
                        {
                            r.Data = null;
                            r.Code = -1;
                            r.Msg = "未找到药格";
                            return r;
                        }
                        else
                        {
                            foreach (var t in m)
                            {
                                MCount = (int)t.Count;

                            }
                            if (MCount != 0)
                            {

                                r.Data = null;
                                r.Code = -1;
                                r.Msg = "药品数量非空，药格删除失败";
                                return r;
                            }
                            else
                            {

                                // 删除表
                                D_MedicineListInfo deldevice = device.Set<D_MedicineListInfo>().Where(x => x.mac == parameter.mac && x.CabintId == parameter.CabintId).FirstOrDefault();
                                //log.Info(deldevice);
                                if (deldevice.Count == null)
                                {
                                    r.Data = null;
                                    r.Code = -1;
                                    r.Msg = "药格删除失败";
                                    return r;
                                }
                                else
                                {

                                    var entry = device.Entry(deldevice);
                                    //设置该对象的状态为删除  
                                    entry.State = EntityState.Deleted;
                                    device.SaveChanges();



                                    m = device.D_MedicineListInfo.Where(x => x.mac == parameter.mac && x.CabintId == parameter.CabintId);

                                    if (m.Count() == 0)
                                    {
                                        CabintIdMsg = "药格删除成功";


                                    }
                                    r.Msg = CabintIdMsg;
                                    r.Code = 0;
                                }
                            }
                        }
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
        /// 9.设备绑定
        /// </summary>

        String Tag;
        String NMac;
        String NTableId;
        String NTableName;

        public ReturnItem<RetDeviceBuilding> DeviceBuilding(DeviceBuildingModel parameter)
        {
            ReturnItem<RetDeviceBuilding> r = new ReturnItem<RetDeviceBuilding>();
            using (MonitoringEntities device = new MonitoringEntities())
            {
                try
                {
                    IQueryable<D_DeviceBuilding> n = device.D_DeviceBuilding.AsQueryable<D_DeviceBuilding>();

                    n = device.D_DeviceBuilding.Where(x => x.mac == parameter.mac || x.tableId == parameter.tableId || x.tableName == parameter.tableName);
                    log.Info(n.Count());
                    if (n.Count() != 0)
                    {
                        foreach (var s in n)
                        {
                            var deviceinfo = new RetDeviceBuilding();
                            deviceinfo.mac = s.mac;
                            deviceinfo.tableName = s.tableName;
                            deviceinfo.tableId = s.tableId;
                            NMac = deviceinfo.mac;
                            NTableId = deviceinfo.tableId;
                            NTableName = deviceinfo.tableName;

                        }
                        log.Info(NMac);
                        log.Info(NTableId);
                        log.Info(NTableName);
                        if (parameter.mac.Equals(NMac))
                        {
                            Tag = "MAC已存在";

                        }
                        if (parameter.tableId.Equals(NTableId))
                        {
                            Tag = "TableId已存在";

                        }
                        if (parameter.tableName.Equals(NTableName))
                        {
                            Tag = "TableName已存在";

                        }
                        r.Data = null;
                        r.Code = -1;
                        r.Msg = Tag;
                        return r;
                    
                }
                    else { 
                        D_DeviceBuilding newdevice = new D_DeviceBuilding()

                        {

                            mac = parameter.mac,
                            IMEI = parameter.IMEI,
                            tableId = parameter.tableId,
                            tableType =parameter.tableType,
                            tableName =parameter.tableName,
                            creattime = DateTime.Now

                        };

                        device.D_DeviceBuilding.Add(newdevice);

                        device.SaveChanges();

                        r.Msg = "设备绑定成功";
                        r.Code = 0;
                        r.mac = parameter.mac;
                        r.Data = new RetDeviceBuilding()
                        {
                            mac = parameter.mac,
                            IMEI = parameter.IMEI,
                            tableName = parameter.tableName,
                            tableId = parameter.tableId,


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


        // <summary>
        /// 9.1查询IMEI下所有绑定的设备
        /// </summary>
        /// <summary>
        
        public ReturnItem<List<RetDeviceBuilding>> GetDeviceBuilding(DeviceBuildingModel parameter)
        {
            ReturnItem<List<RetDeviceBuilding>> r = new ReturnItem<List<RetDeviceBuilding>>();
            List<RetDeviceBuilding> listinfo = new List<RetDeviceBuilding>();
            using (MonitoringEntities device = new MonitoringEntities())
            {

                try
                {

                    
                    IQueryable<D_DeviceBuilding> n = device.D_DeviceBuilding.AsQueryable<D_DeviceBuilding>();

                    n = device.D_DeviceBuilding.Where(x => x.IMEI == parameter.IMEI );
                   
                    if (n.Count() == 0)
                    {
                        r.Data = null;
                        r.Code = -1;
                        r.Msg = "未找该设备";
                        return r;
                    }
                    else
                    {

                      
                            List<D_DeviceBuilding> list = n.ToList<D_DeviceBuilding>();
                            r.Msg = "IMEI下所有绑定的设备查询成功";
                            r.Code = 0;
                         
                            foreach (var item in list)
                            {
                                var deviceinfo = new RetDeviceBuilding();
                                deviceinfo.tableId = item.tableId;
                                deviceinfo.mac = item.mac;
                                deviceinfo.tableName = item.tableName;
                                deviceinfo.tableType = item.tableType;
                                listinfo.Add(deviceinfo);
                            }
                            r.Data = listinfo;
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

        // <summary>
        /// 9.2 切换药柜时选择的是药柜名称
        /// </summary>
        /// <summary>
        public ReturnItem<RetDeviceBuilding> GetDeviceTableId(DeviceBuildingModel parameter)
        {
            ReturnItem<RetDeviceBuilding> r = new ReturnItem<RetDeviceBuilding>();
            using (MonitoringEntities device = new MonitoringEntities())
            {
                try
                {

                    IQueryable<D_DeviceBuilding> n = device.D_DeviceBuilding.AsQueryable<D_DeviceBuilding>();

                        ///var getinfo = device.D_DeviceEnvironmentt.Where(s => s.mac == parameter.mac).OrderByDescending(s => s.creattime).FirstOrDefault();
                        var getinfo = device.D_DeviceBuilding.Where(x => x.IMEI == parameter.IMEI && x.tableName == parameter.tableName).OrderByDescending(s => s.creattime).FirstOrDefault(); 
                       
                        if (getinfo == null)
                        {
                            r.Data = null;
                            r.Code = -1;
                            r.Msg = "未获取到药柜ID";
                            return r;
                        }
                        if (getinfo != null)
                        {
                            r.Msg = "药柜ID信息获取成功";
                            r.Code = 0;

                        r.Data = new RetDeviceBuilding()
                        {
                            tableId = getinfo.tableId,
                            tableType =getinfo.tableType,

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

        // <summary>
        /// 9.3 删除设备
        /// </summary>
        /// <summary>
        String TagMsg;
        public ReturnItem<RetDeviceBuilding> DeleteDevice(DeviceBuildingModel parameter)
        {
            ReturnItem<RetDeviceBuilding> r = new ReturnItem<RetDeviceBuilding>();
            using (MonitoringEntities device = new MonitoringEntities())
            {
                try
                {
                    // 删除表
                    D_DeviceBuilding deldevice = device.Set<D_DeviceBuilding>().Where(x => x.IMEI == parameter.IMEI && x.tableId == parameter.tableId).FirstOrDefault();
                    log.Info(deldevice);
                    if (deldevice == null)
                    {
                        r.Data = null;
                        r.Code = -1;
                        r.Msg = "设备删除失败";
                        return r;
                    }
                    else
                    {

                        var entry = device.Entry(deldevice);
                        //设置该对象的状态为删除  
                        entry.State = EntityState.Deleted;
                        device.SaveChanges();

                        IQueryable<D_DeviceBuilding> n = device.D_DeviceBuilding.AsQueryable<D_DeviceBuilding>();

                        n = device.D_DeviceBuilding.Where(x => x.IMEI == parameter.IMEI && x.tableId == parameter.tableId);

                        if (n.Count() == 0)
                        {
                            TagMsg = "该台设备删除成功";


                        }
                        r.Msg = TagMsg;
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
       
                

        /// 10.获取温度报表数据
        /// </summary>
        public RetGetTemChart GetTemChart(GetTemChartModel parameter)
        {

            RetGetTemChart r = new RetGetTemChart();
            List<String> t = new List<String>();
            List<String> h = new List<String>();
            List<RetGetTemChart> listinfo = new List<RetGetTemChart>();
           
            using (MonitoringEntities device = new MonitoringEntities())
            {

                try
                {

                    IQueryable<D_DeviceEnvironmentt> m = device.D_DeviceEnvironmentt.AsQueryable<D_DeviceEnvironmentt>();
                    IQueryable<D_DeviceBuilding> n = device.D_DeviceBuilding.AsQueryable<D_DeviceBuilding>();

                    n = device.D_DeviceBuilding.Where(x => x.IMEI == parameter.IMEI && x.tableId == parameter.tableId);
                    foreach (var s in n)
                    {
                        var deviceinfo = new RetGetTemChart();
                        deviceinfo.mac = s.mac;
                        parameter.mac = deviceinfo.mac;
                    }
                    log.Info(n.Count());
                    log.Info(parameter.mac);
                    if (n.Count() == 0)
                    {

                        r.msg = "温度历史数据获取失败";
                        return r;
                    }
                    else
                    {
                        m = device.D_DeviceEnvironmentt.Where(x => x.mac == parameter.mac && (parameter.StartDateTime <= x.creattime && x.creattime <= parameter.EndDateTime));
                        log.Info(m.Count());
                        foreach (var s in m)
                        {
                            var deviceinfo = new RetGetTemChart();
                            
                        }
                        if (m.Count() == 0)
                        {

                            r.msg = "温度历史数据获取失败";
                            return r;
                        }
                        else
                        {
                            List<D_DeviceEnvironmentt> list = m.ToList<D_DeviceEnvironmentt>();
                            r.msg = "温度历史数据获取成功";


                            foreach (var item in list)
                            {

                                DateTime a =item.creattime.Value;
                                String aa = a.ToString("yyyy-MM-dd hh:mm:ss").Remove(15, 3);

                                
                              
                                t.Add(aa);
                                h.Add(item.tempc);

                            }
                            
                            log.Info(t.ToArray().Length);
                            
                            List<String> tt = new List<String>();
                            List<String> hh = new List<String>();
                            for (int i=0;i< t.ToArray().Length;i+= 510)
                            {
                                tt.Add(t[i]);
                                hh.Add(h[i]);


                            }
                           
                            r.time = tt;
                            r.tempc = hh;
                        }
                    }
                }

                catch (Exception e)
                {
                    r.msg = "内部错误请重试";
                    log.ErrorFormat("内部错误：{0},{1}", e.Message, e.StackTrace);

                }
            }
            return r;
        }

        /// <summary>
        /// 11.获取湿度报表数据
        /// </summary>

        public RetGetHumChart GetHumChart(GetTemChartModel parameter)

        {

            
            RetGetHumChart r = new RetGetHumChart();

            List<RetGetHumChart> listinfo = new List<RetGetHumChart>();
            List<String> t = new List<String>();
            List<String> h = new List<String>();
            using (MonitoringEntities device = new MonitoringEntities())
            {

                try
                {

                    IQueryable<D_DeviceEnvironmentt> m = device.D_DeviceEnvironmentt.AsQueryable<D_DeviceEnvironmentt>();
                    IQueryable<D_DeviceBuilding> n = device.D_DeviceBuilding.AsQueryable<D_DeviceBuilding>();

                    n = device.D_DeviceBuilding.Where(x => x.IMEI == parameter.IMEI && x.tableId == parameter.tableId);
                    foreach (var s in n)
                    {
                        var deviceinfo = new RetMedicineDetails();
                        deviceinfo.mac = s.mac;
                        parameter.mac = deviceinfo.mac;
                    }
                    if (n.Count() == 0)
                    {
                       
                        r.msg = "湿度历史数据获取失败";
                        return r;
                    }
                    else
                    {
                        
                        m = device.D_DeviceEnvironmentt.Where(x => x.mac == parameter.mac && (parameter.StartDateTime <= x.creattime && x.creattime <= parameter.EndDateTime));
                        log.Info(m.Count());
                        if (m.Count() == 0)
                        {
                           
                            r.msg = "湿度历史数据获取失败";
                            return r;
                        }
                        else
                        {
                            List<D_DeviceEnvironmentt> list = m.ToList<D_DeviceEnvironmentt>();
                            r.msg = "湿度历史数据获取成功";
                                                    
                            foreach (var item in list)
                            {
                                DateTime a = item.creattime.Value;
                                String aa = a.ToString("yyyy-MM-dd hh:mm:ss").Remove(15, 3);
                                t.Add(aa);                               
                                h.Add(item.hum);
                            }

                            List<String> tt = new List<String>();
                            List<String> hh = new List<String>();
                            for (int i = 0; i < t.ToArray().Length; i+=510)
                            {
                                tt.Add(t[i]);
                                hh.Add(h[i]);
                            }
                            r.time = tt;
                            r.hum = hh;
                            
                        }
                    }
                }

                catch (Exception e)
                {
                    r.msg = "内部错误请重试";
                    log.ErrorFormat("内部错误：{0},{1}", e.Message, e.StackTrace);
                    
                }
            }
            return r;
        }

        /// <summary>
        /// 12.获取开关门历史状态数据
        /// </summary>

        public RetGetStatusChart GetStatusChart(GetTemChartModel parameter)

        {


            RetGetStatusChart r = new RetGetStatusChart();

            List<RetGetStatusChart> listinfo = new List<RetGetStatusChart>();
            List<String> t = new List<String>();
            List<int> h = new List<int>();
            using (MonitoringEntities device = new MonitoringEntities())
            {

                try
                {

                    IQueryable<D_DeviceEnvironmentt> m = device.D_DeviceEnvironmentt.AsQueryable<D_DeviceEnvironmentt>();
                    IQueryable<D_DeviceBuilding> n = device.D_DeviceBuilding.AsQueryable<D_DeviceBuilding>();

                    n = device.D_DeviceBuilding.Where(x => x.IMEI == parameter.IMEI && x.tableId == parameter.tableId);
                    foreach (var s in n)
                    {
                        var deviceinfo = new RetMedicineDetails();
                        deviceinfo.mac = s.mac;
                        parameter.mac = deviceinfo.mac;
                    }
                    if (n.Count() == 0)
                    {

                        r.msg = "开关门历史数据获取失败";
                        return r;
                    }
                    else
                    {

                        m = device.D_DeviceEnvironmentt.Where(x => x.mac == parameter.mac && (parameter.StartDateTime <= x.creattime && x.creattime <= parameter.EndDateTime));
                        log.Info(m.Count());
                        if (m.Count() == 0)
                        {

                            r.msg = "开关门历史数据获取失败";
                            return r;
                        }
                        else
                        {
                            List<D_DeviceEnvironmentt> list = m.ToList<D_DeviceEnvironmentt>();
                            r.msg = "开关门历史数据获取成功";

                            foreach (var item in list)
                            {
                                DateTime a = item.creattime.Value;
                                String aa = a.ToString("yyyy-MM-dd hh:mm:ss").Remove(15, 3);
                                int status = item.status.Value;
                                t.Add(aa);
                                h.Add(status);
                            }

                            List<String> tt = new List<String>();
                            List<int> hh = new List<int>();
                            for (int i = 0; i < t.ToArray().Length; i += 510)
                            {
                                tt.Add(t[i]);
                                hh.Add(h[i]);
                            }
                            r.time = tt;
                            r.status = hh;

                        }
                    }
                }

                catch (Exception e)
                {
                    r.msg = "内部错误请重试";
                    log.ErrorFormat("内部错误：{0},{1}", e.Message, e.StackTrace);

                }
            }
            return r;
        }

        /// <summary>
        /// 13.制定报警策略
        /// </summary>

        public ReturnItem<RetAlertPolicies> AlertPolicies(AlertPoliciesModel parameter)
        {
            ReturnItem<RetAlertPolicies> r = new ReturnItem<RetAlertPolicies>();
            using (MonitoringEntities device = new MonitoringEntities())
            {
                try
                {
                    IQueryable<D_DeviceBuilding> n = device.D_DeviceBuilding.AsQueryable<D_DeviceBuilding>();

                    n = device.D_DeviceBuilding.Where(x => x.IMEI == parameter.IMEI && x.tableId == parameter.tableId);
                    foreach (var s in n)
                    {
                        var deviceinfo = new RetMedicineDetails();
                        deviceinfo.mac = s.mac;
                        parameter.mac = deviceinfo.mac;
                    }
                    if (n.Count() == 0)
                    {
                        r.Data = null;
                        r.Code = -1;
                        r.Msg = "未找到相对应的设备";
                        return r;
                    }
                    else
                    {
                        
                        D_AlertPolicies newstrategy = new D_AlertPolicies()

                        {

                            IMEI = parameter.IMEI,
                            mac =parameter.mac,
                            MinTempc = parameter.MinTempc,
                            MaxTempc = parameter.MaxTempc,
                            MinHum = parameter.MinHum,
                            MaxHum = parameter.MaxHum,   
                            Alarmtime = parameter.Alarmtime,                         
                            creattime = DateTime.Now

                        };

                        device.D_AlertPolicies.Add(newstrategy);

                        device.SaveChanges();

                        r.Msg = "报警策略设置成功";
                        r.Code = 0;                       
                        r.Data = new RetAlertPolicies()
                        {
                            IMEI = parameter.IMEI,
                            MinTempc = parameter.MinTempc,
                            MaxTempc = parameter.MaxTempc,
                            MinHum = parameter.MinHum,
                            MaxHum = parameter.MaxHum,

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
        /// 14.温湿度报警信息写入数据库
        /// </summary>
        String Tag1;
        String Tag2;
        String Tag22;
        String Tempc;
        String Hum;
        
        public ReturnItem<List<DeviceMonitoringDAL.D_AlarmStrategy>> AlarmInfo(AlertInfoModel parameter)
        {


            ReturnItem<List<DeviceMonitoringDAL.D_AlarmStrategy>> r = new ReturnItem<List<DeviceMonitoringDAL.D_AlarmStrategy>>();
            List<DeviceMonitoringDAL.D_AlarmStrategy> listinfo = new List<DeviceMonitoringDAL.D_AlarmStrategy>();
        
            using (MonitoringEntities device = new MonitoringEntities())
            {
                try
                {
                    IQueryable<D_DeviceBuilding> n = device.D_DeviceBuilding.AsQueryable<D_DeviceBuilding>();
                    IQueryable<DeviceMonitoringDAL.D_AlarmStrategy> m = device.D_AlarmStrategy.AsQueryable();
                   
                    n = device.D_DeviceBuilding.Where(x => x.IMEI == parameter.IMEI);
                   
                    if (n.Count() == 0)
                    {
                        r.Data = null;
                        r.Code = -1;
                        r.Msg = "未找到相对应的设备";
                        return r;
                    }

                    else
                    { 
                        var deviceBuildings = n.ToArray();

                        for(int i = 0; i < n.Count(); i++)
                        {
                            D_DeviceBuilding b = deviceBuildings[i];
                      
                          
                            var getinfo = device.D_DeviceEnvironmentt.Where(s => s.mac == b.mac).OrderByDescending(s => s.creattime).FirstOrDefault();
                            if (getinfo == null)
                            {
                                continue;
                            }
                            log.Info(getinfo.tempc);
                            double a1 = double.Parse(getinfo.tempc); //实时温度

                            //log.Info(getinfo.hum);
                            double a2 = double.Parse(getinfo.hum);  //实时湿度

                            var getAlertinfo = device.D_AlertPolicies.Where(s => s.mac == b.mac).OrderByDescending(s => s.creattime).FirstOrDefault();
                            if (getAlertinfo == null)
                            {
                                continue;
                            }
                            //log.Info(getAlertinfo.MaxHum);
                            double b1 = double.Parse(getAlertinfo.MaxHum); //最大湿度 40

                            double b2 = double.Parse(getAlertinfo.MinHum); //最小湿度 10
                                                                           //log.Info(b2);

                            //log.Info(getAlertinfo.MaxTempc);
                            double c1 = double.Parse(getAlertinfo.MaxTempc); //最大温度 10

                            //log.Info(getAlertinfo.MinTempc);
                            double c2 = double.Parse(getAlertinfo.MinTempc); //最小温度 -2


                            if (a2 < b2 || a2 > b1 || a1 < c2 || a1 > c1)
                            {

                                if (a2 < b2 || a2 > b1)
                                {
                                    Tag1 = b.tableName + "设备湿度异常";
                                    Hum = getinfo.hum;

                                }

                                if (a1 < c2 || a1 > c1)
                                {
                                    Tag2 = b.tableName + "设备温度异常";
                                    Tempc = getinfo.tempc;

                                }
                                if (!string.IsNullOrEmpty(Tag1) || !string.IsNullOrEmpty(Tag2))
                                {
                                    if (!string.IsNullOrEmpty(Tag1) && !string.IsNullOrEmpty(Tag2))
                                    {
                                        Tag22 = "," + Tag2;

                                    }
                                    else
                                    {
                                        Tag22 = Tag2;

                                    }
                                }
                                DeviceMonitoringDAL.D_AlarmStrategy newstrategy = new DeviceMonitoringDAL.D_AlarmStrategy()
                                {
                                    mac = b.mac,
                                    TempcValue = Tempc,
                                    HumValue = Hum,

                                    Tag = Tag1 + Tag22,
                                    Creattime = DateTime.Now,
                                };
                               
                                device.D_AlarmStrategy.Add(newstrategy);

                                device.SaveChanges();


                                listinfo.Add(newstrategy);

                            }

                        }
                        if(listinfo.FirstOrDefault() == null)
                        {
                            r.Data = null;
                            r.Msg = "当前设备无报警信息";
                            r.Code = -1;
                        }
                        else { 
                        r.Data = listinfo;
                        r.Msg = "报警信息写入成功";
                        r.Code = 0;
                    }
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
        /// 17.开关门报警信息写入数据库
        /// </summary>
        String DoorTag;
      
        public ReturnItem<List<DeviceMonitoringDAL.D_AlarmDoorPolicies>> AlarmDoorInfo(AlertDoorModel parameter)
        {


            ReturnItem<List<DeviceMonitoringDAL.D_AlarmDoorPolicies>> r = new ReturnItem<List<DeviceMonitoringDAL.D_AlarmDoorPolicies>>();
            List<DeviceMonitoringDAL.D_AlarmDoorPolicies> listinfo = new List<DeviceMonitoringDAL.D_AlarmDoorPolicies>();
            using (MonitoringEntities device = new MonitoringEntities())
            {
                try
                {
                    IQueryable<D_DeviceBuilding> n = device.D_DeviceBuilding.AsQueryable<D_DeviceBuilding>();
                    IQueryable<D_DeviceEnvironmentt> m = device.D_DeviceEnvironmentt.AsQueryable<D_DeviceEnvironmentt>();       
                    
                    n = device.D_DeviceBuilding.Where(x => x.IMEI == parameter.IMEI );
              
                    if (n.Count() == 0)
                    {
                        r.Data = null;
                        r.Code = -1;
                        r.Msg = "未找到相对应的设备";
                        return r;
                    }
                    else
                    {
                        var deviceBuilding = n.ToArray();

                        for (int i = 0; i < n.Count(); i++)
                        {
                            D_DeviceBuilding b = deviceBuilding[i];

                            var getAlertinfo = device.D_AlertPolicies.Where(s => s.mac == b.mac).OrderByDescending(s => s.creattime).FirstOrDefault();
                            if (getAlertinfo == null)
                            {
                                continue;
                            }


                            if (getAlertinfo.Alarmtime != null)
                            {

                                if (getAlertinfo.Alarmtime == 30 )
                                {
                                  
                                    m = device.D_DeviceEnvironmentt.Where(s => s.mac == b.mac).OrderByDescending(s => s.creattime).Take(5);

                                    if (m.Count() >= 5)
                                    {
                                        if (m.FirstOrDefault().status == 1)
                                        {
                                            DoorTag = b.tableName + "设备开关门异常";
                                        }
                                    }
                                    else
                                    {
                                        continue;
                                    }
                               
                                
                                
                                }

                               if (getAlertinfo.Alarmtime == 60 )
                                {
                                    m = device.D_DeviceEnvironmentt.Where(s => s.mac == b.mac).OrderByDescending(s => s.creattime).Take(9);
                                    if (m.Count() >= 9)
                                    {
                                        if (m.FirstOrDefault().status == 1)
                                        {
                                            DoorTag = b.tableName + "设备开关门异常";
                                        }
                                    }
                                    else
                                    {
                                        continue;

                                    }

                                }
                                if (getAlertinfo.Alarmtime == 120 )
                                {
                                    m = device.D_DeviceEnvironmentt.Where(s => s.mac == b.mac).OrderByDescending(s => s.creattime).Take(17);
                                    if ( m.Count() >= 17) { 
                                    if (m.FirstOrDefault().status == 1)
                                    {
                                        DoorTag = b.tableName + "设备开关门异常";
                                    }
                                }
                                else
                                    {
                                        continue;

                                    }
                                }
                                if (getAlertinfo.Alarmtime == 180 )
                                {
                                    m = device.D_DeviceEnvironmentt.Where(s => s.mac == b.mac).OrderByDescending(s => s.creattime).Take(23);
                                    if(m.Count() >= 23) { 
                                    if (m.FirstOrDefault().status == 1)
                                    {
                                        DoorTag = b.tableName + "设备开关门异常";
                                    }
                                    }
                                    else
                                    {
                                        continue;

                                    }
                                }
                                if (getAlertinfo.Alarmtime == 300 )
                                {
                                    m = device.D_DeviceEnvironmentt.Where(s => s.mac == b.mac).OrderByDescending(s => s.creattime).Take(43);
                                    if (m.Count() >= 43) { 
                                    if (m.FirstOrDefault().status == 1)
                                    {
                                        DoorTag = b.tableName + "设备开关门异常";
                                       }
                                     }
                                    else
                                    {
                                        continue;

                                    }
                                }
                                DeviceMonitoringDAL.D_AlarmDoorPolicies newstrategy = new DeviceMonitoringDAL.D_AlarmDoorPolicies()
                                {
                                    mac = b.mac,
                                    tag = DoorTag,
                                    Creattime = DateTime.Now,
                                };
                                if (DoorTag != null)
                                {
                                    device.D_AlarmDoorPolicies.Add(newstrategy);

                                    device.SaveChanges();
                                    listinfo.Add(newstrategy);
                                }
                                else {
                                    continue;
                                }

                        }

                          
                        }
                        if (listinfo.FirstOrDefault() == null)
                        {
                            r.Data = null;
                            r.Msg = "当前无开关门异常信息";
                            r.Code = -1;
                        }
                        else
                        {
                            r.Data = listinfo;
                            r.Msg = "开关门异常信息获取成功";
                            r.Code = 0;
                        }
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
        //foreach (var i in m)
        //  {
        //    var deviceinfo = new RetDeviceEnvironmentDataInfo();
        //         deviceinfo.status = (int)i.status;
        //            log.Info(deviceinfo.status);
        //       }
        /// <summary>
        


        /// <summary>
        /// 利用ID获取设备模板列表信息
        /// </summary>
        /// <returns>成功返回用户信息,失败返回Null.</returns>
        public ReturnItem<List<RetDeviceModelInfo>> GetDeviceModelInfo(DeviceModelInfoModel parameter)
        {
            ReturnItem<List<RetDeviceModelInfo>> r = new ReturnItem<List<RetDeviceModelInfo>>();
            List<RetDeviceModelInfo> listinfo = new List<RetDeviceModelInfo>();
            using (MonitoringEntities device = new MonitoringEntities())
            {
                try
                {
                    int PID = int.Parse(parameter.OrgID);
                    IQueryable<D_DeviceModel> deviceList = device.D_DeviceModel.AsQueryable<D_DeviceModel>();
                    deviceList = deviceList
                        //.Join(user.U_Organization, x => x.OrgID, x => x.ID, (a, b) => new { a, b })
                        .Where(x => x.OrgID == PID);
                    if (parameter.Name != null && !"".Equals(parameter.Name))
                    {
                        deviceList = deviceList.Where(a => a.Name.IndexOf(parameter.Name) >= 0);
                    }
                    deviceList = deviceList.OrderByDescending(a => a.CreateTime);
                    if (deviceList == null)
                    {
                        r.Data = null;
                        r.Code = -1;
                        r.Msg = "未找到设备";
                        return r;
                    }
                    if (deviceList != null)
                    {
                        r.Count = deviceList.Count();
                        if (parameter.PageIndex != 0 && parameter.PageSize != 0)
                        {
                            deviceList = deviceList.Skip((parameter.PageIndex - 1) * (parameter.PageSize)).Take(parameter.PageSize);
                        }
                        List<D_DeviceModel> list = deviceList.ToList<D_DeviceModel>();
                        r.Msg = "设备模板信息获取成功";
                        r.Code = 0;
                        foreach (var item in list)
                        {
                            var deviceinfo = new RetDeviceModelInfo();
                            deviceinfo.ID = item.ID;
                            deviceinfo.ModelLabel = item.ModelLabel;
                            deviceinfo.Name = item.Name;
                            deviceinfo.Description = item.Description;
                            deviceinfo.CreateUserID = item.CreateUserID.ToString();
                            deviceinfo.CreateTime = item.CreateTime;
                            listinfo.Add(deviceinfo);
                        }
                        r.Data = listinfo;
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
        /// 新增设备模板信息
        /// </summary>
        public ReturnItem<RetDeviceModelInfo> AddDeviceModelInfo(DeviceModelInfoModel parameter)
        {
            ReturnItem<RetDeviceModelInfo> r = new ReturnItem<RetDeviceModelInfo>();
            using (MonitoringEntities device = new MonitoringEntities())
            {
                try
                {
                    int PID = int.Parse(parameter.OrgID);
                    var deviceinfo = device.D_DeviceModel.Where(x => x.ModelLabel == parameter.ModelLabel && x.OrgID == PID).FirstOrDefault();
                    if (deviceinfo != null)
                    {
                        r.Data = null;
                        r.Code = -1;
                        r.Msg = "设备模板已存在";
                        return r;
                    }
                    if (deviceinfo == null)
                    {
                        string num = "";
                        System.Random Random = new System.Random();
                        num = Random.Next(0, 99999999).ToString();
                        D_DeviceModel newdevice = new D_DeviceModel()
                        {
                            ModelLabel = parameter.ModelLabel == null ? num : parameter.ModelLabel,
                            Name = parameter.Name == null ? "" : parameter.Name,
                            Description = parameter.Description == null ? "" : parameter.Description,
                            CreateTime = DateTime.Now,
                            CreateUserID = Convert.ToInt32(parameter.CreateUserID),
                            OrgID = Convert.ToInt32(parameter.OrgID),
                            SortID = Convert.ToInt32(parameter.SortID)
                        };
                        device.D_DeviceModel.Add(newdevice);
                        device.SaveChanges();

                        var newdevicemodeliteminfo = device.D_DeviceModel.Where(x => x.ModelLabel == parameter.ModelLabel && x.OrgID == PID).FirstOrDefault();

                        foreach (var item in parameter.Domains)
                        {
                            string unit = JsonConvert.SerializeObject(item.Unit);
                            D_DeviceModelItem newitem = new D_DeviceModelItem()
                            {
                                DeviceModelID = newdevicemodeliteminfo.ID,
                                Name = item.Name,
                                PropertyLabel = item.PropertyLabel,
                                Unit = unit,
                                Description = item.Description,
                                OrgID = Convert.ToInt32(parameter.OrgID),
                                ValueType = Convert.ToInt32(item.ValueType.ToString()),
                            };
                            device.D_DeviceModelItem.Add(newitem);
                        }
                        device.SaveChanges();

                        r.Msg = "设备模板新增成功";
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
        /// 获取所有设备模板信息
        /// </summary>
        /// <returns>成功返回模板list,失败返回Null.</returns>
        public ReturnItem<List<RetDeviceModelInfo>> GetAllDeviceModelInfo()
        {
            ReturnItem<List<RetDeviceModelInfo>> r = new ReturnItem<List<RetDeviceModelInfo>>();
            List<RetDeviceModelInfo> deviceModelInfoList = new List<RetDeviceModelInfo>();
            using (MonitoringEntities device = new MonitoringEntities())
            {
                try
                {
                    IQueryable<D_DeviceModel> deviceModelList = device.D_DeviceModel.AsQueryable<D_DeviceModel>()
                        .Where(x => true);
                    //var deviceModelList = device.D_DeviceModel.Where(x => true).ToList();
                    if (deviceModelList == null)
                    {
                        r.Data = null;
                        r.Code = -1;
                        r.Msg = "未找到设备模板";
                        return r;
                    }
                    if (deviceModelList != null)
                    {
                        r.Msg = "设备模板获取成功";
                        r.Code = 0;
                        foreach (var item in deviceModelList)
                        {
                            RetDeviceModelInfo model = new RetDeviceModelInfo();
                            model.ID = item.ID;
                            model.Name = item.Name;
                            model.ModelLabel = item.ModelLabel;
                            model.Description = item.Description;
                            model.UpdateTime = item.UpdateTime;
                            model.CreateUserID = item.CreateUserID.ToString();
                            model.CreateTime = item.CreateTime;
                            model.UpdateUserID = item.UpdateUserID.ToString();
                            model.SortID = item.SortID.ToString();
                            model.OrgID = item.OrgID.ToString();

                            deviceModelInfoList.Add(model);
                        }
                        r.Data = deviceModelInfoList;
                    }
                }
                catch (Exception e)
                {
                    log.ErrorFormat("内部错误：{0},{1}", e.Message, e.StackTrace);
                    r.Msg = "内部错误请重试";
                    r.Code = -1;
                }
            }
            return r;
        }
        /// <summary>
        /// 根据设备模板ID获取该模板的所有设备
        /// </summary>
        /// <returns>成功返回设备列表,失败返回Null.</returns>
        public ReturnItem<List<RetDeviceInfo>> GetDevicesByID(DeviceModelInfoModel parameter)
        {
            ReturnItem<List<RetDeviceInfo>> r = new ReturnItem<List<RetDeviceInfo>>();
            List<RetDeviceInfo> list = new List<RetDeviceInfo>();
            using (MonitoringEntities device = new MonitoringEntities())
            {
                try
                {
                    IQueryable<D_Devices> deviceList = device.D_Devices.AsQueryable<D_Devices>()
                        .Where(x => x.DeviceModelID == parameter.ID);
                    if (deviceList == null)
                    {
                        r.Data = null;
                        r.Code = -1;
                        r.Msg = "没有属于该设备模板的设备";
                        return r;
                    }
                    if (deviceList != null)
                    {
                        r.Msg = "该设备模板的设备列表获取成功";
                        r.Code = 0;
                        foreach (var item in deviceList)
                        {
                            RetDeviceInfo d = new RetDeviceInfo();
                            d.ID = item.ID.ToString();
                            d.DeviceType = item.DeviceType;
                            d.Name = item.Name;
                            d.DeviceLabel = item.DeviceLabel;
                            d.DeviceModelID = item.DeviceModelID.ToString();
                            d.ConnectType = item.ConnectType.ToString();
                            d.DataConnectID = item.DataConnectID.ToString();
                            d.GPS = item.GPS;
                            d.Phone = item.Phone;
                            d.Status = item.Status.ToString();
                            d.Remark = item.Remark;
                            d.UpdateTime = item.UpdateTime;
                            d.CreateUserID = item.CreateUserID.ToString();
                            d.CreateTime = item.CreateTime;
                            d.UpdateUserID = item.UpdateUserID.ToString();
                            d.OrgID = item.OrgID.ToString();
                            d.Manufacturer = item.Manufacturer;
                            d.RunningState = item.RunningState.ToString();
                            d.GroupID = item.GroupID.ToString();
                            list.Add(d);
                        }
                        r.Data = list;
                    }
                }
                catch (Exception e)
                {
                    log.ErrorFormat("内部错误：{0},{1}", e.Message, e.StackTrace);
                    r.Msg = "内部错误请重试";
                    r.Code = -1;
                }
            }
            return r;

        }
        /// <summary>
        /// 利用ID获取设备模板信息
        /// </summary>
        /// <returns>成功返回用户信息,失败返回Null.</returns>
        public ReturnItem<RetDeviceModelInfo> GetDeviceModelInfoByID(DeviceModelInfoModel parameter)
        {
            ReturnItem<RetDeviceModelInfo> r = new ReturnItem<RetDeviceModelInfo>();
            List<RetDeviceModelItemInfo> listinfo = new List<RetDeviceModelItemInfo>();
            using (MonitoringEntities device = new MonitoringEntities())
            {
                try
                {
                    var deviceInfo = device.D_DeviceModel.Where(x => x.ID == parameter.ID).FirstOrDefault();
                    if (deviceInfo == null)
                    {
                        r.Data = null;
                        r.Code = -1;
                        r.Msg = "未找到设备";
                        return r;
                    }
                    if (deviceInfo != null)
                    {
                        r.Msg = "设备模板获取成功";
                        r.Code = 0;
                        r.Data = new RetDeviceModelInfo()
                        {
                            ID = deviceInfo.ID,
                            Name = deviceInfo.Name,
                            ModelLabel = deviceInfo.ModelLabel,
                            Description = deviceInfo.Description,
                            UpdateTime = deviceInfo.UpdateTime,
                            CreateUserID = deviceInfo.CreateUserID.ToString(),
                            CreateTime = deviceInfo.CreateTime,
                            UpdateUserID = deviceInfo.UpdateUserID.ToString(),
                            SortID = deviceInfo.SortID.ToString(),
                            OrgID = deviceInfo.OrgID.ToString()
                        };
                        IQueryable<D_DeviceModelItem> devicemodelList = device.D_DeviceModelItem.AsQueryable<D_DeviceModelItem>()
                            .Where(x => x.DeviceModelID == deviceInfo.ID);
                        if (devicemodelList == null)
                        {
                            r.Data = null;
                            r.Code = -1;
                            r.Msg = "没有属性数据";
                            return r;
                        }
                        if (devicemodelList != null)
                        {
                            r.Msg = "设备模板属性获取成功";
                            r.Code = 0;
                            foreach (var item in devicemodelList)
                            {
                                List<string> unit = new List<string>();
                                if (item.Unit != "" && item.Unit != null)
                                {
                                    unit = JsonConvert.DeserializeObject<List<string>>(item.Unit);
                                }
                                var iteminfo = new RetDeviceModelItemInfo();
                                iteminfo.ID = item.ID;
                                iteminfo.Name = item.Name;
                                iteminfo.DeviceModelID = item.DeviceModelID;
                                iteminfo.Description = item.Description;
                                iteminfo.Unit = unit;
                                iteminfo.PropertyLabel = item.PropertyLabel;
                                iteminfo.StartDateTime = item.StartDateTime;
                                iteminfo.EndDateTime = item.EndDateTime;
                                iteminfo.OrgID = item.OrgID.ToString();
                                iteminfo.ValueType = item.ValueType.ToString();
                                listinfo.Add(iteminfo);
                            }
                            r.Data.Domains = listinfo;
                        }
                    }
                }
                catch (Exception e)
                {
                    log.ErrorFormat("内部错误：{0},{1}", e.Message, e.StackTrace);
                    r.Msg = "内部错误请重试";
                    r.Code = -1;
                }
            }
            return r;
        }

        /// <summary>
        /// 更新设备模板信息
        /// </summary>
        public ReturnItem<RetDeviceModelInfo> UpdateDeviceModelInfo(DeviceModelInfoModel parameter)
        {
            ReturnItem<RetDeviceModelInfo> r = new ReturnItem<RetDeviceModelInfo>();
            using (MonitoringEntities device = new MonitoringEntities())
            {
                try
                {
                    var deviceinfo = device.D_DeviceModel.Where(x => x.ID == parameter.ID).FirstOrDefault();
                    if (deviceinfo == null)
                    {
                        r.Data = null;
                        r.Code = -1;
                        r.Msg = "未找到该设备模板";
                        return r;
                    }
                    if (deviceinfo != null)
                    {
                        deviceinfo.ModelLabel = parameter.ModelLabel == null ? "" : parameter.ModelLabel;
                        deviceinfo.Name = parameter.Name == null ? "" : parameter.Name;
                        deviceinfo.Description = parameter.Description == null ? "" : parameter.Description;
                        deviceinfo.CreateTime = DateTime.Now;
                        deviceinfo.OrgID = Convert.ToInt32(parameter.OrgID);
                        deviceinfo.SortID = parameter.SortID == "" ? Convert.ToInt32(null) : Convert.ToInt32(parameter.SortID);
                        device.SaveChanges();

                        foreach (var item in parameter.Domains)
                        {
                            if (item.State == "0")
                            {
                                var devicemodelitem = device.D_DeviceModelItem.Where(a => a.ID == item.ID).FirstOrDefault();
                                if (devicemodelitem != null)
                                {
                                    string unit = JsonConvert.SerializeObject(item.Unit);
                                    devicemodelitem.Name = item.Name;
                                    devicemodelitem.PropertyLabel = item.PropertyLabel;
                                    devicemodelitem.Unit = unit;
                                    devicemodelitem.ValueType = Convert.ToInt32(item.ValueType);
                                    devicemodelitem.Description = item.Description;
                                    devicemodelitem.OrgID = Convert.ToInt32(parameter.OrgID);
                                    device.SaveChanges();
                                }
                            }
                            if (item.State == "1")
                            {
                                string unit = JsonConvert.SerializeObject(item.Unit);
                                D_DeviceModelItem newitem = new D_DeviceModelItem()
                                {
                                    DeviceModelID = parameter.ID,
                                    Name = item.Name,
                                    PropertyLabel = item.PropertyLabel,
                                    Unit = unit,
                                    Description = item.Description,
                                    ValueType = Convert.ToInt32(item.ValueType),
                                    OrgID = Convert.ToInt32(parameter.OrgID)
                                };
                                device.D_DeviceModelItem.Add(newitem);
                                device.SaveChanges();
                            }
                            if (item.State == "-1")
                            {
                                var devicemodelitem = device.D_DeviceModelItem.Where(a => a.ID == item.ID).FirstOrDefault();
                                if (devicemodelitem != null)
                                {
                                    var entry = device.Entry(devicemodelitem);
                                    entry.State = EntityState.Deleted;
                                    device.SaveChanges();
                                }
                            }
                        }

                        r.Msg = "设备模板新增成功";
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
        /// 删除设备模板信息
        /// </summary>
        public ReturnItem<RetDeviceModelInfo> DeleteDeviceModelInfo(DeviceModelInfoModel parameter)
        {
            ReturnItem<RetDeviceModelInfo> r = new ReturnItem<RetDeviceModelInfo>();
            using (MonitoringEntities device = new MonitoringEntities())
            {
                try
                {
                    D_DeviceModelItem deviceinfo = device.Set<D_DeviceModelItem>().Where(x => x.DeviceModelID == parameter.ID).FirstOrDefault();
                    if (deviceinfo != null)
                    {
                        var entry = device.Entry(deviceinfo);
                        //设置该对象的状态为删除  
                        entry.State = EntityState.Deleted;
                        device.SaveChanges();
                    }

                    D_DeviceModel deldevicemodel = device.Set<D_DeviceModel>().Where(x => x.ID == parameter.ID).FirstOrDefault();

                    if (deldevicemodel == null)
                    {
                        r.Data = null;
                        r.Code = -1;
                        r.Msg = "未找到该设备模板";
                        return r;
                    }
                    if (deldevicemodel != null)
                    {
                        var entrymodel = device.Entry(deldevicemodel);
                        //设置该对象的状态为删除  
                        entrymodel.State = EntityState.Deleted;
                        device.SaveChanges();
                    }

                    r.Msg = "设备模板删除成功";
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
        /// 查询设备模板编号是否存在
        /// </summary>
        public ReturnItem<RetDeviceModelInfo> CheckDeviceInfoByModelLabel(DeviceModelInfoModel parameter)
        {
            ReturnItem<RetDeviceModelInfo> r = new ReturnItem<RetDeviceModelInfo>();
            using (MonitoringEntities device = new MonitoringEntities())
            {
                try
                {
                    int PID = int.Parse(parameter.OrgID);
                    var getinfo = device.D_DeviceModel.Where(s => s.ModelLabel == parameter.ModelLabel && s.OrgID == PID).FirstOrDefault();
                    if (getinfo == null)
                    {
                        r.Data = null;
                        r.Code = 1;
                        r.Msg = "未找到设备编码";
                        return r;
                    }
                    if (getinfo != null)
                    {
                        r.Msg = "已存在设备编码";
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
        /// 查询设备模板属性标识符是否存在
        /// </summary>
        public ReturnItem<RetDeviceModelItemInfo> CheckDeviceInfoByPropertyLabel(DeviceModelItemInfoModel parameter)
        {
            ReturnItem<RetDeviceModelItemInfo> r = new ReturnItem<RetDeviceModelItemInfo>();
            using (MonitoringEntities device = new MonitoringEntities())
            {
                try
                {
                    int PID = int.Parse(parameter.OrgID);
                    var getinfo = device.D_DeviceModelItem.Where(s => s.PropertyLabel == parameter.PropertyLabel && s.OrgID == PID).FirstOrDefault();
                    if (getinfo == null)
                    {
                        r.Data = null;
                        r.Code = 1;
                        r.Msg = "未找到标识符";
                        return r;
                    }
                    if (getinfo != null)
                    {
                        r.Msg = "已存在标识符";
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
        /// 获得设备的分组信息,组织ID和父组织ID获取，不可组合,parameter为空则返回全部
        /// </summary>
        /// <param name="parameter"></param>
        /// <returns></returns>
        public ReturnItem<List<RetDeviceGroupInfo>> GetDeviceGroupInfo(DeviceGroupModel parameter)
        {
            
            ReturnItem<List<RetDeviceGroupInfo>> r = new ReturnItem<List<RetDeviceGroupInfo>>();
            if (CustomConfigParam.IsUseRedis)
            {
                RedisUtils redisUtils = new RedisUtils();
                if (redisUtils.isCurMethodCached(System.Reflection.MethodBase.GetCurrentMethod(),parameter.OrgID))
                {
                    r.Msg = "设备分组信息获取成功";
                    r.Code = 0;
                    r.Data = redisUtils.getCacheContent<List<RetDeviceGroupInfo>>(System.Reflection.MethodBase.GetCurrentMethod(), parameter.OrgID);
                    r.Count = r.Data.Count();
                    return r;
                }
            }
            List<DeviceGroupModel> groupList = new List<DeviceGroupModel>();
            List<RetDeviceGroupInfo> endList = new List<RetDeviceGroupInfo>();
            using (MonitoringEntities device = new MonitoringEntities())
            {
                try
                {
                    IQueryable<D_DeviceGroup> deviceGroup = device.D_DeviceGroup.AsQueryable<D_DeviceGroup>();
                    if (parameter != null)
                    {
                        if (parameter.ID != null && parameter.ID != "")
                        {
                            var GroupID = Convert.ToInt32(parameter.ID);
                            deviceGroup = deviceGroup.Where(s => s.ID == GroupID);
                        }
                        else if (parameter.ParentGroupID != null && parameter.ParentGroupID != "")
                        {
                            var ParentGroupID = Convert.ToInt32(parameter.ParentGroupID);
                            deviceGroup = deviceGroup.Where(s => s.ParentGroupID == ParentGroupID);
                        }
                        if (parameter.OrgID != null && parameter.OrgID != "")
                        {
                            var OrgID = Convert.ToInt32(parameter.OrgID);
                            deviceGroup = deviceGroup.Where(s => s.OrgID == OrgID);
                        }
                    }
                    deviceGroup = deviceGroup.OrderBy(s => s.CreateTime);
                    if (deviceGroup == null)
                    {
                        r.Data = null;
                        r.Code = -1;
                        r.Msg = "未找到设备分组信息";
                        return r;
                    }
                    else if (deviceGroup != null)
                    {
                        r.Count = deviceGroup.Count();
                        r.Msg = "设备分组信息获取成功";
                        r.Code = 0;
                        foreach (var item in deviceGroup)
                        {
                            if (item.ParentGroupID == null)
                            {
                                DeviceGroupModel parentItem = new DeviceGroupModel();
                                parentItem.ID = item.ID.ToString();
                                parentItem.Name = item.Name;
                                parentItem.Label = item.Label;
                                parentItem.ParentGroupID = item.ParentGroupID.ToString();
                                parentItem.CreateUserID = item.CreateUserID.ToString();
                                parentItem.CreateTime = item.CreateTime;
                                parentItem.Description = item.Description;
                                parentItem.UpdateTime = item.UpdateTime;
                                parentItem.UpdateUserID = item.UpdateUserID.ToString();
                                parentItem.OrgID = item.OrgID.ToString();
                                groupList.Add(parentItem);
                            }
                        }
                        endList = CreateTreeInfo(groupList);
                        r.Data = endList;
                    }
                    if (CustomConfigParam.IsUseRedis)
                    {
                        RedisUtils redisUtil = new RedisUtils();
                        redisUtil.saveToRedis(System.Reflection.MethodBase.GetCurrentMethod(), endList, parameter.OrgID);
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

        ///---新加的---
        ///根据名字得到设备分组名字
        public ReturnItem<List<RetDeviceGroupInfo>> GetDeviceGroupInfoById(DeviceGroupModel parameter)
        {
            ReturnItem<List<RetDeviceGroupInfo>> r = new ReturnItem<List<RetDeviceGroupInfo>>();
            List<RetDeviceGroupInfo> result = new List<RetDeviceGroupInfo>();
            using (MonitoringEntities device = new MonitoringEntities())
            {
                try
                {
                    IQueryable<D_DeviceGroup> deviceGroup = device.D_DeviceGroup.AsQueryable<D_DeviceGroup>();
                    if (parameter != null)
                    {
                        if (parameter.ID != null && parameter.ID != "")
                        {
                            var ID = Convert.ToInt32(parameter.ID);
                            deviceGroup = deviceGroup.Where(s => s.ID ==ID);
                        }
                        if (parameter.OrgID != null && parameter.OrgID != "")
                        {
                            var OrgID = Convert.ToInt32(parameter.OrgID);
                            deviceGroup = deviceGroup.Where(s => s.OrgID == OrgID);
                        }
                       // if (parameter.Name != null && parameter.Name != "")
                       // {
                       //     deviceGroup = deviceGroup.Where(s => s.Name == parameter.Name);
                       // }
                    }
                    deviceGroup = deviceGroup.OrderBy(s => s.CreateTime);
                    if (deviceGroup == null)
                    {
                        r.Data = null;
                        r.Code = -1;
                        r.Msg = "未找到设备分组信息";
                        return r;
                    }
                    else if (deviceGroup != null)
                    {
                        r.Count = deviceGroup.Count();
                        r.Msg = "设备分组信息获取成功";
                        r.Code = 0;
                        foreach (var item in deviceGroup)
                        {
                            RetDeviceGroupInfo groupInfo = new RetDeviceGroupInfo();
                            groupInfo.ID = item.ID.ToString();
                            groupInfo.Name = item.Name;
                            groupInfo.Label = item.Label;
                            groupInfo.ParentGroupID = item.ParentGroupID.ToString();
                            groupInfo.CreateUserID = item.CreateUserID.ToString();
                            groupInfo.CreateTime = item.CreateTime;
                            groupInfo.Description = item.Description;
                            groupInfo.UpdateTime = item.UpdateTime;
                            groupInfo.UpdateUserID = item.UpdateUserID.ToString();
                            groupInfo.OrgID = item.OrgID.ToString();
                            groupInfo.Image = item.Image.ToString();
                            result.Add(groupInfo);
                        }
                        r.Data = result;
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
        /// 获得设备的级联分组信息,组织ID和父组织ID获取，不可组合,parameter为空则返回全部
        /// </summary>
        /// <param name="parameter"></param>
        /// <returns></returns>
        public ReturnItem<List<RetGroupCascader>> GetDeviceGroupCasInfo(DeviceGroupModel parameter)
        {
            ReturnItem<List<RetGroupCascader>> r = new ReturnItem<List<RetGroupCascader>>();
            if (CustomConfigParam.IsUseRedis)
            {
                RedisUtils redisUtils = new RedisUtils();
                if (redisUtils.isCurMethodCached(System.Reflection.MethodBase.GetCurrentMethod(), parameter.OrgID))
                {
                    r.Msg = "设备分组信息获取成功";
                    r.Code = 0;
                    r.Data = redisUtils.getCacheContent<List<RetGroupCascader>>(System.Reflection.MethodBase.GetCurrentMethod(), parameter.OrgID);
                    r.Count = r.Data.Count();
                    return r;
                }
            }
            List<DeviceGroupModel> groupList = new List<DeviceGroupModel>();
            List<RetGroupCascader> endList = new List<RetGroupCascader>();
            using (MonitoringEntities device = new MonitoringEntities())
            {
                try
                {
                    IQueryable<D_DeviceGroup> deviceGroup = device.D_DeviceGroup.AsQueryable<D_DeviceGroup>();
                    if (parameter != null)
                    {
                        if (parameter.ID != null && parameter.ID != "")
                        {
                            var GroupID = Convert.ToInt32(parameter.ID);
                            deviceGroup = deviceGroup.Where(s => s.ID == GroupID);
                        }
                        else if (parameter.ParentGroupID != null && parameter.ParentGroupID != "")
                        {
                            var ParentGroupID = Convert.ToInt32(parameter.ParentGroupID);
                            deviceGroup = deviceGroup.Where(s => s.ParentGroupID == ParentGroupID);
                        }
                        if (parameter.OrgID != null && parameter.OrgID != "")
                        {
                            var OrgID = Convert.ToInt32(parameter.OrgID);
                            deviceGroup = deviceGroup.Where(s => s.OrgID == OrgID);
                        }
                    }
                    deviceGroup = deviceGroup.OrderBy(s => s.CreateTime);
                    if (deviceGroup == null)
                    {
                        r.Data = null;
                        r.Code = -1;
                        r.Msg = "未找到设备分组信息";
                        return r;
                    }
                    else if (deviceGroup != null)
                    {
                        r.Count = deviceGroup.Count();
                        r.Msg = "设备分组信息获取成功";
                        r.Code = 0;
                        foreach (var item in deviceGroup)
                        {
                            if (item.ParentGroupID == null)
                            {
                                DeviceGroupModel parentItem = new DeviceGroupModel();
                                parentItem.ID = item.ID.ToString();
                                parentItem.Name = item.Name;
                                parentItem.Label = item.Label;
                                parentItem.ParentGroupID = item.ParentGroupID.ToString();
                                parentItem.CreateUserID = item.CreateUserID.ToString();
                                parentItem.CreateTime = item.CreateTime;
                                parentItem.Description = item.Description;
                                parentItem.UpdateTime = item.UpdateTime;
                                parentItem.UpdateUserID = item.UpdateUserID.ToString();
                                parentItem.OrgID = item.OrgID.ToString();
                                groupList.Add(parentItem);
                            }
                        }
                        endList = CreateCasTreeInfo(groupList);
                        r.Data = endList;
                    }
                    if (CustomConfigParam.IsUseRedis)
                    {
                        RedisUtils redisUtil = new RedisUtils();
                        redisUtil.saveToRedis(System.Reflection.MethodBase.GetCurrentMethod(), r.Data, parameter.OrgID);
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

        ///<summary>
        ///----------------------------新加的--------------------------
        ///根据根和OrgID得到树形结构
        ///</summary>
        ///<param name="parameter"></param>
        ///<returns></returns>
        public ReturnItem<List<RetGroupCascader>> GetDeviceGroupInfoByRoot(DeviceGroupModel parameter)
        {
            ReturnItem<List<RetGroupCascader>> r = new ReturnItem<List<RetGroupCascader>>();
            List<DeviceGroupModel> groupList = new List<DeviceGroupModel>();
            List<RetGroupCascader> endList = new List<RetGroupCascader>();

            using (MonitoringEntities device = new MonitoringEntities())
            {
                try
                {
                    IQueryable<D_DeviceGroup> deviceGroup = device.D_DeviceGroup.AsQueryable<D_DeviceGroup>();
                    if (parameter != null)
                    {
                        if (parameter.OrgID != null && parameter.OrgID != "")
                        {
                            var OrgID = Convert.ToInt32(parameter.OrgID);
                            deviceGroup = deviceGroup.Where(s => s.OrgID == OrgID);
                        }
                        if (parameter.Name != null && parameter.Name != null)
                        {
                            deviceGroup = deviceGroup.Where(s => s.Name == parameter.Name); 
                        }
                        //代表是根结点
                        deviceGroup = deviceGroup.Where(s => s.ParentGroupID == null);
                    }
                    if (deviceGroup == null)
                    {
                        r.Data = null;
                        r.Code = -1;
                        r.Msg = "未找到设备分组信息";
                        return r;
                    }
                    else if (deviceGroup != null)
                    {
                        r.Count = deviceGroup.Count();
                        r.Msg = "设备分组信息获取成功";
                        r.Code = 0;
                        foreach (var item in deviceGroup)
                        {
                            if (item.ParentGroupID == null)
                            {
                                DeviceGroupModel parentItem = new DeviceGroupModel();
                                parentItem.ID = item.ID.ToString();
                                parentItem.Name = item.Name;
                                parentItem.Label = item.Label;
                                parentItem.ParentGroupID = item.ParentGroupID.ToString();
                                parentItem.CreateUserID = item.CreateUserID.ToString();
                                parentItem.CreateTime = item.CreateTime;
                                parentItem.Description = item.Description;
                                parentItem.UpdateTime = item.UpdateTime;
                                parentItem.UpdateUserID = item.UpdateUserID.ToString();
                                parentItem.OrgID = item.OrgID.ToString();
                                groupList.Add(parentItem);
                            }
                        }
                        endList = CreateCasTreeInfo(groupList);
                        r.Data = endList;
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


        ///<summary>
        ///----------------------------新加的--------------------------
        ///根据OrgID得到一级分组的集合
        ///</summary>
        ///<param name="parameter"></param>
        ///<returns></returns>
        public ReturnItem<List<DeviceGroupModel>> GetRootGroup(DeviceGroupModel parameter)
        {
            ReturnItem<List<DeviceGroupModel>> r = new ReturnItem<List<DeviceGroupModel>>();
            List<DeviceGroupModel> groupList = new List<DeviceGroupModel>();
            using (MonitoringEntities device = new MonitoringEntities())
            {
                try
                {
                    IQueryable<D_DeviceGroup> deviceGroup = device.D_DeviceGroup.AsQueryable<D_DeviceGroup>();
                    if (parameter != null)
                    {
                        deviceGroup.Where(s=>s.ParentGroupID == null);
                        if (parameter.OrgID != null && parameter.OrgID != "")
                        {
                            var OrgID = Convert.ToInt32(parameter.OrgID);
                            deviceGroup = deviceGroup.Where(s => s.OrgID == OrgID);
                        }
                    }
                    deviceGroup = deviceGroup.OrderBy(s => s.CreateTime);
                    if (deviceGroup == null)
                    {
                        r.Data = null;
                        r.Code = -1;
                        r.Msg = "未找到设备分组信息";
                        return r;
                    }
                    else if (deviceGroup != null)
                    {
                        r.Count = deviceGroup.Count();
                        r.Msg = "设备分组信息获取成功";
                        r.Code = 0;
                        foreach (var item in deviceGroup)
                        {
                            if (item.ParentGroupID == null)
                            {
                                DeviceGroupModel parentItem = new DeviceGroupModel();
                                parentItem.ID = item.ID.ToString();
                                parentItem.Name = item.Name;
                                parentItem.Label = item.Label;
                                groupList.Add(parentItem);
                            }
                        }
                        r.Data = groupList;
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
        /// 形成树结构
        /// </summary>
        /// <param name="parameter"></param>
        /// <returns></returns>
        public List<RetDeviceGroupInfo> CreateTreeInfo(List<DeviceGroupModel> parameter)
        {
            List<RetDeviceGroupInfo> r = new List<RetDeviceGroupInfo>();
            List<DeviceGroupModel> t = new List<DeviceGroupModel>();
            using (MonitoringEntities device = new MonitoringEntities())
            {
                try
                {
                    IQueryable<D_DeviceGroup> deviceGroup = device.D_DeviceGroup.AsQueryable<D_DeviceGroup>();
                    if (parameter.Count() == 0)
                    {
                        return r;
                    }
                    else if (parameter.Count() != 0)
                    {
                        foreach (var item in parameter)
                        {
                            var c = 0;
                            var groupParameter = new List<DeviceGroupModel>();
                            foreach (var i in deviceGroup)
                            {
                                if (i.ParentGroupID.ToString() == item.ID)
                                {
                                    c = 1;
                                    var groupofParameter = new DeviceGroupModel();

                                    groupofParameter.ID = i.ID.ToString();
                                    groupofParameter.Label = i.Label;
                                    groupofParameter.Name = i.Name;
                                    groupofParameter.ParentGroupID = i.ParentGroupID.ToString();
                                    groupofParameter.CreateTime = i.CreateTime;
                                    groupofParameter.CreateUserID = i.CreateUserID.ToString();
                                    groupofParameter.UpdateTime = i.UpdateTime;
                                    groupofParameter.UpdateUserID = i.UpdateUserID.ToString();
                                    groupofParameter.OrgID = i.OrgID.ToString();
                                    groupofParameter.Description = i.Description;

                                    groupParameter.Add(groupofParameter);
                                }
                            }
                            if (c == 1)
                            {
                                var deviceofGroup = new RetDeviceGroupInfo();

                                deviceofGroup.ID = item.ID;
                                deviceofGroup.Label = item.Label;
                                deviceofGroup.Name = item.Name;
                                deviceofGroup.ParentGroupID = item.ParentGroupID;
                                deviceofGroup.CreateTime = item.CreateTime;
                                deviceofGroup.CreateUserID = item.CreateUserID;
                                deviceofGroup.UpdateTime = item.UpdateTime;
                                deviceofGroup.UpdateUserID = item.UpdateUserID;
                                deviceofGroup.OrgID = item.OrgID;
                                deviceofGroup.Description = item.Description;

                                deviceofGroup.children = CreateTreeInfo(groupParameter);

                                r.Add(deviceofGroup);
                            }
                            else
                            {
                                var deviceofGroup = new RetDeviceGroupInfo();
                                deviceofGroup.ID = item.ID;
                                deviceofGroup.Label = item.Label;
                                deviceofGroup.Name = item.Name;
                                deviceofGroup.ParentGroupID = item.ParentGroupID;
                                deviceofGroup.CreateTime = item.CreateTime;
                                deviceofGroup.CreateUserID = item.CreateUserID;
                                deviceofGroup.UpdateTime = item.UpdateTime;
                                deviceofGroup.UpdateUserID = item.UpdateUserID;
                                deviceofGroup.OrgID = item.OrgID;
                                deviceofGroup.Description = item.Description;
                                deviceofGroup.children = new List<RetDeviceGroupInfo>();

                                r.Add(deviceofGroup);
                            }
                        }
                    }
                }
                catch (Exception e)
                {
                    log.ErrorFormat("内部错误：{0},{1}", e.Message, e.StackTrace);
                }
            }
            return r;
        }

        /// <summary>
        /// 形成级联所需的树结构
        /// </summary>
        /// <param name="parameter"></param>
        /// <returns></returns>
        public List<RetGroupCascader> CreateCasTreeInfo(List<DeviceGroupModel> parameter)
        {
            List<RetGroupCascader> r = new List<RetGroupCascader>();
            using (MonitoringEntities device = new MonitoringEntities())
            {
                try
                {
                    IQueryable<D_DeviceGroup> deviceGroup = device.D_DeviceGroup.AsQueryable<D_DeviceGroup>();
                    if (parameter.Count() == 0)
                    {
                        return r;
                    }
                    else if (parameter.Count() != 0)
                    {
                        foreach (var item in parameter)
                        {
                            var c = 0;
                            var groupParameter = new List<DeviceGroupModel>();
                            foreach (var i in deviceGroup)
                            {
                                if (i.ParentGroupID.ToString() == item.ID)
                                {
                                    c = 1;
                                    var groupofParameter = new DeviceGroupModel();

                                    groupofParameter.ID = i.ID.ToString();
                                    groupofParameter.Label = i.Label;
                                    groupofParameter.Name = i.Name;
                                    groupofParameter.ParentGroupID = i.ParentGroupID.ToString();
                                    groupofParameter.CreateTime = i.CreateTime;
                                    groupofParameter.CreateUserID = i.CreateUserID.ToString();
                                    groupofParameter.UpdateTime = i.UpdateTime;
                                    groupofParameter.UpdateUserID = i.UpdateUserID.ToString();
                                    groupofParameter.OrgID = i.OrgID.ToString();
                                    groupofParameter.Description = i.Description;

                                    groupParameter.Add(groupofParameter);
                                }
                            }
                            if (c == 1)
                            {
                                var deviceofGroup = new RetGroupCascader();

                                deviceofGroup.value = item.ID.ToString();
                                deviceofGroup.label = item.Name;
                                deviceofGroup.children = CreateCasTreeInfo(groupParameter);

                                r.Add(deviceofGroup);
                            }
                            else
                            {
                                var deviceofGroup = new RetGroupCascader();

                                deviceofGroup.value = item.ID.ToString();
                                deviceofGroup.label = item.Name;
                                deviceofGroup.children = null;

                                r.Add(deviceofGroup);
                            }
                        }
                    }
                }
                catch (Exception e)
                {
                    log.ErrorFormat("内部错误：{0},{1}", e.Message, e.StackTrace);
                }
            }
            return r;
        }

        /// <summary>
        /// 新增设备分组到设备分组数据表
        /// </summary>
        /// <param name="parameter"></param>
        /// <returns></returns>
        /// ---------------改动-------------使得分组重名校验只是发生在同一个父目录下
        public ReturnItem<RetDeviceGroupInfo> AddDeviceGroupInfo(DeviceGroupModel parameter)
        {
            ReturnItem<RetDeviceGroupInfo> r = new ReturnItem<RetDeviceGroupInfo>();
            using (MonitoringEntities device = new MonitoringEntities())
            {
                try
                {
                    var PID = Convert.ToInt32(parameter.OrgID);
                    List<D_DeviceGroup> deviceExist = null;
                    if (parameter.ParentGroupID != null && parameter.ParentGroupID != "")
                    {
                        int ParentID = Convert.ToInt32(parameter.ParentGroupID);
                        deviceExist = device.D_DeviceGroup.Where(x => x.Name == parameter.Name && x.OrgID == PID && x.ParentGroupID == ParentID).ToList();
                    }
                    else
                    {
                        deviceExist = device.D_DeviceGroup.Where(x => x.Name == parameter.Name && x.OrgID == PID && x.ParentGroupID==null).ToList();
                    }
                    if (deviceExist.Count() > 0)
                    {
                        r.Data = null;
                        r.Code = -1;
                        r.Msg = "设备分组名称已经存在，请修改名称";
                        return r;
                    }
                    else
                    {
                        D_DeviceGroup newdevice = new D_DeviceGroup()
                        {
                            Label = parameter.Label == null ? "" : parameter.Label,
                            Name = parameter.Name == null ? "" : parameter.Name,
                            Description = parameter.Description == null ? "" : parameter.Description,
                            CreateTime = DateTime.Now,
                            CreateUserID = Convert.ToInt32(parameter.CreateUserID),
                            OrgID = Convert.ToInt32(parameter.OrgID),
                            UpdateTime = null,
                            UpdateUserID = null
                        };
                        if (parameter.ParentGroupID != null && parameter.ParentGroupID != "")
                        {
                            newdevice.ParentGroupID = Convert.ToInt32(parameter.ParentGroupID);
                        }
                        device.D_DeviceGroup.Add(newdevice);
                        device.SaveChanges();
                        r.Msg = "设备分组新增成功";
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
            // 删除redis缓存
            if (CustomConfigParam.IsUseRedis)
            {
                removeDeviceGroupRedisCache(parameter.OrgID);
            }
            return r;
        }
        /// --新加的
        /// <summary>
        /// 添加图片信息
        /// </summary>
        /// -------------新加的
        public ReturnItem<RetDeviceGroupInfo> AddDeviceGroupImage(DeviceGroupModel parameter)
        {
            ReturnItem<RetDeviceGroupInfo> r = new ReturnItem<RetDeviceGroupInfo>();
            using (MonitoringEntities device = new MonitoringEntities())
            {
                try
                {
                    var id = Convert.ToInt32(parameter.ID);
                    var groupinfo = device.D_DeviceGroup.Where(s => s.ID == id).FirstOrDefault();
                    if (groupinfo == null)
                    {
                        r.Data = null;
                        r.Code = -1;
                        r.Msg = "未找到该设备分组";
                    }
                    else
                    {
                         groupinfo.Image = parameter.Image;
                         device.SaveChanges();
                         r.Msg = "设备分组图片添加成功";
                         r.Code = 0;
                    }
                }
                catch (Exception e) {
                    r.Msg = "内部错误请重试";
                    log.ErrorFormat("内部错误：{0},{1}", e.Message, e.StackTrace);
                    r.Code = -1;
                }
            }
            return r;
        }

        /// <summary>
        /// 更新设备分组数据表
        /// </summary>
        /// <param name="parameter"></param>
        /// <returns></returns>
        public ReturnItem<RetDeviceGroupInfo> UpdateDeviceGroupInfo(DeviceGroupModel parameter)
        {
            ReturnItem<RetDeviceGroupInfo> r = new ReturnItem<RetDeviceGroupInfo>();
            using (MonitoringEntities device = new MonitoringEntities())
            {
                try
                {
                    var id = Convert.ToInt32(parameter.ID);
                    var groupinfo = device.D_DeviceGroup.Where(s => s.ID == id).FirstOrDefault();
                    if (groupinfo == null)
                    {
                        r.Data = null;
                        r.Code = -1;
                        r.Msg = "未找到该设备分组";
                    }
                    else
                    {
                        var PID = Convert.ToInt32(parameter.OrgID);
                        var nameExist = device.D_DeviceGroup.Where(s => s.Name == parameter.Name && s.OrgID == PID).ToList();
                        if (nameExist.Count() > 0 && parameter.Name != groupinfo.Name)
                        {
                            r.Data = null;
                            r.Code = -1;
                            r.Msg = "设备分组名称已经存在，请修改名称";
                            return r;
                        }
                        else
                        {
                            groupinfo.Label = parameter.Label == null ? "" : parameter.Label;
                            groupinfo.Name = parameter.Name == null ? "" : parameter.Name;
                            if (parameter.ParentGroupID != null && parameter.ParentGroupID != "")
                            {
                                groupinfo.ParentGroupID = Convert.ToInt32(parameter.ParentGroupID);
                            }
                            groupinfo.Description = parameter.Description == null ? "" : parameter.Description;
                            groupinfo.OrgID = Convert.ToInt32(parameter.OrgID);
                            groupinfo.UpdateTime = DateTime.Now;
                            groupinfo.UpdateUserID = Convert.ToInt32(parameter.UpdateUserID);
                            device.SaveChanges();
                            r.Msg = "设备分组更新成功";
                            r.Code = 0;
                        }                 
                    }
                }
                catch (Exception e)
                {
                    r.Msg = "内部错误请重试";
                    log.ErrorFormat("内部错误：{0},{1}", e.Message, e.StackTrace);
                    r.Code = -1;
                }
            }
            // 删除redis缓存
            if (CustomConfigParam.IsUseRedis)
            {
                removeDeviceGroupRedisCache(parameter.OrgID);
            }
            return r;
        }

        /// <summary>
        /// 删除已经缓存的设备分组数据
        /// </summary>
        /// <param name="orgID"></param>
        private void removeDeviceGroupRedisCache(string orgID) {
            string groupCasInfoKey = "DeviceMonitoringBLL.DeviceInfoBLL.GetDeviceGroupCasInfo:OrgID." + orgID;
            string groupInfoKey = "DeviceMonitoringBLL.DeviceInfoBLL.GetDeviceGroupInfo:OrgID." + orgID;
            try
            {
                RedisClient redisClient = new RedisClient(CustomConfigParam.RedisDbNumber);
                redisClient.KeyDelete(groupCasInfoKey);
                log.InfoFormat("[Redis]Delete key :{0}", groupCasInfoKey);
                redisClient.KeyDelete(groupInfoKey);
                log.InfoFormat("[Redis]Delete key :{0}", groupInfoKey);
            }
            catch (Exception e)
            {
                log.ErrorFormat("内部错误,Delete key failed key {0},{1}：{2},{3}", groupCasInfoKey, groupInfoKey, e.Message, e.StackTrace);
            }

        }

        /// <summary>
        /// 删除设备分组数据表的分组
        /// </summary>
        /// <param name="parameter"></param>
        /// <returns></returns>
        public ReturnItem<RetDeviceGroupInfo> DeleteDeviceGroupInfo(DeviceGroupModel parameter)
        {
            ReturnItem<RetDeviceGroupInfo> r = new ReturnItem<RetDeviceGroupInfo>();
            using (MonitoringEntities device = new MonitoringEntities())
            {
                try
                {
                    var groudID = Convert.ToInt32(parameter.ID);
                    var groupinfo = device.D_DeviceGroup.Where(s => s.ID == groudID).FirstOrDefault();
                    if (groupinfo == null)
                    {
                        r.Data = null;
                        r.Code = -1;
                        r.Msg = "未找到该设备分组";
                        // 删除redis缓存
                        if (CustomConfigParam.IsUseRedis)
                        {
                            removeDeviceGroupRedisCache(parameter.OrgID);
                        }
                        return r;
                    }
                    else
                    {
                        var id = Convert.ToInt32(parameter.ID);
                        var info = device.D_DeviceGroup.Where(x => x.ParentGroupID == id).ToList();
                        if (info.Count() > 0)
                        {
                            r.Data = null;
                            r.Code = -1;
                            r.Msg = "该节点下仍有子节点！";
                            return r;
                        }
                        else
                        {
                            var deldeviceinfo = device.D_Devices.Where(x => x.GroupID == id).ToList();
                            if (deldeviceinfo.Count() > 0)
                            {
                                r.Data = null;
                                r.Code = -1;
                                r.Msg = "该节点下仍有设备！";
                                return r;
                            }
                            else
                            {
                                var entry = device.Entry(groupinfo);
                                //设置该对象的状态为删除  
                                entry.State = EntityState.Deleted;
                                //保存修改
                                device.SaveChanges();
                                
                                r.Msg = "设备分组删除成功！";
                                r.Code = 0;
                                // 删除redis缓存
                                if (CustomConfigParam.IsUseRedis)
                                {
                                    removeDeviceGroupRedisCache(parameter.OrgID);
                                }
                                return r;
                            }
                        }
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
        /// 查询设备标识符是否存在
        /// </summary>
        public ReturnItem<RetDeviceInfo> CheckDeviceInfoByDeviceLabel(DeviceInfoModel parameter)
        {
            ReturnItem<RetDeviceInfo> r = new ReturnItem<RetDeviceInfo>();
            using (MonitoringEntities device = new MonitoringEntities())
            {
                try
                {
                    int PID = int.Parse(parameter.OrgID);
                    var getinfo = device.D_Devices.Where(s => s.DeviceLabel == parameter.DeviceLabel && s.OrgID == PID).FirstOrDefault();
                    if (getinfo == null)
                    {
                        r.Data = null;
                        r.Code = 1;
                        r.Msg = "未找到设备标识符";
                        return r;
                    }
                    if (getinfo != null)
                    {
                        r.Msg = "已存在设备标识符";
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
        /// 查询设备属性标识符是否存在
        /// </summary>
        public ReturnItem<RetDeviceItemInfo> CheckDeviceTtemByPropertyLabel(DeviceItemInfoModel parameter)
        {
            ReturnItem<RetDeviceItemInfo> r = new ReturnItem<RetDeviceItemInfo>();
            using (MonitoringEntities device = new MonitoringEntities())
            {
                try
                {
                    int PID = int.Parse(parameter.OrgID);
                    var getinfo = device.D_DevicesItem.Where(s => s.PropertyLabel == parameter.PropertyLabel && s.OrgID == PID).FirstOrDefault();
                    if (getinfo == null)
                    {
                        r.Data = null;
                        r.Code = 1;
                        r.Msg = "未找到标识符";
                        return r;
                    }
                    if (getinfo != null)
                    {
                        r.Msg = "已存在标识符";
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
        /// 获取设备列表
        /// </summary>
        /// <returns>成功返回设备列表,失败返回Null.</returns>
        public ReturnItem<List<RetDeviceInfo>> GetEquipmentList(DeviceInfoModel parameter, bool deviceItem = true)
        {
            ReturnItem<List<RetDeviceInfo>> r = new ReturnItem<List<RetDeviceInfo>>();
            List<RetDeviceInfo> listinfo = new List<RetDeviceInfo>();
            using (MonitoringEntities device = new MonitoringEntities())
            {
                try
                {
                    IQueryable<D_Devices> deviceList = device.D_Devices.AsQueryable<D_Devices>();
                    if (parameter.DeviceLabel != null && parameter.DeviceLabel != "")
                    {
                        deviceList = deviceList.Where(s => s.DeviceLabel.IndexOf(parameter.DeviceLabel) >= 0);
                    }
                    if (parameter.Name != null && parameter.Name != "")
                    {
                        deviceList = deviceList.Where(s => s.Name.IndexOf(parameter.Name) >= 0);
                    }
                    if (parameter.RunningState != null && parameter.RunningState != "")
                    {
                        int RunningState = Convert.ToInt32(parameter.RunningState);
                        deviceList = deviceList.Where(s => s.RunningState == RunningState);
                    }
                    if (parameter.GroupIDList != null && parameter.GroupIDList.Count > 0)
                    {
                        var newlist = parameter.GroupIDList.Select<string, long>(x => Convert.ToInt32(x)).ToList();
                        deviceList = deviceList.Where(s => newlist.Contains(s.GroupID));
                    }
                    if (parameter.Status != null && parameter.Status != "")
                    {
                        int status = Convert.ToInt32(parameter.Status);
                        deviceList = deviceList.Where(s => s.Status == status);
                    }
                    if (parameter.OrgID.ToString() != null && parameter.OrgID.ToString() != "")
                    {
                        var OrgID = Convert.ToInt32(parameter.OrgID);
                        deviceList = deviceList.Where(s => s.OrgID == OrgID);
                    }
                    deviceList = deviceList.OrderByDescending(s => s.CreateTime);
                    if (deviceList == null)
                    {
                        r.Data = null;
                        r.Code = -1;
                        r.Msg = "未找到设备";
                        return r;
                    }
                    if (deviceList != null)
                    {
                        r.Count = deviceList.Count();
                        if (parameter.PageIndex != 0 && parameter.PageSize != 0)
                        {
                            deviceList = deviceList.Skip((parameter.PageIndex - 1) * parameter.PageSize).Take(parameter.PageSize);
                        }
                        List<D_Devices> list = deviceList.ToList<D_Devices>();
                        r.Msg = "设备信息获取成功";
                        r.Code = 0;
                        foreach (var item in list)
                        {
                            List<RetDeviceItemInfo> itemlistinfo = new List<RetDeviceItemInfo>();
                            var deviceinfo = new RetDeviceInfo();
                            deviceinfo.ID = item.ID.ToString();
                            deviceinfo.DeviceType = item.DeviceType;
                            deviceinfo.Name = item.Name;
                            deviceinfo.DeviceLabel = item.DeviceLabel;
                            var deviceModel = device.D_DeviceModel.Where(x => x.ID == item.DeviceModelID).FirstOrDefault();
                            if (deviceModel != null)
                            {
                                deviceinfo.DeviceModel = deviceModel.Name;
                            }
                            deviceinfo.DeviceModelID = item.DeviceModelID.ToString();
                            deviceinfo.ConnectType = item.ConnectType.ToString();
                            if (item.IoTHubID != null) {
                                var IoTHub = device.D_IoTHubConfiguration.Where(x => x.ID == item.IoTHubID).FirstOrDefault();
                                if (IoTHub != null)
                                {
                                    deviceinfo.IoTHub = IoTHub.Name;
                                }
                            }
                            deviceinfo.IoTHubID = item.IoTHubID.ToString();
                            var DataConnect = device.D_DataConnectConfiguration.Where(x => x.ID == item.DataConnectID).FirstOrDefault();
                            if (DataConnect != null)
                            {
                                deviceinfo.DataConnect = DataConnect.Name;
                            }
                            deviceinfo.DataConnectID = item.DataConnectID.ToString();
                            deviceinfo.Specification = item.Specification;
                            deviceinfo.GPS = item.GPS;
                            deviceinfo.Phone = item.Phone;
                            deviceinfo.Manufacturer = item.Manufacturer;
                            deviceinfo.RunningState = item.RunningState.ToString();
                            deviceinfo.Status = item.Status.ToString();
                            deviceinfo.Remark = item.Remark;
                            deviceinfo.CreateTime = item.CreateTime;
                            deviceinfo.CreateUserID = item.CreateUserID.ToString();
                            deviceinfo.UpdateTime = item.UpdateTime;
                            deviceinfo.UpdateUserID = item.UpdateUserID.ToString();
                            deviceinfo.OrgID = item.OrgID.ToString();
                            deviceinfo.GroupID = item.GroupID.ToString();
                            // 获取TagList
                            List<RetDeviceTag> taglist = new List<RetDeviceTag>();
                            if (item.TagMap != "" && item.TagMap != null)
                            {
                                taglist = JsonConvert.DeserializeObject<List<RetDeviceTag>>(item.TagMap);
                            }
                            deviceinfo.TagList = taglist;
                            if (deviceItem == true) {
                                // 获取设备属性信息
                                var getdeviceitem = device.D_DevicesItem.Where(s => s.DeviceID == item.ID).ToList();
                                if (getdeviceitem != null)
                                {
                                    List<D_DevicesItem> devicelist = getdeviceitem.ToList<D_DevicesItem>();
                                    foreach (var deviceitem in devicelist)
                                    {
                                        List<string> unit = new List<string>();
                                        if (deviceitem.Unit != "" && deviceitem.Unit != null)
                                        {
                                            unit = JsonConvert.DeserializeObject<List<string>>(deviceitem.Unit);
                                        }
                                        var deviceiteminfo = new RetDeviceItemInfo();
                                        deviceiteminfo.ID = deviceitem.ID.ToString();
                                        deviceiteminfo.DeviceID = deviceitem.DeviceID.ToString();
                                        deviceiteminfo.Name = deviceitem.Name;
                                        deviceiteminfo.PropertyLabel = deviceitem.PropertyLabel;
                                        deviceiteminfo.ValueType = deviceitem.ValueType.ToString();
                                        deviceiteminfo.Unit = unit;
                                        deviceiteminfo.Value = deviceitem.Value;
                                        deviceiteminfo.Description = deviceitem.Description;
                                        deviceiteminfo.OrgID = deviceitem.OrgID.ToString();
                                        itemlistinfo.Add(deviceiteminfo);
                                    }
                                    deviceinfo.DeviceItems = itemlistinfo;
                                }
                            }
                            listinfo.Add(deviceinfo);
                        }
                        r.Data = listinfo;
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
        /// 获取设备列表,只有汉王的设备
        /// </summary>
        /// <returns>成功返回设备列表,失败返回Null.</returns>
        public ReturnItem<List<RetDeviceInfo>> GetEquipmentListHanwang(DeviceInfoModel parameter, bool deviceItem = true)
        {
            ReturnItem<List<RetDeviceInfo>> r = new ReturnItem<List<RetDeviceInfo>>();
            List<RetDeviceInfo> listinfo = new List<RetDeviceInfo>();
            using (MonitoringEntities device = new MonitoringEntities())
            {
                try
                {
                    var deviceList = device.D_Devices.Where(s => s.DeviceModelID == 37);//找出模板为汉王的设备
                    deviceList = deviceList.OrderByDescending(s => s.CreateTime);
                    if (deviceList == null)
                    {
                        r.Data = null;
                        r.Code = -1;
                        r.Msg = "未找到设备";
                        return r;
                    }
                    if (deviceList != null)
                    {
                        r.Count = deviceList.Count();
                        if (parameter.PageIndex != 0 && parameter.PageSize != 0)
                        {
                            deviceList = deviceList.Skip((parameter.PageIndex - 1) * parameter.PageSize).Take(parameter.PageSize);
                        }
                        List<D_Devices> list = deviceList.ToList<D_Devices>();
                        r.Msg = "设备信息获取成功";
                        r.Code = 0;
                        foreach (var item in list)
                        {
                            List<RetDeviceItemInfo> itemlistinfo = new List<RetDeviceItemInfo>();
                            var deviceinfo = new RetDeviceInfo();
                            deviceinfo.ID = item.ID.ToString();
                            deviceinfo.DeviceType = item.DeviceType;
                            deviceinfo.Name = item.Name;
                            deviceinfo.DeviceLabel = item.DeviceLabel;
                            var deviceModel = device.D_DeviceModel.Where(x => x.ID == item.DeviceModelID).FirstOrDefault();
                            if (deviceModel != null)
                            {
                                deviceinfo.DeviceModel = deviceModel.Name;
                            }
                            deviceinfo.DeviceModelID = item.DeviceModelID.ToString();
                            deviceinfo.ConnectType = item.ConnectType.ToString();
                            if (item.IoTHubID != null)
                            {
                                var IoTHub = device.D_IoTHubConfiguration.Where(x => x.ID == item.IoTHubID).FirstOrDefault();
                                if (IoTHub != null)
                                {
                                    deviceinfo.IoTHub = IoTHub.Name;
                                }
                            }
                            deviceinfo.IoTHubID = item.IoTHubID.ToString();
                            var DataConnect = device.D_DataConnectConfiguration.Where(x => x.ID == item.DataConnectID).FirstOrDefault();
                            if (DataConnect != null)
                            {
                                deviceinfo.DataConnect = DataConnect.Name;
                            }
                            deviceinfo.DataConnectID = item.DataConnectID.ToString();
                            deviceinfo.Specification = item.Specification;
                            deviceinfo.GPS = item.GPS;
                            deviceinfo.Phone = item.Phone;
                            deviceinfo.Manufacturer = item.Manufacturer;
                            deviceinfo.RunningState = item.RunningState.ToString();
                            deviceinfo.Status = item.Status.ToString();
                            deviceinfo.Remark = item.Remark;
                            deviceinfo.CreateTime = item.CreateTime;
                            deviceinfo.CreateUserID = item.CreateUserID.ToString();
                            deviceinfo.UpdateTime = item.UpdateTime;
                            deviceinfo.UpdateUserID = item.UpdateUserID.ToString();
                            deviceinfo.OrgID = item.OrgID.ToString();
                            deviceinfo.GroupID = item.GroupID.ToString();
                            // 获取TagList
                            List<RetDeviceTag> taglist = new List<RetDeviceTag>();
                            if (item.TagMap != "" && item.TagMap != null)
                            {
                                taglist = JsonConvert.DeserializeObject<List<RetDeviceTag>>(item.TagMap);
                            }
                            deviceinfo.TagList = taglist;
                            if (deviceItem == true)
                            {
                                // 获取设备属性信息
                                var getdeviceitem = device.D_DevicesItem.Where(s => s.DeviceID == item.ID).ToList();
                                if (getdeviceitem != null)
                                {
                                    List<D_DevicesItem> devicelist = getdeviceitem.ToList<D_DevicesItem>();
                                    foreach (var deviceitem in devicelist)
                                    {
                                        List<string> unit = new List<string>();
                                        if (deviceitem.Unit != "" && deviceitem.Unit != null)
                                        {
                                            unit = JsonConvert.DeserializeObject<List<string>>(deviceitem.Unit);
                                        }
                                        var deviceiteminfo = new RetDeviceItemInfo();
                                        deviceiteminfo.ID = deviceitem.ID.ToString();
                                        deviceiteminfo.DeviceID = deviceitem.DeviceID.ToString();
                                        deviceiteminfo.Name = deviceitem.Name;
                                        deviceiteminfo.PropertyLabel = deviceitem.PropertyLabel;
                                        deviceiteminfo.ValueType = deviceitem.ValueType.ToString();
                                        deviceiteminfo.Unit = unit;
                                        deviceiteminfo.Value = deviceitem.Value;
                                        deviceiteminfo.Description = deviceitem.Description;
                                        deviceiteminfo.OrgID = deviceitem.OrgID.ToString();
                                        itemlistinfo.Add(deviceiteminfo);
                                    }
                                    deviceinfo.DeviceItems = itemlistinfo;
                                }
                            }
                            listinfo.Add(deviceinfo);
                        }
                        r.Data = listinfo;
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
        /// 获取某设备属性列表
        /// </summary>
        /// <returns>成功返回设备属性列表,失败返回Null.</returns>
        public ReturnItem<List<RetDeviceItemInfo>> GetEquipmentItemsList(DeviceItemInfoModel parameter)
        {

            ReturnItem<List<RetDeviceItemInfo>> r = new ReturnItem<List<RetDeviceItemInfo>>();
            List<RetDeviceItemInfo> listinfo = new List<RetDeviceItemInfo>();
            using (MonitoringEntities device = new MonitoringEntities())
            {
                try
                {
                    string deviceName = null;
                    var deviceList = device.D_DevicesItem.Where(s => s.DeviceID == parameter.DeviceID).ToList();
                    var deviL = device.D_Devices.Where(a => a.ID == parameter.DeviceID).ToList();
                    if (deviL != null && deviL.Count() != 0)
                    {
                        deviceName = deviL[0].Name;
                    }
                    if (deviceList == null) 
                    {
                        r.Data = null;
                        r.Code = -1;
                        r.Msg = "未找到设备";
                        return r;
                    }
                    if (deviceList != null)
                    {
                        List<D_DevicesItem> devicelist = deviceList.ToList<D_DevicesItem>();
                        foreach (var deviceitem in devicelist)
                        {
                            List<string> unit = new List<string>();
                            if (deviceitem.Unit != "" && deviceitem.Unit != null)
                            {
                                unit = JsonConvert.DeserializeObject<List<string>>(deviceitem.Unit);
                            }
                            var deviceiteminfo = new RetDeviceItemInfo();
                            deviceiteminfo.ID = deviceitem.ID.ToString();
                            deviceiteminfo.DeviceID = deviceitem.DeviceID.ToString();
                            deviceiteminfo.Name = deviceitem.Name;
                            deviceiteminfo.DeviceName = deviceName;
                            deviceiteminfo.PropertyLabel = deviceitem.PropertyLabel;
                            deviceiteminfo.ValueType = deviceitem.ValueType.ToString();
                            deviceiteminfo.Unit = unit;
                            deviceiteminfo.Value = deviceitem.Value;
                            deviceiteminfo.Description = deviceitem.Description;
                            deviceiteminfo.OrgID = deviceitem.OrgID.ToString();
                            listinfo.Add(deviceiteminfo);
                        }
                        r.Data = listinfo;
                        r.Msg = "设备属性获取成功";
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
        /// 启用/禁用设备
        /// </summary>
        public ReturnItem<RetDeviceInfo> EnabledDevice(DeviceInfoModel parameter)
        {
            ReturnItem<RetDeviceInfo> r = new ReturnItem<RetDeviceInfo>();
            using (MonitoringEntities device = new MonitoringEntities())
            {
                try
                {
                    var deviceInfo = device.D_Devices.Where(s => s.ID == parameter.ID).FirstOrDefault();
                    if (deviceInfo == null)
                    {
                        r.Data = null;
                        r.Code = -1;
                        r.Msg = "未找到该设备";
                        return r;
                    }
                    if (deviceInfo != null)
                    {
                        if (parameter.Status != null && parameter.Status.ToString() != "")
                        {
                            deviceInfo.Status = Convert.ToInt32(parameter.Status);
                        }
                        deviceInfo.UpdateUserID = Convert.ToInt32(parameter.UpdateUserID);
                        deviceInfo.UpdateTime = DateTime.Now;
                        device.SaveChanges();

                        r.Msg = "设备信息更新成功";
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
        /// 删除设备信息
        /// </summary>
        public ReturnItem<RetDeviceInfo> DeleteDevice(DeviceInfoModel parameter)
        {
            ReturnItem<RetDeviceInfo> r = new ReturnItem<RetDeviceInfo>();
            using (MonitoringEntities device = new MonitoringEntities())
            {
                try
                {
                    var deldeviceitem = device.D_DevicesItem.Where(a => a.DeviceID == parameter.ID).ToList();
                    if (deldeviceitem != null)
                    {
                        List<D_DevicesItem> list = deldeviceitem.ToList<D_DevicesItem>();
                        foreach (var item in list)
                        {
                            var entry = device.Entry(item);
                            entry.State = EntityState.Deleted;
                        }
                        device.SaveChanges();
                    }
                    D_Devices deldevice = device.Set<D_Devices>().Where(a => a.ID == parameter.ID).FirstOrDefault();
                    if (deldevice != null)
                    {
                        var entry = device.Entry(deldevice);
                        //设置该对象的状态为删除  
                        entry.State = EntityState.Deleted;
                        device.SaveChanges();
                        //保存修改
                    }
                    r.Msg = "信息删除成功";
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
        /// 新增设备
        /// </summary>
        public ReturnItem<RetDeviceInfo> AddDevice(DeviceInfoModel parameter)
        {
            ReturnItem<RetDeviceInfo> r = new ReturnItem<RetDeviceInfo>();
            using (MonitoringEntities device = new MonitoringEntities())
            {
                try
                {
                    int PID = int.Parse(parameter.OrgID);
                    var adddevice = device.D_Devices.Where(s => s.DeviceLabel == parameter.DeviceLabel && s.OrgID == PID).FirstOrDefault();
                    if (adddevice != null)
                    {
                        r.Data = null;
                        r.Code = -1;
                        r.Msg = "设备信息已存在";
                        return r;
                    }
                    if (adddevice == null)
                    {
                        string TagMap = "";
                        if (parameter.TagList.Count > 0) {
                            TagMap = JsonConvert.SerializeObject(parameter.TagList);
                        }
                        D_Devices newdevice = new D_Devices()
                        {
                            DeviceType = parameter.DeviceType,
                            Name = parameter.Name,
                            DeviceLabel = parameter.DeviceLabel,
                            DeviceModelID = Convert.ToInt32(parameter.DeviceModelID),
                            ConnectType = Convert.ToInt32(parameter.ConnectType),
                            DataConnectID = Convert.ToInt32(parameter.DataConnectID),
                            Specification = parameter.Specification,
                            GPS = parameter.GPS,
                            Phone = parameter.Phone,
                            Manufacturer = parameter.Manufacturer,
                            RunningState = Convert.ToInt32(parameter.RunningState),
                            Status = 1,
                            Remark = parameter.Remark,
                            CreateTime = DateTime.Now,
                            CreateUserID = parameter.UserID,
                            OrgID = Convert.ToInt32(parameter.OrgID),
                        };
                        if (parameter.IoTHubID != "" && parameter.IoTHubID != null)
                        {
                            newdevice.IoTHubID = Convert.ToInt32(parameter.IoTHubID);
                        }
                        if (parameter.GroupID != "" && parameter.GroupID != null)
                        {
                            newdevice.GroupID = Convert.ToInt32(parameter.GroupID);
                        }
                        else
                        {
                            newdevice.GroupID = 0;
                        }
                        if (TagMap != "") {
                            newdevice.TagMap = TagMap;
                        }
                        device.D_Devices.Add(newdevice);
                        device.SaveChanges();

                        var getdevice = device.D_Devices.Where(s => s.DeviceLabel == parameter.DeviceLabel && s.OrgID == PID).FirstOrDefault();
                        if (getdevice != null)
                        {
                            parameter.ID = getdevice.ID;
                        }

                        List<DeviceItemInfoModel> list = parameter.DeviceItems;
                        foreach (var item in list)
                        {
                            string unit = JsonConvert.SerializeObject(item.Unit);
                            D_DevicesItem deviceitem = new D_DevicesItem()
                            {
                                DeviceID = parameter.ID,
                                Name = item.Name,
                                PropertyLabel = item.PropertyLabel,
                                ValueType = item.ValueType,
                                Unit = unit,
                                Value = item.Value,
                                Description = item.Description,
                                OrgID = Convert.ToInt32(parameter.OrgID),
                            };
                            device.D_DevicesItem.Add(deviceitem);
                            device.SaveChanges();
                        }

                        r.Msg = "设备信息新增成功";
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
        /// 获取设备
        /// </summary>
        public ReturnItem<RetDeviceInfo> GetDevice(DeviceInfoModel parameter)
        {
            ReturnItem<RetDeviceInfo> r = new ReturnItem<RetDeviceInfo>();
            List<RetDeviceItemInfo> listinfo = new List<RetDeviceItemInfo>();
            using (MonitoringEntities device = new MonitoringEntities())
            {
                try
                {
                    long orgID = string.IsNullOrEmpty(parameter.OrgID) ?0:long.Parse(parameter.OrgID);
                    var getdevice = device.D_Devices.Where(s => 
                    (0 == parameter.ID?true:s.ID == parameter.ID)&&
                    (string.IsNullOrEmpty(parameter.DeviceLabel)?true:s.DeviceLabel==parameter.DeviceLabel)&&
                    (orgID==0 ? true : s.OrgID == orgID) &&
                    (string.IsNullOrEmpty(parameter.Phone) ? true : s.Phone == parameter.Phone)
                    ).FirstOrDefault();
                    IQueryable<D_DeviceGroup> deviceGroup = device.D_DeviceGroup.AsQueryable<D_DeviceGroup>();
                    if (getdevice == null)
                    {
                        r.Data = null;
                        r.Code = -1;
                        r.Msg = "未找到该设备";
                        return r;
                    }
                    List<string> GroupIDList = new List<string>();
                    string parentGroupID = null;
                    if (getdevice.GroupID != 0)
                    {
                       parentGroupID = getdevice.GroupID.ToString();
                    }
                    GroupIDList.Add(parentGroupID);
                    while (parentGroupID != null && parentGroupID != "")
                    {
                        foreach (var item in deviceGroup)
                        {
                            if (parentGroupID == item.ID.ToString())
                            {
                                parentGroupID = item.ParentGroupID.ToString();
                                if (parentGroupID != "")
                                {
                                    GroupIDList.Add(parentGroupID);
                                }
                                break;
                            }
                        }
                    }
                    GroupIDList.Reverse();
                    if (getdevice != null)
                    {
                        var devicemodel = "";
                        var deviceModel = device.D_DeviceModel.Where(x => x.ID == getdevice.DeviceModelID).FirstOrDefault();
                        if (deviceModel != null)
                        {
                            devicemodel = deviceModel.Name;
                        }
                        var iothub = "";
                        if (getdevice.IoTHubID != null) {
                            var iotHub = device.D_IoTHubConfiguration.Where(x => x.ID == getdevice.IoTHubID).FirstOrDefault();
                            if (iotHub != null)
                            {
                                iothub = iotHub.Name;
                            }
                        }
                        var dataconnect = "";
                        var dataConnect = device.D_DataConnectConfiguration.Where(x => x.ID == getdevice.DataConnectID).FirstOrDefault();
                        if (dataConnect != null)
                        {
                            dataconnect = dataConnect.Name;
                        }
                        r.Msg = "设备信息获取成功";
                        r.Code = 0;
                        r.Data = new RetDeviceInfo()
                        {
                            ID = getdevice.ID.ToString(),
                            DeviceType = getdevice.DeviceType,
                            Name = getdevice.Name,
                            DeviceLabel = getdevice.DeviceLabel,
                            DeviceModel = devicemodel,
                            DeviceModelID = getdevice.DeviceModelID.ToString(),
                            ConnectType = getdevice.ConnectType.ToString(),
                            IoTHub = iothub,
                            IoTHubID = getdevice.IoTHubID.ToString(),
                            DataConnect = dataconnect,
                            DataConnectID = getdevice.DataConnectID.ToString(),
                            Specification = getdevice.Specification,
                            GPS = getdevice.GPS,
                            Phone = getdevice.Phone,
                            Manufacturer = getdevice.Manufacturer,
                            RunningState = getdevice.RunningState.ToString(),
                            Status = getdevice.Status.ToString(),
                            Remark = getdevice.Remark,
                            CreateTime = getdevice.CreateTime,
                            GroupIDList = GroupIDList,
                        };
                        // 获取TagList
                        List<RetDeviceTag> taglist = new List<RetDeviceTag>();
                        if (getdevice.TagMap != "" && getdevice.TagMap != null) {
                            taglist = JsonConvert.DeserializeObject<List<RetDeviceTag>>(getdevice.TagMap);
                        }
                        r.Data.TagList = taglist;
                        // 获取DeviceItems
                        var getdeviceitem = device.D_DevicesItem.Where(s => s.DeviceID == getdevice.ID).ToList();
                        if (getdeviceitem == null)
                        {
                            r.Data = null;
                            r.Code = -1;
                            r.Msg = "未找到设备属性";
                            return r;
                        }
                        if (getdeviceitem != null)
                        {
                            List<D_DevicesItem> list = getdeviceitem.ToList<D_DevicesItem>();
                            foreach (var item in list)
                            {
                                List<string> unit = new List<string>();
                                if (item.Unit != "" && item.Unit != null)
                                {
                                    unit = JsonConvert.DeserializeObject<List<string>>(item.Unit);
                                }
                                var deviceiteminfo = new RetDeviceItemInfo();
                                deviceiteminfo.ID = item.ID.ToString();
                                deviceiteminfo.DeviceID = item.DeviceID.ToString();
                                deviceiteminfo.Name = item.Name;
                                deviceiteminfo.PropertyLabel = item.PropertyLabel;
                                deviceiteminfo.ValueType = item.ValueType.ToString();
                                deviceiteminfo.Unit = unit;
                                deviceiteminfo.Value = item.Value;
                                deviceiteminfo.Description = item.Description;
                                deviceiteminfo.OrgID = item.OrgID.ToString();
                                listinfo.Add(deviceiteminfo);
                            }
                            r.Data.DeviceItems = listinfo;
                        }
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
        /// 更新设备
        /// </summary>
        public ReturnItem<RetDeviceInfo> UpdateDevice(DeviceInfoModel parameter)
        {
            ReturnItem<RetDeviceInfo> r = new ReturnItem<RetDeviceInfo>();
            using (MonitoringEntities device = new MonitoringEntities())
            {
                try
                {
                    var getdevice = device.D_Devices.Where(s => s.ID == parameter.ID).FirstOrDefault();
                    if (getdevice == null)
                    {
                        r.Data = null;
                        r.Code = -1;
                        r.Msg = "未找到该设备";
                        return r;
                    }
                    if (getdevice != null)
                    {
                        string TagMap = "";
                        if (parameter.TagList.Count > 0)
                        {
                            TagMap = JsonConvert.SerializeObject(parameter.TagList);
                        }
                        getdevice.DeviceType = parameter.DeviceType;
                        getdevice.Name = parameter.Name;
                        getdevice.DeviceLabel = parameter.DeviceLabel;
                        getdevice.DeviceModelID = Convert.ToInt32(parameter.DeviceModelID);
                        getdevice.ConnectType = Convert.ToInt32(parameter.ConnectType);
                        if (parameter.IoTHubID != "" && parameter.IoTHubID != null)
                        {
                            getdevice.IoTHubID = Convert.ToInt32(parameter.IoTHubID);
                        }
                        else if (parameter.IoTHubID == "")
                        {
                            getdevice.IoTHubID = null;
                        }
                        getdevice.DataConnectID = Convert.ToInt32(parameter.DataConnectID);
                        getdevice.TagMap = TagMap;
                        getdevice.Specification = parameter.Specification;
                        getdevice.GPS = parameter.GPS;
                        getdevice.Phone = parameter.Phone;
                        getdevice.Manufacturer = parameter.Manufacturer;
                        getdevice.RunningState = Convert.ToInt32(parameter.RunningState);
                        getdevice.Status = Convert.ToInt32(parameter.Status);
                        getdevice.Remark = parameter.Remark;
                        getdevice.OrgID = Convert.ToInt32(parameter.OrgID);
                        getdevice.UpdateTime = DateTime.Now;
                        if (parameter.GroupID != "" && parameter.GroupID != null)
                        {
                            getdevice.GroupID = Convert.ToInt32(parameter.GroupID);
                        }
                        else
                        {
                            getdevice.GroupID = 0;
                        }
                        getdevice.UpdateUserID = Convert.ToInt32(parameter.UpdateUserID);
                        device.SaveChanges();
                        List<DeviceItemInfoModel> list = parameter.DeviceItems;
                        foreach (var item in list)
                        {
                            if (item.State == "0")
                            {
                                var deviceitem = device.D_DevicesItem.Where(a => a.ID == item.ID).FirstOrDefault();
                                if (deviceitem != null)
                                {
                                    string unit = JsonConvert.SerializeObject(item.Unit);
                                    deviceitem.Name = item.Name;
                                    deviceitem.PropertyLabel = item.PropertyLabel;
                                    deviceitem.ValueType = item.ValueType;
                                    deviceitem.Unit = unit;
                                    deviceitem.Description = item.Description;
                                    deviceitem.OrgID = Convert.ToInt32(parameter.OrgID);
                                    device.SaveChanges();
                                }
                            }
                            if (item.State == "1")
                            {
                                string unit = JsonConvert.SerializeObject(item.Unit);
                                D_DevicesItem deviceitem = new D_DevicesItem()
                                {
                                    DeviceID = parameter.ID,
                                    Name = item.Name,
                                    PropertyLabel = item.PropertyLabel,
                                    ValueType = item.ValueType,
                                    Unit = unit,
                                    Value = item.Value,
                                    Description = item.Description,
                                    OrgID = Convert.ToInt32(parameter.OrgID),
                                };
                                device.D_DevicesItem.Add(deviceitem);
                                device.SaveChanges();
                            }
                            if (item.State == "-1")
                            {
                                var deviceitem = device.D_DevicesItem.Where(a => a.ID == item.ID).FirstOrDefault();
                                if (deviceitem != null)
                                {
                                    var entry = device.Entry(deviceitem);
                                    entry.State = EntityState.Deleted;
                                    device.SaveChanges();
                                }
                            }
                        }

                        r.Msg = "设备信息更新成功";
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
        /// 内部获取设备列表
        /// </summary>
        /// <returns>成功返回设备列表,失败返回Null.</returns>
        public ReturnItem<List<RetDeviceInfo>> GetDeviceListInside(GetDeviceInfoParameter parameter)
        {
            ReturnItem<List<RetDeviceInfo>> r = new ReturnItem<List<RetDeviceInfo>>();
            List<RetDeviceInfo> listinfo = new List<RetDeviceInfo>();
            using (MonitoringEntities device = new MonitoringEntities())
            {
                try
                {
                    IQueryable<D_Devices> deviceList = device.D_Devices.AsQueryable<D_Devices>();
                    if (parameter.OrgID != null && parameter.OrgID != "")
                    {
                        var OrgID = Convert.ToInt32(parameter.OrgID);
                        deviceList = deviceList.Where(s => s.OrgID == OrgID);
                    }
                    if (parameter.Status != null && parameter.Status != "")
                    {
                        int status = Convert.ToInt32(parameter.Status);
                        deviceList = deviceList.Where(s => s.Status == status);
                    }
                    if (parameter.Phone != null && parameter.Phone != "")
                    {
                        deviceList = deviceList.Where(s => s.Phone == parameter.Phone);
                    }
                    deviceList = deviceList.OrderByDescending(s => s.CreateTime);
                    if (deviceList == null)
                    {
                        r.Data = null;
                        r.Code = -1;
                        r.Msg = "未找到设备";
                        return r;
                    }
                    if (deviceList != null)
                    {
                        List<D_Devices> list = deviceList.ToList<D_Devices>();
                        r.Count = deviceList.Count();
                        r.Msg = "设备信息获取成功";
                        r.Code = 0;
                        foreach (var item in list)
                        {
                            List<RetDeviceItemInfo> itemlistinfo = new List<RetDeviceItemInfo>();
                            var deviceinfo = new RetDeviceInfo();
                            deviceinfo.ID = item.ID.ToString();
                            deviceinfo.DeviceType = item.DeviceType;
                            deviceinfo.Name = item.Name;
                            deviceinfo.DeviceLabel = item.DeviceLabel;
                            var deviceModel = device.D_DeviceModel.Where(x => x.ID == item.DeviceModelID).FirstOrDefault();
                            if (deviceModel != null)
                            {
                                deviceinfo.DeviceModel = deviceModel.Name;
                            }
                            deviceinfo.DeviceModelID = item.DeviceModelID.ToString();
                            deviceinfo.ConnectType = item.ConnectType.ToString();
                            if (item.IoTHubID != null)
                            {
                                var IoTHub = device.D_IoTHubConfiguration.Where(x => x.ID == item.IoTHubID).FirstOrDefault();
                                if (IoTHub != null)
                                {
                                    deviceinfo.IoTHub = IoTHub.Name;
                                }
                            }
                            deviceinfo.IoTHubID = item.IoTHubID.ToString();
                            var DataConnect = device.D_DataConnectConfiguration.Where(x => x.ID == item.DataConnectID).FirstOrDefault();
                            if (DataConnect != null)
                            {
                                deviceinfo.DataConnect = DataConnect.Name;
                            }
                            deviceinfo.DataConnectID = item.DataConnectID.ToString();
                            deviceinfo.Specification = item.Specification;
                            deviceinfo.GPS = item.GPS;
                            deviceinfo.Phone = item.Phone;
                            deviceinfo.Manufacturer = item.Manufacturer;
                            deviceinfo.RunningState = item.RunningState.ToString();
                            deviceinfo.Status = item.Status.ToString();
                            deviceinfo.Remark = item.Remark;
                            deviceinfo.CreateTime = item.CreateTime;
                            deviceinfo.CreateUserID = item.CreateUserID.ToString();
                            deviceinfo.UpdateTime = item.UpdateTime;
                            deviceinfo.UpdateUserID = item.UpdateUserID.ToString();
                            deviceinfo.OrgID = item.OrgID.ToString();
                            deviceinfo.GroupID = item.GroupID.ToString();
                            // 获取TagList
                            List<RetDeviceTag> taglist = new List<RetDeviceTag>();
                            if (item.TagMap != "" && item.TagMap != null)
                            {
                                taglist = JsonConvert.DeserializeObject<List<RetDeviceTag>>(item.TagMap);
                            }
                            deviceinfo.TagList = taglist;
                            // 获取设备属性信息
                            var getdeviceitem = device.D_DevicesItem.Where(s => s.DeviceID == item.ID).ToList();
                            if (getdeviceitem != null)
                            {
                                List<D_DevicesItem> devicelist = getdeviceitem.ToList<D_DevicesItem>();
                                foreach (var deviceitem in devicelist)
                                {
                                    List<string> unit = new List<string>();
                                    if (deviceitem.Unit != "" && deviceitem.Unit != null)
                                    {
                                        unit = JsonConvert.DeserializeObject<List<string>>(deviceitem.Unit);
                                    }
                                    var deviceiteminfo = new RetDeviceItemInfo();
                                    deviceiteminfo.ID = deviceitem.ID.ToString();
                                    deviceiteminfo.DeviceID = deviceitem.DeviceID.ToString();
                                    deviceiteminfo.Name = deviceitem.Name;
                                    deviceiteminfo.PropertyLabel = deviceitem.PropertyLabel;
                                    deviceiteminfo.ValueType = deviceitem.ValueType.ToString();
                                    deviceiteminfo.Unit = unit;
                                    deviceiteminfo.Value = deviceitem.Value;
                                    deviceiteminfo.Description = deviceitem.Description;
                                    deviceiteminfo.OrgID = deviceitem.OrgID.ToString();
                                    itemlistinfo.Add(deviceiteminfo);
                                }
                                deviceinfo.DeviceItems = itemlistinfo;
                            }
                            listinfo.Add(deviceinfo);
                        }
                        r.Data = listinfo;
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
        /// 获取数据库连接配置列表
        /// </summary>
        /// <returns>成功返回配置列表,失败返回Null.</returns>
        public ReturnItem<List<RetDataConnectConfiguration>> GetDataBaseList(DataConnectConfigurationModel parameter)
        {
            ReturnItem<List<RetDataConnectConfiguration>> r = new ReturnItem<List<RetDataConnectConfiguration>>();
            List<RetDataConnectConfiguration> listinfo = new List<RetDataConnectConfiguration>();
            using (MonitoringEntities device = new MonitoringEntities())
            {
                try
                {
                    IQueryable<D_DataConnectConfiguration> getdatabaselist = device.D_DataConnectConfiguration.AsQueryable<D_DataConnectConfiguration>();
                    if (parameter.Name != null && parameter.Name != "")
                    {
                        getdatabaselist = getdatabaselist.Where(s => s.Name.IndexOf(parameter.Name) >= 0);
                    }
                    if (parameter.OrgID.ToString() != null && parameter.OrgID.ToString() != "")
                    {
                        var OrgID = Convert.ToInt32(parameter.OrgID);
                        getdatabaselist = getdatabaselist.Where(s => s.OrgID == OrgID || s.OrgID == -1);
                    }
                    getdatabaselist = getdatabaselist.OrderByDescending(s => s.CreateTime);
                    if (getdatabaselist == null)
                    {
                        r.Data = null;
                        r.Code = -1;
                        r.Msg = "未找到数据库配置信息";
                        return r;
                    }
                    if (getdatabaselist != null)
                    {
                        r.Count = getdatabaselist.Count();
                        if (parameter.PageSize != 0 && parameter.PageIndex != 0) 
                        {
                            getdatabaselist = getdatabaselist.Skip((parameter.PageIndex - 1) * parameter.PageSize).Take(parameter.PageSize);
                        }
                        List<D_DataConnectConfiguration> list = getdatabaselist.ToList<D_DataConnectConfiguration>();
                        r.Msg = "数据库配置信息获取成功";
                        r.Code = 0;
                        foreach (var item in list)
                        {
                            var databaseinfo = new RetDataConnectConfiguration();
                            databaseinfo.ID = item.ID.ToString();
                            databaseinfo.Name = item.Name;
                            databaseinfo.Type = item.Type.ToString();
                            databaseinfo.ServerAddress = item.ServerAddress;
                            databaseinfo.ServerPort = item.ServerPort;
                            databaseinfo.UserName = item.UserName;
                            databaseinfo.PassWord = item.PassWord;
                            databaseinfo.DataBase = item.DataBase;
                            databaseinfo.ConnectString = item.ConnectString;
                            databaseinfo.CreateTime = item.CreateTime;
                            databaseinfo.CreateUserID = item.CreateUserID.ToString();
                            databaseinfo.OrgID = item.OrgID.ToString();
                            listinfo.Add(databaseinfo);
                        }
                        r.Data = listinfo;
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
        /// 获取数据库连接配置
        /// </summary>
        /// <returns>成功返回配置列表,失败返回Null.</returns>
        public ReturnItem<RetDataConnectConfiguration> GetDataConnect(DataConnectConfigurationModel parameter)
        {
            ReturnItem<RetDataConnectConfiguration> r = new ReturnItem<RetDataConnectConfiguration>();
            using (MonitoringEntities device = new MonitoringEntities())
            {
                try
                {
                    var getdatabase = device.D_DataConnectConfiguration.Where(s => s.ID == parameter.ID).FirstOrDefault();
                    if (getdatabase == null)
                    {
                        r.Data = null;
                        r.Code = -1;
                        r.Msg = "未找到数据库配置信息";
                        return r;
                    }
                    if (getdatabase != null)
                    {
                        r.Msg = "数据库配置信息获取成功";
                        r.Code = 0;
                        r.Data = new RetDataConnectConfiguration()
                        {
                            ID = getdatabase.ID.ToString(),
                            Name = getdatabase.Name,
                            Type = getdatabase.Type.ToString(),
                            ServerAddress = getdatabase.ServerAddress,
                            ServerPort = getdatabase.ServerPort,
                            UserName = getdatabase.UserName,
                            PassWord = getdatabase.PassWord,
                            DataBase = getdatabase.DataBase,
                            ConnectString = getdatabase.ConnectString,
                            CreateTime = getdatabase.CreateTime,
                            CreateUserID = getdatabase.CreateUserID.ToString(),
                            OrgID = getdatabase.OrgID.ToString()
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
        /// 新增数据库连接配置
        /// </summary>
        public ReturnItem<RetDataConnectConfiguration> AddDataConnect(DataConnectConfigurationModel parameter)
        {
            ReturnItem<RetDataConnectConfiguration> r = new ReturnItem<RetDataConnectConfiguration>();
            using (MonitoringEntities device = new MonitoringEntities())
            {
                try
                {
                    int PID = int.Parse(parameter.OrgID);
                    var adddataconnect = device.D_DataConnectConfiguration.Where(s => s.Name == parameter.Name && s.OrgID == PID).FirstOrDefault();
                    if (adddataconnect != null)
                    {
                        r.Data = null;
                        r.Code = -1;
                        r.Msg = "数据库连接配置已存在";
                        return r;
                    }
                    if (adddataconnect == null)
                    {
                        D_DataConnectConfiguration newdataconnect = new D_DataConnectConfiguration()
                        {
                            Name = parameter.Name,
                            Type = Convert.ToInt32(parameter.Type),
                            ServerAddress = parameter.ServerAddress,
                            ServerPort = parameter.ServerPort,
                            UserName = parameter.UserName,
                            PassWord = parameter.PassWord,
                            DataBase = parameter.DataBase,
                            ConnectString = parameter.ConnectString,
                            CreateTime = DateTime.Now,
                            CreateUserID = parameter.UserID,
                            OrgID = Convert.ToInt32(parameter.OrgID),
                        };
                        device.D_DataConnectConfiguration.Add(newdataconnect);
                        device.SaveChanges();
                        r.Msg = "数据库连接配置新增成功";
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
        /// 更新数据库连接配置
        /// </summary>
        public ReturnItem<RetDataConnectConfiguration> EditDataConnect(DataConnectConfigurationModel parameter)
        {
            ReturnItem<RetDataConnectConfiguration> r = new ReturnItem<RetDataConnectConfiguration>();
            using (MonitoringEntities device = new MonitoringEntities())
            {
                try
                {
                    var updatedataconnect = device.D_DataConnectConfiguration.Where(s => s.ID == parameter.ID).FirstOrDefault();
                    if (updatedataconnect == null)
                    {
                        r.Data = null;
                        r.Code = -1;
                        r.Msg = "数据库连接配置不存在";
                        return r;
                    }
                    if (updatedataconnect != null)
                    {
                        updatedataconnect.Name = parameter.Name;
                        updatedataconnect.Type = Convert.ToInt32(parameter.Type);
                        updatedataconnect.ServerAddress = parameter.ServerAddress;
                        updatedataconnect.ServerPort = parameter.ServerPort;
                        updatedataconnect.UserName = parameter.UserName;
                        updatedataconnect.PassWord = parameter.PassWord;
                        updatedataconnect.DataBase = parameter.DataBase;
                        updatedataconnect.ConnectString = parameter.ConnectString;
                        updatedataconnect.UpdateTime = DateTime.Now;
                        updatedataconnect.UpdateUserID = Convert.ToInt32(parameter.UpdateUserID);
                        updatedataconnect.OrgID = Convert.ToInt32(parameter.OrgID);
                        device.SaveChanges();
                        r.Msg = "数据库连接配置更新成功";
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
        /// 删除数据库连接配置
        /// </summary>
        public ReturnItem<RetDataConnectConfiguration> DeleteDataConnect(DataConnectConfigurationModel parameter)
        {
            ReturnItem<RetDataConnectConfiguration> r = new ReturnItem<RetDataConnectConfiguration>();
            using (MonitoringEntities device = new MonitoringEntities())
            {
                try
                {
                    var deletedataconnect = device.D_DataConnectConfiguration.Where(s => s.ID == parameter.ID).FirstOrDefault();
                    if (deletedataconnect != null)
                    {
                        var entry = device.Entry(deletedataconnect);
                        //设置该对象的状态为删除
                        entry.State = EntityState.Deleted;
                        device.SaveChanges();
                        //保存修改
                    }
                    r.Msg = "信息删除成功";
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
        /// 获取物接入配置列表
        /// </summary>
        /// <returns>成功返回配置列表,失败返回Null.</returns>
        public ReturnItem<List<RetIoTHubConfiguration>> GetIoTHubDataList(IoTHubConfigurationModel parameter)
        {
            ReturnItem<List<RetIoTHubConfiguration>> r = new ReturnItem<List<RetIoTHubConfiguration>>();
            List<RetIoTHubConfiguration> listinfo = new List<RetIoTHubConfiguration>();
            using (MonitoringEntities device = new MonitoringEntities())
            {
                try
                {
                    IQueryable<D_IoTHubConfiguration> getdatalist = device.D_IoTHubConfiguration.AsQueryable<D_IoTHubConfiguration>();
                    if (parameter.Code != null && parameter.Code != "")
                    {
                        getdatalist = getdatalist.Where(s => s.Code.IndexOf(parameter.Code) >= 0);
                    }
                    if (parameter.Name != null && parameter.Name != "")
                    {
                        getdatalist = getdatalist.Where(s => s.Name.IndexOf(parameter.Name) >= 0);
                    }
                    if (parameter.OrgID.ToString() != null && parameter.OrgID.ToString() != "")
                    {
                        var OrgID = Convert.ToInt32(parameter.OrgID);
                        getdatalist = getdatalist.Where(s => s.OrgID == OrgID);
                    }
                    getdatalist = getdatalist.OrderByDescending(s => s.CreateTime);
                    if (getdatalist == null)
                    {
                        r.Data = null;
                        r.Code = -1;
                        r.Msg = "未找到物接入配置列表信息";
                        return r;
                    }
                    if (getdatalist != null)
                    {
                        r.Count = getdatalist.Count();
                        if (parameter.PageSize != 0 && parameter.PageIndex != 0)
                        {
                            getdatalist = getdatalist.Skip((parameter.PageIndex - 1) * parameter.PageSize).Take(parameter.PageSize);
                        }
                        List<D_IoTHubConfiguration> list = getdatalist.ToList<D_IoTHubConfiguration>();
                        r.Msg = "物接入配置列表信息获取成功";
                        r.Code = 0;
                        foreach (var item in list)
                        {
                            var databaseinfo = new RetIoTHubConfiguration();
                            databaseinfo.ID = item.ID.ToString();
                            databaseinfo.Name = item.Name;
                            databaseinfo.Type = item.Type.ToString();
                            databaseinfo.UserName = item.UserName;
                            databaseinfo.Code = item.Code;
                            databaseinfo.Url = item.Url;
                            databaseinfo.Password = item.Password;
                            databaseinfo.Remark = item.Password;
                            databaseinfo.CreateTime = item.CreateTime;
                            databaseinfo.CreateUserID = item.CreateUserID.ToString();
                            databaseinfo.OrgID = item.OrgID.ToString();
                            listinfo.Add(databaseinfo);
                        }
                        r.Data = listinfo;
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
        /// 获取物接入配置
        /// </summary>
        /// <returns>成功返回配置列表,失败返回Null.</returns>
        public ReturnItem<RetIoTHubConfiguration> GetIoTHubData(IoTHubConfigurationModel parameter)
        {
            ReturnItem<RetIoTHubConfiguration> r = new ReturnItem<RetIoTHubConfiguration>();
            using (MonitoringEntities device = new MonitoringEntities())
            {
                try
                {
                    var getdata = device.D_IoTHubConfiguration.Where(s => s.ID == parameter.ID).FirstOrDefault();
                    if (getdata == null)
                    {
                        r.Data = null;
                        r.Code = -1;
                        r.Msg = "未找到物接入配置信息";
                        return r;
                    }
                    if (getdata != null)
                    {
                        r.Msg = "物接入配置信息获取成功";
                        r.Code = 0;
                        r.Data = new RetIoTHubConfiguration()
                        {
                            ID = getdata.ID.ToString(),
                            Code = getdata.Code,
                            Name = getdata.Name,
                            Type = getdata.Type.ToString(),
                            Url = getdata.Url,
                            UserName = getdata.UserName,
                            Password = getdata.Password,
                            CreateTime = getdata.CreateTime,
                            CreateUserID = getdata.CreateUserID.ToString(),
                            UpdateTime = getdata.UpdateTime,
                            UpdateUserID = getdata.UpdateUserID.ToString(),
                            OrgID = getdata.OrgID.ToString(),
                            Remark = getdata.Remark
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
        /// 新增物接入配置
        /// </summary>
        public ReturnItem<RetIoTHubConfiguration> AddIoTHubData(IoTHubConfigurationModel parameter)
        {
            ReturnItem<RetIoTHubConfiguration> r = new ReturnItem<RetIoTHubConfiguration>();
            using (MonitoringEntities device = new MonitoringEntities())
            {
                try
                {
                    int PID = int.Parse(parameter.OrgID);
                    var adddata = device.D_IoTHubConfiguration.Where(s => s.Name == parameter.Name && s.OrgID == PID).FirstOrDefault();
                    if (adddata != null)
                    {
                        r.Data = null;
                        r.Code = -1;
                        r.Msg = "物接入配置已存在";
                        return r;
                    }
                    if (adddata == null)
                    {
                        D_IoTHubConfiguration newdata = new D_IoTHubConfiguration()
                        {
                            Code = parameter.Code,
                            Name = parameter.Name,
                            Type = Convert.ToInt32(parameter.Type),
                            Url = parameter.Url,
                            UserName = parameter.UserName,
                            Password = parameter.Password,
                            CreateTime = DateTime.Now,
                            CreateUserID = Convert.ToInt32(parameter.CreateUserID),
                            OrgID = Convert.ToInt32(parameter.OrgID),
                            Remark = parameter.Remark
                        };
                        device.D_IoTHubConfiguration.Add(newdata);
                        device.SaveChanges();
                        r.Msg = "物接入配置新增成功";
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
        /// 更新物接入配置
        /// </summary>
        public ReturnItem<RetIoTHubConfiguration> EditIoTHubData(IoTHubConfigurationModel parameter)
        {
            ReturnItem<RetIoTHubConfiguration> r = new ReturnItem<RetIoTHubConfiguration>();
            using (MonitoringEntities device = new MonitoringEntities())
            {
                try
                {
                    var updatedata = device.D_IoTHubConfiguration.Where(s => s.ID == parameter.ID).FirstOrDefault();
                    if (updatedata == null)
                    {
                        r.Data = null;
                        r.Code = -1;
                        r.Msg = "物接入配置不存在";
                        return r;
                    }
                    if (updatedata != null)
                    {
                        updatedata.Code = parameter.Code;
                        updatedata.Name = parameter.Name;
                        updatedata.Type = Convert.ToInt32(parameter.Type);
                        updatedata.Url = parameter.Url;
                        updatedata.UserName = parameter.UserName;
                        updatedata.Password = parameter.Password;
                        updatedata.UpdateTime = DateTime.Now;
                        updatedata.UpdateUserID = Convert.ToInt32(parameter.UpdateUserID);
                        updatedata.OrgID = Convert.ToInt32(parameter.OrgID);
                        updatedata.Remark = parameter.Remark;
                        device.SaveChanges();
                        r.Msg = "物接入配置更新成功";
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
        /// 删除物接入配置
        /// </summary>
        public ReturnItem<RetIoTHubConfiguration> DeleteIoTHubData(IoTHubConfigurationModel parameter)
        {
            ReturnItem<RetIoTHubConfiguration> r = new ReturnItem<RetIoTHubConfiguration>();
            using (MonitoringEntities device = new MonitoringEntities())
            {
                try
                {
                    var deletedata = device.D_IoTHubConfiguration.Where(s => s.ID == parameter.ID).FirstOrDefault();
                    if (deletedata != null)
                    {
                        var entry = device.Entry(deletedata);
                        //设置该对象的状态为删除
                        entry.State = EntityState.Deleted;
                        device.SaveChanges();
                        //保存修改
                    }
                    r.Msg = "信息删除成功";
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
    }

}
