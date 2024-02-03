// See https://aka.ms/new-console-template for more information

Console.WriteLine("请输入文件路径：");
string? path = Console.ReadLine();
while (string.IsNullOrEmpty(path) || !File.Exists(path))
{
    Console.WriteLine("请输入文件路径：");
    path = Console.ReadLine();
}

string sourceFilePath = path;
string sourceFileName = Path.GetFileName(sourceFilePath);
List<string> rows = File.ReadAllLines(sourceFilePath).Distinct().ToList();
int pageIndex = 0;
int pagSize = 50_000;

string saveFilePath = Path.Combine(AppContext.BaseDirectory, "Files");
if (Directory.Exists(saveFilePath))
{
    foreach (string file in Directory.GetFiles(saveFilePath))
    {
        File.Delete(file);
    }
}
else
{
    Directory.CreateDirectory(saveFilePath);
}

List<string> todoSaveList = rows.Take(pagSize).ToList();
while (todoSaveList is { Count: > 0 })
{
    string fileName = Path.Combine(saveFilePath, $"{pageIndex}_{sourceFileName}");
    File.WriteAllLines(fileName, todoSaveList);

    pageIndex++;
    todoSaveList = rows.Skip(pageIndex * pagSize).Take(pagSize).ToList();

    Console.WriteLine($"{Path.GetFileName(fileName)} ---> 保存成功！");
}

Console.WriteLine("按Q退出：");
while (Console.ReadKey().Key == ConsoleKey.Q)
{
    Environment.Exit(0);
}