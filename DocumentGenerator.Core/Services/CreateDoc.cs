using System.Reactive.Subjects;
using OfficeOpenXml;

namespace DocumentGenerator.Core.Services;

public class CreateDoc
{
    public static DateTime dateTime;
    private static ISubject<string> _messageSubject;

    public static void CreateAllDoc(string FilePathReference, string filePathIn, string foldelPathOut,
        ref ISubject<string> messageSubject,
        string substrateFilePath = "")
    {
        _messageSubject = messageSubject;
        dateTime = DateTime.Now;

        if (CreateDictionaryReference(out Dictionary<string, ReferenceMaterialDictionary> references,
                ref FilePathReference) &&
            CreateDictionaryCity(out Dictionary<string, string> cities, ref filePathIn))
        {
            /*WordDiplomSertificat doc = new WordDiplomSertificat(FilePathReference, filePathIn, foldelPathOut, players, cities, references, substrateFilePath);
            */
            // Создаем необходимые документы

            _messageSubject.OnNext("Файлы прочитаны, начало создания!");

            CreateListForDiplomsOffline(out List<PlayersListStruct> playersOnline, ref filePathIn);
            CreateCityes(foldelPathOut, playersOnline, cities, "города-очно");
            CreateModerOffline(foldelPathOut, playersOnline, references);


            CreateListForDiplomsOnline(out List<PlayersListStruct> players, ref filePathIn);
            CreateCityes(foldelPathOut, players, cities);
            CreateModerOnline(foldelPathOut, players, references);

            /*CreateEnd(filePathIn, foldelPathOut, references);*/

            _messageSubject.OnNext($"Всего {players.Count} строк обработано.\n" +
                                   $"Программа выполнена за {(DateTime.Now - dateTime).TotalSeconds:F2} сек.");
        }
    }

