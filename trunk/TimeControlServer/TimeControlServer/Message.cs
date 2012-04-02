using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TimeControlServer
{
    public class Message
    {
        public string number;
        public string text;
        public bool isProcessed = false;
        public Guid id;
        public Message()
        {
            id = Guid.NewGuid();
        }

        public Message(Message source)
        {
            number = source.number;
            text = source.text;
            id = source.id;
        }
        public override string ToString()
        {
            return number + " " + text + " " + isProcessed.ToString() + " " + id.ToString();
        }
    }
}
