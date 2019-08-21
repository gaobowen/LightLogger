using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace LightLog
{
    public class LogCore
    {
        //读写锁，当资源处于写入模式时，其他线程写入需要等待本次写入结束之后才能继续写入
        static ReaderWriterLockSlim InfoLogWriteLock = new ReaderWriterLockSlim();
        static ReaderWriterLockSlim DebugLogWriteLock = new ReaderWriterLockSlim();
        static ReaderWriterLockSlim ErrorLogWriteLock = new ReaderWriterLockSlim();

        public static void WriteTask(LogTask task, string baseDirPath)
        {
            if (task == null || string.IsNullOrEmpty(baseDirPath)) return;
            switch (task.LogLevel)
            {
                case LogLevel.Info:
                    Task.Run(() =>
                    {
                        WriteLog(task.ToString(), baseDirPath, InfoLogWriteLock);
                    });
                    break;
                case LogLevel.Debug:
                    Task.Run(() =>
                    {
                        WriteLog(task.ToString(), baseDirPath, DebugLogWriteLock);
                    });
                    break;
                case LogLevel.Error:
                    WriteLog(task.ToString(), baseDirPath, ErrorLogWriteLock);
                    break;
                default:
                    Task.Run(() =>
                    {
                        WriteLog(task.ToString(), baseDirPath, InfoLogWriteLock);
                    });
                    break;
            }
        }


        public static void WriteLog(string strLog, string baseDirPath, ReaderWriterLockSlim rwlock)
        {
            string sFileName = DateTime.Now.ToString("yyyyMMddHHmmssfff") + ".log0";
            string fullName = Utils.PathCombine(baseDirPath, sFileName);//文件的绝对路径
            //进入读写锁
            rwlock.EnterWriteLock();
            var dir = new DirectoryInfo(baseDirPath);
            var files = dir.GetFiles().ToList();
            var needCheck = files.FirstOrDefault(x => x.Name.Contains(".log0"));
            if (needCheck != null && needCheck.Length > 512000)
            {
                //对日志文件重新分片
                for (int i = 4; i >= 0; i--)
                {
                    if (i == 4)
                    {
                        var find = files.FirstOrDefault(x => x.Extension.Contains("log4"));
                        if (find == null) continue;
                        try
                        {
                            find.Delete();
                            files.Remove(find);
                        }
                        catch (Exception)
                        {
                            break;
                        }
                    }
                    else
                    {
                        var rename = files.FirstOrDefault(x => x.Extension.Contains("log" + i));
                        try
                        {
                            rename?.MoveTo(Path.Combine(baseDirPath, Path.GetFileNameWithoutExtension(rename.Name) + $".log{i + 1}"));
                        }
                        catch (Exception)
                        { }
                    }
                }
            }
            else
            {
                var find = files.FirstOrDefault(x => x.Extension.Contains("log0"));
                try
                {
                    find?.MoveTo(fullName);
                }
                catch (Exception)
                { }
            }
            FileStream fs = null;
            StreamWriter sw = null;
            try
            {
                //验证文件是否存在，有则追加，无则创建
                if (File.Exists(fullName))
                {
                    fs = new FileStream(fullName, FileMode.Append, FileAccess.Write);
                }
                else
                {
                    fs = new FileStream(fullName, FileMode.Create, FileAccess.Write);
                }
                sw = new StreamWriter(fs, Encoding.UTF8);//, Encoding.GetEncoding("gb2312")
                sw.WriteLine(strLog);
            }
            catch (Exception ex)
            { }
            finally
            {
                sw?.Dispose();
                fs?.Dispose();
                rwlock.ExitWriteLock();
            }
        }
    }
}
