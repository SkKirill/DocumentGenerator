using Word = Microsoft.Office.Interop.Word;

namespace DocumentGenerator.Core.Services;

public class WordDocumentGenerator
{
    public void CreateSertificatWithBacking(DiplomStruct diplom, string SavePath, string substrateFilePath)
    {
        var wordApp = new Word.Application();
        // Добавляем новый документ
        Word.Document doc = wordApp.Documents.Add();

        CreateParagrahp(ref doc, diplom.Fio, 1, 264, 4);
        doc.Paragraphs[1].Range.Font.Size = 26;
        doc.Paragraphs[1].Range.Font.Bold = 1;
        if (diplom.City.Length >= 44)
        {
            CreateParagrahp(ref doc, diplom.Birthday, 2, 0, 0);
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

            CreateParagrahp(ref doc, first, 3, 0, 0);
            CreateParagrahp(ref doc, last, 4, 0, 0);

            CreateParagrahp(ref doc, diplom.Competition, 5, 100, 8);
            CreateParagrahp(ref doc, diplom.Age, 6, 0, 8);
            CreateParagrahp(ref doc, diplom.Teacher, 7, 18, 8);
            doc.Paragraphs[7].Range.Font.Size = 15;
        }
        else
        {
            CreateParagrahp(ref doc, diplom.Birthday, 2, 0, 8);
            CreateParagrahp(ref doc, diplom.City, 3, 0, 8);
            CreateParagrahp(ref doc, diplom.Competition, 4, 100, 8);
            CreateParagrahp(ref doc, diplom.Age, 5, 0, 8);
            CreateParagrahp(ref doc, diplom.Teacher, 6, 18, 8);
            doc.Paragraphs[6].Range.Font.Size = 15;
        }

        doc.PageSetup.TopMargin = 0;
        doc.PageSetup.LeftMargin = 0;
        doc.PageSetup.RightMargin = 0;
        doc.PageSetup.BottomMargin = 0;


        Word.Shape shape = doc.Shapes.AddPicture(substrateFilePath, false, true, 0, 0, 0, 0);
        shape.Fill.UserPicture(substrateFilePath);
        shape.Width = doc.PageSetup.PageWidth;
        shape.Height = doc.PageSetup.PageHeight;
        shape.Top = 0;
        shape.Left = 0;

        shape.WrapFormat.Type = Word.WdWrapType.wdWrapBehind;

        // Сохраняем документ
        doc.SaveAs(SavePath);
        doc.Close();
        wordApp.Quit();
    }

    private void CreateParagrahp(ref Word.Document document, string text, int countParagraph,
        int spaceBefor, int spaceAfter)
    {
        Word.Paragraph paragraph = document.Paragraphs.Add();
        Word.Range range = paragraph.Range;
        range.Font.Size = 16;
        range.Font.Name = "Calibri";
        range.Text = text.Trim().Length == 0 ? " " : text.Trim();

        range.InsertParagraphAfter();

        document.Paragraphs[countParagraph].SpaceBefore = spaceBefor;
        document.Paragraphs[countParagraph].SpaceAfter = spaceAfter;
        document.Paragraphs[countParagraph].Format.Alignment = Word.WdParagraphAlignment.wdAlignParagraphCenter;
    }
}