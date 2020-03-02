using DeviceMonitoringBLL.Model.Return.DeviceData;
using System.Collections.Generic;
using System.Linq;


namespace DeviceMonitoringBLL
{
    public class AlgorithmBLL
    {
        /// <summary>
        /// 为对报表进行异常处理，进行一维正态分布分类
        /// </summary>
        /// <param name="device"></param>
        /// <returns></returns>
        public static RetDeviceTableData Gaussain(RetDeviceTableData device)
        {
            RetDeviceTableData deviceSplice = new RetDeviceTableData();
            if (device != null)
            {
                List<double> data = device.Data;
                List<string> time = device.Time;
                List<List<object>> anomCoordination = new List<List<object>>();//实例化，用来接收异常点坐标，可以调取实例化方法
                List<List<object>> normCoordination = new List<List<object>>();//实例化，用来接收正常点坐标，可以调取实例化方法
                //List<List<object>> allCoordination = new List<List<object>>();//实例化，用来接收所有点坐标，可以调取实例化方法
                var average = data.Average();
                double variance;
                double sum = 0;
                var length = data.Count();
                foreach (var d in data)
                {
                    sum += (d - average) * (d - average);
                }
                variance = sum / length;
                for (var i = 0; i < data.Count(); i++)
                {
                    List<object> zuobiao = new List<object>();
                    if (data[i] > average + 2 * variance || data[i] < average - 2 * variance)//取一个合适的系数，这里取的2
                    {
                        zuobiao.Add(time[i]);
                        zuobiao.Add(data[i]);
                        anomCoordination.Add(zuobiao);
                    }
                    else
                    {
                        zuobiao.Add(time[i]);
                        zuobiao.Add(data[i]);
                        normCoordination.Add(zuobiao);

                    }
                    //allCoordination.Add(zuobiao);
                }
                deviceSplice.Data = data;
                deviceSplice.Time = time;
                deviceSplice.DeviceTableList = device.DeviceTableList;
                deviceSplice.DeviceItemName = device.DeviceItemName;
                deviceSplice.Unit = device.Unit;
                deviceSplice.NormCoordination = normCoordination;
                deviceSplice.AnomCoordination = anomCoordination;
                //deviceSplice.AllCoordination = allCoordination;
                return deviceSplice;//包含正常点坐标与异常点坐标
            }
            else
            {
                return deviceSplice = null;       
            }
        }
    }
}
