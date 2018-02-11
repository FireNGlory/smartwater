using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Alexa.SmartHome.Models.Directives.Discover;
using Alexa.SmartHome.Models.Directives.ReportState;
using Alexa.SmartHome.Models.Directives.SetTemperature;
using Alexa.SmartHome.Models.Directives.SetTemperature.Response;
using Alexa.SmartHome.Models.ValueTypes;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using SWH.ApiHost.Code;

namespace SWH.ApiHost.Controllers
{
    public class SmartHomeController : Controller
    {
        private readonly ILogger<SmartHomeController> _logger;
        private readonly ISmartSensorMaster _master;

        public SmartHomeController(ILogger<SmartHomeController> logger, ISmartSensorMaster master)
        {
            _logger = logger;
            _master = master;
        }

        [HttpPost("api/smarthome/discover/{requestId}")]
        public async Task<IActionResult> ProcessDiscovery(Guid requestId, [FromBody] DiscoverDirective directive)
        {
            var resp = new DiscoverResponse
            {
                Event = new DiscoverEvent
                {
                    Header = new Header
                    {
                        MessageId = directive.Directive.Header.MessageId,
                        Name = "Discover.Response",
                        PayloadVersion = "3",
                        Namespace = "Alexa.Discovery"
                    },
                    Payload = new DiscoverEventPayload
                    {
                        Endpoints = new List<Endpoint>
                        {
                            new Endpoint
                            {
                                EndpointId = "smartwater01",
                                Description = "Smart Water Heater Control",
                                DisplayCategories = new List<string>
                                {
                                    "THERMOSTAT",
                                    "TEMPERATURE_SENSOR"
                                },
                                FriendlyName = "Hot Water",
                                ManufacturerName = "Ryan Mack",
                                Cookie = new EmptyObject(),
                                Capabilities = new List<AlexaInterface>
                                {
                                    new AlexaInterface
                                    {
                                        Interface = "Alexa.ThermostatController",
                                        Version = "3",
                                        ProactivelyReported = true,
                                        Retrievable = true,
                                        Properties = new InterfacePropery
                                        {
                                            Supported = new List<SupportedValue>
                                            {
                                                new SupportedValue("lowerSetpoint"),
                                                new SupportedValue("targetSetpoint")
                                            }
                                        }
                                    }, new AlexaInterface
                                    {
                                        Interface = "Alexa.TemperatureSensor",
                                        Version = "3",
                                        ProactivelyReported = false,
                                        Retrievable = true,
                                        Properties = new InterfacePropery
                                        {
                                            Supported = new List<SupportedValue>
                                            {
                                                new SupportedValue("temperature")
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            };
            return await Task.FromResult(Ok(resp));
        }
        
        [HttpPost("api/smarthome/ReportState/{requestId}")]
        public async Task<IActionResult> ReportState(Guid requestId, [FromBody] ReportStateDirective directive)
        {
            var resp = new ContextResponse
            {
                Context = new EndpointContext
                {
                    Properties = new List<ContextProperty>
                    {
                        new ContextProperty
                        {
                            Name = "temperature",
                            Namespace = "Alexa.TemperatureSensor",
                            TimeOfSample = DateTime.UtcNow.ToString("yyyy'-'MM'-'dd'T'HH':'mm':'ss'.'ff'Z'"),
                            UncertaintyInMilliseconds = 500,
                            Value = new EndpointValue
                            {
                                Value = await _master.GetCurrentTemp(),
                                Scale = "FAHRENHEIT"
                            }
                        },
                        new ContextProperty
                        {
                            Name = "lowerSetpoint",
                            Namespace = "Alexa.ThermostatController",
                            TimeOfSample = DateTime.UtcNow.ToString("yyyy'-'MM'-'dd'T'HH':'mm':'ss'.'ff'Z'"),
                            UncertaintyInMilliseconds = 500,
                            Value = new EndpointValue
                            {
                                Value = 98,
                                Scale = "FAHRENHEIT"
                            }
                        },
                        new ContextProperty
                        {
                            Name = "targetSepoint",
                            Namespace = "Alexa.ThermostatController",
                            TimeOfSample = DateTime.UtcNow.ToString("yyyy'-'MM'-'dd'T'HH':'mm':'ss'.'ff'Z'"),
                            UncertaintyInMilliseconds = 500,
                            Value = new EndpointValue
                            {
                                Value = 102,
                                Scale = "FAHRENHEIT"
                            }
                        }
                    }
                },
                Event = new ReportStateEvent
                {
                    Header = new Header
                    {
                        MessageId = directive.Directive.Header.MessageId,
                        Name = "StateReport",
                        PayloadVersion = "3",
                        Namespace = "Alexa",
                        CorrelationToken = directive.Directive.Header.CorrelationToken
                    },
                    Endpoint = new DirectiveEndpoint
                    {
                        Scope = directive.Directive.Endpoint.Scope,
                        EndpointId = "smartwater01"
                    },
                    Payload = new EmptyObject()
                }
            };

            return Ok(resp);
        }

        [HttpPost("api/smarthome/SetTargetTemperature/{requestId}")]
        public async Task<IActionResult> SetTemperature(Guid requestId, [FromBody] SetTemperatureDirective directive)
        {
            /*            using (var tempFile = System.IO.File.OpenWrite(@"J:\temp\request.json"))
                        {
                            await Request.Body.CopyToAsync(tempFile);
                        }*/

            var newValue = directive.Directive.Payload.TargetSetpoint.Value;

            await _master.SetNewTemp((int)newValue);


            var resp = new SetTemperatureResponse
            {
                Context = new EndpointContext
                {
                    Properties = new List<ContextProperty>
                    {
                        new ContextProperty
                        {
                            Name = "temperature",
                            Namespace = "Alexa.TemperatureSensor",
                            TimeOfSample = DateTime.UtcNow.ToString("yyyy'-'MM'-'dd'T'HH':'mm':'ss'.'ff'Z'"),
                            UncertaintyInMilliseconds = 500,
                            Value = new EndpointValue
                            {
                                Value = await _master.GetCurrentTemp(),
                                Scale = "FAHRENHEIT"
                            }
                        },
                        new ContextProperty
                        {
                            Name = "lowerSetpoint",
                            Namespace = "Alexa.ThermostatController",
                            TimeOfSample = DateTime.UtcNow.ToString("yyyy'-'MM'-'dd'T'HH':'mm':'ss'.'ff'Z'"),
                            UncertaintyInMilliseconds = 500,
                            Value = new EndpointValue
                            {
                                Value = newValue,
                                Scale = "FAHRENHEIT"
                            }
                        },
                        new ContextProperty
                        {
                            Name = "targetSepoint",
                            Namespace = "Alexa.ThermostatController",
                            TimeOfSample = DateTime.UtcNow.ToString("yyyy'-'MM'-'dd'T'HH':'mm':'ss'.'ff'Z'"),
                            UncertaintyInMilliseconds = 500,
                            Value = new EndpointValue
                            {
                                Value = newValue,
                                Scale = "FAHRENHEIT"
                            }
                        }
                    }
                },
                Event = new ResponseEvent
                {
                    Header = new Header
                    {
                        MessageId = directive.Directive.Header.MessageId,
                        Name = "Response",
                        PayloadVersion = "3",
                        Namespace = "Alexa",
                        CorrelationToken = directive.Directive.Header.CorrelationToken
                    },
                    Endpoint = new SyncResponseEndpoint
                    {
                        EndpointId = "smartwater01"
                    },
                    Payload = new EmptyObject()
                }
            };

            //System.IO.File.AppendAllText(@"J:\temp\response.json", "\r\n" + JsonConvert.SerializeObject(resp));

            return Ok(resp);
        }

        [HttpPost("api/smarthome/AdjustTargetTemperature/{requestId}")]
        public async Task<IActionResult> AdjustTargetTemperature(Guid requestId, [FromBody] SetDeltaDirective directive)
        {
            var newValue = await _master.GetTempTarget() + directive.Directive.Payload.TargetSetpointDelta.Value;

            await _master.SetNewTemp((int)newValue);

            var resp = new SetTemperatureResponse
            {
                Context = new EndpointContext
                {
                    Properties = new List<ContextProperty>
                    {
                        new ContextProperty
                        {
                            Name = "temperature",
                            Namespace = "Alexa.TemperatureSensor",
                            TimeOfSample = DateTime.UtcNow.ToString("yyyy'-'MM'-'dd'T'HH':'mm':'ss'.'ff'Z'"),
                            UncertaintyInMilliseconds = 500,
                            Value = new EndpointValue
                            {
                                Value = await _master.GetCurrentTemp(),
                                Scale = "FAHRENHEIT"
                            }
                        },
                        new ContextProperty
                        {
                            Name = "lowerSetpoint",
                            Namespace = "Alexa.ThermostatController",
                            TimeOfSample = DateTime.UtcNow.ToString("yyyy'-'MM'-'dd'T'HH':'mm':'ss'.'ff'Z'"),
                            UncertaintyInMilliseconds = 500,
                            Value = new EndpointValue
                            {
                                Value = newValue,
                                Scale = "FAHRENHEIT"
                            }
                        },
                        new ContextProperty
                        {
                            Name = "targetSepoint",
                            Namespace = "Alexa.ThermostatController",
                            TimeOfSample = DateTime.UtcNow.ToString("yyyy'-'MM'-'dd'T'HH':'mm':'ss'.'ff'Z'"),
                            UncertaintyInMilliseconds = 500,
                            Value = new EndpointValue
                            {
                                Value = newValue,
                                Scale = "FAHRENHEIT"
                            }
                        }
                    }
                },
                Event = new ResponseEvent
                {
                    Header = new Header
                    {
                        MessageId = directive.Directive.Header.MessageId,
                        Name = "Response",
                        PayloadVersion = "3",
                        Namespace = "Alexa",
                        CorrelationToken = directive.Directive.Header.CorrelationToken
                    },
                    Endpoint = new SyncResponseEndpoint
                    {
                        EndpointId = "smartwater01"
                    },
                    Payload = new EmptyObject()
                }
            };

            //System.IO.File.AppendAllText(@"J:\temp\response.json", "\r\n" + JsonConvert.SerializeObject(resp));

            return Ok(resp);
        }
    }
}
