<%@ Page Language="C#" %>
<script runat="server">
    // 2019-05-23 2dm
    // this is and old image resizer
    // it's deprecated in 2sxc 10 but since many installations may still have urls pointing to it
    // it simply redirects to the new image resizer

    private void Page_Load(object sender, EventArgs e)
    {
        var image = Request.QueryString["Image"];
        if (string.IsNullOrEmpty(image))
            return;

        var sWidth = Request["Width"];
        var width = 120;
        if (sWidth != null)
            width = Int32.Parse(sWidth);

        if (width > 3200) width = 3200;

        var sHeight = Request["Height"];
        var height = 120;
        if (sHeight != null)
            height = Int32.Parse(sHeight);

        if (height > 3200) height = 3200;

        Response.Redirect(image + "?w=" + width + "&h=" + height + "&mode=max");
    }
</script>