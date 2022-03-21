using System;
#if NETFRAMEWORK
using HttpCompileException = System.Web.HttpCompileException;
#else
// TODO
using HttpCompileException = System.Exception;
#endif

namespace ToSic.Sxc.Code
{
    public static class ErrorHelp
    {
        private const string ErrHelpPre = "Error in your code. ***** Probably https://r.2sxc.org/";
        private const string ErrHelpSuf = " can help! ***** \n What follows is the internal error: ";

        private const string IEntityErrDetection = "error CS0234: The type or namespace name 'IEntity' does not exist in the namespace 'ToSic.Eav";
        private const string IEntityErrorMessage = ErrHelpPre + "ErrIEntity" + ErrHelpSuf;

        private const string DynamicListErrDetection = "error CS1977: Cannot use a lambda expression as an argument to a dynamically dispatched operation without first casting it to a delegate or expression tree type";
        private const string DynamicListErrMessage = ErrHelpPre + "ErrLambda" + ErrHelpSuf;


        private const string DynamicEntityErrDetection = "error CS0246: The type or namespace name 'DynamicEntity' could not be found";
        private const string DynamicEntityErrMessage = ErrHelpPre + "ErrDynamicEntity" + ErrHelpSuf;

        public static void AddHelpIfKnownError(Exception exp, bool autoThrow = true)
        {
            var additionalMsg = HelpText(exp);
            if (additionalMsg == null) return;

            WrapException(exp, additionalMsg, autoThrow);
        }

        public static string HelpText(Exception exp)
        {
            if (exp is HttpCompileException || exp is InvalidCastException)
                if (exp.Message.Contains(IEntityErrDetection))
                    return IEntityErrorMessage;

            if (exp is HttpCompileException)
                if (exp.Message.Contains(DynamicListErrDetection))
                    return DynamicListErrMessage;

            if (exp is HttpCompileException)
                if (exp.Message.Contains(DynamicEntityErrDetection))
                    return DynamicEntityErrMessage;

            return null;

        }


        private static Exception WrapException(Exception ex, string prefix, bool autoThrow)
        {
            var newEx = new Exception(prefix, ex);
            return autoThrow 
                ? throw newEx 
                : newEx;
        }

    }
}
