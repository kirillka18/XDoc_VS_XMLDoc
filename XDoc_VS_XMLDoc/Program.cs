using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Running;
using System;
using System.Text;
using System.Xml.Linq;
using System.Xml;
using System.Reflection;

BenchmarkRunner.Run<XDoc_VS_XMLDoc>();

//[MemoryDiagnoser]
public class XDoc_VS_XMLDoc
{
    public string _filePath;
    public byte[] _xmlByteArray;

    [GlobalSetup]
    public void Setup()
    {
        _filePath = "C:\\Users\\Кирилл\\source\\repos\\XDoc_VS_XMLDoc\\XDoc_VS_XMLDoc\\XmlSamples\\books.xml";

        _xmlByteArray = File.ReadAllBytes(_filePath);
    }

    // Простейшие тесты загрузки/чтения/изменения с файлом books.xml
    [Benchmark]
    public XmlDocument LoadXml_XmlDocument_Load()
    {
        XmlDocument doc = new XmlDocument();
        using MemoryStream ms = new MemoryStream(_xmlByteArray);
        doc.Load(ms);

        return doc;
    }

    [Benchmark]
    public XDocument LoadXml_XDocument_Load()
    {
        using MemoryStream ms = new MemoryStream(_xmlByteArray);
        XDocument doc = XDocument.Load(ms);

        return doc;
    }

    [Benchmark]
    public XmlDocument LoadXml_XmlDocument_FromFile()
    {
        XmlDocument doc = new XmlDocument();
        doc.Load(_filePath);

        return doc;
    }

    [Benchmark]
    public XDocument LoadXml_XDocument_FromFile()
    {
        XDocument doc = XDocument.Load(_filePath);

        return doc;
    }

    // Взаимодействие с OpenOffice Document (обычная таблица)
    // Подгрузка из файла ods в XDoc/XMLDoc
    // Чтение, запись, удаление xml в XDoc/XMLDoc (с и без неймспейсов)

    // Взаимодействие с КЛАДР XML (большая таблица)
    // Подгрузка из файла ods в XDoc/XMLDoc
    // Чтение, запись, удаление xml в XDoc/XMLDoc
}