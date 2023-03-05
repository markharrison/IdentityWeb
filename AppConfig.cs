
using Microsoft.AspNetCore.Mvc;
using Microsoft.Identity.Web.Resource;

namespace IdentityWeb
{
    public class AppConfig
    {
        private string _AdminPWVal;

        private string _APIURLRed;
        private string _ScopesRedRead;
        private string _ScopesRedReadWrite;
        private string _APIURLYellow;
        private string _ScopesYellowRead;
        private string _ScopesYellowReadWrite;
        private string _APIURLBlack;
        private string _ScopesBlackRead;
        private string _ScopesBlackReadWrite;


        public AppConfig(IConfiguration config)
        {
            _AdminPWVal = config.GetValue<string>("AdminPW") ?? "";
            _APIURLRed = config.GetValue<string>("APIURLRed") ?? "";
            _ScopesRedRead = config.GetValue<string>("ScopesRedRead") ?? "";
            _ScopesRedReadWrite = config.GetValue<string>("ScopesRedReadWrite") ?? "";
            _APIURLYellow = config.GetValue<string>("APIURLYellow") ?? "";
            _ScopesYellowRead = config.GetValue<string>("ScopesYellowRead") ?? "";
            _ScopesYellowReadWrite = config.GetValue<string>("ScopesYellowReadWrite") ?? "";
            _APIURLBlack = config.GetValue<string>("APIURLBlack") ?? "";
            _ScopesBlackRead = config.GetValue<string>("ScopesBlackRead") ?? "";
            _ScopesBlackReadWrite = config.GetValue<string>("ScopesBlackReadWrite") ?? "";
        }

        public string AdminPW
        {
            get => this._AdminPWVal;
            set => this._AdminPWVal = value;
        }

        public string APIURLRed
        {
            get => this._APIURLRed;
            set => this._APIURLRed = value;
        }

        public string ScopesRedRead
        {
            get => this._ScopesRedRead;
            set => this._ScopesRedRead = value;
        }

        public string ScopesRedReadWrite
        {
            get => this._ScopesRedReadWrite;
            set => this._ScopesRedReadWrite = value;
        }

        public string APIURLYellow
        {
            get => this._APIURLYellow;
            set => this._APIURLYellow = value;
        }

        public string ScopesYellowRead
        {
            get => this._ScopesYellowRead;
            set => this._ScopesYellowRead = value;
        }

        public string ScopesYellowReadWrite
        {
            get => this._ScopesYellowReadWrite;
            set => this._ScopesYellowReadWrite = value;
        }

        public string APIURLBlack
        {
            get => this._APIURLBlack;
            set => this._APIURLBlack = value;
        }

        public string ScopesBlackRead
        {
            get => this._ScopesBlackRead;
            set => this._ScopesBlackRead = value;
        }

        public string ScopesBlackReadWrite
        {
            get => this._ScopesBlackReadWrite;
            set => this._ScopesBlackReadWrite = value;
        }

    }
}
