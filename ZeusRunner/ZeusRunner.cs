using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.ServiceProcess;
using System.Timers;
using Credentials;
using System.Net;

namespace ZeusRunner
{
    /// <summary>
    /// Additional documentation on how to install the service and troubleshooting can be faound at:
    /// https://dzone.com/articles/create-windows-services-in-c
    /// https://stackoverflow.com/questions/2205744/error-in-installing-windows-service-developed-in-net
    /// https://stackoverflow.com/questions/24228307/error-1053-the-service-did-not-respond-to-the-start-or-control-request-in-a-time
    /// https://stackoverflow.com/questions/20561990/how-to-solve-the-specified-service-has-been-marked-for-deletion-error/20565337#20565337
    /// </summary>
    public partial class ZeusRunner : ServiceBase
    {
        /// <summary> 
        /// Required designer variable.
        /// </summary>
        private IContainer components = null;
        private Timer t;
        //the User Name is the info we want returned by the query.
        internal static int WTS_UserName = 5;
        //just use the current TS server context.
        internal static IntPtr WTS_CURRENT_SERVER_HANDLE = IntPtr.Zero;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Component Designer generated code
        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            // 
            // ZeusRunner
            // 
            this.ServiceName = "ZeusRunner";

        }
        #endregion

        public ZeusRunner()
        {
            InitializeComponent();
            t = new Timer
            {
            #if DEBUG
                Interval = 300
            #else
                Interval = 300000
            #endif
            };
            t.Elapsed += Timer_Elapsed;
        }

        private void Timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            t.Stop();
            Process[] pjava = Process.GetProcessesByName("java");
            Process[] pcmd = Process.GetProcessesByName("cmd");
            List<Process> pjavaFiltered = new List<Process>();
            List<Process> pcmdFiltered = new List<Process>();

            foreach (Process proc in pjava) {
                IntPtr AnswerBytes, AnswerCount;
                string userName = String.Empty;
                if (WTSQuerySessionInformationW(WTS_CURRENT_SERVER_HANDLE,
                                                proc.SessionId,
                                                WTS_UserName,
                                                out AnswerBytes,
                                                out AnswerCount))
                {
                    userName = Marshal.PtrToStringUni(AnswerBytes);
                    Console.WriteLine(userName);
                    Console.WriteLine();
                }
                if (userName == "$rape001") {
                    pjavaFiltered.Add(proc);
                }
            }

            foreach (Process proc in pcmd)
            {
                IntPtr AnswerBytes, AnswerCount;
                string userName = String.Empty;
                if (WTSQuerySessionInformationW(WTS_CURRENT_SERVER_HANDLE,
                                                proc.SessionId,
                                                WTS_UserName,
                                                out AnswerBytes,
                                                out AnswerCount))
                {
                    userName = Marshal.PtrToStringUni(AnswerBytes);
                    Console.WriteLine(userName);
                    Console.WriteLine();
                }
                if (userName == "$rape001")
                {
                    pcmdFiltered.Add(proc);
                }
            }

            if (pjavaFiltered.Count == 0 && pcmdFiltered.Count == 0) {
                /// to create the Xml with credentials, use the following command on powershell:
                /// Get-Credential | Export-Clixml -Path C:\Users\aguiledu\werw.xml
                NetworkCredential credentials = FileDecrypter.Decrypt(@"C:\credZeus.xml");
                ProcessStartInfo info = new ProcessStartInfo(@"E:\NetBeans Projects\ZeusExtractor\ZeusExtractor.bat");
                info.UseShellExecute = false;
                info.UserName = credentials.UserName;
                info.Password = credentials.SecurePassword;
                info.WorkingDirectory = @"E:\NetBeans Projects\ZeusExtractor";
                info.Domain = "AUTH";
                Process zeus = Process.Start(info);
            }
            t.Start();
        }

        protected override void OnStart(string[] args)
        {
            t.Enabled = true;
            t.Start();
        }

        protected override void OnStop()
        {
            t.Stop();
            t.Enabled = false;
        }

        public void onDebug()
        {
            OnStart(null);
        }

        [DllImport("Wtsapi32.dll")]
        public static extern bool WTSQuerySessionInformationW(IntPtr hServer,
            int SessionId,
            int WTSInfoClass,
            out IntPtr ppBuffer,
            out IntPtr pBytesReturned);
    }
}