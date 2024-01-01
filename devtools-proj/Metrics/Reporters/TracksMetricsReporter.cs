using devtools_proj.Metrics.ReporterInterfaces;
using devtools_proj.Persistence;
using MongoDB.Driver;
using Prometheus;

namespace devtools_proj.Metrics.Reporters;

public class TracksMetricsReporter : ITracksMetricsReporter
{
    private readonly ILogger<TracksMetricsReporter> _logger;

    private readonly Gauge _tracksGauge;

    private readonly Counter _tracksTotal;

    private readonly Counter _tracksRemovedTotal;


    public TracksMetricsReporter(IDbContext db, ILogger<TracksMetricsReporter> logger)
    {
        _logger = logger;

        _tracksGauge = Prometheus.Metrics
            .CreateGauge("app_tracks_count",
                "Current number of tracks that were registered in the app.");

        _tracksTotal = Prometheus.Metrics
            .CreateCounter("app_session_tracks_added_total",
                "Total number of tracks that were registered during the session of the app.");

        _tracksRemovedTotal = Prometheus.Metrics
            .CreateCounter("app_session_tracks_removed_total",
                "Total number of tracks that were removed during the session of the app.");

        var count = db.Tracks.Find(_ => true).CountDocuments();
        _tracksGauge.Set(count);
    }

    public void IncrementGauge()
    {
        _tracksGauge.Inc();
        _tracksTotal.Inc();
    }

    public void DecrementGauge()
    {
        _tracksGauge.Dec();
        _tracksRemovedTotal.Inc();
    }
}