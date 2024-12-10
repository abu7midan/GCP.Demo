namespace GCP.Demo
{
    public static class ApiEndpoints
    {
        private const string ApiBase = "api";

        public class Email
        {
            private const string Base = $"{ApiBase}/email";

            public const string Send = Base;
            //public const string Get = $"{ApiBase}/email/{{id:guid}}";
            //public const string GetAll = $"{ApiBase}/email";
            //public const string Update = $"{ApiBase}/email/{{id:guid}}";
            //public const string Delete = $"{ApiBase}/email/{{id:guid}}";
        }
    }
}
