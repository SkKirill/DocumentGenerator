using DocumentGenerator.Core.Services;
using DocumentGenerator.Core.Services.ReaderTables;
using DocumentGenerator.Data.Models.Processing;
using DocumentGenerator.Data.Services.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace DocumentGenerator.Core;

public static class StartServicesCore
{
    public static IServiceCollection AddCore(this IServiceCollection services)
    {
        return services
            .AddSingleton<StartProcess>()
            .AddSingleton<IWriteProcessingNotifier>(provider => provider.GetRequiredService<StartProcess>())
            .AddSingleton<ISubscriber>(provider => provider.GetRequiredService<StartProcess>())
            .AddSingleton<IReadManager, ReadManager>();
    }
}