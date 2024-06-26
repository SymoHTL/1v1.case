﻿namespace Aspire.Web;

public static class TaskExtensions {
    public static TaskAwaiter<(T1, T2)> GetAwaiter<T1, T2>(this (Task<T1>, Task<T2>) task) {
        return CombineTasks(task.Item1, task.Item2).GetAwaiter();
    }
     
    public static  TaskAwaiter<(T1, T2, T3)> GetAwaiter<T1, T2, T3>(this (Task<T1>, Task<T2>, Task<T3>) task) {
        return CombineTasks(task.Item1, task.Item2, task.Item3).GetAwaiter();
    }
    

    private static async Task<(T1, T2)> CombineTasks<T1, T2>(Task<T1> task1, Task<T2> task2) {
        await Task.WhenAll(task1, task2);
        return (task1.Result, task2.Result);
    }
    
    private static async Task<(T1,T2, T3)> CombineTasks<T1, T2, T3>(Task<T1> task1, Task<T2> task2, Task<T3> task3) {
        await Task.WhenAll(task1, task2, task3);
        return (task1.Result, task2.Result, task3.Result);
    }
}