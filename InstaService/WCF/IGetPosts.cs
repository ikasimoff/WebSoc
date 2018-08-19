using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;

namespace InstaService.WCF
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the interface name "IGetPosts" in both code and config file together.
    [ServiceContract]
    public interface IGetPosts
    {
        [OperationContract]
        List<string> DoWork(int id);
    }
}
