using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Networking;
using Windows.Networking.Connectivity;
using Windows.Networking.Sockets;
using Windows.Storage.Streams;

namespace Car_remote_controller.Models
{
    class TCP
    {
        public static string IPSet;
        public static string PortSet;
        /// <summary>

        /// 发送tcp指令，不返回是否成功的结果

        /// </summary>

        /// <param name="command">命令</param>

        /// <param name="ip">ip</param>

        /// <param name="port">端口</param>

        public static async Task SendTCPCommand(string command, string ip, string port)
        {
            StreamSocket socket = new StreamSocket();

            try
            {
                HostName svname = new HostName(ip);

                // 连接

                await socket.ConnectAsync(svname, port);

                //发送数据

                byte[] data;
                DataWriter dw = new DataWriter(socket.OutputStream);
                //必须ASCII

                data = System.Text.Encoding.ASCII.GetBytes(command);
                //发送

                dw.WriteBytes(data);
                await dw.StoreAsync();
                //释放

                dw.DetachStream();

                socket.Dispose();
                /*

                // 接收数据

                DataReader reader = new DataReader(socket.InputStream);

                reader.UnicodeEncoding = UnicodeEncoding.Utf8; //注意

                // 长度

                await reader.LoadAsync(sizeof(uint));

                uint len = reader.ReadUInt32();

                // 读内容

                await reader.LoadAsync(len);

                string msg = reader.ReadString(reader.UnconsumedBufferLength);

                runRecMsg.Text = msg;

                // 释放

                reader.Dispose();

                */
            }
            catch
            {
                //runRecMsg.Text = "error!";

            }
        }
        /// <summary>

        /// 获取当前的ip

        /// </summary>

        /// <returns>无线网卡所在的ip</returns>

        public static string GetIP()
        {
            // 获取本地主机名称列表

            var hosts = NetworkInformation.GetHostNames();
            // 筛选无线或以太网

            var host = hosts.FirstOrDefault(h =>
            {
                bool isIpaddr = (h.Type == Windows.Networking.HostNameType.Ipv4) || (h.Type == Windows.Networking.HostNameType.Ipv6);
                // 如果不是IP地址表示的名称，则忽略

                if (isIpaddr == false)
                {
                    return false;
                }
                IPInformation ipinfo = h.IPInformation;
                // 71表示无线，6表示以太网

                if (ipinfo.NetworkAdapter.IanaInterfaceType == 71 || ipinfo.NetworkAdapter.IanaInterfaceType == 6)
                {
                    return true;
                }
                return false;
            });
            if (host != null)
            {
                return host.DisplayName; //显示IP

            }
            return "没有检测到联网";
        }
        /// <summary>

        /// 自动搜索本机ip段下的服务器ip

        /// </summary>

        /// <param name="ownip">本机ip</param>

        /// <param name="port">目标端口</param>

        /// <returns>目标服务器ip</returns>

        public static async Task<string> SearchIP(string ownip, string port)
        {
            string segment = "";
            string[] str2;
            int count_temp = 0;
            str2 = ownip.Split('.');
            foreach (string i in str2)
            {
                if (count_temp < 3)
                {
                    segment += i + ".";
                    count_temp++;
                }
            }

            for (count_temp = 0; count_temp < 255; count_temp++)
            {
                if (await CheckIP(segment + count_temp, port))
                {
                    return segment + count_temp;
                }
            }

            return "none";
        }
        /// <summary>

        /// 检测是否能连上

        /// </summary>

        /// <param name="ip">目标服务器ip</param>

        /// <param name="port">目标服务器端口</param>

        /// <returns>结果</returns>

        public static async Task<bool> CheckIP(string ip, string port)
        {
            StreamSocket socket = new StreamSocket();
            try
            {
                HostName svname = new HostName(ip);
                // 连接

                await socket.ConnectAsync(svname, port);

                //发送数据

                byte[] data;
                DataWriter dw = new DataWriter(socket.OutputStream);
                //必须ASCII

                data = System.Text.Encoding.ASCII.GetBytes("check");
                //发送

                dw.WriteBytes(data);
                await dw.StoreAsync();
                //释放

                dw.DetachStream();
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}
