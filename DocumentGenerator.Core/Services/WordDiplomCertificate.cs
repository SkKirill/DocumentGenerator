using Xceed.Words.NET;
using Xceed.Document.NET;
using static System.String;

namespace DocumentGenerator.Core.Services;

public struct ReferenceMaterialDictionary
{
    public string TypeCompetition;
    public string AgeRank;
    public string NameCompetition;
}

public struct DiplomStruct
{
    public string Competition;
    public string Age;
    public string Fio;
    public string Birthday;
    public string City;
    public string Teacher;
}

public struct PlayersListStruct
{
    public string? CodeCompetition;
    public string? NameCommand;
    public string? EMail;
    public string? CodeExhibition;
    public string? CodeContest;
    public string? OlympicsContest;
    public string FioPlayers;
    public DateTime BirthdayPlayers;
    public bool IsMen;
    public string SchoolPlayers;
    public string? CityPlayers;
    public string? TeacherPlayers;

    public override string ToString()
    {
        return FioPlayers.PadRight(35, ' ') + " | " + CityPlayers;
    }
}

public class WordDiplomCertificate(
    string folderPathOut,
    List<PlayersListStruct> playerList,
    Dictionary<string, string> citiesDic,
    Dictionary<string,
        ReferenceMaterialDictionary> referencesDic,
    string substrateFilePath)
{
    private const string Competition = "в {0} «{1}»,";
    private const string Olimpic = "в {0} {1},";
    private const string Age = "возрастная категория «{0}»";
    private const string School = "{0} {1}";
    private const string Teacher = "Педагог: {0}";

    private delegate void DiplomCreationDelegate(DiplomStruct diplom, string savePath, string substrateFilePath = null);

    public bool CreateCertificateWithBacking(string type)
    {
        return CreateWord(new WordDocumentGenerator().CreateSertificatWithBacking, type, substrateFilePath);
    }

    public bool CreateCertificate(string type)
    {
        return CreateWord(CreateSertificatWithBacking, type);
    }

    public bool CreateDiploms(string type)
    {
        return CreateWord(CreateDiplom, type, null);
    }

    private bool CreateWord(DiplomCreationDelegate creationDelegate, string folderMain, string substrateFilePath = null)
    {
        var diplomStruct = new DiplomStruct();
        try
        {
            var i = 0;
            while (i < 5/*playerList.Count*/)
            {
                var people = playerList[i];
                Console.WriteLine(i.ToString().PadRight(4, ' ') + " | " + people.ToString().PadRight(50, ' ') + " | " +
                                  folderMain);
                if ((!IsNullOrEmpty(people.CodeCompetition) && !people.CodeCompetition.Contains("не участвую"))
                    || (!IsNullOrEmpty(people.CodeContest) && !people.CodeContest.Contains("не участвую"))
                    || (!IsNullOrEmpty(people.CodeExhibition) && !people.CodeExhibition.Contains("не участвую"))
                    || (!IsNullOrEmpty(people.OlympicsContest) &&
                        !people.OlympicsContest.Contains("не участвую")))
                {
                    var currentPath = folderPathOut + @"\" + folderMain + @"\" + people.CityPlayers + @"\" +
                                      people.FioPlayers;
                    if (!Directory.Exists(folderPathOut + @"\" + folderMain))
                    {
                        // Создаем папку, если она не существует
                        Directory.CreateDirectory(folderPathOut + @"\" + folderMain);
                    }

                    if (!Directory.Exists(folderPathOut + @"\" + folderMain + @"\" + people.CityPlayers))
                    {
                        // Создаем папку, если она не существует
                        Directory.CreateDirectory(folderPathOut + @"\" + folderMain + @"\" + people.CityPlayers);
                    }

                    diplomStruct.Fio =
                        (people.FioPlayers.Split(' ').Length > 0 ? people.FioPlayers.Split(' ')[0] : "") + " " +
                        (people.FioPlayers.Split(' ').Length > 1 ? people.FioPlayers.Split(' ')[1] : "");

                    if (people.SchoolPlayers == "Индивидуальный участник")
                    {
                        diplomStruct.Birthday = "";
                    }
                    else
                    {
                        diplomStruct.Birthday = Format(School, people.IsMen ? "учащийся" : "учащаяся",
                            people.SchoolPlayers);
                    }

                    if (people.CityPlayers != null) diplomStruct.City = citiesDic[people.CityPlayers];
                    diplomStruct.Teacher = Format(Teacher, people.TeacherPlayers);

                    if (!IsNullOrEmpty(people.CodeCompetition) &&
                        !people.CodeCompetition.Contains("не участвую"))
                    {
                        diplomStruct.Competition = Format(Competition, "соревновании",
                            referencesDic[people.CodeCompetition].NameCompetition);
                        diplomStruct.Age = Format(Age, referencesDic[people.CodeCompetition].AgeRank);
                        creationDelegate(diplomStruct, currentPath + people.CodeCompetition, substrateFilePath);
                    }

                    if (!IsNullOrEmpty(people.CodeContest) && !people.CodeContest.Contains("не участвую"))
                    {
                        diplomStruct.Competition = Format(Competition, "конкурсе",
                            referencesDic[people.CodeContest].NameCompetition);
                        diplomStruct.Age = Format(Age, referencesDic[people.CodeContest].AgeRank);
                        creationDelegate(diplomStruct, currentPath + people.CodeContest, substrateFilePath);
                    }

                    if (!IsNullOrEmpty(people.CodeExhibition) && !people.CodeExhibition.Contains("не участвую"))
                    {
                        diplomStruct.Competition = Format(Competition, "выставке",
                            referencesDic[people.CodeExhibition].NameCompetition);
                        diplomStruct.Age = Format(Age, referencesDic[people.CodeExhibition].AgeRank);
                        creationDelegate(diplomStruct, currentPath + people.CodeExhibition, substrateFilePath);
                    }

                    if (!IsNullOrEmpty(people.OlympicsContest) &&
                        !people.OlympicsContest.Contains("не участвую"))
                    {
                        diplomStruct.Competition = Format(Olimpic, "олимпиаде",
                            referencesDic[people.OlympicsContest].NameCompetition);
                        diplomStruct.Age = Format(Age, referencesDic[people.OlympicsContest].AgeRank);
                        creationDelegate(diplomStruct, currentPath + people.OlympicsContest, substrateFilePath);
                    }
                }

                i++;
            }

            return true;
        }
        catch (Exception e)
        {
            Console.WriteLine($"Произошла ошибка при создании {folderMain}!\n Ошибка: " + e);
        }

        return false;
    }

    public void CreateDiplom(DiplomStruct diplom, string savePath, string _ = null)
    {
        // Создаем новый документ
        using (DocX document = DocX.Create(savePath + ".docx"))
        {
            // Добавляем параграфы
            AddParagraph(document, diplom.Competition, 350f, 0f, 16, "Calibri", true);
            AddParagraph(document, "возрастная категория", 6f, 0f, 16, "Calibri", true);
            AddParagraph(document, diplom.Age.Substring(20), 0f, 0f, 16, "Calibri", true);

            // ФИО участника
            var fioParagraph = AddParagraph(document, diplom.Fio, 44f, 12f, 26, "Calibri", true);
            fioParagraph.Bold();

            AddParagraph(document, diplom.Birthday, 6f, 0f, 16, "Calibri", true);
            AddParagraph(document, diplom.City, 0f, 8f, 15, "Calibri", true);
            AddParagraph(document, diplom.Teacher, 18f, 8f, 15, "Calibri", true);

            // Устанавливаем нулевые поля (margin)
            document.MarginTop = 0;
            document.MarginBottom = 0;
            document.MarginLeft = 0;
            document.MarginRight = 0;

            // Сохраняем документ
            document.Save();
        }
    }

    private Xceed.Document.NET.Paragraph AddParagraph(DocX document, string text, float spaceBefore, float spaceAfter,
        int fontSize, string fontName, bool isCentered)
    {
        var paragraph = document.InsertParagraph(text);
        paragraph.Font(fontName).FontSize(fontSize);

        if (isCentered)
        {
            paragraph.Alignment = Alignment.center;
        }

        // Установка отступов (если требуется)
        paragraph.SpacingBefore(spaceBefore);
        paragraph.SpacingAfter(spaceAfter);

        return paragraph;
    }

    public void CreateSertificatWithBacking(DiplomStruct diplom, string savePath, string substrateFilePath = null)
    {
        using (DocX document = DocX.Create(savePath + ".docx"))
        {
            try
            {
                // Добавляем изображение подложки
                if (substrateFilePath != null)
                {
                    var image = document.AddImage(substrateFilePath);
                    var picture = image.CreatePicture();
                    picture.Width = 797;  // Ширина страницы A4 в пикселях
                    picture.Height = 1127; // Высота страницы A4 в пикселях

                    // Вставляем изображение в начало документа
                    var paragraph = document.InsertParagraph();
                    paragraph.Alignment = Alignment.center; // Центрируем изображение
                    paragraph.InsertPicture(picture);

                    // Устанавливаем параметры параграфа, чтобы изображение было как фон
                    paragraph.SpacingBefore(0);
                    paragraph.SpacingAfter(0);
                }

                // Добавляем параграфы
                var fioParagraph = AddParagraph(document, diplom.Fio, 283f, 4f, 26, "Calibri", true);
                fioParagraph.Bold();

                if (diplom.City.Length >= 44)
                {
                    AddParagraph(document, diplom.Birthday, 0f, 0f, 16, "Calibri", true);

                    string first = "";
                    string[] words = diplom.City.Split(' ');
                    int i = 0;
                    while (words.Length > i && (first + words[i]).Length < 45)
                    {
                        first += words[i] + " ";
                        i++;
                    }

                    string last = "";
                    for (int k = i; words.Length > k; k++)
                        last += words[k] + " ";

                    AddParagraph(document, first, 0f, 0f, 16, "Calibri", true);
                    AddParagraph(document, last, 0f, 0f, 16, "Calibri", true);
                    AddParagraph(document, diplom.Competition, 120f, 8f, 16, "Calibri", true);
                    AddParagraph(document, diplom.Age, 0f, 8f, 16, "Calibri", true);
                    AddParagraph(document, diplom.Teacher, 36f, 8f, 15, "Calibri", true);
                }
                else
                {
                    AddParagraph(document, diplom.Birthday, 0f, 8f, 16, "Calibri", true);
                    AddParagraph(document, diplom.City, 0f, 8f, 16, "Calibri", true);
                    AddParagraph(document, diplom.Competition, 120f, 8f, 16, "Calibri", true);
                    AddParagraph(document, diplom.Age, 0f, 8f, 16, "Calibri", true);
                    AddParagraph(document, diplom.Teacher, 36f, 8f, 15, "Calibri", true);
                }

                // Устанавливаем нулевые поля (margin)
                document.MarginTop = 0;
                document.MarginBottom = 0;
                document.MarginLeft = 0;
                document.MarginRight = 0;

                // Сохраняем документ
                document.Save();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка при добавлении фонового изображения: {ex.Message}");
            }
        }
    }
}