using IPFilter.Extensions;
using IPFilter.Parameters;
using System.Globalization;
using System.Net;
using System.Text;

namespace IPFilter
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Parameter parameters;
            try
            {
                if (args.Length == 0)
                {
                    Console.WriteLine("Аргументы будут полностью загружены из файла конфигурации при его наличии.");
                    parameters = ParameterParser.ParseFromXml();
                }
                else
                {
                    parameters = ParameterParser.ParseFromArgs(args);
                }
            }
            catch (Exception ex)
            {
                //считаю, что программа должна работать как классическая консольная утилита и
                //завершать работу при получении невалидных параметров
                Console.WriteLine($"Ошибка: {ex.Message}");
                return;
            }

            var type = typeof(Parameter);
            var fields = type.GetProperties();
            foreach (var field in fields)
                if (field.GetValue(parameters) == null)
                {
                    Console.WriteLine($"Ошибка: не введен параметр {field}");
                    return;
                }

            if (parameters.NeedSave)
                parameters.SaveConfig();

            string log;
            try
            {
                log = File.ReadAllText(parameters.LogPath);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка: {ex.Message}");
                return;
            }

            var lines = log.Split('\n');

            var table = new List<(IPAddress, DateTime)>();

            foreach (var line in lines)
            {
                try
                {
                    //сейчас бы использовать StringBuilder для замены одного символа в строке....
                    var buff = new StringBuilder(line);
                    buff[line.IndexOf(':')] = ';';

                    var row = buff.ToString().Split(';');
                    var address = IPAddress.Parse(row[0]);
                    var date = DateTime.ParseExact(row[1].Replace("\r", String.Empty), "yyyy.MM.dd HH:mm:ss", CultureInfo.InvariantCulture);
                    table.Add((address, date));
                }
                catch
                {
                    Console.WriteLine("Не удалось распарсить содержимое строки: " + line);
                }
            }

            var res = table.Where(x => x.Item2 <= parameters.EndTime && x.Item2 >= parameters.StartTime &&
                                       x.Item1.Address <= parameters.AddressMask.Address &&
                                       x.Item1.Address >= parameters.AddressStart.Address).GroupBy(x => x.Item1);

            using (var stream = new StreamWriter(parameters.OutputPath))
            {
                foreach (var row in res)
                {
                    Console.WriteLine($"{row.Key} {row.Count()}");
                    stream.WriteLine($"{row.Key} {row.Count()}");
                }
            }
        }
    }
}