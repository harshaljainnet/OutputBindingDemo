using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;

namespace OutputBindingDemo;

public class Function1
{
    private readonly ILogger<Function1> _logger;

    public Function1(ILogger<Function1> logger)
    {
        _logger = logger;
    }

    [Function("SubmitFeedback")]
    public async Task<SubmitFeedbackResponse> Run([HttpTrigger(AuthorizationLevel.Function, "post")] HttpRequestData req)
    {
        var feedbackContents = await req.ReadAsStringAsync();
        string blobContents = $"Feedback Received at: {DateTime.UtcNow:u}:\n\n{feedbackContents}";

        return new SubmitFeedbackResponse()
        {
            OutputData = blobContents,
            HttpResponse = req.CreateResponse(System.Net.HttpStatusCode.OK)
        };

    }
}

public class SubmitFeedbackResponse
{
    [BlobOutput("feedback/{rand-guid}.txt", Connection = "AzureWebJobsStorage")]
    public string OutputData { get; set; }

    public HttpResponseData HttpResponse { get; set; }
}