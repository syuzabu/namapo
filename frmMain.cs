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

namespace namapo
{

    /* IPアドレス用共用体 */
    [StructLayout(LayoutKind.Explicit, Size = 4)]
    struct T_IPA
    {
        [FieldOffset(0)]
        public uint dat;

        [FieldOffset(0)]  public byte b0;
        [FieldOffset(1)]  public byte b1;
        [FieldOffset(2)]  public byte b2;
        [FieldOffset(3)]  public byte b3;
    }

    /* MACアドレス用共用体 */
    [StructLayout(LayoutKind.Explicit, Size = 6)]
    struct T_MAC
    {
        [FieldOffset(0)]
        public uint dat1;
        [FieldOffset(4)]
        public ushort dat2;

        [FieldOffset(0)]  public byte b0;
        [FieldOffset(1)]  public byte b1;
        [FieldOffset(2)]  public byte b2;
        [FieldOffset(3)]  public byte b3;
        [FieldOffset(4)]  public byte b4;
        [FieldOffset(5)]  public byte b5;
    }


  public partial class frmMain : Form
  {
      public const short ETH_HDR_SIZE = 14;
      public const short IP_HDR_SIZE = 20;
      public const short UDP_HDR_SIZE = 8;

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
        if (dec) {
            foreach (string sDat in sHex)
            {
              bHex[n++] = ("" != sDat) ? Convert.ToByte(sDat, 10) : (byte)0x00;
            }
        }
        else {
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
        csum += (ulong)((data[n] << 8) + data[n + 1]);  n += 2;
        csum += (ulong)((data[n] << 8) + data[n + 1]);  n += 2;
        /* dst addr */
        csum += (ulong)((data[n] << 8) + data[n + 1]);  n += 2;
        csum += (ulong)((data[n] << 8) + data[n + 1]);  n += 2;
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


    unsafe private void btnSend_Click(object sender, EventArgs e)
    {
        int inv_ms = Convert.ToInt32(txtInv.Text);
        short framesize = Convert.ToInt16(txtFrasize.Text);
        byte[] byt;

        byte[] fbyt = new byte[1518];
        int n = 0;

        /*---- Ethernet ヘッダー ---------------------------------*/
        /* ローカルMACアドレス */
        byt = str2hexs(txtMacR.Text, '-');
        fixed (byte* wk = &fbyt[n])
        {
            T_MAC* pmac = (T_MAC*)wk;
            pmac->b0 = byt[0];
            pmac->b1 = byt[1];
            pmac->b2 = byt[2];
            pmac->b3 = byt[3];
            pmac->b4 = byt[4];
            pmac->b5 = byt[5];
        }
        n += sizeof(T_MAC);

        /* リモートMACアドレス */
        byt = str2hexs(txtMacL.Text, '-');
        fixed (byte* wk = &fbyt[n])
        {
            T_MAC* pmac = (T_MAC*)wk;
            pmac->b0 = byt[0];
            pmac->b1 = byt[1];
            pmac->b2 = byt[2];
            pmac->b3 = byt[3];
            pmac->b4 = byt[4];
            pmac->b5 = byt[5];
        }
        n += sizeof(T_MAC);

        fbyt[n++] = 0x08;
        fbyt[n++] = 0x00;


        /*---- IP ヘッダー ---------------------------------------*/
        fbyt[n++] = 0x45;
        fbyt[n++] = 0x00;
        fixed (byte* wk = &fbyt[n])     /* Length */
        {
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
        fixed (byte* wk = &fbyt[n])
        {
            T_IPA* pipa = (T_IPA*)wk;
            pipa->b0 = byt[0];
            pipa->b1 = byt[1];
            pipa->b2 = byt[2];
            pipa->b3 = byt[3];
        }
        n += sizeof(T_IPA);

        /* リモートIPアドレス */
        byt = str2hexs(txtIPaR.Text, '.', true);
        fixed (byte* wk = &fbyt[n])
        {
            T_IPA* pipa = (T_IPA*)wk;
            pipa->b0 = byt[0];
            pipa->b1 = byt[1];
            pipa->b2 = byt[2];
            pipa->b3 = byt[3];
        }
        n += sizeof(T_IPA);

        /*---- UDP ヘッダー --------------------------------------*/
        fixed (byte* wk = &fbyt[n])     /* Source Port */
        {
            *(short*)wk = IPAddress.HostToNetworkOrder(Convert.ToInt16(txtPortL.Text));
        }
        n += sizeof(short);

        fixed (byte* wk = &fbyt[n])     /* Destination Port */
        {
            *(short*)wk = IPAddress.HostToNetworkOrder(Convert.ToInt16(txtPortR.Text));
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

        /*--------------------------------------------------------*/

        /* SharpPcapデバイスオープン */
        Debug.WriteLine("SelectedIndex : " + cbxNic.SelectedIndex);
        LibPcapLiveDevice dev = LibPcapLiveDeviceList.Instance[cbxNic.SelectedIndex];
        dev.Open();

        for (int x = 0; x < inv_ms; x++)
        {
            dev.SendPacket(fbyt);
            System.Threading.Thread.Sleep(1);
        }
        
        dev.Close();
    }

    private void frmMain_Load(object sender, EventArgs e)
    {
      for (int i = 0; i < LibPcapLiveDeviceList.Instance.Count; i++)
      {
        LibPcapLiveDevice dev = LibPcapLiveDeviceList.Instance[i];

        cbxNic.Items.Add(string.Format("{0}: {1} ({2}) - {3}",
          i, dev.Interface.MacAddress, dev.Interface.Description, dev.Interface.FriendlyName));
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
          Debug.WriteLine("Addr         :" + addr.Addr);
          Debug.WriteLine("Netmask      :" + addr.Netmask);
          Debug.WriteLine("Broadaddr    :" + addr.Broadaddr);
          Debug.WriteLine("Dstaddr      :" + addr.Dstaddr);
        }

        Debug.WriteLine("Description  : " + dev.Interface.Description);
        Debug.WriteLine("");
      }
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

    // イベントハンドラ
    private static void OnPacketArrival(object sender, CaptureEventArgs e)
    {
        DateTime time = e.Packet.Timeval.Date;
        int len = e.Packet.Data.Length;
        Debug.WriteLine(String.Format("{0}:{1}:{2},{3} Len={4}",
                                        time.Hour, time.Minute, time.Second, time.Millisecond, len));
        Debug.WriteLine(e.Packet.ToString());
    }

    private void btnRecv_Click(object sender, EventArgs e)
    {
        // 参考: http://d.hatena.ne.jp/machi_pon/20100131/1264901795

        /* SharpPcapデバイスオープン */
        Debug.WriteLine("SelectedIndex : " + cbxNic.SelectedIndex);
        LibPcapLiveDevice dev = LibPcapLiveDeviceList.Instance[cbxNic.SelectedIndex];

        dev.OnPacketArrival += OnPacketArrival;

        //dev.Filter = "ip.addr==" + txtIPaR.Text;
        //dev.Filter += " and udp.srcport==" + txtPortR.Text;
        dev.Open();

        dev.StartCapture();
        System.Threading.Thread.Sleep(Convert.ToInt32(txtInv.Text) * 1000);
        dev.StopCapture();

        Debug.WriteLine(dev.Statistics.ToString());

        dev.Close();
    }
  }
}