    public static void CreateEnd(string folderPathIn, string foldelPathOut,
        Dictionary<string, ReferenceMaterialDictionary> referencesDic)
    {
        // Создаем основной файл результатов
        using var package = new ExcelPackage();
        var worksheet = package.Workbook.Worksheets.Add("Сводка");

        // Заголовки для первого листа
        worksheet.Cells["A1"].Value = "Название команды";
        worksheet.Cells["B1"].Value = "ФИО участника";
        worksheet.Cells["C1"].Value = "Дата рождения";
        worksheet.Cells["D1"].Value = "Учебное заведение";
        worksheet.Cells["E1"].Value = "ФИО Руководителя";
        worksheet.Cells["F1"].Value = "Населенный пункт";
        worksheet.Cells["G1"].Value = "E-mail";
        worksheet.Cells["H1"].Value = "участвовал в";

        // Форматирование заголовков
        for (int col = 1; col <= 8; col++)
        {
            worksheet.Column(col).AutoFit();
            worksheet.Cells[1, col].Style.WrapText = true;
            worksheet.Cells[1, col].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
            worksheet.Cells[1, col].Style.Font.Bold = true;
        }

        // Создаем второй файл для прибывших участников
        using var tytPackage = new ExcelPackage();
        var worksheetTyt = tytPackage.Workbook.Worksheets.Add("Прибывшие участники");

        // Заголовки для второго листа
        worksheetTyt.Cells["A1"].Value = "Название команды";
        worksheetTyt.Cells["B1"].Value = "ФИО участника";
        worksheetTyt.Cells["C1"].Value = "Дата рождения";
        worksheetTyt.Cells["D1"].Value = "Учебное заведение";
        worksheetTyt.Cells["E1"].Value = "ФИО Руководителя";
        worksheetTyt.Cells["F1"].Value = "Населенный пункт";
        worksheetTyt.Cells["G1"].Value = "E-mail";
        worksheetTyt.Cells["H1"].Value = "участвовал в";

        // Форматирование заголовков второго листа
        for (int col = 1; col <= 8; col++)
        {
            worksheetTyt.Column(col).AutoFit();
            worksheetTyt.Cells[1, col].Style.WrapText = true;
            worksheetTyt.Cells[1, col].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
            worksheetTyt.Cells[1, col].Style.Font.Bold = true;
        }

        int count = 2; // Счетчик строк для основного файла
        int countTyt = 2; // Счетчик строк для второго файла

        foreach (var kvp in referencesDic)
        {
            string path = Path.Combine(folderPathIn, kvp.Key + ".xlsx");

            if (File.Exists(path))
            {
                using var inputPackage = new ExcelPackage(new FileInfo(path));
                var inputSheet = inputPackage.Workbook.Worksheets.FirstOrDefault();

                if (inputSheet != null)
                {
                    // Добавляем название конкурса
                    worksheet.Cells[count, 1].Merge = true;
                    worksheet.Cells[count, 1].Value =
                        $"{kvp.Value.TypeCompetition} «{kvp.Value.NameCompetition}», код {kvp.Key}";
                    worksheet.Cells[count, 1].Style.HorizontalAlignment =
                        OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                    worksheet.Cells[count, 1].Style.Font.Bold = true;
                    worksheet.Row(count).Height = 30;

                    count++;

                    // Обработка мест призеров
                    for (int place = 1; place <= 3; place++)
                    {
                        for (int i = 7; i <= inputSheet.Dimension.Rows; i++)
                        {
                            if (inputSheet.Cells[i, 10]?.Value?.ToString() == place.ToString())
                            {
                                worksheet.Cells[count, 1].Value = place;
                                worksheet.Cells[count, 2].Value = inputSheet.Cells[i, 11]?.Value;
                                worksheet.Cells[count, 3].Value = inputSheet.Cells[i, 2]?.Value;
                                worksheet.Cells[count, 4].Value = inputSheet.Cells[i, 3]?.Value;
                                worksheet.Cells[count, 5].Value = inputSheet.Cells[i, 4]?.Value;
                                worksheet.Cells[count, 6].Value = inputSheet.Cells[i, 5]?.Value;
                                worksheet.Cells[count, 7].Value = inputSheet.Cells[i, 6]?.Value;
                                worksheet.Cells[count, 8].Value = inputSheet.Cells[i, 7]?.Value;

                                count++;
                            }
                        }
                    }

                    // Обработка прибывших участников
                    for (int i = 7; i <= inputSheet.Dimension.Rows; i++)
                    {
                        if (inputSheet.Cells[i, 8]?.Value?.ToString() == "1")
                        {
                            worksheetTyt.Cells[countTyt, 1].Value = inputSheet.Cells[i, 2]?.Value;
                            worksheetTyt.Cells[countTyt, 2].Value = inputSheet.Cells[i, 3]?.Value;
                            worksheetTyt.Cells[countTyt, 3].Value = inputSheet.Cells[i, 4]?.Value;
                            worksheetTyt.Cells[countTyt, 4].Value = inputSheet.Cells[i, 5]?.Value;
                            worksheetTyt.Cells[countTyt, 5].Value = inputSheet.Cells[i, 6]?.Value;
                            worksheetTyt.Cells[countTyt, 6].Value = inputSheet.Cells[i, 7]?.Value;
                            worksheetTyt.Cells[countTyt, 7].Value = inputSheet.Cells[i, 9]?.Value;
                            worksheetTyt.Cells[countTyt, 8].Value = kvp.Key;

                            countTyt++;
                        }
                    }

                    _messageSubject.OnNext(
                        $"Прочитан файл {kvp.Value.TypeCompetition}, код {kvp.Key}. Всего участников посетило -> {countTyt - 2}");
                }
            }
            else
            {
                _messageSubject.OnNext($"Файл к {kvp.Value.TypeCompetition} - {kvp.Key} -> не найден");
            }
        }

        // Сохраняем оба файла
        if (!Directory.Exists(foldelPathOut))
        {
            Directory.CreateDirectory(foldelPathOut);
        }

        var resultsFilePath = Path.Combine(foldelPathOut, "Результаты.xlsx");
        var participantsFilePath = Path.Combine(foldelPathOut, "Прибывшие_участники.xlsx");

        File.WriteAllBytes(resultsFilePath, package.GetAsByteArray());
        File.WriteAllBytes(participantsFilePath, tytPackage.GetAsByteArray());

        _messageSubject.OnNext($"Файлы успешно созданы: {resultsFilePath} и {participantsFilePath}");
    }

