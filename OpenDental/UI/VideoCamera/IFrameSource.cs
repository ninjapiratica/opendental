using System;

namespace OpenDental.UI
{
    public interface IFrameSource
    {
        event Action<IFrameSource, byte[], System.Drawing.Size> NewFrame;

        void StartFrameCapture();
        void StopFrameCapture();
    }
}
