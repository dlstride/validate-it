namespace RabbitMQValidator.Models
{
    public class RabbitQueue
    {
        public RabbitQueue(string name, string messages, int messagesReady)
        {
            this.Name = name;
            this.Messages = messages;
            this.MessagesReady = messagesReady;            
        }

        public string Name { get; }

        public string Messages { get; }

        public int MessagesReady { get; }
    }
}