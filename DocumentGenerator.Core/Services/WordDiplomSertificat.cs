using System.Drawing;
using System;
using System.Collections.Generic;
using System.IO;
using Xceed.Words.NET;
using Xceed.Document.NET;
using Xceed.Words.NET;
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

public class WordDiplomSertificat
{
    private readonly string _foldelPathOut;
    private readonly List<PlayersListStruct> _playerList;
    private readonly Dictionary<string, string> _citiesDic;
    private readonly Dictionary<string, ReferenceMaterialDictionary> _referencesDic;
    private readonly string _substrateFilePath;

    public WordDiplomSertificat(string filePathReference, string filePathIn, string foldelPathOut,
        List<PlayersListStruct> playerList, Dictionary<string, string> citiesDic, Dictionary<string,
            ReferenceMaterialDictionary> referencesDic, string substrateFilePath)
    {
        _foldelPathOut = foldelPathOut;
        _playerList = playerList;
        _citiesDic = citiesDic;
        _referencesDic = referencesDic;
        _substrateFilePath = substrateFilePath;
    }

    private void CreateDiplom(DiplomStruct diplom, string savePath)
    {
        // Создаем новый документ
        using (DocX document = DocX.Create(savePath + ".docx"))
        {
            // Добавляем параграфы
            AddParagraph(document, diplom.Competition, 360f, 0f, 16, "Calibri", true);
            AddParagraph(document, "возрастная категория", 6f, 0f, 16, "Calibri", true);
            AddParagraph(document, diplom.Age.Substring(20), 0f, 0f, 16, "Calibri", true);

            // ФИО участника
            var fioParagraph = AddParagraph(document, diplom.Fio, 60f, 12f, 26, "Calibri", true);
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

    private Paragraph AddParagraph(DocX document, string text, float spaceBefore, float spaceAfter,
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

    private void CreateSertificatWithBacking(DiplomStruct diplom, string savePath)
    {
        using (DocX document = DocX.Create(savePath + ".docx"))
        {
            try
            {
                // Добавляем изображение подложки
                var image = document.AddImage(_substrateFilePath);
                var picture = image.CreatePicture();
                document.InsertParagraph("").InsertPicture(picture);

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