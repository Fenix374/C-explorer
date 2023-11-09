using System;
using System.IO;
using Newtonsoft.Json;
using System.Xml.Serialization;
using System.Text;

public class Figure
{
    public string Name { get; set; }
    public double Width { get; set; }
    public double Height { get; set; }

    public Figure() { }

    public Figure(string name, double width, double height)
    {
        Name = name;
        Width = width;
        Height = height;
    }
}

public class FileHandler
{
    private string _filePath;

    public FileHandler(string filePath)
    {
        _filePath = filePath;
    }

    private Figure DeserializeJson(string content)
    {
        return JsonConvert.DeserializeObject<Figure>(content);
    }

    private Figure DeserializeXml(string content)
    {
        XmlSerializer serializer = new XmlSerializer(typeof(Figure));
        using (TextReader reader = new StringReader(content))
        {
            return (Figure)serializer.Deserialize(reader);
        }
    }

    public Figure Load()
    {
        string content = File.ReadAllText(_filePath);
        string extension = Path.GetExtension(_filePath);

        switch (extension)
        {
            case ".json":
                return DeserializeJson(content);
            case ".xml":
                return DeserializeXml(content);
            default:
                return new Figure(content, 0, 0);
        }
    }

    private string SerializeJson(Figure figure)
    {
        return JsonConvert.SerializeObject(figure);
    }

    private string SerializeXml(Figure figure)
    {
        XmlSerializer xmlSerializer = new XmlSerializer(typeof(Figure));
        using (StringWriter textWriter = new StringWriter())
        {
            xmlSerializer.Serialize(textWriter, figure);
            return textWriter.ToString();
        }
    }

    public void Save(Figure figure, string newExtension)
    {
        string content;
        string newFilePath = Path.ChangeExtension(_filePath, newExtension);

        switch (newExtension)
        {
            case ".json":
                content = SerializeJson(figure);
                break;
            case ".xml":
                content = SerializeXml(figure);
                break;
            default:
                content = figure.Name;
                break;
        }

        File.WriteAllText(newFilePath, content);
    }
}

public class Program
{
    public static void Main()
    {
        Console.WriteLine("Введите путь к файлу:");
        string filePath = Console.ReadLine();

        FileHandler fileHandler = new FileHandler(filePath);
        Figure figure = fileHandler.Load();

        Console.WriteLine($"Name: {figure.Name}");
        Console.WriteLine($"Width: {figure.Width}");
        Console.WriteLine($"Height: {figure.Height}");

        Console.WriteLine("Нажмите F1 для сохранения файла, Esc для выхода.");

        while (true)
        {
            ConsoleKeyInfo keyInfo = Console.ReadKey(true);

            if (keyInfo.Key == ConsoleKey.F1)
            {
                Console.WriteLine("Введите новое расширение файла (.json, .xml, .txt):");
                string newExtension = Console.ReadLine();
                fileHandler.Save(figure, newExtension);
                Console.WriteLine($"Файл сохранен как {Path.GetFileName(filePath)}{newExtension}.");
            }
            else if (keyInfo.Key == ConsoleKey.Escape)
            {
                break;
            }
        }
    }
}
