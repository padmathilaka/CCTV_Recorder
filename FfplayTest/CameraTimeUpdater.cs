using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Diagnostics;
using System.Threading;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Drawing.Text;
using System.Text.RegularExpressions;
using System.Configuration;
using Microsoft.Win32;
using System.Windows.Forms.VisualStyles;
using System.ServiceModel;
using System.ServiceModel.Channels;
using NVRRecordingSystem.OnvifDevice;
using NVRRecordingSystem.OnvifServices;
using System.Net;
using Microsoft.Web.Services3;
using Microsoft.Web.Services3.Security.Tokens;


namespace FfplayTest
{
    public class CameraTimeUpdater
    {

        public CameraTimeUpdater()
        {

        }


        public void UpdateCameraDateTime(string _ipaddress,string _userName, string _password, string _version)
        {
            try
            {
                EndpointAddress endPointAddress = new EndpointAddress("http://" + _ipaddress + "/onvif/device_service");


                if (_version == "New")
                {

                    var httpTransportBinding = new HttpTransportBindingElement { AuthenticationScheme = AuthenticationSchemes.Digest };
                    var textMessageEncodingBinding = new TextMessageEncodingBindingElement { MessageVersion = MessageVersion.CreateVersion(EnvelopeVersion.Soap12, AddressingVersion.None) };
                    var customBinding = new CustomBinding(textMessageEncodingBinding, httpTransportBinding);
                    var passwordDigestBehavior = new PasswordDigestBehavior(_userName, _password);
                    var temp = new DeviceClient(customBinding, endPointAddress);
                    temp.Endpoint.Behaviors.Add(passwordDigestBehavior);

                    String[] arrayString = new String[4];

                    String res = temp.GetDeviceInformation(out arrayString[0], out arrayString[1], out arrayString[2], out  arrayString[3]);
                    NVRRecordingSystem.OnvifDevice.SystemDateTime _sysdateTime = temp.GetSystemDateAndTime();


                    Date _date = new Date();
                    _date.Year = System.DateTime.Now.Year;
                    _date.Month = System.DateTime.Now.Month;
                    _date.Day = System.DateTime.Now.Day;

                    Time _time = new Time();
                    _time.Hour = System.DateTime.Now.Hour;
                    _time.Minute = System.DateTime.Now.Minute;
                    _time.Second = System.DateTime.Now.Second;

                    NVRRecordingSystem.OnvifDevice.TimeZone _ttzone = new NVRRecordingSystem.OnvifDevice.TimeZone();
                    _ttzone.TZ = "UTC−05:00";


                    NVRRecordingSystem.OnvifDevice.DateTime _dateTime = new NVRRecordingSystem.OnvifDevice.DateTime();
                    _dateTime.Date = _date;
                    _dateTime.Time = _time;

                    temp.SetSystemDateAndTimeAsync(SetDateTimeType.Manual, false, _ttzone, _dateTime);
                }
                else
                {
                    CustomBinding bind = new CustomBinding("DeviceBinding");
                    DeviceClient temp = new DeviceClient(bind, endPointAddress);

                    String[] arrayString = new String[4];

                    String res = temp.GetDeviceInformation(out arrayString[0], out arrayString[1], out arrayString[2], out  arrayString[3]);
                    NVRRecordingSystem.OnvifDevice.SystemDateTime _sysdateTime = temp.GetSystemDateAndTime();


                    Date _date = new Date();
                    _date.Year = System.DateTime.Now.Year;
                    _date.Month = System.DateTime.Now.Month;
                    _date.Day = System.DateTime.Now.Day;

                    Time _time = new Time();
                    _time.Hour = System.DateTime.Now.Hour;
                    _time.Minute = System.DateTime.Now.Minute;
                    _time.Second = System.DateTime.Now.Second;

                    NVRRecordingSystem.OnvifDevice.TimeZone _ttzone = new NVRRecordingSystem.OnvifDevice.TimeZone();
                    _ttzone.TZ = "UTC−05:00";


                    NVRRecordingSystem.OnvifDevice.DateTime _dateTime = new NVRRecordingSystem.OnvifDevice.DateTime();
                    _dateTime.Date = _date;
                    _dateTime.Time = _time;

                    temp.SetSystemDateAndTimeAsync(SetDateTimeType.Manual, false, _ttzone, _dateTime);
                }
                
            }
            catch
            {

            }
        }



    }
}
