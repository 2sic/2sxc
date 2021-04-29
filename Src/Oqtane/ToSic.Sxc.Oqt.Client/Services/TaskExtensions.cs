using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ToSic.Sxc.Oqt.Client.Services
{
    /// <summary>
    /// Defines extension methods for <see cref="Task"/> and <see cref="ValueTask"/>.
    /// </summary>
    public static class TaskExtensions
    {
        /// <summary>
        /// Synchronously await the results of an asynchronous operation without deadlocking; ignoring cancellation.
        /// </summary>
        /// <param name="task">
        /// The <see cref="Task"/> representing the pending operation.
        /// </param>
        public static void AwaitCompletion(this ValueTask task)
        {
            new SynchronousAwaiter(task, true).GetResult();
        }

        /// <summary>
        /// Synchronously await the results of an asynchronous operation without deadlocking; ignoring cancellation.
        /// </summary>
        /// <param name="task">
        /// The <see cref="Task"/> representing the pending operation.
        /// </param>
        public static void AwaitCompletion(this Task task)
        {
            new SynchronousAwaiter(task, true).GetResult();
        }

        /// <summary>
        /// Synchronously await the results of an asynchronous operation without deadlocking.
        /// </summary>
        /// <param name="task">
        /// The <see cref="Task"/> representing the pending operation.
        /// </param>
        /// <typeparam name="T">
        /// The result type of the operation.
        /// </typeparam>
        /// <returns>
        /// The result of the operation.
        /// </returns>
        public static T AwaitResult<T>(this Task<T> task)
        {
            return new SynchronousAwaiter<T>(task).GetResult();
        }

        /// <summary>
        /// Synchronously await the results of an asynchronous operation without deadlocking.
        /// </summary>
        /// <param name="task">
        /// The <see cref="Task"/> representing the pending operation.
        /// </param>
        public static void AwaitResult(this Task task)
        {
            new SynchronousAwaiter(task).GetResult();
        }

        /// <summary>
        /// Synchronously await the results of an asynchronous operation without deadlocking.
        /// </summary>
        /// <param name="task">
        /// The <see cref="ValueTask"/> representing the pending operation.
        /// </param>
        /// <typeparam name="T">
        /// The result type of the operation.
        /// </typeparam>
        /// <returns>
        /// The result of the operation.
        /// </returns>
        public static T AwaitResult<T>(this ValueTask<T> task)
        {
            return new SynchronousAwaiter<T>(task).GetResult();
        }

        /// <summary>
        /// Synchronously await the results of an asynchronous operation without deadlocking.
        /// </summary>
        /// <param name="task">
        /// The <see cref="ValueTask"/> representing the pending operation.
        /// </param>
        public static void AwaitResult(this ValueTask task)
        {
            new SynchronousAwaiter(task).GetResult();
        }

        /// <summary>
        /// Ignore the <see cref="OperationCanceledException"/> if the operation is cancelled.
        /// </summary>
        /// <param name="task">
        /// The <see cref="Task"/> representing the asynchronous operation whose cancellation is to be ignored.
        /// </param>
        /// <returns>
        /// The <see cref="Task"/> representing the asynchronous operation whose cancellation is ignored.
        /// </returns>
        public static async Task IgnoreCancellationResult(this Task task)
        {
            try
            {
                await task.ConfigureAwait(false);
            }
            catch (OperationCanceledException)
            {
            }
        }

        /// <summary>
        /// Ignore the <see cref="OperationCanceledException"/> if the operation is cancelled.
        /// </summary>
        /// <param name="task">
        /// The <see cref="ValueTask"/> representing the asynchronous operation whose cancellation is to be ignored.
        /// </param>
        /// <returns>
        /// The <see cref="ValueTask"/> representing the asynchronous operation whose cancellation is ignored.
        /// </returns>
        public static async ValueTask IgnoreCancellationResult(this ValueTask task)
        {
            try
            {
                await task.ConfigureAwait(false);
            }
            catch (OperationCanceledException)
            {
            }
        }

        /// <summary>
        /// Ignore the results of an asynchronous operation allowing it to run and die silently in the background.
        /// </summary>
        /// <param name="task">
        /// The <see cref="Task"/> representing the asynchronous operation whose results are to be ignored.
        /// </param>
        public static async void IgnoreResult(this Task task)
        {
            try
            {
                await task.ConfigureAwait(false);
            }
            catch
            {
                // ignore exceptions
            }
        }

        /// <summary>
        /// Ignore the results of an asynchronous operation allowing it to run and die silently in the background.
        /// </summary>
        /// <param name="task">
        /// The <see cref="ValueTask"/> representing the asynchronous operation whose results are to be ignored.
        /// </param>
        public static async void IgnoreResult(this ValueTask task)
        {
            try
            {
                await task.ConfigureAwait(false);
            }
            catch
            {
                // ignore exceptions
            }
        }
    }

    /// <summary>
    /// Internal class for waiting for asynchronous operations that have a result.
    /// </summary>
    /// <typeparam name="TResult">
    /// The result type.
    /// </typeparam>
    public class SynchronousAwaiter<TResult>
    {
        /// <summary>
        /// The manual reset event signaling completion.
        /// </summary>
        private readonly ManualResetEvent manualResetEvent;

        /// <summary>
        /// The exception thrown by the asynchronous operation.
        /// </summary>
        private Exception exception;

        /// <summary>
        /// The result of the asynchronous operation.
        /// </summary>
        private TResult result;

        /// <summary>
        /// Initializes a new instance of the <see cref="SynchronousAwaiter{TResult}"/> class.
        /// </summary>
        /// <param name="task">
        /// The task representing an asynchronous operation.
        /// </param>
        public SynchronousAwaiter(Task<TResult> task)
        {
            this.manualResetEvent = new ManualResetEvent(false);
            this.WaitFor(task);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SynchronousAwaiter{TResult}"/> class.
        /// </summary>
        /// <param name="task">
        /// The task representing an asynchronous operation.
        /// </param>
        public SynchronousAwaiter(ValueTask<TResult> task)
        {
            this.manualResetEvent = new ManualResetEvent(false);
            this.WaitFor(task);
        }

        /// <summary>
        /// Gets a value indicating whether the operation is complete.
        /// </summary>
        public bool IsComplete => this.manualResetEvent.WaitOne(0);

        /// <summary>
        /// Synchronously get the result of an asynchronous operation.
        /// </summary>
        /// <returns>
        /// The result of the asynchronous operation.
        /// </returns>
        public TResult GetResult()
        {
            this.manualResetEvent.WaitOne();
            return this.exception != null ? throw this.exception : this.result;
        }

        /// <summary>
        /// Tries to synchronously get the result of an asynchronous operation.
        /// </summary>
        /// <param name="operationResult">
        /// The result of the operation.
        /// </param>
        /// <returns>
        /// The result of the asynchronous operation.
        /// </returns>
        public bool TryGetResult(out TResult operationResult)
        {
            if (this.IsComplete)
            {
                operationResult = this.exception != null ? throw this.exception : this.result;
                return true;
            }

            operationResult = default;
            return false;
        }

        /// <summary>
        /// Background "thread" which waits for the specified asynchronous operation to complete.
        /// </summary>
        /// <param name="task">
        /// The task.
        /// </param>
        private async void WaitFor(Task<TResult> task)
        {
            try
            {
                this.result = await task.ConfigureAwait(false);
            }
            catch (Exception exception)
            {
                this.exception = exception;
            }
            finally
            {
                this.manualResetEvent.Set();
            }
        }

        /// <summary>
        /// Background "thread" which waits for the specified asynchronous operation to complete.
        /// </summary>
        /// <param name="task">
        /// The task.
        /// </param>
        private async void WaitFor(ValueTask<TResult> task)
        {
            try
            {
                this.result = await task.ConfigureAwait(false);
            }
            catch (Exception exception)
            {
                this.exception = exception;
            }
            finally
            {
                this.manualResetEvent.Set();
            }
        }
    }

    /// <summary>
    /// Internal class for  waiting for  asynchronous operations that have no result.
    /// </summary>
    public class SynchronousAwaiter
    {
        /// <summary>
        /// The manual reset event signaling completion.
        /// </summary>
        private readonly ManualResetEvent manualResetEvent = new ManualResetEvent(false);

        /// <summary>
        /// The exception thrown by the asynchronous operation.
        /// </summary>
        private Exception exception;

        /// <summary>
        /// Initializes a new instance of the <see cref="SynchronousAwaiter{TResult}"/> class.
        /// </summary>
        /// <param name="task">
        /// The task representing an asynchronous operation.
        /// </param>
        /// <param name="ignoreCancellation">
        /// Indicates whether to ignore cancellation. Default is false.
        /// </param>
        public SynchronousAwaiter(Task task, bool ignoreCancellation = false)
        {
            this.manualResetEvent = new ManualResetEvent(false);
            this.WaitFor(task, ignoreCancellation);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SynchronousAwaiter{TResult}"/> class.
        /// </summary>
        /// <param name="task">
        /// The task representing an asynchronous operation.
        /// </param>
        /// <param name="ignoreCancellation">
        /// Indicates whether to ignore cancellation. Default is false.
        /// </param>
        public SynchronousAwaiter(ValueTask task, bool ignoreCancellation = false)
        {
            this.manualResetEvent = new ManualResetEvent(false);
            this.WaitFor(task, ignoreCancellation);
        }

        /// <summary>
        /// Gets a value indicating whether the operation is complete.
        /// </summary>
        public bool IsComplete => this.manualResetEvent.WaitOne(0);

        /// <summary>
        /// Synchronously get the result of an asynchronous operation.
        /// </summary>
        public void GetResult()
        {
            this.manualResetEvent.WaitOne();
            if (this.exception != null)
            {
                throw this.exception;
            }
        }

        /// <summary>
        /// Background "thread" which waits for the specified asynchronous operation to complete.
        /// </summary>
        /// <param name="task">
        /// The task.
        /// </param>
        /// <param name="ignoreCancellation">
        /// Indicates whether to ignore cancellation. Default is false.
        /// </param>
        private async void WaitFor(Task task, bool ignoreCancellation)
        {
            try
            {
                await task.ConfigureAwait(false);
            }
            catch (OperationCanceledException)
            {
            }
            catch (Exception exception)
            {
                this.exception = exception;
            }
            finally
            {
                this.manualResetEvent.Set();
            }
        }

        /// <summary>
        /// Background "thread" which waits for the specified asynchronous operation to complete.
        /// </summary>
        /// <param name="task">
        ///     The task.
        /// </param>
        /// <param name="ignoreCancellation">
        /// Indicates whether to ignore cancellation. Default is false.
        /// </param>
        private async void WaitFor(ValueTask task, bool ignoreCancellation)
        {
            try
            {
                await task.ConfigureAwait(false);
            }
            catch (OperationCanceledException)
            {
            }
            catch (Exception exception)
            {
                this.exception = exception;
            }
            finally
            {
                this.manualResetEvent.Set();
            }
        }
    }
}
