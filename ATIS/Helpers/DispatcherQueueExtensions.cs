using Windows.System;

namespace ATIS.Helpers;

public static class DispatcherQueueExtensions
{
    public static Task EnqueueAsync(this DispatcherQueue dispatcher, Action action)
    {
        var tcs = new TaskCompletionSource<object>();

        dispatcher.TryEnqueue(() =>
        {
            try
            {
                action();
                tcs.SetResult(null);
            }
            catch (Exception ex)
            {
                tcs.SetException(ex);
            }
        });

        return tcs.Task;
    }
}
