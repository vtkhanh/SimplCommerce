namespace SimplCommerce.Infrastructure.ResultTypes
{
    public class ActionFeedback
    {
        public bool Success;

        public string ErrorMessage;

        protected ActionFeedback(bool success, string errorMessage) => (Success, ErrorMessage) = (success, errorMessage);

        public static ActionFeedback Succeed() => new ActionFeedback(true, string.Empty);

        public static ActionFeedback Fail(string errorMessage) => new ActionFeedback(false, errorMessage);
    }

    public class ActionFeedback<T> : ActionFeedback
    {
        public T Result;

        private ActionFeedback(T result, bool success, string errorMessage) : base(success, errorMessage)
        {
            Result = result;
        }

        public static ActionFeedback<T> Succeed(T result) => new ActionFeedback<T>(result, true, string.Empty);

        public new static ActionFeedback<T> Fail(string errorMessage) => new ActionFeedback<T>(default, false, errorMessage);
    }
}
