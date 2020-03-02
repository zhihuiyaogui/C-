using DeviceMonitoringBLL.Model.Parameter.DeviceMonitoring;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeviceMonitoringBLL.Model.Return.DeviceMonitoring
{
    public class RetMedicineDetails
    {
        public long Id { get; set; }

        public int MedicinePrice { get; set; }

        public long MedicineId { get; set; }

        public string MedicineName { get; set; }

        public int Count { get; set; }


        public List<RetMedicineInfo> Data { get; set; }

        public string manuName { get; set; }

        public string CabintId { get; set; }
        public string Description { get; set; }

        public string spec { get; set; }

        public string mac { get; set; }
        public string Image { get; set; }
        public string tableId { get; set; }
        public string tableName { get; set; }

    }
}
