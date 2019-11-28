namespace ToSic.SexyContent.WebApi
{
    internal class RouteParts
    {
        public const string EditionKey = "edition";
        public const string EditionToken = "{" + EditionKey + "}";

        public const string ControllerKey = "controller";
        public const string ControllerToken = "{" + ControllerKey + "}";

        public const string ActionKey = "action";
        public const string ActionToken = "{" + ActionKey + "}";

        public const string PathApiContAct = "api/" + ControllerToken + "/" + ActionToken;
    }
}

