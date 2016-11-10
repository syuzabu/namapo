using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using System.Diagnostics;
using System.Net;
using SharpPcap;
using SharpPcap.LibPcap;
using SharpPcap.WinPcap;
using System.Runtime.InteropServices;
using System.Timers;
using System.Threading;
using System.Threading.Tasks;
using System.Net.NetworkInformation;

namespace namapo
{

    public partial class frmMain : Form
    {
        public const short ETH_HDR_SIZE = 14;
        public const short IP_HDR_SIZE = 20;
        public const short UDP_HDR_SIZE = 8;

        public const short MAC_ADDR_SIZE = 6;
        public const short IP_ADDR_SIZE = 4;

        public const int MBPS_CONV1 = 125000;   /* 1000 * 1000 / 8 */
        public const int MBPS_CONV2 = 131072;   /* 1024 * 1024 / 8 */
        public const int CALC_DIFF_SIZE1 = 0;   /* framesize */
        public const int CALC_DIFF_SIZE2 = ETH_HDR_SIZE + IP_HDR_SIZE + UDP_HDR_SIZE;   /* payload */

        public System.Timers.Timer tmMain;

        public frmMain()
        {
            InitializeComponent();
        }

        public void update_ip_csum(byte[] data, int ip_head = 14)
        {
            ulong csum = 0;

            for (int n = ip_head; n <= (ip_head + 18); n += 2)
            {
                if (n == (ip_head + 10)) continue;  /* skip csum fields */
                csum += (ulong)((data[n] << 8) + data[n + 1]);
            }

            csum = (csum & 0xFFFF) + (csum >> 16);
            csum = 0xFFFF - csum;

            data[ip_head + 10] = (byte)(csum >> 8);
            data[ip_head + 11] = (byte)(csum & 0xFF);
        }

        public byte[] str2hexs(string str, char separator, bool dec = false)
        {
            string[] sHex = str.Split(separator);
            byte[] bHex = new byte[sHex.Length];
            StringBuilder sb = new StringBuilder(sHex.Length);
            int n;

            sb.Clear();
            n = 0;
            if (dec)
            {
                foreach (string sDat in sHex)
                {
                    bHex[n++] = ("" != sDat) ? Convert.ToByte(sDat, 10) : (byte)0x00;
                }
            }
            else
            {
                foreach (string sDat in sHex)
                {
                    bHex[n++] = ("" != sDat) ? Convert.ToByte(sDat, 16) : (byte)0x00;
                }
            }

            return bHex;
        }

        public void update_udp_csum(byte[] data, int udp_head = 34)
        {
            ulong csum = 0;
            int n;
            int udp_dlen = (data[udp_head + 4] << 8) + data[udp_head + 5];

            n = udp_head - 8;
            /* src addr */
            csum += (ulong)((data[n] << 8) + data[n + 1]); n += 2;
            csum += (ulong)((data[n] << 8) + data[n + 1]); n += 2;
            /* dst addr */
            csum += (ulong)((data[n] << 8) + data[n + 1]); n += 2;
            csum += (ulong)((data[n] << 8) + data[n + 1]); n += 2;
            //csum += 0x0011;
            csum += (ulong)IPAddress.HostToNetworkOrder((short)0x0011);
            csum += (ulong)IPAddress.HostToNetworkOrder((short)udp_dlen);   /* UDP data length */



            data[udp_head + 6] = 0;
            data[udp_head + 7] = 0;
            for (n = udp_head; n < (udp_head + udp_dlen); n++)
            {
                if (0 != ((n - udp_head) & 0x01))
                {
                    csum += (ulong)(data[n] << 8);
                }
                else
                {
                    csum += data[n];
                }
                //csum += (ulong)((data[n] << 8) + data[n + 1]);
            }

            csum = (csum & 0xFFFF) + (csum >> 16);
            csum = 0xFFFF - csum;

            data[udp_head + 6] = (byte)(csum & 0xFF);
            data[udp_head + 7] = (byte)(csum >> 8);
        }

        // 参考: http://furuya02.hatenablog.com/entry/20111107/1399766912
        // http://furuya02.hatenablog.com/entry/20111106/1399766898
        // http://furuya02.hatenablog.com/entry/20111109/1320804694
        [DllImport("iphlpapi.dll")]
        extern static int GetIpNetTable(IntPtr pTcpTable, ref int pdwSize, bool bOrder);

