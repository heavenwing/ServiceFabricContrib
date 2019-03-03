using System;

namespace ServiceFabricContrib
{
    /// <summary>
    /// Result DTO of RemotingService method
    /// </summary>
    [Serializable]
    public class RemotingResult
    {
        /// <summary>
        /// Default failed code, -1
        /// </summary>
        public const int DefaultFailedCode = -1;

        protected const string ZeroFailedCode_ErrMsg = "failed code must not be 0";

        /// <summary>
        /// Success status, FailedCode=0
        /// </summary>
        public bool IsSuccess => FailedCode == 0;

        /// <summary>
        /// Failed reasons of RemotingService method (friendly for End-User)
        /// </summary>
        public string FailedMessage { get; set; }

        /// <summary>
        /// Failed code of RemotingService method
        /// </summary>
        public int FailedCode { get; set; }

        /// <summary>
        /// Exception message of RemotingService method (not friendly for End-User)
        /// </summary>
        public string ExceptionMessage { get; set; }

        /// <summary>
        /// Create a failed result with default code
        /// </summary>
        /// <returns></returns>
        public static RemotingResult Fail()
        {
            return new RemotingResult
            {
                FailedCode = DefaultFailedCode,
            };
        }

        /// <summary>
        /// Create a failed result with code
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        public static RemotingResult Fail(int code)
        {
            if (code == 0)
                throw new ArgumentOutOfRangeException(ZeroFailedCode_ErrMsg, nameof(code));

            return new RemotingResult
            {
                FailedCode = code
            };
        }

        /// <summary>
        /// Create a failed result with code and message
        /// </summary>
        /// <param name="code"></param>
        /// <param name="msg"></param>
        /// <returns></returns>
        public static RemotingResult Fail(int code, string msg)
        {
            if (code == 0)
                throw new ArgumentOutOfRangeException(ZeroFailedCode_ErrMsg, nameof(code));

            return new RemotingResult
            {
                FailedCode = code,
                FailedMessage = msg,
            };
        }

        /// <summary>
        /// Create a successful result
        /// </summary>
        /// <returns></returns>
        public static RemotingResult Success() => new RemotingResult();
    }

    /// <summary>
    /// Generic result DTO of RemotingService method with Output value
    /// </summary>
    /// <typeparam name="T"></typeparam>
    [Serializable]
    public class RemotingResult<T> : RemotingResult
    {
        /// <summary>
        /// Output value
        /// </summary>
        public T Output { get; set; }

        /// <summary>
        /// Create a successful result with output
        /// </summary>
        /// <param name="output"></param>
        /// <returns></returns>
        public static RemotingResult<T> Success(T output) =>
            new RemotingResult<T>() { Output = output };

        /// <summary>
        /// Create a failed result with default code
        /// </summary>
        /// <returns></returns>
        new public static RemotingResult<T> Fail()
        {
            return new RemotingResult<T>
            {
                FailedCode = DefaultFailedCode,
            };
        }

        /// <summary>
        /// Create a failed result with code
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        new public static RemotingResult<T> Fail(int code)
        {
            if (code == 0)
                throw new ArgumentOutOfRangeException(ZeroFailedCode_ErrMsg, nameof(code));

            return new RemotingResult<T>
            {
                FailedCode = code
            };
        }

        /// <summary>
        /// Create a failed result with message and code
        /// </summary>
        /// <param name="code"></param>
        /// <param name="msg"></param>
        /// <returns></returns>
        new public static RemotingResult<T> Fail(int code, string msg)
        {
            if (code == 0)
                throw new ArgumentOutOfRangeException(ZeroFailedCode_ErrMsg, nameof(code));

            return new RemotingResult<T>
            {
                FailedCode = code,
                FailedMessage = msg,
            };
        }
    }
}
