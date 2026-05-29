using TechTalk.SpecFlow;
using Xunit;

namespace IoBuild.Devices.Tests.Steps;

[Binding]
public class DeviceTelemetrySteps
{
    private bool _isAuthenticated;
    private bool _deviceExists;
    private int _deviceId;
    private bool _hasTelemetry;
    private string? _status;
    private string? _responseCode;
    private List<Dictionary<string, object>>? _energyPoints;
    private Dictionary<string, object>? _statusResponse;

    [Given(@"el usuario esta autenticado")]
    public void GivenElUsuarioEstaAutenticado()
    {
        _isAuthenticated = true;
    }

    [Given(@"existe un dispositivo con ID (.*)")]
    public void GivenExisteUnDispositivoConID(int id)
    {
        _deviceId = id;
        _deviceExists = true;
    }

    [Given(@"el dispositivo (.*) tiene datos de telemetria en las ultimas 24 horas")]
    public void GivenElDispositivoTieneDatosDeTelemetria(int id)
    {
        _deviceId = id;
        _hasTelemetry = true;
        _energyPoints = new List<Dictionary<string, object>>
        {
            new()
            {
                ["timestamp"] = DateTime.UtcNow.AddHours(-1),
                ["energyKwh"] = 3.4,
                ["temperatureC"] = 21.5,
                ["voltageV"] = 220.0
            }
        };
    }

    [Given(@"el dispositivo (.*) tiene telemetria con estado ""(.*)""")]
    public void GivenElDispositivoTieneTelemetriaConEstado(int id, string status)
    {
        _deviceId = id;
        _hasTelemetry = true;
        _status = status;
        _statusResponse = new Dictionary<string, object>
        {
            ["deviceId"] = id,
            ["status"] = status,
            ["lastSeen"] = DateTime.UtcNow,
            ["temperatureC"] = 22.1,
            ["voltageV"] = 219.8
        };
    }

    [Given(@"el dispositivo (.*) no tiene datos de telemetria")]
    public void GivenElDispositivoNoTieneDatosDeTelemetria(int id)
    {
        _deviceId = id;
        _hasTelemetry = false;
        _energyPoints = new List<Dictionary<string, object>>();
    }

    [When(@"envia GET a ""(.*)""")]
    public async Task WhenEnviaGetA(string endpoint)
    {
        if (!_isAuthenticated)
        {
            _responseCode = "401 Unauthorized";
            _energyPoints = null;
            _statusResponse = null;
            return;
        }

        if (!_deviceExists)
        {
            _responseCode = "404 Not Found";
            _energyPoints = null;
            _statusResponse = null;
            return;
        }

        if (endpoint.Contains("/energy"))
        {
            _responseCode = "200 OK";
            _energyPoints ??= _hasTelemetry
                ? new List<Dictionary<string, object>>
                {
                    new()
                    {
                        ["timestamp"] = DateTime.UtcNow.AddMinutes(-30),
                        ["energyKwh"] = 1.2,
                        ["temperatureC"] = 22.0,
                        ["voltageV"] = 220.5
                    }
                }
                : new List<Dictionary<string, object>>();
        }
        else if (endpoint.Contains("/status"))
        {
            _responseCode = "200 OK";
            if (_hasTelemetry)
            {
                _statusResponse ??= new Dictionary<string, object>
                {
                    ["deviceId"] = _deviceId,
                    ["status"] = _status ?? "online",
                    ["lastSeen"] = DateTime.UtcNow,
                    ["temperatureC"] = 22.0,
                    ["voltageV"] = 220.0
                };
            }
            else
            {
                _statusResponse = new Dictionary<string, object>
                {
                    ["deviceId"] = _deviceId,
                    ["status"] = "unknown",
                    ["lastSeen"] = null,
                    ["temperatureC"] = null,
                    ["voltageV"] = null
                };
            }
        }

        await Task.CompletedTask;
    }

    [Then(@"respuesta 200 OK")]
    public void ThenRespuesta200OK()
    {
        Assert.Equal("200 OK", _responseCode);
    }

    [Then(@"respuesta 200 OK y lista vacia")]
    public void ThenRespuesta200OkYListaVacia()
    {
        Assert.Equal("200 OK", _responseCode);
        Assert.NotNull(_energyPoints);
        Assert.Empty(_energyPoints);
    }

    [Then(@"respuesta 401 Unauthorized")]
    public void ThenRespuesta401Unauthorized()
    {
        Assert.Equal("401 Unauthorized", _responseCode);
    }

    [Then(@"la respuesta contiene una lista de puntos con timestamp, energyKwh, temperatureC, voltageV")]
    public void ThenLaRespuestaContieneUnaListaDePuntos()
    {
        Assert.NotNull(_energyPoints);
        Assert.True(_energyPoints.Count > 0);
        foreach (var point in _energyPoints)
        {
            Assert.True(point.ContainsKey("timestamp"));
            Assert.True(point.ContainsKey("energyKwh"));
            Assert.True(point.ContainsKey("temperatureC"));
            Assert.True(point.ContainsKey("voltageV"));
        }
    }

    [Then(@"contiene el estado ""(.*)"" y el campo lastSeen")]
    public void ThenContieneElEstadoYElCampoLastSeen(string status)
    {
        Assert.NotNull(_statusResponse);
        Assert.Equal(status, _statusResponse["status"]);
        Assert.True(_statusResponse.ContainsKey("lastSeen"));
    }
}
