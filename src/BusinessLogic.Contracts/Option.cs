using System;

namespace ShowScraper.BusinessLogic.Contracts
{
    public abstract class Option<T>
    {
        internal Option()
        {

        }

        public sealed class Ok : Option<T>
        {
            public Ok(T content)
            {
                Content = content;
            }

            public T Content { get; }
        }

        public sealed class Conflict : Option<T>
        {

        }

        public sealed class NotFound : Option<T>
        {

        }

        public sealed class PreconditionViolation : Option<T>
        {
            public PreconditionViolation(string friendlyMessage, Exception exception = null)
            {
                FriendlyMessage = friendlyMessage;
                Exception = exception;
            }

            public string FriendlyMessage { get; }
            public Exception Exception { get; }
        }
    }
}
