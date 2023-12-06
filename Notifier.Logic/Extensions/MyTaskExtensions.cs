using System.Runtime.CompilerServices;

namespace Notifier.Logic.Extensions
{
    public static class MyTaskExtensions
    {
        public static TaskAwaiter<(T1, T2)> GetAwaiter<T1, T2>(this (Task<T1>, Task<T2>) tasks)
        {
            async Task<(T1, T2)> CombineTasks()
            {
                var (task1, task2) = tasks;
                await Task.WhenAll(task1, task2);
                return (task1.Result, task2.Result);
            }
            return CombineTasks().GetAwaiter();
        }
    }
}
