namespace ToSic.Sxc.Adam
{
    public abstract class ContainerBase
    {
        //public readonly AdamAppContext AppContext;

        //protected ContainerBase(AdamAppContext appContext)
        //{
        //    AppContext = appContext;
        //}



        /// <summary>
        /// Root of this container
        /// </summary>
        public string Root => GeneratePath("");


        /// <summary>
        /// Figure out the path to a subfolder within this container
        /// </summary>
        /// <param name="subFolder"></param>
        /// <returns></returns>
        protected abstract string GeneratePath(string subFolder);

        
    }
}