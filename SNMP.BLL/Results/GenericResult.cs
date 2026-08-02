using System;
using System.Collections.Generic;
using System.Text;

namespace Snmp.Business.Results
{
    public class Result<T> : Result
    {
        public T? Value { get; }

        private Result (T value): base(true,null)
        {
            Value = value;
        }

        private Result(string error) : base(false, error)
        {
            Value = default;
        }

        public static Result<T> Success(T value) => new Result<T>(value);

        public static Result<T> Failure(string error) => new Result<T>(error);
    }
}
