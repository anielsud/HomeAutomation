using HarmonyHub;
using HarmonyHub.Entities.Response;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RemoteControlServer
{
    class HarmonyController
    {
        public HarmonyController(string host, string username, string password)
        {
            Host = host;
            Username = username;
            Password = password;
        }

        private string Host { get; set; }
        private string Username { get; set; }
        private string Password { get; set; }

        public HarmonyClient Client { get; private set; }

        public delegate void StatusUpdateHandler(string status);

        public event StatusUpdateHandler Status;

        private void OnUpdateStatus(string status)
        {
            if(Status != null)
            {
                Status.Invoke(status);
            }
        }
        public async Task Connect()
        {
            
            OnUpdateStatus("Connecting... ");
            //First create our client and login
            if (File.Exists("SessionToken"))
            {
                var sessionToken = File.ReadAllText("SessionToken");
                OnUpdateStatus("Reusing token: " + sessionToken);
                Client = HarmonyClient.Create(Host, sessionToken);
            }
            else
            {
                if (string.IsNullOrEmpty(Password) || string.IsNullOrEmpty(Username))
                {
                    throw new Exception("Credentials missing!");
                }

                OnUpdateStatus("authenticating with Logitech servers...");
                Client = await HarmonyClient.Create(Host, Username, Password);
                File.WriteAllText("SessionToken", Client.Token);
            }

            OnUpdateStatus("Fetching Harmony Hub configuration...");

            //Fetch our config
            harmonyConfig = await Client.GetConfigAsync();
            //            debugOutput(harmonyConfig);
            foreach (var device in harmonyConfig.Devices)
            {
                devices.Add(device.Label, device);
            }

        }

        private Config harmonyConfig { get; set; }
        private Dictionary<string, Device> devices = new Dictionary<string, Device>();

        public void TVPowerOn()
        {
            Client.SendCommandAsync(devices["TV"].Id, "PowerOn");
        }

        public void TVPowerOff()
        {
            Client.SendCommandAsync(devices["TV"].Id, "PowerOff");
        }

        public void CblPressOK()
        {
            Client.SendCommandAsync(devices["Cable"].Id, "Select");
        }

        public void CblPressGuide()
        {
            Client.SendCommandAsync(devices["Cable"].Id, "Guide");
        }

        private void debugOutput(Config aConfig)
        {
            //Add our devices
            foreach (Device device in aConfig.Devices.Where(t=>t.Label == "Cable"))
            {
                OnUpdateStatus("device id:" + device.Id + $", { device.Label} ({ device.DeviceTypeDisplayName}/{ device.Model})");

                foreach (ControlGroup cg in device.ControlGroups)
                {
                    OnUpdateStatus("  control group:" + cg.Name);
                    foreach (Function f in cg.Functions)
                    {
                        OnUpdateStatus("    function:" + f.Name + ", label:" + f.Label);
                    }
                }
            }
        }

    }
}