    public static void CreateCityes(string foldelPathOut,
        List<PlayersListStruct> playerList, Dictionary<string, string> citiesDic, string whereLine = "города-дист")
    {
        playerList.Sort((item, second) =>
            string.Compare(item.FioPlayers, second.FioPlayers, StringComparison.Ordinal));


        Dictionary<string, HashSet<string>> schools = new Dictionary<string, HashSet<string>>();

        foreach (var kvpair in citiesDic)
        {
            schools[kvpair.Key] = new HashSet<string>();

            foreach (var people in playerList)
            {
                if (kvpair.Key.Equals(people.CityPlayers))
                {
                    schools[kvpair.Key].Add(people.SchoolPlayers);
                }
            }
        }

        int k = 0;

        foreach (var kvpair in citiesDic)
        {
            foreach (var currentSchool in schools[kvpair.Key])
            {
                _messageSubject.OnNext($"Создание файла городу: {kvpair.Key}");

                using var package = new ExcelPackage();
                var worksheet = package.Workbook.Worksheets.Add("Лист первичной регистрации");

                // Объединяем ячейки A1-J1
                worksheet.Cells["A1:J1"].Merge = true;
                worksheet.Cells["A1"].Value =
                    "XI Межрегиональный открытый фестиваль научно-технического творчества «РОБОАРТ-2025»";
                worksheet.Cells["A1"].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                worksheet.Cells["A1"].Style.Font.Bold = true;

                // Лист первичной регистрации
                worksheet.Cells["A2:J2"].Merge = true;
                worksheet.Cells["A2"].Value = "Лист первичной регистрации участников";
                worksheet.Cells["A2"].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;

                // Название города
                worksheet.Cells["A3:J3"].Merge = true;
                worksheet.Cells["A3"].Value = kvpair.Value;
                worksheet.Cells["A3"].Style.Font.Bold = true;
                worksheet.Cells["A3"].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;

                // Название школы
                worksheet.Cells["A4:J4"].Merge = true;
                worksheet.Cells["A4"].Value = currentSchool;
                worksheet.Cells["A4"].Style.Font.UnderLine = true;
                worksheet.Cells["A4"].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;

                // Заголовки таблицы
                worksheet.Cells["A6:A7"].Merge = true;
                worksheet.Cells["A6:A7"].Style.WrapText = true;
                worksheet.Cells["A6"].Value = "№ П/П";
                
                worksheet.Cells["B6:B7"].Merge = true;
                worksheet.Cells["B6:B7"].Style.WrapText = true;
                worksheet.Cells["B6"].Value = "Фамилия, Имя, Отчество участника";
                
                worksheet.Cells["C6:C7"].Merge = true;
                worksheet.Cells["C6"].Value = "Фамилия, Имя, Отчество руководителя команды";
                worksheet.Cells["C6"].Style.WrapText = true;
                
                worksheet.Cells["D6:D7"].Merge = true;
                worksheet.Cells["D6"].Value = "Согласие на обработку персональных данных участника";
                worksheet.Cells["D6"].Style.WrapText = true;
                
                worksheet.Cells["E6:E7"].Merge = true;
                worksheet.Cells["E6"].Value = "Согласие на обработку персональных данных руководителя";
                worksheet.Cells["E6"].Style.WrapText = true;
                
                worksheet.Cells["F6:F7"].Merge = true;
                worksheet.Cells["F6"].Value = "Приказ или расписка ответственности";
                worksheet.Cells["F6"].Style.WrapText = true;
                
                worksheet.Cells["G6:H6"].Merge = true;
                worksheet.Cells["G6"].Value = "Талоны на питание";
                worksheet.Cells["G6"].Style.WrapText = true;
                
                worksheet.Cells["G7"].Value = "обед";
                worksheet.Cells["H7"].Value = "завтрак";
                
                worksheet.Cells["I6:I7"].Merge = true;
                worksheet.Cells["I6"].Value = "Значки";
                worksheet.Cells["I6"].Style.WrapText = true;
                
                worksheet.Cells["J6:J7"].Merge = true;
                worksheet.Cells["J6"].Value = "Подпись педагога";
                worksheet.Cells["J6"].Style.WrapText = true;
                
                // Форматирование границ
                for (int col = 1; col <= 10; col++)
                {
                    worksheet.Column(col).Width = 15;
                    worksheet.Cells[$"{(char)('A' + col - 1)}6:{(char)('A' + col - 1)}7"].BorderOutline();
                }

                int row = 8;
                HashSet<string> swap = new HashSet<string>();

                foreach (var people in playerList)
                {
                    if (people.CityPlayers.Equals(kvpair.Key) && people.SchoolPlayers.Equals(currentSchool) &&
                        !swap.Contains(people.FioPlayers))
                    {
                        swap.Add(people.FioPlayers);

                        worksheet.Cells[row, 1].Value = row - 7;
                        worksheet.Cells[row, 2].Value = people.FioPlayers;
                        worksheet.Cells[row, 3].Value = people.TeacherPlayers;
                        worksheet.Cells[row, 4].Value = ""; // Согласие на обработку данных участника
                        worksheet.Cells[row, 5].Value = ""; // Согласие на обработку данных руководителя
                        worksheet.Cells[row, 6].Value = ""; // Приказ или расписка ответственности
                        worksheet.Cells[row, 7].Value = "1"; // Талоны на питание (обед)
                        worksheet.Cells[row, 8].Value = "1"; // Талоны на питание (завтрак)
                        worksheet.Cells[row, 9].Value = ""; // Значки
                        worksheet.Cells[row, 10].Value = ""; // Подпись педагога

                        // Форматирование строк
                        for (int col = 1; col <= 10; col++)
                        {
                            worksheet.Cells[row, col].BorderOutline();
                        }

                        row++;
                    }
                }
                
                worksheet.Cells[row, 2].Value = "Руководитель команды";
                worksheet.Cells[row, 7].Value = "1"; // Количество обедов
                worksheet.Cells[row, 8].Value = "1"; // Количество завтраков
                
                row += 1;
                
                // Итоговая строка
                worksheet.Cells[row, 2].Value = "Итого";
                worksheet.Cells[row, 2].Style.Font.Bold = true;
                worksheet.Cells[row, 7].Value = row - 8; // Количество обедов
                worksheet.Cells[row, 7].Style.Font.Bold = true;
                worksheet.Cells[row, 8].Value = row - 8; // Количество завтраков
                worksheet.Cells[row, 8].Style.Font.Bold = true;

                row += 1;
                
                for (int col = 1; col <= 10; col++)
                {
                    worksheet.Cells[row, col].BorderOutline();
                    worksheet.Cells[row-1, col].BorderOutline();
                    worksheet.Cells[row-2, col].BorderOutline();
                }

                row += 2;

                worksheet.Cells[row, 2].Value = "Сведения заполнил";
                worksheet.Cells[$"C{row}:E{row}"].Merge = true;
                worksheet.Cells[$"C{row}:E{row}"].Style.Border.Bottom.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                worksheet.Cells[$"C{row + 1}:E{row + 1}"].Merge = true;
                worksheet.Cells[$"C{row + 1}"].Value = "ФИО";
                worksheet.Cells[$"C{row}"].Style.HorizontalAlignment =
                    OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                worksheet.Cells[$"C{row + 1}"].Style.HorizontalAlignment =
                    OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                worksheet.Cells[$"I{row}:J{row}"].Merge = true;
                worksheet.Cells[$"I{row}:J{row}"].Style.Border.Bottom.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                worksheet.Cells[$"I{row + 1}:J{row + 1}"].Merge = true;
                worksheet.Cells[$"I{row + 1}"].Value = "Подпись";
                worksheet.Cells[$"I{row + 1}"].Style.HorizontalAlignment =
                    OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                worksheet.Cells[$"I{row}"].Style.HorizontalAlignment =
                    OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;

                worksheet.Column(1).Width = 7;
                worksheet.Column(2).Width = 33;
                worksheet.Column(3).Width = 33;
                worksheet.Column(4).Width = 22;
                worksheet.Column(5).Width = 22;
                worksheet.Column(6).Width = 16;
                worksheet.Column(7).Width = 7;
                worksheet.Column(8).Width = 7;
                worksheet.Column(9).Width = 7;
                worksheet.Column(10).Width = 17;

                worksheet.Rows[6].Height = 28.8;

                // Сохраняем файл
                if (!Directory.Exists(Path.Combine(foldelPathOut, whereLine)))
                {
                    Directory.CreateDirectory(Path.Combine(foldelPathOut, whereLine));
                }

                var cityFileName = Path.Combine(foldelPathOut, whereLine,
                        $"{kvpair.Key.Replace(@"\", "").Replace("\"", "")} {currentSchool.Replace("\n", "").Replace(@"\", "").Replace("\"", "")}.xlsx")
                    .Replace("/", "");

                File.WriteAllBytes(cityFileName, package.GetAsByteArray());

                _messageSubject.OnNext($"Создан файл: {kvpair.Value} -> {row - 8} участников");
                k += row - 8;
            }
        }

