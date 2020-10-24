using System;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using Meadow;
using Meadow.Devices;
using Meadow.Foundation;
using Meadow.Foundation.Leds;
using Meadow.Gateway.WiFi;
using RCP;
using RCP.Transporter;

namespace MeadowApplication2
{
    public class MeadowApp : App<F7Micro, MeadowApp>
    {
        RgbPwmLed onboardLed;

        public MeadowApp()
        {
            Initialize();
            CycleColors(1000);
        }

        void Initialize()
        {
            Console.WriteLine("Initialize hardware...");

            Device.InitWiFiAdapter().Wait();

            if (Device.WiFiAdapter.Connect("myssid", "mypwd").ConnectionStatus != ConnectionStatus.Success)
            {
                throw new Exception("Cannot connect to network, applicaiton halted.");
            }
            else
                Console.WriteLine("connected to wifi...");

            //Console.WriteLine("type: " + Device.WiFiAdapter.Networks.FirstOrDefault().TypeOfNetwork.ToString());
            //Console.WriteLine("protocol: " + Device.WiFiAdapter.Networks.FirstOrDefault().Protocol.ToString());
            //Console.WriteLine("phytype: " + Device.WiFiAdapter.Networks.FirstOrDefault().Phy.ToString());

            Console.WriteLine("starting rabbit...");
            var Rabbit = new RCPServer();
            Console.WriteLine("adding transporter...");
            Rabbit.AddTransporter(new FleckWebsocketServerTransporter("0.0.0.0", 10000));
            Console.WriteLine("adding param...");
            var param = Rabbit.CreateNumberParameter<float>("My FLoat");
            //param now is of type NumberParameter<float> in group "Root"
            param.Value = 0.5f;
            param.Minimum = -1.0f;
            param.Maximum = 1.0f;

            // listen for value updates on the parameter
            param.ValueUpdated += (s, a) => Console.WriteLine("value: " + param.Value.ToString());

            onboardLed = new RgbPwmLed(device: Device,
                redPwmPin: Device.Pins.OnboardLedRed,
                greenPwmPin: Device.Pins.OnboardLedGreen,
                bluePwmPin: Device.Pins.OnboardLedBlue,
                3.3f, 3.3f, 3.3f,
                Meadow.Peripherals.Leds.IRgbLed.CommonType.CommonAnode);
        }

        public static string GetLocalIPAddress()
        {
            var host = Dns.GetHostEntry(Dns.GetHostName());
            foreach (var ip in host.AddressList)
            {
                if (ip.AddressFamily == AddressFamily.InterNetwork)
                {
                    return ip.ToString();
                }
            }
            throw new Exception("No network adapters with an IPv4 address in the system!");
        }

        private void Param_ValueUpdated(object sender, EventArgs e)
        {
            Console.WriteLine("value: " + (sender as INumberParameter).Value);
        }

        void CycleColors(int duration)
        {
            Console.WriteLine("Cycle colors...");

            while (true)
            {
                ShowColorPulse(Color.Blue, duration);
                ShowColorPulse(Color.Cyan, duration);
                ShowColorPulse(Color.Green, duration);
                ShowColorPulse(Color.GreenYellow, duration);
                ShowColorPulse(Color.Yellow, duration);
                ShowColorPulse(Color.Orange, duration);
                ShowColorPulse(Color.OrangeRed, duration);
                ShowColorPulse(Color.Red, duration);
                ShowColorPulse(Color.MediumVioletRed, duration);
                ShowColorPulse(Color.Purple, duration);
                ShowColorPulse(Color.Magenta, duration);
                ShowColorPulse(Color.Pink, duration);
            }
        }

        void ShowColorPulse(Color color, int duration = 1000)
        {
            onboardLed.StartPulse(color, (uint)(duration / 2));
            Thread.Sleep(duration);
            onboardLed.Stop();
        }

        void ShowColor(Color color, int duration = 1000)
        {
            Console.WriteLine($"Color: {color}");
            onboardLed.SetColor(color);
            Thread.Sleep(duration);
            onboardLed.Stop();
        }
    }
}
