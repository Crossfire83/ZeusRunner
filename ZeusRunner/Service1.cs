﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.ServiceProcess;
using System.Timers;

namespace ZeusRunner
{
    public partial class Service1 : ServiceBase
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
            components = new Container();
            this.ServiceName = "Service1";
        }
        #endregion

        public Service1()
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
            Process[] pjava = Process.GetProcessesByName("java.exe");
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
                if (userName == "auth\\perezrau") {
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
                if (userName == "perezrau")
                {
                    pcmdFiltered.Add(proc);
                }
            }

            if (pjava.Length == 0 && pcmd.Length == 0) { }
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