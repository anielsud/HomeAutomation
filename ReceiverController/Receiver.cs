using PioneerAvrControlLib;
using PioneerAvrControlLib.DataSources;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReceiverController
{
    public class Receiver
    {
        private PioneerConnection connection { get; set; }
        private string IP { get; set; }
        public Receiver(string ip)
        {
            IP = ip;
        }

        public void Connect()
        {
            var source = new TcpClientDataSource(IP, 23);
            var arg = new List<WritableDataSource> { source };
            connection = new PioneerAvrControlLib.PioneerConnection(arg);

            connection.Start();

        }

        public void PowerOn()
        {
            connection.SendMessage(new PowerOn());
        }

        public void PowerOff()
        {
            connection.SendMessage(new PowerOff());
        }

        public void SetVolume(string vol)
        {
            connection.SendMessage(new VolumeSet(int.Parse(vol)));
        }

        public enum ReceiverInput { PC, Cable }
        public void SwitchInput(ReceiverInput input)
        {
            InputType newinput = InputType.HDMI_3;
            if (input == ReceiverInput.Cable)
            {
                newinput = InputType.CBL_Satelite;
            }
            if(input == ReceiverInput.PC)
            {
                newinput = InputType.HDMI_3;
            }
            connection.SendMessage(new InputTypeChange(newinput));
        }
    }
}
