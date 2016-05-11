#region License
// Copyright 2016 Adam Latchem
// 
//    Licensed under the Apache License, Version 2.0 (the "License"); 
//    you may not use this file except in compliance with the License. 
//    You may obtain a copy of the License at
// 
//        http://www.apache.org/licenses/LICENSE-2.0 
//
//    Unless required by applicable law or agreed to in writing, software
//    distributed under the License is distributed on an "AS IS" BASIS,
//    WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied. 
//    See the License for the specific language governing permissions and 
//    limitations under the License. 
//
#endregion
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Almond.Utility
{
    public class Threading
    {
        /// <summary>
        /// Run a codeBlock for upto timeoutInSeconds seconds
        /// </summary>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="codeBlock"></param>
        /// <param name="timeoutInSeconds"></param>
        /// <returns></returns>
        public static TResult RunWithTimeout<TResult>(Func<TResult> codeBlock, int timeoutInSeconds)
        {
            if (timeoutInSeconds < 0)
                throw new ArgumentException("Timeout must be non negative");

            ManualResetEvent completedSignal = new ManualResetEvent(false);
            completedSignal.Reset();

            TResult workerResult = default(TResult);
            Exception exception = null;
            Task.Factory.StartNew(() => {
                try
                {
                    workerResult = codeBlock();
                }
                catch (Exception e)
                {
                    exception = e;
                }
                finally
                {
                    completedSignal.Set();
                }
            });

            bool completed = completedSignal.WaitOne(
                timeoutInSeconds == 0 ? System.Threading.Timeout.Infinite : 1000 * (int)timeoutInSeconds);
            if (!completed)
                throw new TimeoutException(string.Format("Timeout after {0} seconds", timeoutInSeconds));
            if (exception != null)
                throw exception;

            return workerResult;
        }
    }
}
