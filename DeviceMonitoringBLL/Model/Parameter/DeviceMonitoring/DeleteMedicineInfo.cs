using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeviceMonitoringBLL.Model.Parameter.DeviceMonitoring
{
    
   public class DeleteMedicineInfo
    {
        public long Id { get; set; }

        public int MedicinePrice { get; set; }

        public long MedicineId { get; set; }

        public string MedicineName { get; set; }

        public int Count { get; set; }

        public Nullable<System.DateTime> CreatTime { get; set; }

        public Nullable<System.DateTime> StartDateTime { get; set; }

        public Nullable<System.DateTime> EndDateTime { get; set; }

        public string Description { get; set; }

        public string Image { get; set; }
    }
}
