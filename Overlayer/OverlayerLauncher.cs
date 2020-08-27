using System;
using System.Activities;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static Overlayer.OverlayerContainer;

namespace Overlayer
{
    public partial class OverlayerLauncher : Form
    {
        private const int GWL_EXSTYLE = -20;
        private const int WS_EX_NOACTIVATE = 0x08000000;
        private const int WS_EX_TOPMOST = 0x00000008;
        public IntPtr[] childs = new IntPtr[100];
        public int childCount = 0;

        [DllImport("user32.dll")]
        public static extern IntPtr SetWindowLong(IntPtr hWnd, int nIndex, int dwNewLong);

        [DllImport("user32.dll")]
        public static extern int GetWindowLong(IntPtr hWnd, int nIndex);

        [DllImport("user32.dll")]
        public static extern int FindWindow(string lpClassName, string lpWindowName);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        static extern int GetWindowText(IntPtr hWnd, StringBuilder lpString, int nMaxCount);

        [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        static extern int GetWindowTextLength(IntPtr hWnd);

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool IsWindow(IntPtr hWnd);

        public static string GetWindowTitle(IntPtr hWnd)
        {
            var length = GetWindowTextLength(hWnd);
            var title = new StringBuilder(length);
            GetWindowText(hWnd, title, length);
            return title.ToString();
        }

        public void SetWindowUnfocusable(IntPtr hWnd)
        {
            if (IsWindow(hWnd))
            {
                SetWindowLong(hWnd, GWL_EXSTYLE, GetWindowLong(hWnd, GWL_EXSTYLE) | WS_EX_NOACTIVATE);
            }
            else
            {
                //MessageBox.Show("Window Not Found", "404");
            }
        }

        public void SetWindowFocusable(IntPtr hWnd)
        {
            if (IsWindow(hWnd))
            {
                SetWindowLong(hWnd, GWL_EXSTYLE, GetWindowLong(hWnd, GWL_EXSTYLE) & ~WS_EX_NOACTIVATE);
            }
            else
            {
                //MessageBox.Show("Window Not Found", "404");
            }
        }

        public static OverlayerLauncher _overlayerLauncher;
        public OverlayerLauncher()
        {
            InitializeComponent();
            _overlayerLauncher = this;
            var OverlayerContainer = new OverlayerContainer();
            OverlayerContainer.Show();
            SetWindowUnfocusable(Handle);
            _overlayerContainer.dockIt(Handle);

            // Modifier keys codes: Alt = 1, Ctrl = 2, Shift = 4, Win = 8
            // Compute the addition of each combination of the keys you want to be pressed
            // ALT+CTRL = 1 + 2 = 3 , CTRL+SHIFT = 2 + 4 = 6...
            // Keys.Oem3 = `
            RegisterHotKey(this.Handle, MYACTION_HOTKEY_ID, 2, (int)Keys.Oem3);
        }

        private void PreventFocusFromClassBtn_Click(object sender, EventArgs e)
        {
            IntPtr hWnd = new IntPtr(FindWindow(textBox.Text, null));
            _overlayerContainer.dockIt(hWnd);

        }

        private void PreventFocusFromTitleBtn_Click(object sender, EventArgs e)
        {
            IntPtr hWnd = new IntPtr(FindWindow(null, textBox2.Text));
            _overlayerContainer.dockIt(hWnd);
        }

        private void PreventFocusFromBothBtn_Click(object sender, EventArgs e)
        {
            IntPtr hWnd = new IntPtr(FindWindow(textBox.Text, textBox2.Text));
            _overlayerContainer.dockIt(hWnd);
        }
        
        private void RunAndDockBtn_Click(object sender, EventArgs e)
        {
            _overlayerContainer.runAndDock(textBox3.Text);
        }

        public void undockAll()
        {
            for (int i = 0; i < childCount; i++)
            {
                _overlayerContainer.TopMost = false;
                _overlayerContainer.undockIt(childs[i]);
                SetWindowFocusable(childs[i]);
            }
        }

        public void setFocusableAll()
        {
            for (int i = 0; i < childCount; i++)
            {
                SetWindowFocusable(childs[i]);
            }
        }

        private void GitHubLink_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e) => Process.Start("https://github.com/Ingan121/Overlayer");

        private void OverlayerLauncher_FormClosing(object sender, FormClosingEventArgs e)
        {
            undockAll();
        }

        private void OverlayerLauncher_Resize(object sender, EventArgs e)
        {
            if (WindowState == FormWindowState.Minimized)
            {
                _overlayerContainer.Visible = false;
            }
        }

        private void focusableChkBox_CheckedChanged(object sender, EventArgs e)
        {
            if (focusableChkBox.Checked)
            {
                setFocusableAll();
            }
            else
            {
                _overlayerContainer.redockAll();
            }
        }

        //Hotkey

        // DLL libraries used to manage hotkeys
        [DllImport("user32.dll")]
        public static extern bool RegisterHotKey(IntPtr hWnd, int id, int fsModifiers, int vlc);

        [DllImport("user32.dll")]
        public static extern bool UnregisterHotKey(IntPtr hWnd, int id);

        const int MYACTION_HOTKEY_ID = 1;

        protected override void WndProc(ref Message m)
        {
            if (m.Msg == 0x0312 && m.WParam.ToInt32() == MYACTION_HOTKEY_ID)
            {
                // My hotkey has been typed

                // Do what you want here
                WindowState = FormWindowState.Normal;
                _overlayerContainer.Visible = true;
            }
            base.WndProc(ref m);
        }
    }
}