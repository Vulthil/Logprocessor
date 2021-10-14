using System;

namespace UI.Client.Config
{
    public class ServerStatus
    {
        public bool IsOnline { get; set; }
    }
    public class Store
    {

        private PersistentStorageQueryConfiguration _queryConfiguration;

        public PersistentStorageQueryConfiguration QueryConfiguration
        {
            get => _queryConfiguration;
            set
            {
                _queryConfiguration = value;
                NotifyStateChanged();
            }
        }

        public event Action OnChange;

        private void NotifyStateChanged() => OnChange?.Invoke();
    }


    public class PersistentStorageQueryConfiguration
    {
        public string QueryUrl { get; set; }
        public string LeftDelimit { get; set; } = "{";
        public string RightDelimit { get; set; } = "}";
        public string Token { get; set; } = "sessionid";

        public string GenerateUrl(string sessionId)
        {
            var replace = LeftDelimit + Token + RightDelimit;
            return QueryUrl.Replace(replace, sessionId);
        }
    }

}