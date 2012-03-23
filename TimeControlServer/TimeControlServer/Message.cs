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
        public Message()
        {
        }

        public Message(Message source)
        {
            number = source.number;
            text = source.text;
        }
        public string ToString()
        {
            return number + " " + text + " " + isProcessed.ToString(); 
        }
    }
}
