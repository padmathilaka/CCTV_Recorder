using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace NVRRecordingSystem
{
    public partial class frmCameraDiscovery : Form
    {
        // UI controls (created in code so no Designer changes are needed)
        private Button _btnScan;
        private Button _btnCopyJson;
        private Button _btnClose;
        private Label _lblStatus;
        private DataGridView _grdResults;
        private ProgressBar _progress;

        private readonly BindingList<DiscoveryRow> _rows = new BindingList<DiscoveryRow>();
        private CancellationTokenSource _cts;

        public frmCameraDiscovery()
        {
            InitializeComponent();
            BuildUi();
        }

        public void frmCameraDiscovery_Load(object sender, EventArgs e)
        {

        }

        // ============================================================
        // Grid row model
        // ============================================================
        private sealed class DiscoveryRow
        {
            public string IpAddress { get; set; }
            public string Rtsp554 { get; set; }      // "Open" / ""
            public string Onvif { get; set; }        // "Yes" / ""
            public string Vendor { get; set; }
            public string Model { get; set; }
            public string OnvifXAddr { get; set; }
        }

        // ============================================================
        // UI construction
        // ============================================================
        private void BuildUi()
        {
            Text = "Camera Discovery";
            StartPosition = FormStartPosition.CenterParent;
            MinimumSize = new Size(760, 420);
            Size = new Size(860, 500);

            _btnScan = new Button
            {
                Text = "Scan Network",
                Location = new Point(12, 12),
                Size = new Size(120, 30)
            };
            _btnScan.Click += BtnScan_Click;

            _btnCopyJson = new Button
            {
                Text = "Copy as JSON",
                Location = new Point(140, 12),
                Size = new Size(120, 30),
                Enabled = false
            };
            _btnCopyJson.Click += BtnCopyJson_Click;

            _btnClose = new Button
            {
                Text = "Close",
                Size = new Size(90, 30),
                Anchor = AnchorStyles.Top | AnchorStyles.Right
            };
            _btnClose.Location = new Point(ClientSize.Width - _btnClose.Width - 12, 12);
            _btnClose.Click += (s, e) => Close();

            _lblStatus = new Label
            {
                Text = "Ready. Click 'Scan Network' to search for cameras (ONVIF + RTSP port 554).",
                Location = new Point(12, 50),
                AutoSize = true
            };

            _progress = new ProgressBar
            {
                Location = new Point(12, 70),
                Size = new Size(ClientSize.Width - 24, 8),
                Style = ProgressBarStyle.Marquee,
                MarqueeAnimationSpeed = 0,
                Visible = false,
                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right
            };

            _grdResults = new DataGridView
            {
                Location = new Point(12, 86),
                Size = new Size(ClientSize.Width - 24, ClientSize.Height - 98),
                Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right,
                ReadOnly = true,
                AllowUserToAddRows = false,
                AllowUserToDeleteRows = false,
                AutoGenerateColumns = true,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                RowHeadersVisible = false,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill
            };
            _grdResults.DataSource = _rows;

            Controls.Add(_btnScan);
            Controls.Add(_btnCopyJson);
            Controls.Add(_btnClose);
            Controls.Add(_lblStatus);
            Controls.Add(_progress);
            Controls.Add(_grdResults);

            FormClosing += (s, e) => { try { _cts?.Cancel(); } catch { } };

            Shown += (s, e) => SetHeaders();
        }

        private void SetHeaders()
        {
            SetHeader(nameof(DiscoveryRow.IpAddress), "IP Address");
            SetHeader(nameof(DiscoveryRow.Rtsp554), "RTSP 554");
            SetHeader(nameof(DiscoveryRow.Onvif), "ONVIF");
            SetHeader(nameof(DiscoveryRow.Vendor), "Vendor");
            SetHeader(nameof(DiscoveryRow.Model), "Model");
            SetHeader(nameof(DiscoveryRow.OnvifXAddr), "ONVIF Service URL");
        }

        private void SetHeader(string colName, string header)
        {
            if (_grdResults.Columns.Contains(colName))
                _grdResults.Columns[colName].HeaderText = header;
        }

        // ============================================================
        // Scan button
        // ============================================================
        private async void BtnScan_Click(object sender, EventArgs e)
        {
            _btnScan.Enabled = false;
            _btnCopyJson.Enabled = false;
            _rows.Clear();

            _progress.Visible = true;
            _progress.MarqueeAnimationSpeed = 30;
            _lblStatus.Text = "Scanning... (ONVIF multicast probe + TCP scan of port 554 on local /24 subnets)";

            _cts = new CancellationTokenSource();

            try
            {
                var discovery = new CameraDiscovery();
                var found = await discovery.DiscoverAsync(
                    onvifTimeoutMs: 4000,
                    rtspConnectTimeoutMs: 400,
                    ct: _cts.Token);

                foreach (var cam in found)
                {
                    _rows.Add(new DiscoveryRow
                    {
                        IpAddress = cam.IpAddress,
                        Rtsp554 = cam.RtspPortOpen ? "Open" : "",
                        Onvif = cam.FoundViaOnvif ? "Yes" : "",
                        Vendor = cam.Vendor ?? "",
                        Model = cam.Model ?? "",
                        OnvifXAddr = cam.OnvifXAddr ?? ""
                    });
                }

                _lblStatus.Text = found.Count == 0
                    ? "Scan complete. No devices found (check firewall / VLAN / subnet)."
                    : $"Scan complete. Found {found.Count} device(s) with ONVIF and/or RTSP port 554 open.";

                _btnCopyJson.Enabled = found.Count > 0;
            }
            catch (OperationCanceledException)
            {
                _lblStatus.Text = "Scan cancelled.";
            }
            catch (Exception ex)
            {
                _lblStatus.Text = "Scan failed: " + ex.Message;
                MessageBox.Show(ex.Message, "Discovery Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                _progress.MarqueeAnimationSpeed = 0;
                _progress.Visible = false;
                _btnScan.Enabled = true;
            }
        }

        // ============================================================
        // Copy results as nvrsettings.json camera entries
        // ============================================================
        private void BtnCopyJson_Click(object sender, EventArgs e)
        {
            try
            {
                var sb = new StringBuilder();
                int i = 1;

                foreach (var row in _rows)
                {
                    // Guess the RTSP path from vendor (edit after pasting if wrong)
                    bool looksDahua = (row.Vendor ?? "").IndexOf("dahua", StringComparison.OrdinalIgnoreCase) >= 0;

                    string path = looksDahua ? "/cam/realmonitor" : "/Streaming/Channels/101";
                    string query = looksDahua ? "channel=1&subtype=0&unicast=true&proto=Onvif" : "";

                    sb.AppendLine("    {");
                    sb.AppendLine($"      \"Name\": \"NewCamera{i}\",");
                    sb.AppendLine($"      \"IpAddress\": \"{row.IpAddress}\",");
                    sb.AppendLine($"      \"FolderName\": \"NewCamera{i}\",");
                    sb.AppendLine($"      \"Tag\": \"NewCamera{i}\",");
                    sb.AppendLine($"      \"RtspHost\": \"{row.IpAddress}\",");
                    sb.AppendLine("      \"RtspPort\": 554,");
                    sb.AppendLine($"      \"RtspPath\": \"{path}\",");
                    sb.AppendLine($"      \"RtspQuery\": \"{query}\",");
                    sb.AppendLine("      \"Username\": \"admin\",");
                    sb.AppendLine("      \"Password\": \"CHANGE_ME\",");
                    sb.AppendLine("      \"RtspUrl\": \"\"");
                    sb.AppendLine("    },");
                    i++;
                }

                if (sb.Length == 0)
                {
                    MessageBox.Show("Nothing to copy.", "Copy as JSON",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                Clipboard.SetText(sb.ToString());
                MessageBox.Show(
                    "Camera entries copied to clipboard.\n\n" +
                    "Paste them into the \"Cameras\" array of nvrsettings.json,\n" +
                    "then fix Name, Password, and RtspPath/RtspQuery as needed.\n" +
                    "Remember: no comma after the LAST entry in the array.",
                    "Copy as JSON",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Copy Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // ============================================================
        // ============================================================
        //  Embedded discovery engine (ONVIF WS-Discovery + port scan)
        // ============================================================
        // ============================================================
        private sealed class CameraDiscovery
        {
            private const string WsdMulticastAddress = "239.255.255.250";
            private const int WsdMulticastPort = 3702;
            private const int DefaultRtspPort = 554;

            public sealed class DiscoveredCamera
            {
                public string IpAddress { get; set; }
                public int RtspPort { get; set; } = DefaultRtspPort;
                public bool RtspPortOpen { get; set; }
                public bool FoundViaOnvif { get; set; }
                public string OnvifXAddr { get; set; }
                public string Vendor { get; set; }
                public string Model { get; set; }
                public string Name { get; set; }
            }

            public async Task<List<DiscoveredCamera>> DiscoverAsync(
                int onvifTimeoutMs = 4000,
                int rtspConnectTimeoutMs = 400,
                CancellationToken ct = default(CancellationToken))
            {
                var byIp = new Dictionary<string, DiscoveredCamera>(StringComparer.OrdinalIgnoreCase);

                var onvifTask = SafeProbeOnvifAsync(onvifTimeoutMs, ct);
                var portTask = SafeScanRtspPortAsync(rtspConnectTimeoutMs, ct);

                await Task.WhenAll(onvifTask, portTask).ConfigureAwait(false);

                foreach (var cam in onvifTask.Result)
                {
                    if (string.IsNullOrEmpty(cam.IpAddress)) continue;
                    if (!byIp.TryGetValue(cam.IpAddress, out var existing))
                    {
                        byIp[cam.IpAddress] = cam;
                    }
                    else
                    {
                        existing.FoundViaOnvif = true;
                        existing.OnvifXAddr = existing.OnvifXAddr ?? cam.OnvifXAddr;
                        existing.Vendor = existing.Vendor ?? cam.Vendor;
                        existing.Model = existing.Model ?? cam.Model;
                        existing.Name = existing.Name ?? cam.Name;
                    }
                }

                foreach (var ip in portTask.Result)
                {
                    if (!byIp.TryGetValue(ip, out var existing))
                    {
                        byIp[ip] = new DiscoveredCamera
                        {
                            IpAddress = ip,
                            RtspPort = DefaultRtspPort,
                            RtspPortOpen = true
                        };
                    }
                    else
                    {
                        existing.RtspPortOpen = true;
                    }
                }

                return byIp.Values.OrderBy(c => IpSortKey(c.IpAddress)).ToList();
            }

            // ---------------- ONVIF WS-Discovery ----------------
            private async Task<List<DiscoveredCamera>> SafeProbeOnvifAsync(int timeoutMs, CancellationToken ct)
            {
                try { return await ProbeOnvifAsync(timeoutMs, ct).ConfigureAwait(false); }
                catch { return new List<DiscoveredCamera>(); }
            }

            private async Task<List<DiscoveredCamera>> ProbeOnvifAsync(int timeoutMs, CancellationToken ct)
            {
                var results = new Dictionary<string, DiscoveredCamera>(StringComparer.OrdinalIgnoreCase);
                var multicastEp = new IPEndPoint(IPAddress.Parse(WsdMulticastAddress), WsdMulticastPort);

                var sockets = new List<UdpClient>();
                try
                {
                    foreach (var local in GetLocalIPv4Addresses())
                    {
                        UdpClient udp = null;
                        try
                        {
                            udp = new UdpClient(new IPEndPoint(local, 0));
                            udp.EnableBroadcast = true;
                            try { udp.JoinMulticastGroup(multicastEp.Address, local); } catch { }

                            byte[] probe = BuildProbeMessage();
                            udp.Send(probe, probe.Length, multicastEp);
                            sockets.Add(udp);
                        }
                        catch
                        {
                            try { udp?.Dispose(); } catch { }
                        }
                    }

                    var deadline = DateTime.UtcNow.AddMilliseconds(timeoutMs);
                    while (DateTime.UtcNow < deadline && !ct.IsCancellationRequested)
                    {
                        foreach (var udp in sockets)
                        {
                            try
                            {
                                if (udp.Available <= 0) continue;

                                var from = new IPEndPoint(IPAddress.Any, 0);
                                byte[] data = udp.Receive(ref from);
                                string xml = Encoding.UTF8.GetString(data);

                                var cam = ParseProbeMatch(xml, from.Address.ToString());
                                if (cam != null && !results.ContainsKey(cam.IpAddress))
                                    results[cam.IpAddress] = cam;
                            }
                            catch { }
                        }

                        await Task.Delay(50, ct).ConfigureAwait(false);
                    }
                }
                finally
                {
                    foreach (var s in sockets)
                    {
                        try { s.Dispose(); } catch { }
                    }
                }

                return results.Values.ToList();
            }

            private static byte[] BuildProbeMessage()
            {
                string messageId = "uuid:" + Guid.NewGuid().ToString();
                string soap =
                    "<?xml version=\"1.0\" encoding=\"UTF-8\"?>" +
                    "<e:Envelope xmlns:e=\"http://www.w3.org/2003/05/soap-envelope\" " +
                                "xmlns:w=\"http://schemas.xmlsoap.org/ws/2004/08/addressing\" " +
                                "xmlns:d=\"http://schemas.xmlsoap.org/ws/2005/04/discovery\" " +
                                "xmlns:dn=\"http://www.onvif.org/ver10/network/wsdl\">" +
                    "<e:Header>" +
                        "<w:MessageID>" + messageId + "</w:MessageID>" +
                        "<w:To e:mustUnderstand=\"true\">urn:schemas-xmlsoap-org:ws:2005:04:discovery</w:To>" +
                        "<w:Action e:mustUnderstand=\"true\">http://schemas.xmlsoap.org/ws/2005/04/discovery/Probe</w:Action>" +
                    "</e:Header>" +
                    "<e:Body>" +
                        "<d:Probe>" +
                            "<d:Types>dn:NetworkVideoTransmitter</d:Types>" +
                        "</d:Probe>" +
                    "</e:Body>" +
                    "</e:Envelope>";

                return Encoding.UTF8.GetBytes(soap);
            }

            private static DiscoveredCamera ParseProbeMatch(string xml, string fromIp)
            {
                if (string.IsNullOrEmpty(xml)) return null;
                if (xml.IndexOf("ProbeMatch", StringComparison.OrdinalIgnoreCase) < 0) return null;

                string xaddrs = ExtractBetween(xml, "XAddrs");
                string firstXaddr = null;
                string ip = fromIp;

                if (!string.IsNullOrWhiteSpace(xaddrs))
                {
                    firstXaddr = xaddrs.Split(new[] { ' ', '\t', '\r', '\n' },
                                              StringSplitOptions.RemoveEmptyEntries)
                                       .FirstOrDefault();
                    var parsedIp = TryGetHostFromUrl(firstXaddr);
                    if (!string.IsNullOrEmpty(parsedIp))
                        ip = parsedIp;
                }

                var cam = new DiscoveredCamera
                {
                    IpAddress = ip,
                    FoundViaOnvif = true,
                    OnvifXAddr = firstXaddr
                };

                string scopes = ExtractBetween(xml, "Scopes");
                if (!string.IsNullOrWhiteSpace(scopes))
                {
                    cam.Vendor = DecodeScope(FindScope(scopes, "/name/"))
                              ?? DecodeScope(FindScope(scopes, "/manufacturer/"));
                    cam.Model = DecodeScope(FindScope(scopes, "/hardware/"));
                    cam.Name = DecodeScope(FindScope(scopes, "/location/"));
                }

                return cam;
            }

            // ---------------- RTSP port 554 scan ----------------
            private async Task<List<string>> SafeScanRtspPortAsync(int connectTimeoutMs, CancellationToken ct)
            {
                try { return await ScanRtspPortAsync(connectTimeoutMs, ct).ConfigureAwait(false); }
                catch { return new List<string>(); }
            }

            private async Task<List<string>> ScanRtspPortAsync(int connectTimeoutMs, CancellationToken ct)
            {
                var hits = new List<string>();
                var targets = new List<string>();

                foreach (var local in GetLocalIPv4Addresses())
                {
                    byte[] b = local.GetAddressBytes();
                    if (b.Length != 4) continue;

                    string prefix = $"{b[0]}.{b[1]}.{b[2]}.";
                    for (int host = 1; host <= 254; host++)
                    {
                        string ip = prefix + host;
                        if (!targets.Contains(ip))
                            targets.Add(ip);
                    }
                }

                using (var throttle = new SemaphoreSlim(64))
                {
                    var tasks = targets.Select(async ip =>
                    {
                        await throttle.WaitAsync(ct).ConfigureAwait(false);
                        try
                        {
                            if (await IsPortOpenAsync(ip, DefaultRtspPort, connectTimeoutMs, ct).ConfigureAwait(false))
                            {
                                lock (hits) { hits.Add(ip); }
                            }
                        }
                        catch { }
                        finally { throttle.Release(); }
                    });

                    await Task.WhenAll(tasks).ConfigureAwait(false);
                }

                return hits;
            }

            private static async Task<bool> IsPortOpenAsync(string ip, int port, int timeoutMs, CancellationToken ct)
            {
                using (var client = new TcpClient())
                {
                    try
                    {
                        var connectTask = client.ConnectAsync(ip, port);
                        var timeoutTask = Task.Delay(timeoutMs, ct);
                        var completed = await Task.WhenAny(connectTask, timeoutTask).ConfigureAwait(false);

                        return completed == connectTask && client.Connected;
                    }
                    catch
                    {
                        return false;
                    }
                }
            }

            // ---------------- Helpers ----------------
            private static List<IPAddress> GetLocalIPv4Addresses()
            {
                var list = new List<IPAddress>();
                try
                {
                    foreach (var ni in NetworkInterface.GetAllNetworkInterfaces())
                    {
                        if (ni.OperationalStatus != OperationalStatus.Up) continue;
                        if (ni.NetworkInterfaceType == NetworkInterfaceType.Loopback) continue;

                        foreach (var ua in ni.GetIPProperties().UnicastAddresses)
                        {
                            if (ua.Address.AddressFamily == AddressFamily.InterNetwork &&
                                !IPAddress.IsLoopback(ua.Address))
                            {
                                list.Add(ua.Address);
                            }
                        }
                    }
                }
                catch { }

                return list.Distinct().ToList();
            }

            private static string ExtractBetween(string xml, string localName)
            {
                var m = Regex.Match(
                    xml,
                    @"<[^>]*?:?" + Regex.Escape(localName) + @"[^>]*?>(.*?)</[^>]*?:?" + Regex.Escape(localName) + ">",
                    RegexOptions.IgnoreCase | RegexOptions.Singleline);
                return m.Success ? m.Groups[1].Value.Trim() : null;
            }

            private static string TryGetHostFromUrl(string url)
            {
                if (string.IsNullOrWhiteSpace(url)) return null;
                try { return new Uri(url).Host; } catch { return null; }
            }

            private static string FindScope(string scopes, string marker)
            {
                if (string.IsNullOrWhiteSpace(scopes)) return null;
                foreach (var token in scopes.Split(new[] { ' ', '\t', '\r', '\n' },
                                                   StringSplitOptions.RemoveEmptyEntries))
                {
                    int idx = token.IndexOf(marker, StringComparison.OrdinalIgnoreCase);
                    if (idx >= 0)
                        return token.Substring(idx + marker.Length);
                }
                return null;
            }

            private static string DecodeScope(string value)
            {
                if (string.IsNullOrWhiteSpace(value)) return null;
                try { return Uri.UnescapeDataString(value); } catch { return value; }
            }

            private static long IpSortKey(string ip)
            {
                if (IPAddress.TryParse(ip, out var addr) &&
                    addr.AddressFamily == AddressFamily.InterNetwork)
                {
                    byte[] b = addr.GetAddressBytes();
                    return ((long)b[0] << 24) | ((long)b[1] << 16) | ((long)b[2] << 8) | b[3];
                }
                return long.MaxValue;
            }
        }
    }
}