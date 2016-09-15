using System;
using System.IO;
using System.Security.Cryptography.X509Certificates;

namespace AddressProcessing.CSV
{
    /*
        2) Refactor this class into clean, elegant, rock-solid & well performing code, without over-engineering.
           Assume this code is in production and backwards compatibility must be maintained.
    */

   /* 
    I have tried to break everything down using interfaces. I would have  ideally liked if i could have moved the 
    file mode flag outside of the concrete class but didn't go ahead as we had to maintain backwards compatibility.

    Had to create a constructor to inject the dependencies for testing purposes.

    Also the writin and reading part are now in separate modules and can be tested separately.
    
   */

    public class CSVReaderWriter : IFileReaderWriter
    {
        private IFileReader _reader;
        private IFileWriter _writer;

        private string _fileName;

        public CSVReaderWriter(IFileReader reader, IFileWriter writer)
        {
            _reader = reader;
            _writer = writer;
        }

        public CSVReaderWriter()
        {
            
        }

        [Flags]
        public enum Mode { Read = 1, Write = 2 };

        public void Open(string fileName, Mode mode)
        {
            if (mode == Mode.Read)
            {
                if (_reader == null)
                {
                    _reader = new FileReader();
                }
                _reader.Open(fileName);
            }
            else if (mode == Mode.Write)
            {
                if (_writer == null)
                {
                    _writer = new FileWriter();
                }
                _writer.Open(fileName);
            }
            else
            {
                throw new Exception("Unknown file mode for " + fileName);
            }
        }

        public bool Read(out string column1, out string column2)
        {
            const int FIRST_COLUMN = 0;
            const int SECOND_COLUMN = 1;

            string line;
            string[] columns;

            char[] separator = { '\t' };

            line = ReadLine();

            if (line == null)
            {
                column1 = null;
                column2 = null;

                return false;
            }

            columns = line.Split(separator);

            if (columns.Length == 0)
            {
                column1 = null;
                column2 = null;

                return false;
            }
            else
            {
                column1 = columns[FIRST_COLUMN];
                column2 = columns[SECOND_COLUMN];

                return true;
            }
        }

        public void Write(params string[] columns)
        {
            string outPut = "";

            for (int i = 0; i < columns.Length; i++)
            {
                outPut += columns[i];
                if ((columns.Length - 1) != i)
                {
                    outPut += "\t";
                }
            }

            _writer.WriteLine(outPut);
        }

        private string ReadLine()
        {
            return _reader.ReadLine();
        }

        public void Close()
        {
            if (_writer != null)
            {
                _writer.Close();
            }

            if (_reader != null)
            {
                _reader.Close();
            }
        }
    }

    public interface IFileReaderWriter
    {
        bool Read(out string column1, out string column2);

        void Write(params string[] columns);

        void Open(string fileName, CSVReaderWriter.Mode mode);

        void Close();
    }

    public interface IFileReader
    {
        void Open(string fileName);

        string ReadLine();

        void Close();
    }

    public interface IFileWriter
    {
        void Open(string fileName);

        void WriteLine( string line);

        void Close();
    }

    public class FileReader : IFileReader
    {
        private StreamReader _readerStream;

        public void Open(string fileName)
        {
            _readerStream = File.OpenText(fileName);
        }

        public string ReadLine()
        {
            return _readerStream.ReadLine();
        }

        public void Close()
        {
            _readerStream?.Close();
        }
    }

    public class FileWriter : IFileWriter
    {
        private StreamWriter _writer;

        public void Open(string fileName)
        {
            var fileInfo = new FileInfo(fileName);
            _writer = fileInfo.CreateText();
        }

        public void WriteLine(string line)
        {
            _writer.Write(line);
        }

        public void Close()
        {
            _writer?.Close();
        }
    }
}
