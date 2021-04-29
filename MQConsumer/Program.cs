using System;
using System.Linq;
using NewLife;
using NewLife.RocketMQ;

namespace MQConsumer
{
    class Program
    {
        static void Main(string[] args)
        {
            
            //测试消费消息
            var consumer = new NewLife.RocketMQ.Consumer
            {
                Topic = "lmi_datas",
               // Group = "CID_ONSAPI_OWNER",
                NameServerAddress = "172.10.11.233:9876",
                //设置每次接收消息只拉取一条信息
                BatchSize = 1,
                //FromLastOffset = true,
                //SkipOverStoredMsgCount = 0,
                //BatchSize = 20,
                //Log = NewLife.Log.XTrace.Log,
            };
            consumer.OnConsume = (q, ms) =>
            {
                string mInfo = $"BrokerName={q.BrokerName},QueueId={q.QueueId},Length={ms.Length}";
                Console.WriteLine(mInfo);
                foreach (var item in ms.ToList())
                {
                    string msg = $"消息：msgId={item.MsgId},key={item.Keys}，产生时间【{item.BornTimestamp.ToDateTime()}】，内容>{item.Body.ToStr()}";
                    Console.WriteLine(msg);
                }
                //   return false;//通知消息队：不消费消息
                return true;		//通知消息队：消费了消息
            };

            consumer.Start();
            Console.WriteLine("消息接收测试");
            Console.ReadLine();
        } 
    }
}
