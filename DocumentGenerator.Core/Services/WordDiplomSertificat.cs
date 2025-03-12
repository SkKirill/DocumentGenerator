using System.Drawing;
using Spire.Doc;
using Spire.Doc.Documents;
using Spire.Doc.Fields;
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

class WordDiplomSertificat
{
    private const string Competition = "в {0} «{1}»,";
    private const string Olimpic = "в {0} {1},";
    private const string Age = "возрастная категория «{0}»";
    private const string School = "{0} {1}";
    private const string BirthdaySchool = "{0} {1} {2}";
    private const string Teacher = "Педагог: {0}";

    private delegate void DiplomCreationDelegate(DiplomStruct diplom, string savePath);

    private string _filePathReference;
    private string _filePathIn;
    private readonly string _foldelPathOut;
    private readonly string _substrateFilePath;
    private readonly List<PlayersListStruct> _playerList;
    private readonly Dictionary<string, string> _citiesDic;
    private readonly Dictionary<string, ReferenceMaterialDictionary> _referencesDic;

    public WordDiplomSertificat(string filePathReference, string filePathIn, string foldelPathOut,
        List<PlayersListStruct> playerList, Dictionary<string, string> citiesDic, Dictionary<string,
            ReferenceMaterialDictionary> referencesDic, string substrateFilePath)
    {
        _filePathReference = filePathReference;
        _filePathIn = filePathIn;
        _foldelPathOut = foldelPathOut;
        _playerList = playerList;
        _citiesDic = citiesDic;
        _referencesDic = referencesDic;
        _substrateFilePath = substrateFilePath;
    }

    public bool CreateCertificateWithBacking()
    {
        return CreateWord(CreateSertificatWithBacking, "сертификаты с подложкой-очно");
    }

    public bool CreateCertificate()
    {
        return CreateWord(CreateSertificat, "сертификаты-дист");
    }

    public bool CreateDiploms()
    {
        return CreateWord(CreateDiplom, "дипломы-дист");
    }

    private void CreateParagraph(ref Document document, string text,
        float spaceBefore, float spaceAfter)
    {
        Section section = document.Sections[0]; // Или получите нужную секцию
        Paragraph paragraph = section.AddParagraph();

        TextRange textRange = paragraph.AppendText(text.Trim().Length == 0 ? " " : text.Trim());

        // Форматирование текста
        textRange.CharacterFormat.FontSize = 16;
        textRange.CharacterFormat.FontName = "Calibri";

        // Форматирование параграфа
        paragraph.Format.BeforeSpacing = spaceBefore;
        paragraph.Format.AfterSpacing = spaceAfter;
        paragraph.Format.HorizontalAlignment = HorizontalAlignment.Center;
    }

    private void CreateDiplom(DiplomStruct diplom, string savePath)
    {
        // Создаем новый документ
        Document doc = new Document();
        Section section = doc.AddSection();

        // Используем ранее определенный метод CreateParagraph (с исправленным кодом)
        CreateParagraph(ref doc, diplom.Competition, 360f, 0f);

        CreateParagraph(ref doc, "возрастная категория", 6f, 0f);
        CreateParagraph(ref doc, diplom.Age.Substring(20), 0f, 0f);

        CreateParagraph(ref doc, diplom.Fio, 60f, 12f);
        // Форматирование FIO (параграф 4)
        Paragraph fioParagraph = section.Paragraphs[3]; // Индексация с 0
        if (fioParagraph.ChildObjects.Count > 0 && fioParagraph.ChildObjects[0] is TextRange)
        {
            TextRange fioTextRange = (TextRange)fioParagraph.ChildObjects[0];
            fioTextRange.CharacterFormat.FontSize = 26;
            fioTextRange.CharacterFormat.Bold = true;
        }

        CreateParagraph(ref doc, diplom.Birthday, 6f, 0f);
        CreateParagraph(ref doc, diplom.City, 0f, 8f);

        CreateParagraph(ref doc, diplom.Teacher, 18f, 8f);
        // Форматирование city (параграф 6)
        Paragraph cityParagraph = section.Paragraphs[5]; // Индексация с 0
        if (cityParagraph.ChildObjects.Count > 0 && cityParagraph.ChildObjects[0] is TextRange)
        {
            TextRange cityTextRange = (TextRange)cityParagraph.ChildObjects[0];
            cityTextRange.CharacterFormat.FontSize = 15;
        }

        // Устанавливаем нулевые поля (margin)
        doc.Sections[0].PageSetup.Margins.Top = 0;
        doc.Sections[0].PageSetup.Margins.Left = 0;
        doc.Sections[0].PageSetup.Margins.Right = 0;
        doc.Sections[0].PageSetup.Margins.Bottom = 0;

        // Сохраняем документ
        doc.SaveToFile(savePath, FileFormat.Docx); // Или другой желаемый формат
    }

