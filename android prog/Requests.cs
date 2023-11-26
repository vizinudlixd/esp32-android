using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Net.Http;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;

namespace Manager_app
{
    class Requests
    {
        private string IP;

        public string Ip { get => IP; set => IP = value; }

        public Requests() 
        { 
            IP = "192.168.1.200";
        }

        public async Task<string> SendRequestAsync(string endpoint, bool isSync)
        {
            using (HttpClient client = new HttpClient())
            {
                try
                {
                    string url = $"http://{IP}:80/{endpoint}";
                    HttpResponseMessage response = await client.GetAsync(url);

                    string responseString = await response.Content.ReadAsStringAsync();
                    string[] responseStringT = responseString.Split(' ');

                    if (responseStringT[0] == "200" && !isSync)
                    {
                        return $"{(responseStringT[1] == "ON" ? "200 On" : "200 Off")}";
                    }
                    else if (responseStringT[0] == "102" && isSync)
                    {
                        return $"{(responseStringT[1] == "ON" ? "102 On" : "102 Off")}";

                    }
                    else return "500 ERR";
                }
                catch (Exception ex)
                {
                    return ex.Message;
                }
            }
        }

        public async Task<string> SendSensorRequestAsync(string endpoint, bool isSync, int sensor)
        {
            using (HttpClient client = new HttpClient())
            {
                try
                {
                    string url = $"http://{IP}:80/{endpoint}";
                    HttpResponseMessage response = await client.GetAsync(url);

                    string responseString = await response.Content.ReadAsStringAsync();
                    string[] responseStringT = responseString.Split(' ');

                    if (responseStringT[0] == "200" && responseStringT[1] == "TEMPON")
                    {
                        return $"200 Hőmérő";
                    }
                    else if (responseStringT[0] == "200" && responseStringT[1] == "HUMIDITYON")
                    {
                        return $"200 Páratartalom";
                    }

                    if (responseStringT[0] == "300" && sensor == 1)
                    {
                        return $"301 {responseStringT[1]}";
                    }
                    else if (responseStringT[0] == "300" && sensor == 0)
                    {
                        return $"302 {responseStringT[1]}";
                    }

                    if (responseStringT[0] == "102" && isSync && responseStringT[1] == "TEMP")
                    {
                        return $"103 Hőmérő";
                    }
                    else if (responseStringT[0] == "102" && isSync && responseStringT[1] == "HUMIDITY")
                    {
                        return $"104 Páratartalom";
                    }


                    return "";
                }
                catch (Exception ex)
                {
                    return ex.Message;
                }
            }
        }
    }
}
