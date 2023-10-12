const string configFileName = "config.ini";
if (!File.Exists(configFileName))
{
    Console.WriteLine("Ошибка: нет конфигурационного файла");
    return;
}

var lines = File.ReadAllLines(configFileName);
var settings = lines.ToDictionary(
    x => x.Split('=').First(),
    y => y.Split('=').Last());

var path = settings["pathToDelete"];
var url = settings["urlToCheck"]; 
var pingFrequency = TimeSpan.Parse(settings["pingFrequency"]);

if (string.IsNullOrEmpty(path) || !Directory.Exists(path))
{
    Console.WriteLine("Ошибка: папка для удаления не указана или не существует");
    return;
}
Console.WriteLine($"Папка {path} найдена");

if (string.IsNullOrEmpty(url))
{
    Console.WriteLine("Ошибка: урл для проверки не указан");
    return;
}
Console.WriteLine($"Путь {url} указан");

while (true)
{
    using (var client = new HttpClient())
    {
        var getResult = await client.GetAsync(url);
        if (!getResult.IsSuccessStatusCode)
        {
            var files = Directory.GetFiles(path);
            for (var i = 0; i < files.Count(); i++)
            {
                File.Delete(files[i]);
            }
            Console.WriteLine("Url не найден, удаление произведено");
        }
        else
            Console.WriteLine("Url найден, удаление не нужно");
    }
    Thread.Sleep(pingFrequency);
}