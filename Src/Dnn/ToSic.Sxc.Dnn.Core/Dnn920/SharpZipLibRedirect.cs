//using System;
//using System.Linq;
//using System.Reflection;

//namespace ToSic.SexyContent.Dnn920
//{
//    public class SharpZipLibRedirect
//    {

//        public static bool AlreadyRun { get; private set; }

//        public static void RegisterSharpZipLibRedirect()
//        {
//            if (AlreadyRun) return;
//            // stop any further attempts to access this
//            AlreadyRun = true;

//            try
//            {
//                ResolveRenamedSharpZipLib("SharpZipLib", "ICSharpCode.SharpZipLib");
//            }
//            catch
//            {
//                /* ignore */ 
//            }
//        }


//        // Note: got the original of code from this blog: https://blog.slaks.net/2013-12-25/redirecting-assembly-loads-at-runtime/
//        // and the updated version https://gist.github.com/markvincze/10148fbeb41a57c841c7

//        ///<summary>Adds an AssemblyResolve handler to redirect all attempts to load a specific assembly name to the specified version.</summary>
//        private static void ResolveRenamedSharpZipLib(string oldName, string newName)
//        {
//            Assembly Handler(object sender, ResolveEventArgs args)
//            {
//                // only check for access to old SharpZipLib
//                if (!args.Name.StartsWith(oldName))
//                    return null;

//                // If the original was not found, it should find the new one - otherwise it will be null
//                var alreadyLoadedAssembly = AppDomain.CurrentDomain.GetAssemblies().FirstOrDefault(a => a.GetName().Name == newName);
//                return alreadyLoadedAssembly;
//            }

//            AppDomain.CurrentDomain.AssemblyResolve += Handler;
//        }
//    }
//}
