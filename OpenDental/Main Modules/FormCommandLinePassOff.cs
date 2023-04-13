using System;
using System.Net.Sockets;
using System.Xml.Serialization;

namespace OpenDental
{
    public partial class FormCommandLinePassOff : FormODBase
    {
        public string[] CommandLineArgs;

        public FormCommandLinePassOff()
        {
            InitializeComponent();
            InitializeLayoutManager();
        }

        private void FormCommandLinePassOff_Load(object sender, EventArgs e)
        {
            try
            {
                TcpClient client = new TcpClient("localhost", 2123);
                NetworkStream ns = client.GetStream();
                XmlSerializer serializer = new XmlSerializer(typeof(string[]));
                serializer.Serialize(ns, CommandLineArgs);
                ns.Close();
                client.Close();
            }
            catch { }
            Close();
        }
    }
}
