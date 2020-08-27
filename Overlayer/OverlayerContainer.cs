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
using System.Runtime.InteropServices;
using static Overlayer.OverlayerLauncher;

namespace Overlayer
{
    public partial class OverlayerContainer : Form
    {
        private IntPtr hWndOriginalParent;

        [DllImport("user32.dll")]
        public static extern IntPtr SetParent(IntPtr hWndChild, IntPtr hWndNewParent);

        [DllImport("user32.dll", SetLastError = true)]
        public static extern bool MoveWindow(IntPtr hWnd, int X, int Y, int nWidth, int nHeight, bool bRepaint);

        [DllImport("user32.dll")]
        public static extern int FindWindow(string lpClassName, string lpWindowName);

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool IsWindow(IntPtr hWnd);

        public void dockIt(IntPtr hWnd)
        {
            if (IsWindow(hWnd))
            {
                try
                {
                    //Prevent kicking out of game (but makes textboxes unusable)
                    if (!_overlayerLauncher.focusableChkBox.Checked) _overlayerLauncher.SetWindowUnfocusable(hWnd);

                    //Disable super-topmost as it is impossible to dock in that state
                    TopMost = false;

                    //Windows API call to change the parent of the target window.
                    //It returns the hWnd of the window's parent prior to this call.
                    hWndOriginalParent = SetParent(hWnd, this.Handle);

                    //Wire up the event to keep the window sized to match the control
                    this.SizeChanged += new EventHandler(this_Resize);

                    _overlayerLauncher.childs[_overlayerLauncher.childCount] = hWnd;
                    _overlayerLauncher.childCount++;

                    //Re-enable super-topmost
                    TopMost = true;

                    //Once-docked windows retain super-topmost after undocking?????!!
                    //undockIt(hWnd);
                }
                catch { }
            }
            else
            {
                //MessageBox.Show("Window Not Found", "404");
            }
        }

        private static Process pDocked;

        public void runAndDock(string cmdline)
        {
            //pDocked = Process.Start(@cmdline);
            StartProcessNoActivate(cmdline);
            //pDocked.WaitForInputIdle(1000); //wait for the window to be ready for input;
            try
            {
                pDocked.Refresh();              //update process info
                if (pDocked.HasExited)
                {
                    return; //abort if the process finished before we got a handle.
                }
            }
            catch
            {
                System.Threading.Thread.Sleep(2000);
            }
            dockIt(pDocked.MainWindowHandle);
        }

        public void undockIt(IntPtr hWnd)
        {
            //Restores the application to it's original parent.
            SetParent(hWnd, hWndOriginalParent);
        }

        public void redockAll()
        {
            for (int i = 0; i < _overlayerLauncher.childCount; i++)
            {
                _overlayerLauncher.SetWindowUnfocusable(_overlayerLauncher.childs[i]);
            }
        }

        private void this_Resize(object sender, EventArgs e)
        {
            //Change the docked windows size to match its parent's size. 
            //MoveWindow(hWnd, 0, 0, this.Width, this.Height, true);
        }

        public static OverlayerContainer _overlayerContainer;
        public OverlayerContainer()
        {
            InitializeComponent();
            _overlayerContainer = this;
        }

        private void OverlayerContainer_FormClosing(object sender, FormClosingEventArgs e)
        {
            _overlayerLauncher.undockAll();
        }

        //For prevent focus on RunAndDock
        [StructLayout(LayoutKind.Sequential)]
        struct STARTUPINFO
        {
            public Int32 cb;
            public string lpReserved;
            public string lpDesktop;
            public string lpTitle;
            public Int32 dwX;
            public Int32 dwY;
            public Int32 dwXSize;
            public Int32 dwYSize;
            public Int32 dwXCountChars;
            public Int32 dwYCountChars;
            public Int32 dwFillAttribute;
            public Int32 dwFlags;
            public Int16 wShowWindow;
            public Int16 cbReserved2;
            public IntPtr lpReserved2;
            public IntPtr hStdInput;
            public IntPtr hStdOutput;
            public IntPtr hStdError;
        }

        [StructLayout(LayoutKind.Sequential)]
        internal struct PROCESS_INFORMATION
        {
            public IntPtr hProcess;
            public IntPtr hThread;
            public int dwProcessId;
            public int dwThreadId;
        }

        [DllImport("kernel32.dll")]
        static extern bool CreateProcess(
            string lpApplicationName,
            string lpCommandLine,
            IntPtr lpProcessAttributes,
            IntPtr lpThreadAttributes,
            bool bInheritHandles,
            uint dwCreationFlags,
            IntPtr lpEnvironment,
            string lpCurrentDirectory,
            [In] ref STARTUPINFO lpStartupInfo,
            out PROCESS_INFORMATION lpProcessInformation
        );

        [DllImport("kernel32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool CloseHandle(IntPtr hObject);

        const int STARTF_USESHOWWINDOW = 1;
        const int SW_SHOWNOACTIVATE = 4;
        const int SW_SHOWMINNOACTIVE = 7;


        public static void StartProcessNoActivate(string cmdLine)
        {
            STARTUPINFO si = new STARTUPINFO();
            si.cb = Marshal.SizeOf(si);
            si.dwFlags = STARTF_USESHOWWINDOW;
            si.wShowWindow = SW_SHOWNOACTIVATE;

            PROCESS_INFORMATION pi = new PROCESS_INFORMATION();

            CreateProcess(null, cmdLine, IntPtr.Zero, IntPtr.Zero, true,
                0, IntPtr.Zero, null, ref si, out pi);

            pDocked = Process.GetProcessById(pi.dwProcessId);
            CloseHandle(pi.hProcess);
            CloseHandle(pi.hThread);
        }
    }
}
