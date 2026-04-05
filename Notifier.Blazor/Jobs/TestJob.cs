using System.Diagnostics;
using Notifier.Matrix.Services;
using Notifier.Vk.Contract;
using Notifier.Vk.Models;

namespace Notifier.Blazor.Jobs;

internal class TestJob : Job<TestJob>
{
    private readonly IVkVideoRestClient _client;
    private readonly MatrixCredentialsService _matrixService;
    public TestJob(ILogger<TestJob> logger, IVkVideoRestClient client, MatrixCredentialsService matrixService) : base(logger)
    {
        _client = client;
        _matrixService = matrixService;
    }

    protected override async Task Run()
    {
        // var ownerId = "203677279";
        // var playlistId = "4";
        // await foreach (var v in _client.GetVideos(ownerId, VkOwnerType.Group, playlistId))
        // {
        //     Debugger.Break();
        // }

        var token = await _matrixService.GetAccessToken();
        Debugger.Break();
    }
}