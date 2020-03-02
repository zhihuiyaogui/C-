using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeviceMonitoringBLL.Model.Return.DeviceMonitoring
{
    public class RetMedicineInfo
    {
        public long Id { get; set; }

        public string mac { get; set; }

        public string CabintId { get; set; }

        public Nullable<int> MedicinePrice { get; set; }

        public Nullable<long> MedicineId { get; set; }

        public string MedicineName { get; set; }

        public Nullable<int> Count { get; set; }

        public string manuName { get; set; }

        public string spec { get; set; }

        public Nullable<System.DateTime> CreatTime { get; set; }

        public Nullable<System.DateTime> StartDateTime { get; set; }

        public Nullable<System.DateTime> EndDateTime { get; set; }

        public string Description { get; set; }

        public string Image { get; set; }
    }
}