    private void CreateSertificat(DiplomStruct diplom, string savePath)
    {
        // Создаем новый документ
        Document doc = new Document();
        Section section = doc.AddSection();

        // Используем ранее определенный метод CreateParagraph (с исправленным кодом)
        CreateParagraph(ref doc, diplom.Fio, 283f, 4f);
        Paragraph fioParagraph = section.Paragraphs[0]; // Индексация с 0

        if (fioParagraph.ChildObjects.Count > 0 && fioParagraph.ChildObjects[0] is TextRange)
        {
            TextRange fioTextRange = (TextRange)fioParagraph.ChildObjects[0];
            fioTextRange.CharacterFormat.FontSize = 26;
            fioTextRange.CharacterFormat.Bold = true;
        }

        if (diplom.City.Length >= 44)
        {
            CreateParagraph(ref doc, diplom.Birthday, 0f, 0f);
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

            CreateParagraph(ref doc, first, 0f, 0f);
            CreateParagraph(ref doc, last, 0f, 0f);

            CreateParagraph(ref doc, diplom.Competition, 120f, 8f);
            CreateParagraph(ref doc, diplom.Age, 0f, 8f);
            CreateParagraph(ref doc, diplom.Teacher, 36f, 8f);

            Paragraph teacherParagraph = section.Paragraphs[6];
            if (teacherParagraph.ChildObjects.Count > 0 && teacherParagraph.ChildObjects[0] is TextRange)
            {
                TextRange teacherTextRange = (TextRange)teacherParagraph.ChildObjects[0];
                teacherTextRange.CharacterFormat.FontSize = 15;
            }
        }
        else
        {
            CreateParagraph(ref doc, diplom.Birthday, 0f, 8f);
            CreateParagraph(ref doc, diplom.City, 0f, 8f);
            CreateParagraph(ref doc, diplom.Competition, 120f, 8f);
            CreateParagraph(ref doc, diplom.Age, 0f, 8f);
            CreateParagraph(ref doc, diplom.Teacher, 36f, 8f);
            Paragraph teacherParagraph = section.Paragraphs[5];
            if (teacherParagraph.ChildObjects.Count > 0 && teacherParagraph.ChildObjects[0] is TextRange)
            {
                TextRange teacherTextRange = (TextRange)teacherParagraph.ChildObjects[0];
                teacherTextRange.CharacterFormat.FontSize = 15;
            }
        }

        // Устанавливаем нулевые поля (margin)
        doc.Sections[0].PageSetup.Margins.Top = 0;
        doc.Sections[0].PageSetup.Margins.Left = 0;
        doc.Sections[0].PageSetup.Margins.Right = 0;
        doc.Sections[0].PageSetup.Margins.Bottom = 0;

        // Сохраняем документ
        doc.SaveToFile(savePath, FileFormat.Docx); // Или другой желаемый формат
    }

    private void CreateSertificatWithBacking(DiplomStruct diplom, string savePath)
    {
        // Создаем новый документ
        var doc = new Document();
        var section = doc.AddSection();

        // Добавляем изображение подложки
        try
        {
            // Load the image
            var image = Image.FromFile(_substrateFilePath);

            // Check if the image was loaded successfully
            if (image != null)
            {
                // Set page size to the same size of the image
                doc.Sections[0].PageSetup.PageSize = doc.Sections[0].PageSetup.PageSize with { Width = image.Width };
                doc.Sections[0].PageSetup.PageSize =
                    doc.Sections[0].PageSetup.PageSize with { Height = image.Height };

                // Create Picture Watermark
                PictureWatermark pictureWatermark = new PictureWatermark();
                pictureWatermark.Picture = image;
                pictureWatermark.Scaling = 100; // Adjust scaling if needed
                pictureWatermark.IsWashout = false; // Set to false if you don't want the watermark to be faded

                // Apply the picture watermark to the document, not the section
                doc.Watermark = pictureWatermark;
            }
            else
            {
                Console.WriteLine("Error: Could not load the background image.");
                return; // Exit the method if there's an error with the background
            }

            // Set margins to zero
            doc.Sections[0].PageSetup.Margins.Top = 0;
            doc.Sections[0].PageSetup.Margins.Bottom = 0;
            doc.Sections[0].PageSetup.Margins.Left = 0;
            doc.Sections[0].PageSetup.Margins.Right = 0;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error adding background image: {ex.Message}");
            // Handle the exception (e.g., log it, show an error message)
            return; // Exit the method if there's an error with the background
        }

        // Используем ранее определенный метод CreateParagraph (с исправленным кодом)
        CreateParagraph(ref doc, diplom.Fio, 283f, 4f);
        Paragraph fioParagraph = section.Paragraphs[0];

        if (fioParagraph.ChildObjects.Count > 0 && fioParagraph.ChildObjects[0] is TextRange)
        {
            TextRange fioTextRange = (TextRange)fioParagraph.ChildObjects[0];
            fioTextRange.CharacterFormat.FontSize = 26;
            fioTextRange.CharacterFormat.Bold = true;
        }

        if (diplom.City.Length >= 44)
        {
            CreateParagraph(ref doc, diplom.Birthday, 0f, 0f);
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

            CreateParagraph(ref doc, first, 0f, 0f);
            CreateParagraph(ref doc, last, 0f, 0f);

            CreateParagraph(ref doc, diplom.Competition, 120f, 8f);
            CreateParagraph(ref doc, diplom.Age, 0f, 8f);
            CreateParagraph(ref doc, diplom.Teacher, 36f, 8f);
            Paragraph teacherParagraph = section.Paragraphs[6];
            if (teacherParagraph.ChildObjects.Count > 0 && teacherParagraph.ChildObjects[0] is TextRange)
            {
                TextRange teacherTextRange = (TextRange)teacherParagraph.ChildObjects[0];
                teacherTextRange.CharacterFormat.FontSize = 15;
            }
        }
        else
        {
            CreateParagraph(ref doc, diplom.Birthday, 0f, 8f);
            CreateParagraph(ref doc, diplom.City, 0f, 8f);
            CreateParagraph(ref doc, diplom.Competition, 120f, 8f);
            CreateParagraph(ref doc, diplom.Age, 0f, 8f);
            CreateParagraph(ref doc, diplom.Teacher, 36f, 8f);
            Paragraph teacherParagraph = section.Paragraphs[5];
            if (teacherParagraph.ChildObjects.Count > 0 && teacherParagraph.ChildObjects[0] is TextRange)
            {
                TextRange teacherTextRange = (TextRange)teacherParagraph.ChildObjects[0];
                teacherTextRange.CharacterFormat.FontSize = 15;
            }
        }

        // Сохраняем документ
        doc.SaveToFile(savePath, FileFormat.Docx); // Или другой желаемый формат
    }

