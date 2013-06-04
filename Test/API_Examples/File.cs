using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics.Contracts;

namespace API_Examples
{
	public enum FileOpenMode
	{
		Read,
		Write
	}

	public class File
	{
        //public string fileName;
        public FileOpenMode mode;
        public bool open;

		public File(string fileName)
		{
            //this.fileName = fileName;
            open = false;
		}

        public bool IsOpen
        {
            get { return open; }
        }

        public FileOpenMode Mode
        {
            get
            {
                Contract.Requires(open);
                return mode;
            }
        }

        public void Open(FileOpenMode mode)
        {
            Contract.Requires(!open);

            this.mode = mode;
            open = true;
        }

        public byte[] Read()
        {
            Contract.Requires(open);
            Contract.Requires(mode == FileOpenMode.Read);

            return null;
        }

        public void Write(byte[] data)
        {
            Contract.Requires(open);
            Contract.Requires(mode == FileOpenMode.Write);
        }

        public void Close()
        {
            Contract.Requires(open);

            open = false;
        }
	}
}
