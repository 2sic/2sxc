//using System;
//using System.Globalization;
//using System.IO;
//using System.Web;

//namespace ToSic.Sxc.Engines.RoslynCompile
//{
//    public class HelperResult : IHtmlString
//    {
//        private readonly Action<TextWriter> action;

//        /// <summary>
//        /// Initializes a new instance of the <see cref="action"/> class,
//        /// with the provided <paramref name="action"/>.
//        /// </summary>
//        /// <param name="action">The action that should be used to produce the result.</param>
//        public HelperResult(Action<TextWriter> action)
//        {
//            if (action == null)
//            {
//                throw new ArgumentNullException("action");
//            }

//            this.action = action;
//        }

//        /// <summary>
//        /// Returns a HTML formatted <see cref="string"/> that represents the current <see cref="string"/>.
//        /// </summary>
//        /// <returns>A HTML formatted <see cref="HelperResult"/> that represents the current <see cref="HelperResult"/>.</returns>
//        public string ToHtmlString()
//        {
//            return this.ToString();
//        }

//        /// <summary>
//        /// Returns a <see cref="string"/> that represents the current <see cref="string"/>.
//        /// </summary>
//        /// <returns>A <see cref="HelperResult"/> that represents the current <see cref="HelperResult"/>.</returns>
//        public override string ToString()
//        {
//            using (var stringWriter = new StringWriter(CultureInfo.InvariantCulture))
//            {
//                this.action(stringWriter);
//                return stringWriter.ToString();
//            }
//        }

//        /// <summary>
//        /// Writes the output of the <see cref="writer"/> to the provided <paramref name="writer"/>.
//        /// </summary>
//        /// <param name="writer">A <see cref="HelperResult"/> instance that the output should be written to.</param>
//        public void WriteTo(TextWriter writer)
//        {
//            this.action(writer);
//        }


//        /// <summary>
//        /// Writes the output of the <see cref="writer"/> to the provided <paramref name="writer"/>.
//        /// </summary>
//        /// <param name="writer">A <see cref="val"/> instance that the output should be written to.</param>
//        /// <param name="val"></param>
//        public void WriteTo(TextWriter writer, object val)
//        {
//            writer.Write(val);
//        }
//    }
//}