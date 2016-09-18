using ReceiverController;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace RemoteControlServer
{
    class Program
    {
        static void Main(string[] args)
        {
            string username = "";
            string password = "";

            try
            {
                XmlDocument xdoc = new XmlDocument();
                xdoc.Load("password.config");
                username = xdoc.SelectSingleNode("//Harmony/username").Value;
                password = xdoc.SelectSingleNode("//Harmony/password").Value;
            }catch(Exception e)
            {
                throw new Exception("password file not supplied", e);
            }

            HarmonyController h = new HarmonyController("192.168.0.6", username, password);
            Receiver r = new Receiver("192.168.0.5");
            h.Status += H_Status;
            h.Connect();
            r.Connect();
            while (true)
            {
                string newstate = Console.ReadLine();
                if (newstate.StartsWith("set volume "))
                {
                    r.SetVolume(newstate.Replace("set volume ", ""));
                }
                if(newstate == "watch tv")
                {
                    h.TVPowerOn();
                    r.SwitchInput(Receiver.ReceiverInput.Cable);
                    h.CblPressOK();
                    h.CblPressGuide();
                    
                }
                if (newstate == "watch pc")
                {
                    h.TVPowerOn();
                    r.SwitchInput(Receiver.ReceiverInput.PC);
                }
                if(newstate == "shut down")
                {
                    r.SetVolume("0");
                    h.TVPowerOff();

                }
                if(newstate == "ok")
                {
                    h.CblPressOK();
                }
                if(newstate == "guide")
                {
                    h.CblPressGuide();
                }
                //if (newstate == "receiver on")
                //{
                //    r.PowerOn();
                //}
                //if (newstate == "receiver off")
                //{
                //    r.PowerOff();
                //}
            }
        }

        private static void H_Status(string status)
        {
            Console.WriteLine(status);
        }
    }
}
