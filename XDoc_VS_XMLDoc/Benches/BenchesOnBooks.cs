using BenchmarkDotNet.Attributes;
using System.Xml;
using System.Xml.Linq;

namespace XDoc_VS_XMLDoc.Benches;

[MemoryDiagnoser]
public class BenchesOnBooks
{
    private string _filePath;
    private byte[] _xmlByteArray;

    [GlobalSetup]
    public void Setup()
    {
        _filePath = "C:\\Users\\Кирилл\\source\\repos\\XDoc_VS_XMLDoc\\XDoc_VS_XMLDoc\\XmlSamples\\books.xml";
        _xmlByteArray = File.ReadAllBytes(_filePath);
    }

    //// Простейшие тесты с файлом books.xml
    //// Подгрузка из байтового массива/файла ods в XDoc/XMLDoc
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
        XmlElement newElement = doc.CreateElement("book");

        // Добавление атрибута к элементу
        newElement.SetAttribute("category", "fiction");

        // Создание и добавление дочерних элементов
        XmlElement titleElement = doc.CreateElement("title");
        titleElement.InnerText = "The Great Gatsby";
        newElement.AppendChild(titleElement);

        XmlElement authorElement = doc.CreateElement("author");
        authorElement.InnerText = "F. Scott Fitzgerald";
        newElement.AppendChild(authorElement);

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
        XElement newElement = new XElement("book",
            new XAttribute("category", "fiction"),
            new XElement("title", "The Great Gatsby"),
            new XElement("author", "F. Scott Fitzgerald")
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
        XmlNode nodeToRemove = doc.SelectSingleNode("catalog").SelectSingleNode("//book[@id='bk102']");

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
        XElement nodeToRemove = doc.Element("catalog")
                                   .Elements("book")
                                   .FirstOrDefault(b => (string)b.Attribute("id") == "bk102");

        // Удаление узла из документа
        nodeToRemove?.Remove();
    }
}
