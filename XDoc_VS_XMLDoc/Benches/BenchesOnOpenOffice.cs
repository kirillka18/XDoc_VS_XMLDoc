using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Validators;
using Iced.Intel;
using System.Data;
using System.Globalization;
using System.IO.Compression;
using System.Reflection.PortableExecutable;
using System.Xml;
using System.Xml.Linq;
using System.Xml.XPath;

namespace XDoc_VS_XMLDoc.Benches;

//[MemoryDiagnoser]
public class BenchesOnOpenOffice
{
    private string _filePath;
    private byte[] _xmlByteArray;

    private static string[,] _namespaces = new string[,]
    {
            {"table", "urn:oasis:names:tc:opendocument:xmlns:table:1.0"},
            {"office", "urn:oasis:names:tc:opendocument:xmlns:office:1.0"},
            {"style", "urn:oasis:names:tc:opendocument:xmlns:style:1.0"},
            {"text", "urn:oasis:names:tc:opendocument:xmlns:text:1.0"},
            {"draw", "urn:oasis:names:tc:opendocument:xmlns:drawing:1.0"},
            {"fo", "urn:oasis:names:tc:opendocument:xmlns:xsl-fo-compatible:1.0"},
            {"dc", "http://purl.org/dc/elements/1.1/"},
            {"meta", "urn:oasis:names:tc:opendocument:xmlns:meta:1.0"},
            {"number", "urn:oasis:names:tc:opendocument:xmlns:datastyle:1.0"},
            {"presentation", "urn:oasis:names:tc:opendocument:xmlns:presentation:1.0"},
            {"svg", "urn:oasis:names:tc:opendocument:xmlns:svg-compatible:1.0"},
            {"chart", "urn:oasis:names:tc:opendocument:xmlns:chart:1.0"},
            {"dr3d", "urn:oasis:names:tc:opendocument:xmlns:dr3d:1.0"},
            {"math", "http://www.w3.org/1998/Math/MathML"},
            {"form", "urn:oasis:names:tc:opendocument:xmlns:form:1.0"},
            {"script", "urn:oasis:names:tc:opendocument:xmlns:script:1.0"},
            {"ooo", "http://openoffice.org/2004/office"},
            {"ooow", "http://openoffice.org/2004/writer"},
            {"oooc", "http://openoffice.org/2004/calc"},
            {"dom", "http://www.w3.org/2001/xml-events"},
            {"xforms", "http://www.w3.org/2002/xforms"},
            {"xsd", "http://www.w3.org/2001/XMLSchema"},
            {"xsi", "http://www.w3.org/2001/XMLSchema-instance"},
            {"rpt", "http://openoffice.org/2005/report"},
            {"of", "urn:oasis:names:tc:opendocument:xmlns:of:1.2"},
            {"rdfa", "http://docs.oasis-open.org/opendocument/meta/rdfa#"},
            {"config", "urn:oasis:names:tc:opendocument:xmlns:config:1.0"}
    };


    [GlobalSetup]
    public void Setup()
    {
        _filePath = "C:\\Users\\Кирилл\\source\\repos\\XDoc_VS_XMLDoc\\XDoc_VS_XMLDoc\\XmlSamples\\Receipt.ods";
        var stream = File.Open(_filePath, FileMode.Open);

        using ZipArchive zArchive = new(stream);
        ZipArchiveEntry? entry = zArchive.GetEntry("content.xml");

        // Проверка на правильное считывание файла
        if (entry is null)
        {
            throw new InvalidOperationException();
        }

        var streamXML = entry.Open();

        using (var ms = new MemoryStream())
        {
            streamXML.CopyTo(ms);
            _xmlByteArray = ms.ToArray();
        }
    }

