using BenchmarkDotNet.Attributes;
using System.Xml;
using System.Xml.Linq;

namespace XDoc_VS_XMLDoc.Benches;

[MemoryDiagnoser]
public class BenchesOnKladr
{
    private string _filePath;
    private byte[] _xmlByteArray;

    [GlobalSetup]
    public void Setup()
    {
        _filePath = "C:\\Users\\Кирилл\\source\\repos\\XDoc_VS_XMLDoc\\XDoc_VS_XMLDoc\\XmlSamples\\AS_HOUSES_PARAMS_20240425.XML";
        _xmlByteArray = File.ReadAllBytes(_filePath);
    }

    // Взаимодействие с КЛАДР XML (большая таблица)
    // Подгрузка из файла ods в XDoc/XMLDoc
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

    // Добавление, удаление xml в XDoc/XMLDoc
    [Benchmark]
    public void AppendChildNode_XmlDocument()
    {
        // Берем байтовый массив файла books.xml и создаем doc для правильного теста
        XmlDocument doc = new XmlDocument();
        using MemoryStream ms = new MemoryStream(_xmlByteArray);
        doc.Load(ms);

        // Создание нового элемента
        XmlElement newElement = doc.CreateElement("PARAM");

        // Добавление атрибута к элементу
        newElement.SetAttribute("ID", "1478659922");
        newElement.SetAttribute("OBJECTID", "161336797");
        newElement.SetAttribute("CHANGEID", "572982067");
        newElement.SetAttribute("CHANGEIDEND", "593820497");
        newElement.SetAttribute("TYPEID", "4");
        newElement.SetAttribute("UPDATEDATE", "2024-04-22");
        newElement.SetAttribute("VALUE", "4010");
        newElement.SetAttribute("STARTDATE", "2023-11-16");
        newElement.SetAttribute("ENDDATE", "2024-04-22");

        // Добавление нового элемента в корневой элемент документа
        doc.DocumentElement.AppendChild(newElement);
    }

    [Benchmark]
    public void AppendChildNode_XDocument()
    {
        // Берем байтовый массив файла books.xml и создаем doc для правильного теста
        using MemoryStream ms = new MemoryStream(_xmlByteArray);
        XDocument doc = XDocument.Load(ms);

        // Создание нового элемента
        XElement newElement = new XElement("PARAM",
            new XAttribute("ID", "1478659922"),
            new XAttribute("OBJECTID", "161336797"),
            new XAttribute("CHANGEID", "572982067"),
            new XAttribute("CHANGEIDEND", "593820497"),
            new XAttribute("TYPEID", "4"),
            new XAttribute("UPDATEDATE", "2024-04-22"),
            new XAttribute("VALUE", "4010"),
            new XAttribute("STARTDATE", "2023-11-16"),
            new XAttribute("ENDDATE", "2024-04-22")
        );

        // Добавление нового элемента в корневой элемент документа
        doc.Root.Add(newElement);
    }

    [Benchmark]
    public void RemoveChildNode_XmlDocument()
    {
        // Берем байтовый массив файла books.xml и создаем doc для правильного теста
        XmlDocument doc = new XmlDocument();
        using MemoryStream ms = new MemoryStream(_xmlByteArray);
        doc.Load(ms);

        // Нахождение узла, который нужно удалить
        XmlNode nodeToRemove = doc.SelectSingleNode("PARAMS").SelectSingleNode("//PARAM[@ID='1442587212']");

        // Удаление узла из документа
        nodeToRemove.ParentNode.RemoveChild(nodeToRemove);
    }

    [Benchmark]
    public void RemoveChildNode_XDocument()
    {
        // Берем байтовый массив файла books.xml и создаем doc для правильного теста
        using MemoryStream ms = new MemoryStream(_xmlByteArray);
        XDocument doc = XDocument.Load(ms);

        // Нахождение узла, который нужно удалить
        XElement nodeToRemove = doc.Element("PARAMS")
                                   .Elements("PARAM")
                                   .FirstOrDefault(b => (string)b.Attribute("ID") == "1442587212");

        // Удаление узла из документа
        nodeToRemove?.Remove();
    }
}
