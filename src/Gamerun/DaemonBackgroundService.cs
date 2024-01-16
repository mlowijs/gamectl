using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Gamerun;

public class DaemonBackgroundService(ILogger<DaemonBackgroundService> logger) : BackgroundService
{
    private static readonly TimeSpan QueryInterval = TimeSpan.FromSeconds(1);

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        bool? isCharging = null;
        
        while (!stoppingToken.IsCancellationRequested)
        {
            var currentState = Sysfs.GetBatteryChargerConnected();

            if (isCharging != currentState)
            {
                isCharging = currentState;

                var tdp = isCharging.Value ? Configuration.Values.DaemonAcTdp : Configuration.Values.DaemonDcTdp;

                if (tdp is not null)
                {
                    Ryzenadj.SetTdp(tdp.Value);
                    logger.LogInformation("Set TDP to {Tdp}", tdp);
                }
            }
            
            await Task.Delay(QueryInterval, stoppingToken);
        }
    }
}