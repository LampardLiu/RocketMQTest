# RocketMQTest
 
@[TOC]

# NewLift.RocketMQ
Github地址[https://github.com/NewLifeX/NewLife.RocketMQ](https://github.com/NewLifeX/NewLife.RocketMQ)
Nuget地址：[https://www.nuget.org/packages/NewLife.RocketMQ/1.5.2021.410](https://www.nuget.org/packages/NewLife.RocketMQ/1.5.2021.410)
或者在Nuget上自己搜索NewLife.RocketMQ。

# Rocket Producer(生产者）
用于连接mq，并且生产数据

```csharp
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

```


# RocketMQ Consumer（消费者)
连接指定主题lmi_datas(这个主题我是通过网页端创建的）

```csharp
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
                NameServerAddress = "172.10.11.233:9876",
                //设置每次接收消息只拉取一条信息
                BatchSize = 1 
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

```


源码地址：[https://github.com/LampardLiu/RocketMQTest](https://github.com/LampardLiu/RocketMQTest)
