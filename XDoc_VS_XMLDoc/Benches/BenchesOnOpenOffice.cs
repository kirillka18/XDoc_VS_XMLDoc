using BenchmarkDotNet.Attributes;
using System.IO.Compression;
using System.Reflection.PortableExecutable;
using System.Xml;
using System.Xml.Linq;

namespace XDoc_VS_XMLDoc.Benches;

//[MemoryDiagnoser]
public class BenchesOnOpenOffice
{
    private string _filePath;
    private byte[] _xmlByteArray;
    private Stream _xDocBooksStream;

    [GlobalSetup]
    public void Setup()
    {
        _filePath = "C:\\Users\\Кирилл\\source\\repos\\XDoc_VS_XMLDoc\\XDoc_VS_XMLDoc\\XmlSamples\\Receipt.xml";
        _xmlByteArray = File.ReadAllBytes(_filePath);

        var stream = File.Open(_filePath, FileMode.Open);

        using ZipArchive zArchive = new(stream);
        ZipArchiveEntry? entry = zArchive.GetEntry("content.xml");

        // Проверка на правильное считывание файла
        if (entry is null)
        {
            throw new InvalidOperationException();
        }

        _xDocBooksStream = entry.Open();
    }

    // Взаимодействие с OpenOffice Document (обычная таблица)
    // Подгрузка из файла ods в XDoc/XMLDoc
    // Чтение, запись, удаление xml в XDoc/XMLDoc (с и без неймспейсов)

    [Benchmark]
    public XmlDocument LoadXml_XmlDocument_Load()
    {
        XmlDocument doc = new XmlDocument();
        doc.Load(_xDocBooksStream);

        return doc;
    }


    [Benchmark]
    public XDocument LoadXml_XDocument_Load()
    {
        XDocument doc = XDocument.Load(_xDocBooksStream);

        return doc;
    }

    // Добавление, удаление xml в XDoc/XMLDoc
    //[Benchmark]
    public void AppendChildNode_XmlDocument()
    {
        
    }
}
