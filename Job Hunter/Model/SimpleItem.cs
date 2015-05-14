using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Job_Hunter.Model
{
    public class SimpleItem
    {
        protected long _id;
        protected long _position;
        private string _title;
        protected string _note;

        public SimpleItem(long id, long position, string title, string note)
        {
            if(String.IsNullOrWhiteSpace(title))
            {
                throw new ArgumentException("Empty title field");
            }
            _title = title;
            _id = id;
            _position = position;
            _note = note;
        }

        public long Id
        {
            get { return _id; }
        }

        public long Position
        {
            get { return _position; }
        }

        public string Title
        {
            get { return _title; }
        }

        public string Note
        {
            get { return _note; }
        }
    }
}
