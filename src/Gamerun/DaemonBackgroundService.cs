using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Gamerun;

public class DaemonBackgroundService : BackgroundService
{
    private static readonly TimeSpan QueryInterval = TimeSpan.FromSeconds(1);

    private readonly ILogger<DaemonBackgroundService> _logger;

    public DaemonBackgroundService(ILogger<DaemonBackgroundService> logger)
    {
        _logger = logger;
    }

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
                    _logger.LogInformation("Set TDP to {Tdp}", tdp);
                }
            }
            
            await Task.Delay(QueryInterval, stoppingToken);
        }
    }
}