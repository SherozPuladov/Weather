namespace WeatherSolution.Models.Weather.Charts;

public record LineChart(
    string caption,
    string subcaption,
    string yaxisname,
    string xaxisname,
    string forceaxislimits,
    string pixelsperpoint,
    string pixelsperlabel,
    string compactdatamode,
    string dataseparator,
    string theme
    );
