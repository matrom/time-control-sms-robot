using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TimeControlServer
{
    public class Message
    {
        public string From;
        public string To;
        public string text;
        public bool isProcessed = false;
        public Guid id;
        public Message()
        {
            id = Guid.NewGuid();
        }

        public Message(Message source)
        {
            From = source.From;
            To = source.To;
            text = source.text;
            id = source.id;
            isProcessed = source.isProcessed;
        }
        public void CopyContents(Message source)
        {
            From = source.From;
            To = source.To;
            text = source.text;
        }
        public override string ToString()
        {
            return isProcessed.ToString() + " From "+From + " To " + To + " " + text + " " + id.ToString();
        }
    }
}
