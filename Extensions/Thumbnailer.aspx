<%@ Page Language="C#" %>
<%@ OutputCache Duration="86400" VaryByParam="Image;Width;Height" %>
<%@ Import Namespace="System.Drawing" %>
<%@ Import Namespace="System.Drawing.Drawing2D" %>
<%@ Import Namespace="System.Drawing.Imaging" %>
<script runat="server">
    private void Page_Load(object sender, EventArgs e)
    {
        var Image = Request.QueryString["Image"];
        if (String.IsNullOrEmpty(Image))
        {
            ErrorResult();
            return;
        }

        var sWidth = Request["Width"];
        var Width = 120;
        if (sWidth != null)
            Width = Int32.Parse(sWidth);

        var sHeight = Request["Height"];
        var Height = 120;
        if (sHeight != null)
            Height = Int32.Parse(sHeight);

        var Path = Server.MapPath(Request.ApplicationPath + Image);

        if(!Path.Contains(Server.MapPath(Request.ApplicationPath)))
        {
            ErrorResult();
            return;
        }
        
        var bmp = CreateThumbnail(Path, Width, Height);

        if (bmp == null)
        {
            ErrorResult();
            return;
        }

        string OutputFilename = null;
        OutputFilename = Request.QueryString["OutputFilename"];

        //Set Image codec of JPEG type, the index of JPEG codec is "1"
        var codec = ImageCodecInfo.GetImageEncoders()[1];
        //Set the parameters for defining the quality of the thumbnail
        var eParams = new EncoderParameters(1);
        eParams.Param[0] = new EncoderParameter(System.Drawing.Imaging.Encoder.Quality, 95L);
        
        // Put user code to initialize the page here
        Response.ContentType = "image/jpeg";
        bmp.Save(Response.OutputStream, codec, eParams);
        bmp.Dispose();
    }

    private void ErrorResult()
    {
        Response.Clear();
        Response.ContentType = "image/png";
        var NotFoundImage = System.Drawing.Image.FromFile(Server.MapPath("~/DesktopModules/ToSIC_SexyContent/Extensions/404.png"));
        NotFoundImage.Save(Response.OutputStream, ImageFormat.Png);
        NotFoundImage.Dispose();
        
        // Send status code, skip iis customerrors
        Response.StatusCode = 404;
        Response.TrySkipIisCustomErrors = true;
        Response.End();
    }
    /// <summary>
    /// Creates a resized bitmap from an existing image on disk.
    /// Call Dispose on the returned Bitmap object</summary>
    /// Bitmap or null
    /// <param name="lcFilename"></param>
    /// <param name="lnWidth"></param>
    /// <param name="lnHeight"></param>
    /// <returns></returns>
    public static Bitmap CreateThumbnail(string lcFilename, int lnWidth, int lnHeight)
    {
        Bitmap bmpOut = null;
        try
        {
            var loBMP = new Bitmap(lcFilename);
            var loFormat = loBMP.RawFormat;
            decimal lnRatio;
            var lnNewWidth = 0;
            var lnNewHeight = 0;
            //*** If the image is smaller than a thumbnail just return it
            if (loBMP.Width < lnWidth && loBMP.Height < lnHeight)
                return loBMP;

            lnRatio = (decimal)lnWidth / loBMP.Width;
            lnNewWidth = lnWidth;
            var lnTemp = loBMP.Height * lnRatio;
            lnNewHeight = (int)lnTemp;
            
            if (lnNewHeight > lnHeight)
            {
                lnRatio = (decimal)lnHeight / loBMP.Height;
                lnNewHeight = lnHeight;
                lnTemp = loBMP.Width * lnRatio;
                lnNewWidth = (int)lnTemp;
            }

            // System.Drawing.Image imgOut =
            //      loBMP.GetThumbnailImage(lnNewWidth,lnNewHeight,
            //                              null,IntPtr.Zero);
            // *** This code creates cleaner (though bigger) thumbnails and properly
            // *** and handles GIF files better by generating a white background for
            // *** transparent images (as opposed to black)
            bmpOut = new Bitmap(lnNewWidth, lnNewHeight);
            var g = Graphics.FromImage(bmpOut);
            g.InterpolationMode = InterpolationMode.HighQualityBicubic;
            g.SmoothingMode = SmoothingMode.HighQuality;
            g.PixelOffsetMode = PixelOffsetMode.HighQuality;
            g.CompositingQuality = CompositingQuality.HighQuality;

            g.FillRectangle(Brushes.White, 0, 0, lnNewWidth, lnNewHeight);
            g.DrawImage(loBMP, 0, 0, lnNewWidth, lnNewHeight);
            loBMP.Dispose();
        }
        catch
        {
            return null;
        }
        return bmpOut;
    }
</script>