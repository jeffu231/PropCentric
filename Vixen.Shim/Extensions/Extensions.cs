using Vixen.Sys.Managers;

namespace Vixen.Extensions;
using Sys;

public static class Extensions
{
    extension(NodeManager nodeManager) 
    {
        public ElementNode PropRootNode => VixenSystem.Nodes.RootNode;  //fake this for the POC.
    }
}