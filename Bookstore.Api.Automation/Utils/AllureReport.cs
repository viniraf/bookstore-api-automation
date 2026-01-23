using RestSharp;
using System.Text;
using System.Text.Json;
using Allure.Net.Commons;

namespace Bookstore.Api.Automation.Utils
{
    /// <summary>
    /// Centralized utility for Allure reporting.
    /// Provides methods to attach steps, API calls, and assertion results.
    /// </summary>
    public static class AllureReport
    {
        private static readonly JsonSerializerOptions JsonOptions = new()
        {
            WriteIndented = true,
            PropertyNameCaseInsensitive = true
        };

        /// <summary>
        /// Adds an Arrange step with attached data.
        /// </summary>
        public static void Arrange(string stepName, object? data)
        {
            AddStep(stepName, () =>
            {
                if (data != null)
                {
                    var json = SerializeToJson(data);
                    AttachJson($"Arrange Data", json);
                }
            });
        }

        /// <summary>
        /// Attaches API call details (request and response).
        /// </summary>
        public static void AttachApiCall(object? request, RestResponse response, string endpoint)
        {
            AddStep($"API Call - {endpoint}", () =>
            {
                if (request != null)
                {
                    var requestJson = SerializeToJson(request);
                    AttachJson("Request Body", requestJson);
                }

                var responseDetails = new StringBuilder();
                responseDetails.AppendLine($"Status Code: {response.StatusCode}");
                responseDetails.AppendLine($"Status Description: {response.StatusDescription}");
                responseDetails.AppendLine($"Content Length: {response.Content?.Length ?? 0}");

                AllureApi.AddAttachment("Response Info", "text/plain", responseDetails.ToString());

                if (!string.IsNullOrEmpty(response.Content))
                {
                    try
                    {
                        AttachJson("Response Body", response.Content);
                    }
                    catch
                    {
                        AttachText("Response Body", response.Content);
                    }
                }

                if (response.ErrorException != null)
                {
                    AttachText("Error", response.ErrorException.Message);
                }
            });
        }

        /// <summary>
        /// Adds validation step with assertions.
        /// </summary>
        public static void Assertions(AssertionBuilder assertions)
        {
            AddStep("Validations", () =>
            {
                AttachText("Assertions", assertions.Build());
            });
        }

        /// <summary>
        /// Generic step method for custom operations.
        /// </summary>
        public static void AddStep(string stepName, Action action)
        {
            AllureApi.Step(stepName, action);
        }

        /// <summary>
        /// Attaches formatted text content.
        /// </summary>
        private static void AttachText(string name, string content)
        {
            AllureApi.AddAttachment(name, "text/plain", content);
        }

        /// <summary>
        /// Attaches JSON formatted content.
        /// </summary>
        private static void AttachJson(string name, string content)
        {
            AllureApi.AddAttachment(name, "application/json", content);
        }

        /// <summary>
        /// Serializes object to JSON string.
        /// </summary>
        private static string SerializeToJson(object? obj)
        {
            if (obj == null)
                return "null";

            try
            {
                return JsonSerializer.Serialize(obj, JsonOptions);
            }
            catch
            {
                return obj.ToString() ?? "Unable to serialize";
            }
        }

        /// <summary>
        /// Builder for grouping multiple assertions with formatted output.
        /// </summary>
        public class AssertionBuilder
        {
            private readonly List<(string field, string expected, string actual)> _assertions = [];

            public AssertionBuilder Add(string field, object? expected, object? actual)
            {
                _assertions.Add((field, expected?.ToString() ?? "null", actual?.ToString() ?? "null"));
                return this;
            }

            public string Build()
            {
                if (_assertions.Count == 0)
                    return "No assertions recorded";

                var sb = new StringBuilder();
                sb.AppendLine("ASSERTIONS SUMMARY");
                sb.AppendLine("========================================");

                foreach (var (field, expected, actual) in _assertions)
                {
                    sb.AppendLine();
                    sb.AppendLine($"Field: {field}");
                    sb.AppendLine($"Expected: {expected}");
                    sb.AppendLine($"Actual: {actual}");
                    sb.AppendLine($"Status: {(expected == actual ? "? PASS" : "? FAIL")}");
                    sb.AppendLine("----------------------------------------");
                }

                return sb.ToString();
            }
        }
    }
}
