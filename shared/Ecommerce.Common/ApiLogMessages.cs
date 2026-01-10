namespace Ecommerce.Common
{
    public static class ApiLogMessages
    {
        public const string GetListError = "Error while getting list";
        public const string GetByIdError = "Error while getting by id {Id}";
        public const string CreateError = "Error while creating record";
        public const string UpdateError = "Error while updating record {Id}";
        public const string DeleteError = "Error while deleting record {Id}";
        public const string ValidationFailed = "Validation failed: {Details}";
        public const string UnhandledException = "Unhandled exception: {Message}";
    }
}
