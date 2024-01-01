namespace devtools_proj.Metrics.ReporterInterfaces;

public interface ITracksMetricsReporter
{
    public void IncrementGauge();

    public void DecrementGauge();
}