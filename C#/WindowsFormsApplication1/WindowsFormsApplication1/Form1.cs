using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO.Ports;
namespace WindowsFormsApplication1
{
    public partial class Form1 : Form
    {
        public SerialPort port = new SerialPort("COM2", 9600);
        public Form1()
        {
            InitializeComponent();
            try { 
            port.Open();
                }
            catch (Exception e)
            {
                Label ayy = new Label();
                ayy.Text="Arduino Not Connected";
                Form lmao = new Form();
                lmao.Controls.Add(ayy);
                lmao.ShowDialog();
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            port.Write(textBox1.Text);
        }

    }
}
