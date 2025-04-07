using DocumentGenerator.UI.Models.Pages;
using DocumentGenerator.UI.ViewModels.Pages;

namespace DocumentGenerator.UI.Services.WindowsNavigation;

public interface IViewNavigation
{ 
    void OnRedirect(ViewTypes targetView);
}