namespace WeatherSolution.Models.Weather;

public record WeatherDataRm<T>(
    T chart,
    List<Category> categories,
    List<DataSet> dataset
    );
