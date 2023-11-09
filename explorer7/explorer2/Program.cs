using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;

public class FileManager
{
    private static string currentDirectory;

    public static void Start()
    {
        Console.WriteLine("Выберите диск:");

        DriveInfo[] drives = DriveInfo.GetDrives();
        for (int i = 0; i < drives.Length; i++)
        {
            Console.WriteLine($"{i + 1}. {drives[i].Name}");
        }

        int selectedDriveIndex = ReadIntegerInput("Введите номер диска:");
        if (selectedDriveIndex < 1 || selectedDriveIndex > drives.Length)
        {
            Console.WriteLine("Неправильный номер диска.");
            return;
        }

        currentDirectory = drives[selectedDriveIndex - 1].RootDirectory.FullName;
        ShowDirectoryContents();
    }

    private static void ShowDirectoryContents()
    {
        int currentItemIndex = 0;

        while (true)
        {
            Console.Clear();
            Console.WriteLine($"Текущая директория: {currentDirectory}");

            DirectoryInfo directoryInfo = new DirectoryInfo(currentDirectory);
            FileSystemInfo[] fileSystemInfos = directoryInfo.GetFileSystemInfos();

            for (int i = 0; i < fileSystemInfos.Length; i++)
            {
                FileSystemInfo info = fileSystemInfos[i];
                string itemName = info is DirectoryInfo ? $"[{info.Name}]" : info.Name;
                string selectionMarker = i == currentItemIndex ? ">" : " ";
                Console.WriteLine($"{selectionMarker} {i + 1}. {itemName}");
            }

            var key = Console.ReadKey(true).Key;

            switch (key)
            {
                case ConsoleKey.UpArrow: 
                    currentItemIndex = (currentItemIndex - 1 + fileSystemInfos.Length) % fileSystemInfos.Length;
                    break;
                case ConsoleKey.DownArrow: 
                    currentItemIndex = (currentItemIndex + 1) % fileSystemInfos.Length;
                    break;
                case ConsoleKey.Enter:
                    FileSystemInfo selectedInfo = fileSystemInfos[currentItemIndex];
                    if (selectedInfo is DirectoryInfo)
                    {
                        currentDirectory = selectedInfo.FullName;
                    }
                    else if (selectedInfo is FileInfo)
                    {
                        OpenFile(selectedInfo.FullName);
                    }
                    break;
                case ConsoleKey.Escape: 
                    GoToParentDirectory();
                    break;
            }
        }
    }


    private static void GoToParentDirectory()
    {
        DirectoryInfo currentInfo = new DirectoryInfo(currentDirectory);
        if (currentInfo.Parent != null)
        {
            currentDirectory = currentInfo.Parent.FullName;
        }
    }

    private static void OpenFile(string filePath)
    {
        {
            try
            {
                Process.Start(new ProcessStartInfo(filePath) { UseShellExecute = true });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Could not open file: {ex.Message}");
            }
        }

        Console.WriteLine($"Запуск файла: {filePath}");
    }

    private static int ReadIntegerInput(string message)
    {
        int result;
        while (true)
        {
            Console.Write($"{message} ");
            if (int.TryParse(Console.ReadLine(), out result))
            {
                return result;
            }
            else
            {
                Console.WriteLine("Неправильный ввод. Попробуйте еще раз.");
            }
        }
    }
}


public class Program
{
    public static void Main()
    {
        FileManager.Start();
    }
}