    private bool CreateWord(DiplomCreationDelegate creationDelegate, string folderMain)
    {
        var diplomStruct = new DiplomStruct();
        try
        {
            var i = 0;
            var people = _playerList[i];
            while (i < _playerList.Count)
            {
                Console.WriteLine(i.ToString().PadRight(4, ' ') + " | " + people.ToString().PadRight(50, ' ') + " | " +
                                  folderMain);
                if ((!IsNullOrEmpty(people.CodeCompetition) && !people.CodeCompetition.Contains("не участвую"))
                    || (!IsNullOrEmpty(people.CodeContest) && !people.CodeContest.Contains("не участвую"))
                    || (!IsNullOrEmpty(people.CodeExhibition) && !people.CodeExhibition.Contains("не участвую"))
                    || (!IsNullOrEmpty(people.OlympicsContest) &&
                        !people.OlympicsContest.Contains("не участвую")))
                {
                    var currentPath = _foldelPathOut + @"\" + folderMain + @"\" + people.CityPlayers + @"\" +
                                      people.FioPlayers;
                    if (!Directory.Exists(_foldelPathOut + @"\" + folderMain))
                    {
                        // Создаем папку, если она не существует
                        Directory.CreateDirectory(_foldelPathOut + @"\" + folderMain);
                    }

                    if (!Directory.Exists(_foldelPathOut + @"\" + folderMain + @"\" + people.CityPlayers))
                    {
                        // Создаем папку, если она не существует
                        Directory.CreateDirectory(_foldelPathOut + @"\" + folderMain + @"\" + people.CityPlayers);
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

                    if (people.CityPlayers != null) diplomStruct.City = _citiesDic[people.CityPlayers];
                    diplomStruct.Teacher = Format(Teacher, people.TeacherPlayers);

                    if (!IsNullOrEmpty(people.CodeCompetition) &&
                        !people.CodeCompetition.Contains("не участвую"))
                    {
                        diplomStruct.Competition = Format(Competition, "соревновании",
                            _referencesDic[people.CodeCompetition].NameCompetition);
                        diplomStruct.Age = Format(Age, _referencesDic[people.CodeCompetition].AgeRank);
                        creationDelegate(diplomStruct, currentPath + people.CodeCompetition);
                    }

                    if (!IsNullOrEmpty(people.CodeContest) && !people.CodeContest.Contains("не участвую"))
                    {
                        diplomStruct.Competition = Format(Competition, "конкурсе",
                            _referencesDic[people.CodeContest].NameCompetition);
                        diplomStruct.Age = Format(Age, _referencesDic[people.CodeContest].AgeRank);
                        creationDelegate(diplomStruct, currentPath + people.CodeContest);
                    }

                    if (!IsNullOrEmpty(people.CodeExhibition) && !people.CodeExhibition.Contains("не участвую"))
                    {
                        diplomStruct.Competition = Format(Competition, "выставке",
                            _referencesDic[people.CodeExhibition].NameCompetition);
                        diplomStruct.Age = Format(Age, _referencesDic[people.CodeExhibition].AgeRank);
                        creationDelegate(diplomStruct, currentPath + people.CodeExhibition);
                    }

                    if (!IsNullOrEmpty(people.OlympicsContest) &&
                        !people.OlympicsContest.Contains("не участвую"))
                    {
                        diplomStruct.Competition = Format(Olimpic, "олимпиаде",
                            _referencesDic[people.OlympicsContest].NameCompetition);
                        diplomStruct.Age = Format(Age, _referencesDic[people.OlympicsContest].AgeRank);
                        creationDelegate(diplomStruct, currentPath + people.OlympicsContest);
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
}