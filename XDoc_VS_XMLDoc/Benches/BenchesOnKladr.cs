using BenchmarkDotNet.Attributes;
using System.Xml;
using System.Xml.Linq;

namespace XDoc_VS_XMLDoc.Benches;

//[MemoryDiagnoser]
public class BenchesOnKladr
{
    private string _filePath;
    private byte[] _xmlByteArray;
    private XmlDocument _xmlDocBooks;
    private XDocument _xDocBooks;

    [GlobalSetup]
    public void Setup()
    {
        _filePath = "C:\\Users\\Кирилл\\source\\repos\\XDoc_VS_XMLDoc\\XDoc_VS_XMLDoc\\XmlSamples\\books.xml";
        _xmlByteArray = File.ReadAllBytes(_filePath);
        _xmlDocBooks = new();
        _xmlDocBooks.Load(_filePath);
        _xDocBooks = XDocument.Load(_filePath);
    }

    // Взаимодействие с КЛАДР XML (большая таблица)
    // Подгрузка из файла ods в XDoc/XMLDoc
    // Чтение, запись, удаление xml в XDoc/XMLDoc
}
