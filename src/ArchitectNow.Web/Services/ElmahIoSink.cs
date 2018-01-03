using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Elmah.Io.Client;
using Elmah.Io.Client.Models;
using Serilog.Core;
using Serilog.Events;

namespace ArchitectNow.Web.Services
{
   /// <summary>
    /// Writes log events to the elmah.io service.
    /// </summary>
    public class ElmahIoSink : ILogEventSink
    {
        readonly IFormatProvider _formatProvider;
        readonly Guid _logId;
        readonly IElmahioAPI _client;
        private readonly string _version;

        /// <summary>
        /// Construct a sink that saves logs to the specified storage account.
        /// </summary>
        /// <param name="formatProvider">Supplies culture-specific formatting information, or null.</param>
        /// <param name="apiKey">An API key from the organization containing the log.</param>
        /// <param name="logId">The log ID as found on the elmah.io website.</param>
        public ElmahIoSink(IFormatProvider formatProvider, string apiKey, Guid logId)
        {
            _formatProvider = formatProvider;
            _logId = logId;
            _client = ElmahioAPI.Create(apiKey);
            var entryAssembly = Assembly.GetEntryAssembly();
            var assemblyName = entryAssembly.GetName();
            _version = assemblyName.Version.ToString();
        }

        /// <summary>
        /// Construct a sink that saves logs to the specified logger. The purpose of this
        /// constructor is to re-use an existing client from ELMAH or similar.
        /// </summary>
        /// <param name="formatProvider">Supplies culture-specific formatting information, or null.</param>
        /// <param name="client">The client to use.</param>
        public ElmahIoSink(IFormatProvider formatProvider, IElmahioAPI client)
        {
            _formatProvider = formatProvider;
            _client = client;
        }

        /// <summary>
        /// Emit the provided log event to the sink.
        /// </summary>
        /// <param name="logEvent">The log event to write.</param>
        public void Emit(LogEvent logEvent)
        {
            var source = logEvent.Properties.TryGetValue("Source", out var sourceProperty) ? (sourceProperty as ScalarValue)?.Value?.ToString() : string.Empty;
            var user = logEvent.Properties.TryGetValue("User", out var userProperty) ? (userProperty as ScalarValue)?.Value?.ToString() : string.Empty;
            var url = logEvent.Properties.TryGetValue("RequestPath", out var requestPathProperty)
                ? (requestPathProperty as ScalarValue)?.Value?.ToString()
                : string.Empty;
            int.TryParse(
                logEvent.Properties.TryGetValue("StatusCode", out var statusCodeProperty) ? (statusCodeProperty as ScalarValue)?.Value?.ToString() : string.Empty,
                out var statusCode);

            var message = new CreateMessage
            {
                Title = logEvent.RenderMessage(_formatProvider),
                Severity = LevelToSeverity(logEvent),
                DateTime = logEvent.Timestamp.DateTime.ToUniversalTime(),
                Detail = logEvent.Exception?.ToString(),
                Data = PropertiesToData(logEvent),
                Type = Type(logEvent),
                Source = source,
                Hostname = Environment.MachineName,
                User = user,
                Url = url,
                Version = _version,
                StatusCode = statusCode > 0 ? statusCode : (int?) null
            };

            _client.Messages.CreateAndNotify(_logId, message);
        }

        private string Type(LogEvent logEvent)
        {
            return logEvent.Exception?.GetBaseException().GetType().FullName;
        }

        static List<Item> PropertiesToData(LogEvent logEvent)
        {
            var data = new List<Item>();
            if (logEvent.Exception != null)
            {
                data.AddRange(
                    logEvent.Exception.Data.Keys.Cast<object>()
                        .Select(key => new Item {Key = key.ToString(), Value = logEvent.Exception.Data[key].ToString()}));
            }

            data.AddRange(logEvent.Properties.Select(p => new Item {Key = p.Key, Value = p.Value.ToString()}));
            return data;
        }

        static string LevelToSeverity(LogEvent logEvent)
        {
            switch (logEvent.Level)
            {
                case LogEventLevel.Debug:
                    return Severity.Debug.ToString();
                case LogEventLevel.Error:
                    return Severity.Error.ToString();
                case LogEventLevel.Fatal:
                    return Severity.Fatal.ToString();
                case LogEventLevel.Verbose:
                    return Severity.Verbose.ToString();
                case LogEventLevel.Warning:
                    return Severity.Warning.ToString();
                default:
                    return Severity.Information.ToString();
            }
        }
    }
}