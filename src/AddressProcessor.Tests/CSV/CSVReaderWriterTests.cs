using AddressProcessing.CSV;
using NSubstitute;
using NUnit.Framework;

namespace Csv.Tests
{
    [TestFixture]
    public class CSVReaderWriterTests
    {
        private IFileReaderWriter _component;
        private IFileReader _reader;
        private IFileWriter _writer;


        [SetUp]
        public void Setup()
        {
            _reader = Substitute.For<IFileReader>();
            _writer = Substitute.For<IFileWriter>();
             
            _component = new CSVReaderWriter(_reader, _writer);
        }

        [Test]
        public void OpenFile_InReadMode_FileIsOpenedInReadMode()
        {
             //Act
            var fileName = "someFileName";
            _component.Open(fileName, CSVReaderWriter.Mode.Read);

            //Assert
            Assert.IsNotNull(_reader);
            _reader.Received().Open(fileName);

            _writer.DidNotReceive().Open(fileName);
        }

        [Test]
        public void OpenFile_InWriteMode_FileIsOpenedInWriteMode()
        {
            //Act
            var fileName = "someFileName";
            _component.Open(fileName, CSVReaderWriter.Mode.Write);

            //Assert
            Assert.IsNotNull(_writer);
            _writer.Received().Open(fileName);

            _reader.DidNotReceive().Open(fileName);
        }

        [Test]
        public void ReadMethod_WhenInvoked_ReturnsTextInColumns()
        {
            // Arrange
            _reader.ReadLine().Returns($"test1\ttest2");
            string column1 = string.Empty;
            string column2 = string.Empty;

            //Act 
            _component.Read(out column1, out column2);

            //Assert
            Assert.AreEqual(column1,"test1");
            Assert.AreEqual(column2,"test2");
        }

        [Test]
        public void WriteMethod_WhenInvoked_WritesToConsole()
        {
            // Act
            _component.Write("col1", "col2");

            // Assert
            _writer.Received().WriteLine($"col1\tcol2");      
        }

        [Test]
        public void CloseMethod_WhenInvoked_ClosesStream()
        {
            // Act 
            _component.Close();

            // Assert
            _writer.Received().Close();
            _reader.Received().Close();
        }
    }
}
