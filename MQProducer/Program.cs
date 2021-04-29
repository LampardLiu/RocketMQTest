using NewLife.Log;
using NewLife.RocketMQ;
using System;
using System.Threading.Tasks;

namespace MQProducer
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");
            //mq对象
            using var mq = new Producer
            {
                //主题
                Topic = "lmi_datas",
                //服务地址
                NameServerAddress = "127.0.0.1:9876",
            };

            mq.Start();
            int i = 1;
            //轮询发消息
            while (true)
            {
                var content = DateTime.Now.ToString("yyyy年MM月dd日 HH:mm:ss.fff");
                var message = new NewLife.RocketMQ.Protocol.Message()
                {
                    BodyString = content,
                    Keys = (i++).ToString(),
                    Tags = i % 2 == 0 ? "even" : "odd",
                    Flag = 0,
                    WaitStoreMsgOK = true
                };
                //发送消息（生产消息）
                var sr = mq.Publish(message);
                string log = $"发送成功的消息，内容>{content},MsgId={sr.MsgId},BrokerName= {sr.Queue.BrokerName} ,QueueId={sr.Queue.QueueId},QueueOffset= {sr.QueueOffset}";
                Console.WriteLine(log);
                Task.Delay(TimeSpan.FromSeconds(10)).Wait();
            }
        }
    }
}
