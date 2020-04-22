namespace ToSic.Sxc.Dnn
{
    /// <summary>
    /// This is the type used by code-behind classes of razor components
    /// </summary>
    public abstract class RazorComponentCode: RazorComponent
    {
        public virtual void OnRender() { }

        /// <summary>
        /// Override this method to also run any code automatically. <br/>
        /// It's meant for things like setting page headers etc. <br/>
        /// Note that it's run at the end of the render-cycle.
        /// </summary>
        public virtual void OnRenderComplete() { }

    }
}
