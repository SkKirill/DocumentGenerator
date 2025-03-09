using OfficeOpenXml;

namespace DocumentGenerator.Core.Services;

public class StartService
{
    private Dictionary<string, string> _headers;
    public List<string> NameLayouts { get; set; }
    public List<string> SourceNames { get; set; }
    public string FolderTo { get; set; }

    public StartService(List<string> nameLayouts, List<string> sourceNames, string folderTo)
    {
        _headers = new Dictionary<string, string>();
        SourceNames = sourceNames;
        FolderTo = folderTo;
        NameLayouts = nameLayouts;
        Console.OutputEncoding = System.Text.Encoding.UTF8;

        ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

        foreach (var path in sourceNames)
        {
            using var package = new ExcelPackage(new FileInfo(path));

            foreach (var nameExcelWorksheet in package.Workbook.Worksheets)
            {
                foreach (var item in nameExcelWorksheet.Cells
                             .Where(item => item.ToString().Contains("1") && item.ToString().Length == 2))
                {
                    Console.WriteLine(item + " " + item.Value);
                    if (item.Value.ToString() != null)
                    {
                        _headers.Add(item.ToString(), item.Value.ToString()!);
                    }
                }
            }
        }

        Console.WriteLine("Starting document generator.");
        foreach (var item in NameLayouts)
        {
            Console.WriteLine(item);
        }
    }

    public void ChangeNameLayout(List<string> name)
    {
        NameLayouts.Clear();
        NameLayouts.AddRange(name);
    }

    public void ChangeFolderTo(string folder)
    {
        FolderTo = folder;
    }
}