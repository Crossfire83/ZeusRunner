using System.Runtime.Serialization.Formatters.Binary;
using System.Runtime.Serialization;
using System.IO;
using System.Text;
using System;

namespace Logs
{
    public class Log
    {
        public Log()
        {   }

        public void captureError(string msg)
        {   saveLogFile("Error",msg);   }

        public void captureEvent(string msg)
        {   saveLogFile("Event", msg);  }

        public static void saveLogFile(string filename,string msg)
        {   String ruta = filename;
            IFormatter formateador = new BinaryFormatter();
            byte[] a;
            char[] b;
            b = msg.ToCharArray();
            a = new byte[b.Length];
            lock(a)
            {	Stream flujo = new FileStream(ruta + ".log",FileMode.Append,FileAccess.Write);
				for(int i = 0;i < a.Length;i++)
            	{   a[i] = (byte)b[i]; }
            	try
                {   string t = "\r\n\r\n" + DateTime.Now.ToString();
                    t = t + Environment.NewLine;
                    byte[] tm = Encoding.ASCII.GetBytes(t);
                    flujo.Write(tm,0,tm.Length);
                    flujo.Write(a,0,a.Length);
                }
                catch(Exception){}
                flujo.Close();
            }
        }
    }
}
