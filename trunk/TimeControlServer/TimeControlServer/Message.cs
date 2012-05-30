using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;

namespace TimeControlServer
{
    public class Message : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        private string _from;
        public string From
        {
            get { return _from; }
            set
            {
                _from = value;
                this.NotifyPropertyChanged("From");
            }
        }
        private string _to;
        public string To
        {
            get { return _to; }
            set
            {
                _to = value;
                this.NotifyPropertyChanged("To");
            }
        }
        private string _text;
        public string text
        {
            get { return _text; }
            set
            {
                _text = value;
                this.NotifyPropertyChanged("text");
            }
        }
        private bool _isProcessed = false;
        public bool isProcessed
        {
            get { return _isProcessed; }
            set
            {
                _isProcessed = value;
                this.NotifyPropertyChanged("isProcessed");
            }
        }
        private Guid _id;
        public Guid id
        {
            get { return _id; }
            set
            {
                _id = value;
                this.NotifyPropertyChanged("id");
            }
        }
        private DateTime _timeStamp = DateTime.Now;
        public DateTime timeStamp
        {
            get { return _timeStamp; }
            set
            {
                _timeStamp = value;
                this.NotifyPropertyChanged("timeStamp");
            }
        }
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
        private void NotifyPropertyChanged(string name)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(name));
        }
    }
}