    [Benchmark]
    public XmlDocument LoadXml_XmlDocument_Load()
    {
        using MemoryStream ms = new MemoryStream(_xmlByteArray);
        XmlDocument doc = new XmlDocument();
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
    public void ReadXmlCellData_XmlDocument()
    {
        using MemoryStream ms = new MemoryStream(_xmlByteArray);
        XmlDocument doc = new XmlDocument();
        doc.Load(ms);

        XmlNamespaceManager nmsManager = new XmlNamespaceManager(doc.NameTable);

        for (int i = 0; i < _namespaces.GetLength(0); i++)
            nmsManager.AddNamespace(_namespaces[i, 0], _namespaces[i, 1]);

        DataSet odsFile = new DataSet("MyData");

        foreach (XmlNode tableNode in doc.SelectNodes("/office:document-content/office:body/office:spreadsheet/table:table", nmsManager))
            odsFile.Tables.Add(this.GetSheet(tableNode, nmsManager, ControlType.OnlyRead));
    }

    [Benchmark]
    public void ReadXmlCellData_XDocument()
    {
        using MemoryStream ms = new MemoryStream(_xmlByteArray);
        XDocument doc = XDocument.Load(ms);

        var reader = doc.CreateReader();
        XmlNamespaceManager nmsManager = new XmlNamespaceManager(reader.NameTable);

        for (int i = 0; i < _namespaces.GetLength(0); i++)
            nmsManager.AddNamespace(_namespaces[i, 0], _namespaces[i, 1]);

        var nodes = doc.XPathSelectElements("/office:document-content/office:body/office:spreadsheet/table:table", nmsManager);
        // Больше nmsManager не нужен

        foreach (XElement node in nodes)
            GetSheet(node, ControlType.OnlyRead);
    }

    [Benchmark]
    public void AppendXmlCellData_XmlDocument()
    {
        using MemoryStream ms = new MemoryStream(_xmlByteArray);
        XmlDocument doc = new XmlDocument();
        doc.Load(ms);

        XmlNamespaceManager nmsManager = new XmlNamespaceManager(doc.NameTable);

        for (int i = 0; i < _namespaces.GetLength(0); i++)
            nmsManager.AddNamespace(_namespaces[i, 0], _namespaces[i, 1]);

        DataSet odsFile = new DataSet("MyData");

        foreach (XmlNode tableNode in doc.SelectNodes("/office:document-content/office:body/office:spreadsheet/table:table", nmsManager))
            odsFile.Tables.Add(this.GetSheet(tableNode, nmsManager, ControlType.AddCell));
    }

    [Benchmark]
    public void AppendXmlCellData_XDocument()
    {
        using MemoryStream ms = new MemoryStream(_xmlByteArray);
        XDocument doc = XDocument.Load(ms);

        var reader = doc.CreateReader();
        XmlNamespaceManager nmsManager = new XmlNamespaceManager(reader.NameTable);

        for (int i = 0; i < _namespaces.GetLength(0); i++)
            nmsManager.AddNamespace(_namespaces[i, 0], _namespaces[i, 1]);

        var nodes = doc.XPathSelectElements("/office:document-content/office:body/office:spreadsheet/table:table", nmsManager);
        // Больше nmsManager не нужен

        foreach (XElement node in nodes)
            GetSheet(node, ControlType.AddCell);
    }

    [Benchmark]
    public void RemoveXmlCellData_XmlDocument()
    {
        using MemoryStream ms = new MemoryStream(_xmlByteArray);
        XmlDocument doc = new XmlDocument();
        doc.Load(ms);

        XmlNamespaceManager nmsManager = new XmlNamespaceManager(doc.NameTable);

        for (int i = 0; i < _namespaces.GetLength(0); i++)
            nmsManager.AddNamespace(_namespaces[i, 0], _namespaces[i, 1]);

        DataSet odsFile = new DataSet("MyData");

        foreach (XmlNode tableNode in doc.SelectNodes("/office:document-content/office:body/office:spreadsheet/table:table", nmsManager))
            odsFile.Tables.Add(this.GetSheet(tableNode, nmsManager, ControlType.DeleteCell));
    }

    [Benchmark]
    public void RemoveXmlCellData_XDocument()
    {
        using MemoryStream ms = new MemoryStream(_xmlByteArray);
        XDocument doc = XDocument.Load(ms);

        var reader = doc.CreateReader();
        XmlNamespaceManager nmsManager = new XmlNamespaceManager(reader.NameTable);

        for (int i = 0; i < _namespaces.GetLength(0); i++)
            nmsManager.AddNamespace(_namespaces[i, 0], _namespaces[i, 1]);

        var nodes = doc.XPathSelectElements("/office:document-content/office:body/office:spreadsheet/table:table", nmsManager);
        // Больше nmsManager не нужен

        foreach (XElement node in nodes)
            GetSheet(node, ControlType.DeleteCell);
    }

    #region Доп методы по обработке OpenOfficeXML
    private DataTable GetSheet(XmlNode tableNode, XmlNamespaceManager nmsManager, ControlType controlType)
    {
        DataTable sheet = new DataTable(tableNode.Attributes["table:name"].Value);

        XmlNodeList rowNodes = tableNode.SelectNodes("table:table-row", nmsManager);

        int rowIndex = 0;
        foreach (XmlNode rowNode in rowNodes)
            this.GetRow(rowNode, sheet, nmsManager, ref rowIndex, controlType);

        return sheet;
    }

    private void GetRow(XmlNode rowNode, DataTable sheet, XmlNamespaceManager nmsManager, ref int rowIndex, ControlType controlType)
    {
        XmlAttribute rowsRepeated = rowNode.Attributes["table:number-rows-repeated"];
        if (rowsRepeated == null || Convert.ToInt32(rowsRepeated.Value, CultureInfo.InvariantCulture) == 1)
        {
            while (sheet.Rows.Count < rowIndex)
                sheet.Rows.Add(sheet.NewRow());

            DataRow row = sheet.NewRow();

            XmlNodeList cellNodes = rowNode.SelectNodes("table:table-cell", nmsManager);

            int cellIndex = 0;
            foreach (XmlNode cellNode in cellNodes)
            {
                var value = GetCell(cellNode, row, nmsManager, ref cellIndex);

                // Если запущен бенч по добавлению - то дублируем только 1 ячейку в узловую строку
                if (controlType is ControlType.AddCell)
                {
                    if (value is "Акт 1 о работе и оказании услуг от бригадир")
                        rowNode.AppendChild(cellNode);
                }

                // Если запущен бенч по удалению - то удаляем только 1 ячейку в узловую строку
                if (controlType is ControlType.DeleteCell)
                {
                    if (value is "Акт 1 о работе и оказании услуг от бригадир")
                        rowNode.RemoveChild(cellNode);
                }
            }

            sheet.Rows.Add(row);

            rowIndex++;
        }
        else
        {
            rowIndex += Convert.ToInt32(rowsRepeated.Value, CultureInfo.InvariantCulture);
        }

        if (sheet.Rows.Count == 0)
        {
            sheet.Rows.Add(sheet.NewRow());
            sheet.Columns.Add();
        }
    }

    private string GetCell(XmlNode cellNode, DataRow row, XmlNamespaceManager nmsManager, ref int cellIndex)
    {
        XmlAttribute cellRepeated = cellNode.Attributes["table:number-columns-repeated"];
        string cellValue = this.ReadCellValue(cellNode);

        int repeats = 1;
        if (cellRepeated != null)
        {
            repeats = Convert.ToInt32(cellRepeated.Value, CultureInfo.InvariantCulture);
        }

        if (!String.IsNullOrEmpty(cellValue))
        {
            for (int i = 0; i < repeats; i++)
            {
                DataTable sheet = row.Table;

                while (sheet.Columns.Count <= cellIndex)
                    sheet.Columns.Add();

                row[cellIndex] = cellValue;

                cellIndex++;
            }
        }
        else
        {
            cellIndex += repeats;
        }
        return cellValue;
    }

    private string ReadCellValue(XmlNode cell)
    {
        XmlAttribute cellVal = cell.Attributes["office:value"];

        if (cellVal == null)
            return String.IsNullOrEmpty(cell.InnerText) ? null : cell.InnerText;
        else
            return cellVal.Value;
    }
    #endregion

    #region Доп методы по обработке OpenOfficeX

    public static void GetSheet(XElement tableNode, ControlType controlType)
    {
        XElement[] rowNodes = tableNode.Elements().Where(x => x.Name.LocalName == "table-row").ToArray();
        int rowIndex = 0;
        var calcSheet = new List<string>();
        foreach (XElement rowNode in rowNodes)
        {
            calcSheet.AddRange(GetRow(rowNode, rowIndex, controlType));
            rowIndex++;
        }
    }

    public static List<string> GetRow(XElement rowNode, int rowIndex, ControlType controlType)
    {
        XElement[] cellNodes = rowNode.Elements().Where(x => x.Name.LocalName == "table-cell").ToArray();
        int cellIndex = 0;
        var calcRow = new List<string>();

        foreach (XElement cellNode in cellNodes)
        {
            // решение для обычных ячеек
            var value = GetCell(cellNode, rowIndex, cellIndex);
            // Если запущен бенч по добавлению - то дублируем только 1 ячейку в узловую строку
            if (controlType is ControlType.AddCell)
            {
                if (value is "Акт 1 о работе и оказании услуг от бригадир")
                    rowNode.Add(cellNode);
            }

            // Если запущен бенч по удалению - то удаляем только 1 ячейку в узловую строку
            if (controlType is ControlType.DeleteCell)
            {
                if (value is "Акт 1 о работе и оказании услуг от бригадир")
                    cellNode.Remove();
            }

            calcRow.Add(value);
            cellIndex++;
        }

        return calcRow;
    }

    public static string GetCell(XElement cellNode, int rowIndex, int cellIndex)
    {
        string? textValue = cellNode.Elements().FirstOrDefault(x => x.Name.LocalName == "p")?.Value;
        return textValue;
    }
    #endregion
}
