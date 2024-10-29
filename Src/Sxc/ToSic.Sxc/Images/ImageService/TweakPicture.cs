//namespace ToSic.Sxc.Images;

//internal record TweakPicture(ImageDecoratorVirtual VDec, ImageSpecs Img, PictureSpecs Pic, object ToolbarObj): TweakImage(VDec, Img, Pic, ToolbarObj), ITweakImage
//{
//    public ITweakImage PictureClass(string pictureClass)
//        => this with { Pic = Pic with { Class = pictureClass } };

//    public ITweakImage PictureAttributes(IDictionary<string, string> attributes)
//        => this with { Pic = Pic with { Attributes = CreateAttribDic(attributes, nameof(attributes)) } };
    
//    public ITweakImage PictureAttributes(IDictionary<string, object> attributes)
//        => this with { Pic = Pic with { Attributes = CreateAttribDic(attributes, nameof(attributes)) } };

//    public ITweakImage PictureAttributes(object attributes)
//        => this with { Pic = Pic with { Attributes = CreateAttribDic(attributes, nameof(attributes)) } };

//}
