using System;

namespace LightLog
{
    public class LogTask
    {
        public DateTime CreatTime { get; set; } = DateTime.Now;
        public LogLevel LogLevel { set; get; } = LogLevel.Info;
        public object Sender { get; set; }
        public object Msg { get; set; }
        public LogTask(LogLevel lv, object sender, object msg)
        {
            LogLevel = lv;
            Sender = sender;
            Msg = msg;
        }
        public override string ToString()
        {
            string type = string.Empty;
            if (Sender == null) type = "Sender is null";
            if (Sender is string)
                type = Sender as string;
            else
                type = Sender.GetType().Name;
            return $"[{CreatTime.ToString("yyyy-MM-dd HH:mm:ss fff")}] : {type} => {Msg?.ToString()}";
        }
    }
}
