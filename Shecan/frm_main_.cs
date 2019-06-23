/*
    ابزار استفاده از سرویس شکن
    (سرویس دور زدن تحریم های اینترنتی)
    
    Service Website: http://Shecan.ir
    UnOfficial Client Website: http://AlphaTech-Group.ir


Reference : https://stackoverflow.com/questions/40291375/how-to-change-dns-with-c-sharp-on-windows-10
*/
using System;
using System.Linq;
using System.Net.NetworkInformation;
using System.Windows.Forms;
using System.Management;
using System.Security.Principal;
using System.Net;


namespace Shecan
{
    public partial class frm_main_ : Form
    {
        public frm_main_()
        {
            InitializeComponent();
        }
        public bool IsUserAdministrator()
            {
                bool isAdmin;
                try
                {
                    WindowsIdentity user = WindowsIdentity.GetCurrent();
                    WindowsPrincipal principal = new WindowsPrincipal(user);
                    isAdmin = principal.IsInRole(WindowsBuiltInRole.Administrator);
                }
                catch (UnauthorizedAccessException ex)
                {
                    isAdmin = false;

                }
                catch (Exception ex)
                {
                    isAdmin = false;
                }
                return isAdmin;
            }

    public  NetworkInterface GetActiveEthernetOrWifiNetworkInterface()
        {
            var Nic = NetworkInterface.GetAllNetworkInterfaces().FirstOrDefault(
                a => a.OperationalStatus == OperationalStatus.Up &&
                (a.NetworkInterfaceType == NetworkInterfaceType.Wireless80211 || a.NetworkInterfaceType == NetworkInterfaceType.Ethernet) &&
                a.GetIPProperties().GatewayAddresses.Any(g => g.Address.AddressFamily.ToString() == "InterNetwork"));

            return Nic;
        }
        public void SetDNS(string[] DnsString)
        {
            string[] Dns =  DnsString ;
            var CurrentInterface = GetActiveEthernetOrWifiNetworkInterface();
            if (CurrentInterface == null) return;

            ManagementClass objMC = new ManagementClass("Win32_NetworkAdapterConfiguration");
            ManagementObjectCollection objMOC = objMC.GetInstances();
            foreach (ManagementObject objMO in objMOC)
            {
                if ((bool)objMO["IPEnabled"])
                {
                    if (objMO["Caption"].ToString().Contains(CurrentInterface.Description))
                    {
                        ManagementBaseObject objdns = objMO.GetMethodParameters("SetDNSServerSearchOrder");
                        if (objdns != null)
                        {
                            objdns["DNSServerSearchOrder"] = Dns;
                            objMO.InvokeMethod("SetDNSServerSearchOrder", objdns, null);
                        }
                    }
                }
            }
        }
        public  void UnsetDNS()
        {
            var CurrentInterface = GetActiveEthernetOrWifiNetworkInterface();
            if (CurrentInterface == null) return;

            ManagementClass objMC = new ManagementClass("Win32_NetworkAdapterConfiguration");
            ManagementObjectCollection objMOC = objMC.GetInstances();
            foreach (ManagementObject objMO in objMOC)
            {
                if ((bool)objMO["IPEnabled"])
                {
                    if (objMO["Caption"].ToString().Contains(CurrentInterface.Description))
                    {
                        ManagementBaseObject objdns = objMO.GetMethodParameters("SetDNSServerSearchOrder");
                        if (objdns != null)
                        {
                            objdns["DNSServerSearchOrder"] = null;
                            objMO.InvokeMethod("SetDNSServerSearchOrder", objdns, null);
                        }
                    }
                }
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                var ips = Dns.GetHostEntry("dns.shecan.ir").AddressList;
                string[] DNSs= { ips[0].ToString(), ips[1].ToString() };
                SetDNS(DNSs);
            }
            catch (Exception)
            {
                MessageBox.Show("فعال سازی با مشکل مواجه شد", "خطا", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            MessageBox.Show("فعال سازی با موفقیت انجام شد", "فعال شد", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
            
        }

        private void button3_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            try
            {
                UnsetDNS();
            }
            catch (Exception)
            {
                MessageBox.Show("غیرفعال سازی با مشکل مواجه شد","خطا",MessageBoxButtons.OK,MessageBoxIcon.Error);
                return;
            }
            MessageBox.Show("غیرفعال سازی با موفقیت انجام شد", "غیرفعال شد", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
        }

        private void frm_main__Load(object sender, EventArgs e)
        {
            if (!IsUserAdministrator())
            {
                MessageBox.Show("شما باید با دسترسی مدیر سیستم از این نرم افزار استفاده کنید", "دسترسی محدود", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                Application.Exit();
            }
            
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start("http://shecan.ir");
        }

        private void دربارهسرویسشکنToolStripMenuItem_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start("http://shecan.ir");
        }

        private void دربارهسازندهابزارToolStripMenuItem_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start("http://alphatech-group.ir");
        }
    }
}
