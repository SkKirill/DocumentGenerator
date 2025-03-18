using System.Reactive.Subjects;
using OfficeOpenXml;

namespace DocumentGenerator.Core.Services;

public class StartService
{
    private Dictionary<string, string> _headers;
    public List<string> NameLayouts { get; set; }
    public List<string> SourceNames { get; set; }
    public string References { get; set; }
    public string FolderTo { get; set; }
    public string PathToImage { get; set; }

    public IObservable<string> Message => _message;

    private ISubject<string> _message;

    public StartService(List<string> nameLayouts, List<string> sourceNames, string folderTo, string referenses, string image)
    {
        PathToImage = image;
        References = referenses;
        _headers = new Dictionary<string, string>();
        SourceNames = sourceNames;
        FolderTo = folderTo;
        NameLayouts = nameLayouts;
        Console.OutputEncoding = System.Text.Encoding.UTF8;
        ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
        _message = new Subject<string>();
    }

    public void Start()
    {
        /*foreach (var path in sourceNames)
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
        }*/
        _message.OnNext("Starting document generator.");
        CreateDoc.CreateAllDoc(References, SourceNames.First(), FolderTo, 
            ref _message, PathToImage);

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