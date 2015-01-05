using AForge.Video.DirectShow;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace nmct.ba.cashlessproject.helper
{
    public class CameraDevices
    {
        public FilterInfoCollection Devices { get; private set; }
        public VideoCaptureDevice Current { get; set; }

        public CameraDevices()
        {
            Devices = new FilterInfoCollection(FilterCategory.VideoInputDevice);
        }

        public void SelectCamera(int index)
        {
            if (index >= Devices.Count)
            {
                throw new ArgumentOutOfRangeException("index");
            }
            Current = new VideoCaptureDevice(Devices[index].MonikerString);
        }
    }
}
