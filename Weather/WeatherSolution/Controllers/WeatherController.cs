using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http;
using System.Net;

using WeatherSolution.Models;
using WeatherSolution.Models.Weather;
using WeatherSolution.Models.Weather.Charts;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace WeatherSolution.Controllers;

public class WeatherController : Controller
{
    private readonly ILogger<WeatherController> _logger;

    public WeatherController(ILogger<WeatherController> logger)
    {
        _logger = logger;
    }

    public IActionResult Index()
    {
        return View();
    }

    [HttpGet]
    public IActionResult Weather(string station, string start, string end)
    {
        if (station == null)
            return NotFound();

        JToken dataToken = GetData();

        IEnumerable<JToken> children = dataToken.Children();

        var tavg = GetChilds("tavg");
        var tmin = GetChilds("tmin");
        var tmax = GetChilds("tmax");

        var date = new List<Label>();
        foreach (var ch in children)
            date.Add(new Label((string)ch["date"]));

        var response = CreateWeatherDataRm();
        Console.WriteLine(response);
        return Ok(response);

        JToken GetData()
        {
            var url = $"https://meteostat.p.rapidapi.com/stations/monthly" +
                $"?station={station}&start={start}&end={end}";

            var request = (HttpWebRequest)WebRequest.Create(url);
            request.Method = "GET";

            request.Headers["X-RapidAPI-Key"] = "0f5e3ec321mshe20a41707387a8ap1828b9jsne2988a5e53ba";
            request.Headers["X-RapidAPI-Host"] = "meteostat.p.rapidapi.com";

            using var webResponse = request.GetResponse();

            using var webStream = webResponse.GetResponseStream();

            using var reader = new StreamReader(webStream);

            var data = reader.ReadToEnd();

            JObject json = JObject.Parse(data);

            return json["data"];
        }

        List<Data> GetChilds(string childName)
        {
            List<Data> child = new List<Data>();
            foreach (var ch in children)
                child.Add(new Data((double)ch[childName]));
            return child;
        }

        WeatherDataRm<RadarChart> CreateWeatherDataRm()
        {
            return new WeatherDataRm<RadarChart>(
                chart: new RadarChart(
                    caption: $"Weather in {station}",
                    subCaption: "Based on data collected last year",
                    numberPrefix: "",
                    theme: "fusion",
                    radarfillcolor: "#ffffff"
                    ),

                categories: new List<Category> {
                    new Category(
                        date
                        ),
                },

                dataset: new List<DataSet> {
                    new DataSet(
                        seriesname: "Temperature average",
                        tavg
                        ),
                    new DataSet(
                        seriesname: "Minimal temperature",
                        tmin
                        ),
                    new DataSet(
                        seriesname: "Maximum temperature",
                        tmax
                        )
                }

            );

        }
    }
}
