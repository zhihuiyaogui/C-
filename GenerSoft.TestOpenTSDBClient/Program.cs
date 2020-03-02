using GenerSoft.OpenTSDB.Client;
using System;
using System.Collections.Generic;
using System.IO;

namespace GenerSoft.TestOpenTSDBClient
{
    public class testclass{
        public static string opentsdburl = "http://172.22.15.132:10042";
        public void testPutData()
        {
            OpentsdbClient client = new OpentsdbClient(opentsdburl);
            try
            {
                Dictionary<string, string> tagMap = new Dictionary<string, string>();
                tagMap.Add("chl", "hqdApp");
                client.putData("metric-t", DateTimeUtil.parse("20160627 12:15", "yyyyMMdd HH:mm"), 210L, tagMap);
                client.putData("metric-t", DateTimeUtil.parse("20160627 12:17", "yyyyMMdd HH:mm"), 180L, tagMap);
                client.putData("metric-t", DateTimeUtil.parse("20160627 13:20", "yyyyMMdd HH:mm"), 180L, tagMap);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }

        public static void testGetData()
        {
            OpentsdbClient client = new OpentsdbClient(opentsdburl);
            try
            {
                Dictionary<string, string> tagMap = new Dictionary<string, string>();
                tagMap.Add("waterpump", "waterpump4");
                string resContent = client.getData("current", tagMap, OpentsdbClient.AGGREGATOR_AVG, "1m", "2018-03-27 10:20:00", "2018-03-27 10:25:00");
                Console.WriteLine(resContent);
                //log.info(">>>" + resContent);
                //Dictionary<string, string> tagMap = new Dictionary<string, string>();
                //tagMap.Add("host", "szawd");
                //OpenTSDB.Client.opentsdb.client.response.QueryLastResponse res = client.queryLastData("sys.batch.test6", tagMap, 500);
                //if (null != res)
                //{
                //    Console.WriteLine(res.value);
                //}

            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }

        public void testGetData2()
        {
            OpentsdbClient client = new OpentsdbClient(opentsdburl);
            try
            {
                Dictionary<string, string> tagMap = new Dictionary<string, string>();
                tagMap.Add("chl", "hqdApp");
                IDictionary<string, Dictionary<string, object>> tagsValuesMap = (IDictionary<string, Dictionary<string, object>>)client.getData("metric-t", tagMap, OpentsdbClient.AGGREGATOR_SUM, "1h", "2016-06-27 10:00:00", "2016-06-30 11:00:00", "yyyyMMdd hh");
                foreach (var it in tagsValuesMap)
                {
                    string tags = it.Key.ToString();
                    Console.WriteLine(">> tags: " + tags);
                    IDictionary<string, Object> tvMap = tagsValuesMap[tags];
                    foreach (var it2 in tvMap)
                    {
                        string time = it2.Key.ToString();
                        Console.WriteLine(" >> " + time + " <-> " + tvMap[time]);
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }


        public static List<string> readFileToList(string fileName)
        {
            FileStream fs = new FileStream(fileName, FileMode.Open, FileAccess.Read);
            List<string> list = new List<string>();
            StreamReader m_streamReader = new StreamReader(fs);//中文乱码加上System.Text.Encoding.Default,或则 System.Text.Encoding.GetEncoding("GB2312")
                                                               //使用StreamReader类来读取文件
            m_streamReader.BaseStream.Seek(0, SeekOrigin.Begin);
            // 从数据流中读取每一行，直到文件的最后一行，并在richTextBox1中显示出内容

            OpentsdbClient client = new OpentsdbClient(opentsdburl);

            string strLine = m_streamReader.ReadLine();
            int i = 1;
            while (strLine != null)
            {
                //list.Add(strLine);
                Array temp = strLine.Split('\t');
                Dictionary<string, string> tagMap = new Dictionary<string, string>();
                //DateTime dt = DateTimeUtil.parse(temp.GetValue(1).ToString(), "yyyy/MM/dd HH:mm");
                DateTime dt1 = Convert.ToDateTime(temp.GetValue(1));
                tagMap.Add("room", "room"+temp.GetValue(0));
                //nq_cu_heat	nq_instant_energy	nq_cu_flow	nq_instant_flowrate	nq_entrance_temp	nq_exit_temp	nq_diff_tmp
                bool bool1 = client.putData("nq_cu_heat", dt1, Convert.ToDouble(temp.GetValue(2)), tagMap);
                bool bool2 = client.putData("nq_instant_energy", dt1, Convert.ToDouble(temp.GetValue(3)), tagMap);
                bool bool3 = client.putData("nq_cu_flow", dt1, Convert.ToDouble(temp.GetValue(4)), tagMap);
                bool bool4 = client.putData("nq_instant_flowrate", dt1, Convert.ToDouble(temp.GetValue(5)), tagMap);
                bool bool5 = client.putData("nq_entrance_temp", dt1, Convert.ToDouble(temp.GetValue(6)), tagMap);
                bool bool6 = client.putData("nq_exit_temp", dt1, Convert.ToDouble(temp.GetValue(7)), tagMap);
                bool bool7 = client.putData("nq_diff_tmp", dt1, Convert.ToDouble(temp.GetValue(8)), tagMap);
                Console.WriteLine(i.ToString() + ":" + bool1 + " " + bool2 + " " + bool3 + " " + bool4 + " " + bool5 + " " + bool6 + " " + bool7);

                strLine = m_streamReader.ReadLine();
                i++;
            }
            //关闭此StreamReader对象
            m_streamReader.Close();
            return list;


        }
        public static void writeListToFile(List<string> pList, string myFileName)
        {
            if (File.Exists(myFileName))
            {
                try
                {
                    File.Delete(myFileName);
                }
                finally { };
            }
            //创建一个文件流，用以写入或者创建一个StreamWriter
            System.IO.FileStream fs = new System.IO.FileStream(myFileName, FileMode.OpenOrCreate, FileAccess.Write);
            StreamWriter m_streamWriter = new StreamWriter(fs);
            m_streamWriter.Flush();
            // 使用StreamWriter来往文件中写入内容
            m_streamWriter.BaseStream.Seek(0, SeekOrigin.Begin);
            // 把richTextBox1中的内容写入文件
            for (int i = 0; i < pList.Count; i++)
            {


                m_streamWriter.WriteLine(pList[i]);
            }
            //关闭此文件
            m_streamWriter.Flush();
            m_streamWriter.Close();
        }
    }

    class Program
    {
        
        static void Main(string[] args)
        {
            // 测试git 再测试
        }

    }
}
