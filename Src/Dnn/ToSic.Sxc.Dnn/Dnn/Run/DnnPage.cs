//using System.Collections.Generic;
//using ToSic.Eav.Apps.Run;
//using ToSic.Sxc.Web;

//namespace ToSic.Sxc.Dnn.Run
//{
//    public class DnnPage: IPage
//    {
//        public DnnPage(int id, string url)
//        {
//            Id = id;
//            Url = url ?? "url-unknown";
//        }

//        public int Id { get; }
//        public string Url { get; }

//        #region Parameters / URL Parameters

//        public List<KeyValuePair<string, string>> Parameters
//        {
//            get => _parameters ??
//                   (_parameters = Eav.Factory.Resolve<IHttp>().QueryStringKeyValuePairs());
//            set => _parameters = value;
//        }
//        private List<KeyValuePair<string, string>> _parameters;

//        #endregion

//    }


//}
