using System.Linq;
using Zeroconf;

namespace Fg.SolarProductionAlerter.HomeWizard
{
    internal static class HomeWizardDeviceResolver
    {
        public static async Task<HomeWizardDevice?> FindHomeWizardDeviceAsync(string deviceName)
        {
            return (await FindHomeWizardDevicesAsync()).FirstOrDefault(d => d.Name == deviceName);
        }

        public static async Task<IEnumerable<HomeWizardDevice>> FindHomeWizardDevicesAsync()
        {
            IReadOnlyList<IZeroconfHost> results = await ZeroconfResolver.ResolveAsync("_hwenergy._tcp.local.");

            var devices = new List<HomeWizardDevice>();

            foreach (var device in results)
            {
                devices.Add(new HomeWizardDevice(device.DisplayName, device.IPAddress));
            }

            return devices;
        }
    }

    internal class HomeWizardDevice
    {
        public HomeWizardDevice(string device, string ipAddress)
        {
            Name = device;
            IPAddress = ipAddress;
        }

        public string Name { get; }
        public string IPAddress { get; }
    }
}