        _messageSubject.OnNext($"Всего: {k} участников");
    }

    public static void CreateModerOnline(string foldelPathOut, List<PlayersListStruct> playerList,
        Dictionary<string, ReferenceMaterialDictionary> referencesDic)
    {
        playerList.Sort((item, second) =>
            string.Compare(item.NameCommand, second.NameCommand, StringComparison.Ordinal));

        int k = 0;

        foreach (var kvpair in referencesDic)
        {
            _messageSubject.OnNext(
                $"Создание для модераторов файла: {kvpair.Value.TypeCompetition} {kvpair.Value.NameCompetition}");

            using var package = new ExcelPackage();
            var worksheet = package.Workbook.Worksheets.Add("Список участников");

            // Заголовки
            worksheet.Cells["A1:I1"].Merge = true;
            worksheet.Cells["A1"].Value =
                "XI Межрегиональный открытый фестиваль научно-технического творчества «РОБОАРТ-2025»";
            worksheet.Cells["A1"].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
            worksheet.Cells["A1"].Style.Font.Bold = true;

            worksheet.Cells["A2:I2"].Merge = true;
            worksheet.Cells["A2"].Value = "Список участников";
            worksheet.Cells["A2"].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;

            worksheet.Cells["A3:I3"].Merge = true;
            worksheet.Cells["A3"].Value =
                $"{kvpair.Value.TypeCompetition} {kvpair.Value.NameCompetition}, код {kvpair.Key}";
            worksheet.Cells["A3"].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;

            worksheet.Cells["A4:I4"].Merge = true;
            worksheet.Cells["A4"].Value = kvpair.Value.AgeRank;
            worksheet.Cells["A4"].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;

            // Колонки таблицы
            worksheet.Cells["A6"].Value = "№ П/П";
            worksheet.Cells["B6"].Value = "Название команды";
            worksheet.Cells["C6"].Value = "ФИО участника";
            worksheet.Cells["D6"].Value = "Дата рождения";
            worksheet.Cells["E6"].Value = "Учебное заведение";
            worksheet.Cells["F6"].Value = "ФИО руководителя";
            worksheet.Cells["G6"].Value = "Населенный пункт";
            worksheet.Cells["H6"].Value = "Отметка о прибытии";
            worksheet.Cells["I6"].Value = "e-mail";

            for (int col = 1; col <= 9; col++)
            {
                worksheet.Cells[$"{(char)('A' + col - 1)}6"].BorderOutline();
            }

            int row = 6;

            foreach (var people in playerList)
            {
                if ((people.CodeContest?.Contains(kvpair.Key) ?? false) ||
                    (people.CodeCompetition?.Contains(kvpair.Key) ?? false) ||
                    (people.CodeExhibition?.Contains(kvpair.Key) ?? false) ||
                    (people.OlympicsContest?.Contains(kvpair.Key) ?? false))
                {
                    row++;

                    worksheet.Cells[row, 1].Value = row - 6; // № П/П
                    worksheet.Cells[row, 2].Value = people.NameCommand; // Название команды
                    worksheet.Cells[row, 3].Value = people.FioPlayers; // ФИО участника
                    worksheet.Cells[row, 4].Value = people.BirthdayPlayers.ToShortDateString(); // Дата рождения
                    worksheet.Cells[row, 5].Value = people.SchoolPlayers; // Учебное заведение
                    worksheet.Cells[row, 6].Value = people.TeacherPlayers; // ФИО руководителя
                    worksheet.Cells[row, 7].Value = people.CityPlayers; // Населенный пункт
                    worksheet.Cells[row, 8].Value = ""; // Отметка о прибытии
                    worksheet.Cells[row, 9].Value = people.EMail; // e-mail

                    for (int col = 1; col <= 9; col++)
                    {
                        worksheet.Cells[row, col].BorderOutline();
                    }
                }
            }

            worksheet.Column(1).Width = 6;
            worksheet.Column(2).Width = 17.5;
            worksheet.Column(3).Width = 33;
            worksheet.Column(4).Width = 14;
            worksheet.Column(5).Width = 25;
            worksheet.Column(6).Width = 33;
            worksheet.Column(7).Width = 16.5;
            worksheet.Column(8).Width = 18.5;
            worksheet.Column(9).Width = 30;

            // Сохраняем файл
            if (!Directory.Exists(Path.Combine(foldelPathOut, "модерам-дист")))
            {
                Directory.CreateDirectory(Path.Combine(foldelPathOut, "модерам-дист"));
            }

            var moderFileName = Path.Combine(foldelPathOut, "модерам-дист",
                $"{kvpair.Key.Replace(@"\", "").Replace("\"", "")}.xlsx");
            File.WriteAllBytes(moderFileName, package.GetAsByteArray());

            _messageSubject.OnNext(
                $"Создан файл: {kvpair.Value.TypeCompetition} {kvpair.Value.NameCompetition} -> {row - 6} участников");
            k += row - 6;
        }

        _messageSubject.OnNext($"Всего: {k} участников");
    }

    public static void CreateModerOffline(string foldelPathOut, List<PlayersListStruct> playerList,
        Dictionary<string, ReferenceMaterialDictionary> referencesDic)
    {
        playerList.Sort((item, second) =>
            string.Compare(item.NameCommand, second.NameCommand, StringComparison.Ordinal));


        int k = 0;

        foreach (var kvpair in referencesDic)
        {
            _messageSubject.OnNext(
                $"Создание для модераторов файла: {kvpair.Value.TypeCompetition} {kvpair.Value.NameCompetition}");

            using var package = new ExcelPackage();
            var worksheet = package.Workbook.Worksheets.Add("Список участников");

            // Заголовки
            worksheet.Cells["A1:I1"].Merge = true;
            worksheet.Cells["A1"].Value =
                "XI Межрегиональный открытый фестиваль научно-технического творчества «РОБОАРТ-2025»";
            worksheet.Cells["A1"].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
            worksheet.Cells["A1"].Style.Font.Bold = true;

            worksheet.Cells["A2:I2"].Merge = true;
            worksheet.Cells["A2"].Value = "Список участников";
            worksheet.Cells["A2"].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;

            worksheet.Cells["A3:I3"].Merge = true;
            worksheet.Cells["A3"].Value =
                $"{kvpair.Value.TypeCompetition} {kvpair.Value.NameCompetition}, код {kvpair.Key}";
            worksheet.Cells["A3"].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;

            worksheet.Cells["A4:I4"].Merge = true;
            worksheet.Cells["A4"].Value = kvpair.Value.AgeRank;
            worksheet.Cells["A4"].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;

            // Колонки таблицы
            worksheet.Cells["A6"].Value = "№ П/П";
            worksheet.Cells["B6"].Value = "Название команды";
            worksheet.Cells["C6"].Value = "ФИО участника";
            worksheet.Cells["D6"].Value = "Дата рождения";
            worksheet.Cells["E6"].Value = "Учебное заведение";
            worksheet.Cells["F6"].Value = "ФИО руководителя";
            worksheet.Cells["G6"].Value = "Населенный пункт";
            worksheet.Cells["H6"].Value = "Отметка о прибытии";
            worksheet.Cells["I6"].Value = "e-mail";

            for (int col = 1; col <= 9; col++)
            {
                worksheet.Cells[$"{(char)('A' + col - 1)}6"].BorderOutline();
            }

            int row = 6;

            foreach (var people in playerList)
            {
                if ((people.CodeContest?.Contains(kvpair.Key) ?? false) ||
                    (people.CodeCompetition?.Contains(kvpair.Key) ?? false) ||
                    (people.CodeExhibition?.Contains(kvpair.Key) ?? false) ||
                    (people.OlympicsContest?.Contains(kvpair.Key) ?? false))
                {
                    row++;

                    worksheet.Cells[row, 1].Value = row - 6; // № П/П
                    worksheet.Cells[row, 2].Value = people.NameCommand; // Название команды
                    worksheet.Cells[row, 3].Value = people.FioPlayers; // ФИО участника
                    worksheet.Cells[row, 4].Value = people.BirthdayPlayers.ToShortDateString(); // Дата рождения
                    worksheet.Cells[row, 5].Value = people.SchoolPlayers; // Учебное заведение
                    worksheet.Cells[row, 6].Value = people.TeacherPlayers; // ФИО руководителя
                    worksheet.Cells[row, 7].Value = people.CityPlayers; // Населенный пункт
                    worksheet.Cells[row, 8].Value = ""; // Отметка о прибытии
                    worksheet.Cells[row, 9].Value = people.EMail; // e-mail

                    for (int col = 1; col <= 9; col++)
                    {
                        worksheet.Cells[row, col].BorderOutline();
                    }
                }
            }

            worksheet.Column(1).Width = 6;
            worksheet.Column(2).Width = 17.5;
            worksheet.Column(3).Width = 33;
            worksheet.Column(4).Width = 14;
            worksheet.Column(5).Width = 25;
            worksheet.Column(6).Width = 33;
            worksheet.Column(7).Width = 16.5;
            worksheet.Column(8).Width = 18.5;
            worksheet.Column(9).Width = 30;

            // Сохраняем файл
            if (!Directory.Exists(Path.Combine(foldelPathOut, "модерам-очно")))
            {
                Directory.CreateDirectory(Path.Combine(foldelPathOut, "модерам-очно"));
            }

            var moderFileName = Path.Combine(foldelPathOut, "модерам-очно",
                $"{kvpair.Key.Replace(@"\", "").Replace("\"", "")}.xlsx");
            File.WriteAllBytes(moderFileName, package.GetAsByteArray());

            _messageSubject.OnNext(
                $"Создан файл: {kvpair.Value.TypeCompetition} {kvpair.Value.NameCompetition} -> {row - 6} участников");
            k += row - 6;
        }

        _messageSubject.OnNext($"Всего: {k} участников");
    }

    public static bool CreateDictionaryCity(out Dictionary<string, string> dictionary, ref string filePath)
    {
        dictionary = null;

        try
        {
            using var package = new ExcelPackage(new FileInfo(filePath));
            var excelTable = package.Workbook.Worksheets["Населенные пункты"];
            dictionary = new Dictionary<string, string>();

            for (int i = 1; i <= excelTable.Dimension.Rows; i++)
            {
                if (excelTable.Cells[i, 2]?.Value != null && excelTable.Cells[i, 3]?.Value != null)
                {
                    dictionary.Add(
                        excelTable.Cells[i, 2].Value.ToString(),
                        excelTable.Cells[i, 3].Value.ToString());
                }
            }

            _messageSubject.OnNext(
                $"Успешное чтение файла с городами: {filePath} за {(DateTime.Now - dateTime).TotalSeconds:F2} сек.");
            return true;
        }
        catch (Exception e)
        {
            _messageSubject.OnNext($"Произошла ошибка с файлом: {filePath}\nОшибка: {e.Message}");
        }

        return false;
    }

    public static bool CreateDictionaryReference(out Dictionary<string, ReferenceMaterialDictionary> dictionary,
        ref string filePath)
    {
        dictionary = null;

        try
        {
            using var package = new ExcelPackage(new FileInfo(filePath));
            var excelTable = package.Workbook.Worksheets.First();
            dictionary = new Dictionary<string, ReferenceMaterialDictionary>();

            for (int i = 2; i <= excelTable.Dimension.Rows; i++)
            {
                if (excelTable.Cells[i, 1]?.Value != null &&
                    excelTable.Cells[i, 2]?.Value != null &&
                    excelTable.Cells[i, 3]?.Value != null &&
                    excelTable.Cells[i, 4]?.Value != null)
                {
                    dictionary.Add(excelTable.Cells[i, 1].Value.ToString(),
                        new ReferenceMaterialDictionary
                        {
                            TypeCompetition = excelTable.Cells[i, 2].Value.ToString(),
                            AgeRank = excelTable.Cells[i, 3].Value.ToString(),
                            NameCompetition = excelTable.Cells[i, 4].Value.ToString()
                        });
                }
            }

            _messageSubject.OnNext(
                $"Успешное чтение файла: {filePath} за {(DateTime.Now - dateTime).TotalSeconds:F2} сек.");
            return true;
        }
        catch (Exception e)
        {
            _messageSubject.OnNext($"Произошла ошибка с файлом: {filePath}\nОшибка: {e.Message}");
        }

        return false;
    }

    public static bool CreateListForDiplomsOffline(out List<PlayersListStruct> list, ref string filePath)
    {
        list = null;

        try
        {
            using var package = new ExcelPackage(new FileInfo(filePath));
            var excelTable = package.Workbook.Worksheets["Очно"];
            list = new List<PlayersListStruct>();

            for (int i = 2; i <= excelTable.Dimension.Rows; i++)
            {
                if (excelTable.Cells[i, 4]?.Value != null)
                {
                    list.Add(new PlayersListStruct
                    {
                        CodeCompetition = excelTable.Cells[i, 13]?.Value?.ToString(),
                        CodeExhibition = excelTable.Cells[i, 14]?.Value?.ToString(),
                        CodeContest = excelTable.Cells[i, 15]?.Value?.ToString(),
                        OlympicsContest = null /*excelTable.Cells[i, 15]?.Value?.ToString()*/,
                        FioPlayers = excelTable.Cells[i, 4]?.Value?.ToString(),
                        BirthdayPlayers = excelTable.Cells[i, 5]?.Value != null
                            ? DateTime.FromOADate((double)excelTable.Cells[i, 5].Value)
                            : default,
                        SchoolPlayers = excelTable.Cells[i, 12]?.Value?.ToString(),
                        CityPlayers = excelTable.Cells[i, 11]?.Value?.ToString(),
                        TeacherPlayers = excelTable.Cells[i, 18]?.Value?.ToString(),
                        IsMen = excelTable.Cells[i, 21]?.Value?.ToString() == "М",
                        NameCommand = excelTable.Cells[i, 8]?.Value?.ToString(),
                        EMail = excelTable.Cells[i, 20]?.Value?.ToString()
                    });
                }
            }

            _messageSubject.OnNext(
                $"Успешное чтение файла: {filePath} за {(DateTime.Now - dateTime).TotalSeconds:F2} сек.");
            return true;
        }
        catch (Exception e)
        {
            _messageSubject.OnNext($"Произошла ошибка с файлом: {filePath}\nОшибка: {e.Message}");
        }

        return false;
    }

    public static bool CreateListForDiplomsOnline(out List<PlayersListStruct> list, ref string filePath)
    {
        list = null;

        try
        {
            using var package = new ExcelPackage(new FileInfo(filePath));
            var excelTable = package.Workbook.Worksheets["Дистанционно"];
            list = new List<PlayersListStruct>();

            for (int i = 2; i <= excelTable.Dimension.Rows; i++)
            {
                if (excelTable.Cells[i, 4]?.Value != null)
                {
                    list.Add(new PlayersListStruct
                    {
                        CodeCompetition = excelTable.Cells[i, 12]?.Value?.ToString(),
                        CodeExhibition = excelTable.Cells[i, 13]?.Value?.ToString(),
                        CodeContest = excelTable.Cells[i, 14]?.Value?.ToString(),
                        OlympicsContest = excelTable.Cells[i, 15]?.Value?.ToString(),
                        FioPlayers = excelTable.Cells[i, 3]?.Value?.ToString(),
                        BirthdayPlayers = excelTable.Cells[i, 4]?.Value != null
                            ? DateTime.FromOADate((double)excelTable.Cells[i, 4].Value)
                            : default,
                        SchoolPlayers = excelTable.Cells[i, 11]?.Value?.ToString(),
                        CityPlayers = excelTable.Cells[i, 10]?.Value?.ToString(),
                        TeacherPlayers = excelTable.Cells[i, 18]?.Value?.ToString(),
                        IsMen = excelTable.Cells[i, 21]?.Value?.ToString() == "М",
                        NameCommand = excelTable.Cells[i, 7]?.Value?.ToString(),
                        EMail = excelTable.Cells[i, 20]?.Value?.ToString()
                    });
                }
            }

            _messageSubject.OnNext(
                $"Успешное чтение файла: {filePath} за {(DateTime.Now - dateTime).TotalSeconds:F2} сек.");
            return true;
        }
        catch (Exception e)
        {
            _messageSubject.OnNext($"Произошла ошибка с файлом: {filePath}\nОшибка: {e.Message}");
        }

        return false;
    }
}