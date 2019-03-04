using System;

namespace FiresharpCore.EventStreaming
{
    public class ValueChangedEventArgs : EventArgs
    {
        public ValueChangedEventArgs(string path, string data, string oldData)
        {
            OldData = oldData;
            Path = path;
            Data = data;
        }

        public string Path { get; private set; }
        public string Data { get; private set; }
        public string OldData { get; private set; }
    }
}