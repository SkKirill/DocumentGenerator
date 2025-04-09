using OfficeOpenXml;

namespace DocumentGenerator.Core.Services;

public static class ExcelExtentions
{
    public static ExcelRange BorderOutline(this ExcelRange range)
    {
        range.Style.Border.Top.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
        range.Style.Border.Bottom.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
        range.Style.Border.Left.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
        range.Style.Border.Right.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
        return range;
    }
}