        [DllImport("Iphlpapi.dll")]
        extern static int CreateIpNetEntry(IntPtr pArpEntry);

        [DllImport("iphlpapi.dll")]
        extern static int DeleteIpNetEntry(IntPtr pArpEntry);

        [StructLayout(LayoutKind.Sequential)]
        public struct MIB_IPNETROW
        {
            public int Index;
            public int PhysAddrLen;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 8)]
            public byte[] PhysAddr;
            public int Addr;
            public int Type;
        }
        enum MIB_IPNET_TYPE
        {
            OTHER = 1,//その他
            INVALID = 2,//無効
            DYNAMIC = 3,//動的
            STATIC = 4//静的
        }

        public List<MIB_IPNETROW> arp_get()
        {
            List<MIB_IPNETROW> ar = new List<MIB_IPNETROW>();

            /* arpテーブルの状態を取得する( arp -a と同義) */
            int size = 0;
            GetIpNetTable(IntPtr.Zero, ref size, true);     //必要サイズの取得
            var p = Marshal.AllocHGlobal(size);             //メモリ割当て
            if (GetIpNetTable(p, ref size, true) == 0)
            {    //データの取得
                var num = Marshal.ReadInt32(p);             //MIB_IPNETTABLE.dwNumEntries(データ数)
                var ptr = IntPtr.Add(p, 4);
                for (int i = 0; i < num; i++)
                {
                    ar.Add((MIB_IPNETROW)Marshal.PtrToStructure(ptr, typeof(MIB_IPNETROW)));
                    ptr = IntPtr.Add(ptr, Marshal.SizeOf(typeof(MIB_IPNETROW)));//次のデータ
                }
                Marshal.FreeHGlobal(p);  //メモリ開放
            }

            return ar;
        }

        public int arp_check(int ipa)
        {
            int ret = 0;
            List<MIB_IPNETROW> ar = arp_get();
            
            ///* arpテーブルの状態を取得する( arp -a と同義) */
            //int size = 0;
            //GetIpNetTable(IntPtr.Zero, ref size, true);     //必要サイズの取得
            //var p = Marshal.AllocHGlobal(size);             //メモリ割当て
            //if (GetIpNetTable(p, ref size, true) == 0) {    //データの取得
            //    var num = Marshal.ReadInt32(p);             //MIB_IPNETTABLE.dwNumEntries(データ数)
            //    var ptr = IntPtr.Add(p, 4);
            //    for (int i = 0; i < num; i++) {
            //        ar.Add((MIB_IPNETROW)Marshal.PtrToStructure(ptr, typeof(MIB_IPNETROW)));
            //        ptr = IntPtr.Add(ptr, Marshal.SizeOf(typeof(MIB_IPNETROW)));//次のデータ
            //    }
            //    Marshal.FreeHGlobal(p);  //メモリ開放
            //}

            /* デバイスのインデックス値から対象のarpテーブルを求める */
            int did = ((CbxObjectItem)cbxNic.SelectedItem).index;
            foreach (var x in ar.Where(x => x.Index == did))
            {
                Console.WriteLine("{0}  {1:x}  {2}  {3}",
                    x.Index, x.Addr, x.PhysAddr, x.Type);

                if (x.Addr == ipa)
                {
                    ret = x.Type;
                }
            }

            return ret;
        }

        public void arp_update(int ipa, byte [] mac, int stat)
        {

            var pArpEntry = Marshal.AllocHGlobal(Marshal.SizeOf(typeof(MIB_IPNETROW)));
            var index = ((CbxObjectItem)cbxNic.SelectedItem).index;
            var ip = new IPAddress((long)ipa);
            //var mac = new byte[] { 0x01, 0x02, 0x03, 0x04, 0x05, 0x06 };//MACアドレス 

            var p = pArpEntry;//作業用ポインタ

            Marshal.WriteInt32(p, index);//Index
            p = IntPtr.Add(p, 4);

            Marshal.WriteInt32(p, 6);//PhysAddrLen
            p = IntPtr.Add(p, 4);

            Marshal.Copy(mac, 0, p, 6);//PhysAddr
            p = IntPtr.Add(p, 8);

            Marshal.WriteInt32(p, (int)ipa);//Addr
            p = IntPtr.Add(p, 4);

            Marshal.WriteInt32(p, stat);//Type
            p = IntPtr.Add(p, 4);

            var ret = CreateIpNetEntry(pArpEntry);
            if (ret == 0)
            {
                Console.WriteLine("Success");
            }
            else
            {
                Console.WriteLine(string.Format("ERROR 0x{0:x}", ret));
            }

            Marshal.FreeHGlobal(pArpEntry);
        }

        public void arp_delete(int ipa, byte[] mac, int stat)
        {
            int ret = -1;
            List<MIB_IPNETROW> ar = arp_get();

            /* デバイスのインデックス値から対象のarpテーブルを求める */
            int did = ((CbxObjectItem)cbxNic.SelectedItem).index;
            foreach (var x in ar.Where(x => x.Index == did))
            {
                Console.WriteLine("{0}  {1:x}  {2}  {3}",
                    x.Index, x.Addr, x.PhysAddr, x.Type);

                if (x.Addr == ipa)
                {
                    IntPtr x_ptr = Marshal.AllocCoTaskMem(Marshal.SizeOf(x));
                    Marshal.StructureToPtr(x, x_ptr, false);
                    ret =DeleteIpNetEntry(x_ptr);
                    Marshal.FreeCoTaskMem(x_ptr);
                    break;
                }
            }

            if (ret == 0)
            {
                Console.WriteLine("Success");
            }
            else
            {
                Console.WriteLine(string.Format("ERROR 0x{0:x}", ret));
            }
        }

        unsafe private void btnSend_Click(object sender, EventArgs e)
        {
            int inv_ms = Convert.ToInt32(txtInv.Text);
            byte[] byt;
            int n = 0;

            byte[] fbyt = new byte[1518];

            if (chkUseWS.Checked)
            {
                IPAddress ipa = ((CbxObjectItem)cbxNic.SelectedItem).ipv4.Addr.ipAddress;

                Array.Resize(ref fbyt, 65535 - (IP_HDR_SIZE + UDP_HDR_SIZE));

                //UdpClientオブジェクトを作成する
                System.Net.Sockets.UdpClient udp =
                    new System.Net.Sockets.UdpClient(
                        new IPEndPoint(ipa, Convert.ToInt16(txtPortL.Text)));

                int calc_sec = Convert.ToInt32(txtInv.Text);
                int mbps_conv = (cbxUnit.SelectedIndex == 0) ? MBPS_CONV1 : MBPS_CONV2;
                ulong rw_old;
                total_cnt = rw_cnt = rw_old = 0;
                rw_bytes = new ulong[calc_sec + 3]; /* 初回と最後尾のバッファは作業用 */
                tmMain = new System.Timers.Timer();
                tmMain.Elapsed += new ElapsedEventHandler(tmMain_Elapsed);
                tmMain.Interval = 1000;

                TaskFactory tf = new TaskFactory();
                CancellationTokenSource cts = new CancellationTokenSource();

                long dstip = IPAddress.Parse(txtIPaR.Text).Address;
                udp.Connect(new IPAddress(dstip), Convert.ToInt32(txtPortR.Text));

                /* ARPキャッシュに静的に登録されてなければ登録する */
                int arp_ret = arp_check((int)dstip);
                if (arp_ret != (int)MIB_IPNET_TYPE.STATIC)
                {
                    byt = str2hexs(txtMacR.Text, '-');
                    arp_update((int)dstip, byt, (int)MIB_IPNET_TYPE.STATIC);
                }


                Task tsk = tf.StartNew(() =>
                {
                    for (; ; )
                    {
                        rw_bytes[rw_cnt] += (ulong)udp.Send(fbyt, fbyt.Length);
                        cts.Token.ThrowIfCancellationRequested();   /* キャンセル発行確認 */
                    }

                }, cts.Token);

                txtResS.Clear();

                /* 初回の測定値は捨てる */
                tmMain.Start();
                while (rw_cnt == rw_old)
                {
                    System.Threading.Thread.Sleep(100);
                    System.Windows.Forms.Application.DoEvents();
                }

                /* 2回目以降から測定を開始 (配列の最後尾1つ手前で終了) */
                rw_old = rw_cnt;
                do
                {
                    if (rw_cnt != rw_old)
                    {
                        rw_old = rw_cnt;
                        txtResS.AppendText(string.Format("{0:f3} Mbps/sec" + Environment.NewLine,
                            rw_bytes[rw_old - 1] / (double)mbps_conv));
                        txtResS.Refresh();
                    }
                    System.Threading.Thread.Sleep(100);
                    System.Windows.Forms.Application.DoEvents();
                } while (rw_cnt <= (ulong)calc_sec + 1);
                tmMain.Stop();

                try
                {
                    cts.Cancel();       /* 送信終了 */
                }
                catch (AggregateException)
                {
                }
                finally
                {
                    //UdpClientを閉じる
                    udp.Close();

                    /* arpキャッシュから削除する */
                    //if (arp_ret != (int)MIB_IPNET_TYPE.STATIC)
                    {
                        byt = str2hexs(txtMacR.Text, '-');
                        arp_delete((int)dstip, byt, (int)MIB_IPNET_TYPE.STATIC);
                    }

                }


            }
            else
            {
                short framesize = Convert.ToInt16(txtFrasize.Text);

                /*---- Ethernet ヘッダー ---------------------------------*/
                /* ローカルMACアドレス */
                byt = str2hexs(txtMacR.Text, '-');
                Array.Copy(byt, 0, fbyt, n, MAC_ADDR_SIZE);
                n += MAC_ADDR_SIZE;

                /* リモートMACアドレス */
                byt = str2hexs(txtMacL.Text, '-');
                Array.Copy(byt, 0, fbyt, n, MAC_ADDR_SIZE);
                n += MAC_ADDR_SIZE;

                fbyt[n++] = 0x08;
                fbyt[n++] = 0x00;


                /*---- IP ヘッダー ---------------------------------------*/
                fbyt[n++] = 0x45;
                fbyt[n++] = 0x00;
                fixed (byte* wk = &fbyt[n])     /* Length */
                {
                    framesize -= ETH_HDR_SIZE;
                    *(short*)wk = IPAddress.HostToNetworkOrder((short)framesize);
                }
                n += sizeof(short);
                fixed (byte* wk = &fbyt[n])     /* Identification */
                {
                    *(short*)wk = IPAddress.HostToNetworkOrder((short)1);
                }
                n += sizeof(short);
                fbyt[n++] = 0x00;               /* Flags */
                fbyt[n++] = 0x00;               /* Fragment offset */
                fbyt[n++] = 0x40;               /* TTL */
                fbyt[n++] = 0x11;               /* Protocol (UDP) */
                fbyt[n++] = 0x00;               /* IP checksum */
                fbyt[n++] = 0x00;

                /* ローカルIPアドレス */
                byt = str2hexs(txtIPaL.Text, '.', true);
                Array.Copy(byt, 0, fbyt, n, IP_ADDR_SIZE);
                n += IP_ADDR_SIZE;

                /* リモートIPアドレス */
                byt = str2hexs(txtIPaR.Text, '.', true);
                Array.Copy(byt, 0, fbyt, n, IP_ADDR_SIZE);
                n += IP_ADDR_SIZE;

                /*---- UDP ヘッダー --------------------------------------*/
                fixed (byte* wk = &fbyt[n])     /* Source Port */
                {
                    *(short*)wk = IPAddress.HostToNetworkOrder((short)Convert.ToUInt16(txtPortL.Text));
                }
                n += sizeof(short);

                fixed (byte* wk = &fbyt[n])     /* Destination Port */
                {
                    *(short*)wk = IPAddress.HostToNetworkOrder((short)Convert.ToUInt16(txtPortR.Text));
                }
                n += sizeof(short);

                fixed (byte* wk = &fbyt[n])     /* Length */
                {
                    framesize -= IP_HDR_SIZE;
                    *(short*)wk = IPAddress.HostToNetworkOrder((short)framesize);
                }
                n += sizeof(short);

                fbyt[n++] = 0x00;               /* UDP checksum */
                fbyt[n++] = 0x00;

                /* Payload */
                framesize -= UDP_HDR_SIZE;
                n += framesize;

                Array.Resize(ref fbyt, n);


                /* Update checksum */
                update_ip_csum(fbyt);
                update_udp_csum(fbyt);

                if (cbxCalc.SelectedIndex == 1)
                {   /* Payload */
                    n -= CALC_DIFF_SIZE2;           /* ヘッダ分を引く */
                }

                /*--------------------------------------------------------*/

                int calc_sec = Convert.ToInt32(txtInv.Text);
                int mbps_conv = (cbxUnit.SelectedIndex == 0) ? MBPS_CONV1 : MBPS_CONV2;
                ulong rw_old;
                total_cnt = rw_cnt = rw_old = 0;
                rw_bytes = new ulong[calc_sec + 3]; /* 初回と最後尾のバッファは作業用 */
                tmMain = new System.Timers.Timer();
                tmMain.Elapsed += new ElapsedEventHandler(tmMain_Elapsed);
                tmMain.Interval = 1000;

                TaskFactory tf = new TaskFactory();
                CancellationTokenSource cts = new CancellationTokenSource();

                Debug.WriteLine("SelectedIndex : " + cbxNic.SelectedIndex);
                /* SharpPcapデバイスオープン */
                LibPcapLiveDevice dev = LibPcapLiveDeviceList.Instance[cbxNic.SelectedIndex];
                dev.Open();

                Task tsk = tf.StartNew(() =>
                {
                    for (; ; )
                    {
                        dev.SendPacket(fbyt, fbyt.Length);
                        rw_bytes[rw_cnt] += (ulong)n;
                        cts.Token.ThrowIfCancellationRequested();   /* キャンセル発行確認 */
                    }

                }, cts.Token);

                txtResS.Clear();

                /* 初回の測定値は捨てる */
                tmMain.Start();
                while (rw_cnt == rw_old)
                {
                    System.Threading.Thread.Sleep(100);
                    System.Windows.Forms.Application.DoEvents();
                }

                /* 2回目以降から測定を開始 (配列の最後尾1つ手前で終了) */
                rw_old = rw_cnt;
                do
                {
                    if (rw_cnt != rw_old)
                    {
                        rw_old = rw_cnt;
                        txtResS.AppendText(string.Format("{0:f3} Mbps/sec" + Environment.NewLine,
                            rw_bytes[rw_old - 1] / (double)mbps_conv));
                        txtResS.Refresh();
                    }
                    System.Threading.Thread.Sleep(100);
                    System.Windows.Forms.Application.DoEvents();
                } while (rw_cnt <= (ulong)calc_sec + 1);
                tmMain.Stop();

                try
                {
                    cts.Cancel();       /* 送信終了 */
                }
                catch (AggregateException)
                {
                }
                finally
                {
                    dev.Close();
                }
            }



#if true

#else

            ///* SharpPcapデバイスオープン */
            //Debug.WriteLine("SelectedIndex : " + cbxNic.SelectedIndex);
            //LibPcapLiveDevice dev = LibPcapLiveDeviceList.Instance[cbxNic.SelectedIndex];
            //dev.Open();
            //dev.NonBlockingMode = true;

            //for (int x = 0; x < inv_ms; x++)
            //{
            //    dev.SendPacket(fbyt, fbyt.Length);
            //}

            //dev.Close();

            Array.Resize(ref fbyt, 65500);


            //UdpClientオブジェクトを作成する
            System.Net.Sockets.UdpClient udp =
                new System.Net.Sockets.UdpClient(new IPEndPoint(IPAddress.Parse("172.16.0.100"), 60000));

            udp.Connect(IPAddress.Parse(txtIPaR.Text), Convert.ToInt32(txtPortR.Text));

            for (int x = 0; x < inv_ms; x++)
            {
                udp.Send(fbyt, fbyt.Length);

            }

            //UdpClientを閉じる
            udp.Close();

#endif
        }

        private void frmMain_Load(object sender, EventArgs e)
        {
            btnReload.PerformClick();

            cbxUnit.SelectedIndex = 0;
            cbxCalc.SelectedIndex = 0;

            nudPayload_ValueChanged(nudPayload, new EventArgs());
        }

        private void nudPayload_ValueChanged(object sender, EventArgs e)
        {
            short framesize;

            framesize = Convert.ToInt16(nudPayload.Value);
            framesize += ETH_HDR_SIZE;
            framesize += IP_HDR_SIZE;
            framesize += UDP_HDR_SIZE;

            txtFrasize.Text = framesize.ToString();

        }

        static ulong[] rw_bytes;
        static ulong rw_cnt;
        static ulong total_cnt;
        static bool bEnd;
        static int diff_size;
        static DateTime dtStart, dtEnd;

        // イベントハンドラ
        private void OnPacketArrival(object sender, CaptureEventArgs e)
        {
            /* 秒あたりの受信バイト数 */
            rw_bytes[rw_cnt] += (ulong)(e.Packet.Data.Length - diff_size);

            /* 送受信バイト数 */
            total_cnt += (ulong)e.Packet.Data.Length;

            /* パケット取得時間の記録 */
            if (bEnd)
            {
                /* 終了時間 */
                dtEnd = e.Packet.Timeval.Date;
            }
            else
            {
                /* 開始時間 */
                dtStart = e.Packet.Timeval.Date;
                bEnd = true;
            }
        }

        void tmMain_Elapsed(object sender, ElapsedEventArgs e)
        {
            rw_cnt++;
        }

        private void btnRecv_Click(object sender, EventArgs e)
        {
            // 参考: http://d.hatena.ne.jp/machi_pon/20100131/1264901795

            int calc_sec = Convert.ToInt32(txtInv.Text);
            ulong rw_old;
            string sFilter;
            int mbps_conv = (cbxUnit.SelectedIndex == 0) ? MBPS_CONV1 : MBPS_CONV2;

            diff_size = (cbxCalc.SelectedIndex == 0) ? CALC_DIFF_SIZE1 : CALC_DIFF_SIZE2;

            txtResR.Clear();

            /* SharpPcapデバイスオープン */
            Debug.WriteLine("SelectedIndex : " + cbxNic.SelectedIndex);
            LibPcapLiveDevice dev = LibPcapLiveDeviceList.Instance[cbxNic.SelectedIndex];
            dev.OnPacketArrival += OnPacketArrival;
            dev.Open(DeviceMode.Promiscuous);

            /* フィルタ設定 */
            // 参考: https://www.winpcap.org/docs/docs_40_2/html/group__language.html
            sFilter = "";
            if (chkMacL.Checked)
            {
                if (sFilter != "")  sFilter += " and ";
                sFilter += "ether dst " + txtMacL.Text;
            }
            if (chkIPaL.Checked)
            {
                if (sFilter != "")  sFilter += " and ";
                sFilter += "dst host " + txtIPaL.Text;
            }
            if (chkPortL.Checked)
            {
                if (sFilter != "")  sFilter += " and ";
                sFilter += "udp dst port " + txtPortL.Text;
            }
            dev.Filter = sFilter;

            bEnd = false;
            total_cnt = rw_cnt = rw_old = 0;
            rw_bytes = new ulong[calc_sec + 3]; /* 初回と最後尾のバッファは作業用 */
            tmMain = new System.Timers.Timer();
            tmMain.Elapsed += new ElapsedEventHandler(tmMain_Elapsed);
            tmMain.Interval = 1000;
            tmMain.Start();

            dev.StartCapture();

            /* 初回の測定値は捨てる */
            while (rw_cnt == rw_old)
            {
                System.Threading.Thread.Sleep(100);
                System.Windows.Forms.Application.DoEvents();
            }

            /* 2回目以降から測定を開始 (配列の最後尾1つ手前で終了) */
            rw_old = rw_cnt;
            do
            {
                if (rw_cnt != rw_old)
                {
                    rw_old = rw_cnt;
                    txtResR.AppendText(string.Format("{0:f3} Mbps/sec" + Environment.NewLine,
                        rw_bytes[rw_old - 1] / (double)mbps_conv));
                    txtResR.Refresh();
                }
                System.Threading.Thread.Sleep(100);
                System.Windows.Forms.Application.DoEvents();
            } while (rw_cnt <= (ulong)calc_sec + 1);

            dev.StopCapture();
            tmMain.Stop();

            //Debug.WriteLine(dev.Statistics.ToString());

            TimeSpan ts = dtEnd - dtStart;
            txtResR.Text += string.Format(@"
---- Summary ----
  recv-pkts  : {0}
  drop-pkts  : {1}
  totalbytes : {2}
  recv-begin : {3}
  recv-end   : {4}
  begin-end  : {5:f3} secs
  calc speed : {6:f3} Mbps/sec
", dev.Statistics.ReceivedPackets, dev.Statistics.DroppedPackets,
 total_cnt, dtStart, dtEnd, ts.TotalSeconds, (total_cnt / (double)mbps_conv / ts.TotalSeconds)
 );


            dev.Close();
        }

        private void btnIFset_Click(object sender, EventArgs e)
        {
            /* SharpPcapデバイスオープン */
            Debug.WriteLine("SelectedIndex : " + cbxNic.SelectedIndex);
            CbxObjectItem oi = (CbxObjectItem)cbxNic.SelectedItem;

            
            txtMacL.Text = oi.dev.Interface.MacAddress.ToString();
            txtIPaL.Text = oi.ipv4.Addr.ToString();
        }

        private void btnReload_Click(object sender, EventArgs e)
        {

            NetworkInterface[] nicList = NetworkInterface.GetAllNetworkInterfaces();

            cbxNic.Items.Clear();
            for (int i = 0; i < LibPcapLiveDeviceList.Instance.Count; i++)
            {
                LibPcapLiveDevice dev = LibPcapLiveDeviceList.Instance[i];

                /* NICデバイスのインデックスを取得 */
                int did = 0;
                foreach (var ni in nicList.Where
                  (x => dev.Interface.MacAddress.GetAddressBytes().SequenceEqual(
                  x.GetPhysicalAddress().GetAddressBytes()))){
                      did = ni.GetIPProperties().GetIPv4Properties().Index;
                }

                PcapAddress ipv4 = null;
                foreach (PcapAddress addr in dev.Interface.Addresses)
                {
                    ipv4 = addr;
                    if (addr.Addr.sa_family == 2)  /* IPv4*/
                    {
                        break;
                    }
                }

                int cidr = (ipv4.Addr.ipAddress.Address == 0) ? 0 : 32;
                for (; cidr != 0; cidr--)
                {
                    if (0 != (ipv4.Netmask.ipAddress.Address & (1 << (cidr - 1))))
                    {
                        break;
                    }
                }

                string sItem = string.Format("{0:x2}: {1} - {2}/{3} - {5} ({4})",
                  did, dev.Interface.MacAddress, ipv4.Addr, cidr, dev.Interface.Description, dev.Interface.FriendlyName);
                cbxNic.Items.Add(new CbxObjectItem(sItem, did, dev, ipv4));
            }
            cbxNic.SelectedIndex = 0;


            /* デバイス列挙 */
            foreach (LibPcapLiveDevice dev in LibPcapLiveDeviceList.Instance)
            {
                Debug.WriteLine("FriendlyName : " + dev.Interface.FriendlyName);
                Debug.WriteLine("Name         : " + dev.Interface.Name);
                Debug.WriteLine("MacAddress   : " + dev.Interface.MacAddress);
                Debug.WriteLine("Flags        : " + dev.Interface.Flags);

                foreach (PcapAddress addr in dev.Interface.Addresses)
                {
                    Debug.WriteLine("Addr-family  :" + addr.Addr.sa_family.ToString());
                    Debug.WriteLine("Addr         :" + addr.Addr);
                    Debug.WriteLine("Netmask      :" + addr.Netmask);
                    Debug.WriteLine("Broadaddr    :" + addr.Broadaddr);
                    Debug.WriteLine("Dstaddr      :" + addr.Dstaddr);
                }

                Debug.WriteLine("Description  : " + dev.Interface.Description);
                Debug.WriteLine("");
            }
        }

        private void chkUseWS_CheckedChanged(object sender, EventArgs e)
        {
            bool bEna = !chkUseWS.Checked;

            txtMacL.Enabled = bEna;
            txtIPaL.Enabled = bEna;
            nudPayload.Enabled = bEna;

            chkUpdateARP.Enabled = !bEna;
            txtMacR.Enabled = bEna || (chkUpdateARP.Enabled && chkUpdateARP.Checked);
        }

    }

    public class CbxObjectItem
    {
        public string text;
        public int index;
        public LibPcapLiveDevice dev;
        public PcapAddress ipv4;

        public CbxObjectItem(string text, int index, LibPcapLiveDevice dev, PcapAddress ipv4)
        {
            this.text = text;
            this.index = index;
            this.dev = dev;
            this.ipv4 = ipv4;
        }

        public override string ToString()
        {
            return text;
        }
    }
}
