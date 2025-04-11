using System.Threading.Tasks;
using DocumentGenerator.UI.Models.Pages;
using DocumentGenerator.UI.ViewModels.Pages;

namespace DocumentGenerator.UI.Services.WindowsNavigation;

public interface IViewNavigation
{ 
    Task<bool> OnRedirect(ViewTypes targetView);
}